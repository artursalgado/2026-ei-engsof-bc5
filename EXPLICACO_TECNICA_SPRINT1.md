# EXPLICACO_TECNICA_SPRINT1

Este documento descreve com detalhes técnicos a implementação do projeto GestaoTalentos (Tema A), para aluno e professor.

## 1. Estrutura da Solução (O 'Porquê' das Pastas)

A solução está organizada em 5 projetos (pastas) distintos:

- `GestaoTalentos.Domain`:
  - Contém as entidades de negócios (User e Record) e as interfaces de repositório.
  - Porquê: separa regras de domínio das infraestruturas, facilita testes e evolução.

- `GestaoTalentos.Infrastructure`:
  - Implementa persistência: `AppDbContext`, `UserRepository`, `RecordRepository`.
  - Porquê: esta camada lida com detalhes do banco (PostgreSQL/EF) enquanto a API permanece limpa.

- `GestaoTalentos.API`:
  - Contém os endpoints REST, configuração de DI (injeção de dependência), autenticação e autorização.
  - Porquê: expõe a aplicação com regras de segurança e workflow, mantendo a lógica de negócio separada.

- `GestaoTalentos.Client`:
  - Aplicação Blazor WebAssembly (front-end), estrutura de UI.
  - Porquê: coloca apresentação do usuário separada do backend; adequa requisito de Blazor.

- `GestaoTalentos.Tests`:
  - Contém testes unitários/básicos para validação do código.
  - Porquê: garante qualidade, facilita correções e demonstra compromisso com TDD.

Esta separação segue o que foi exigido no Sprint: arquitetura em camadas, responsabilidade única e manutenção fácil.

## 2. Base de Dados e Persistência (PostgreSQL)

### 2.1 Requisito PostgreSQL

O projeto usa PostgreSQL como motor de dados, conforme enunciado. O `Npgsql.EntityFrameworkCore.PostgreSQL` está instalado e configurado no `GestaoTalentos.API` e `GestaoTalentos.Infrastructure`.

### 2.2 Entity Framework

Usamos Entity Framework (EF) para a tradução de objetos C# para tabelas SQL.

### 2.3 AppDbContext

- Arquivo: `GestaoTalentos.Infrastructure/AppDbContext.cs`.

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Record> Records { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Apresentacao> Apresentacoes { get; set; } = null!;
}
```

- O `AppDbContext` define `DbSet<User>` e `DbSet<Record>`.
- Cada `DbSet<T>` corresponde a uma tabela no PostgreSQL.
- Quando a aplicação roda, o EF mapeia propriedades C# (`Id`, `Username`, `Role`) para colunas na tabela `Users`.
- `context.Database.EnsureCreated()` cria as tabelas automaticamente, se não existirem.

## 3. Autenticação e os 3 Níveis (Requisito Geral 1)

### 3.1 Model User e UserRole

Em `GestaoTalentos.Domain/User.cs`:

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}

public enum UserRole
{
    User,
    UserManager,
    Admin
}
```

### 3.2 BCrypt

Usamos `BCrypt.Net` para hash de senhas:
- Antes de gravar password no banco, aplicamos `BCrypt.HashPassword(...)`.
- No login, comparamos com `BCrypt.Verify(...)`.
- Isso impede armazenar senhas em texto plano e protege contra vazamentos.

### 3.3 JWT

O serviço Web API usa JWT (JSON Web Tokens) para autenticação:
- Em `Program.cs`, configuramos `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)`.
- Ao fazer login, geramos token com claims de `user id`, `username`, `role`.
- O token é usado no header HTTP `Authorization: Bearer <token>`.

### 3.4 Níveis de acesso

Definimos policies em `Program.cs`:
- `UserPolicy` (User, UserManager, Admin)
- `UserManagerPolicy` (UserManager, Admin)
- `AdminPolicy` (Admin)

Endpoints:
- `/users/me`: token válido (UserPolicy)
- `/users` CRUD: Admin/UserManager
- `/users/{id}/role`: Admin
- `/records` CRUD controla visibilidade por role (User: own+shared, UserManager/Admin: full)

### 3.5 Seed Admin automático

No arranque de `Program.cs`:

```csharp
if (!await userRepository.AnyAsync())
{
    var admin = new User
    {
        Username = "admin",
        Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
        Role = UserRole.Admin
    };
    await userRepository.AddAsync(admin);
}
```

Isso implementa o requisito de criar um Admin conhecido no primeiro arranque. O docente vê credenciais `admin/admin123` prontas para login.

## 4. Padrões de Desenho (Design Patterns)

### 4.1 Repository Pattern

Patrão: abstrai acesso a dados em classes específicas.

- `IUserRepository` e `IRecordRepository` definem operações sem detalhes de DB.
- `UserRepository` e `RecordRepository` implementam essas operações usando EF Tech.

Benefícios:
- API depende de interfaces (inversão de controle).
- Código testável (mock de repositório em testes).
- Mudança de DB (ex: PostgreSQL para outro) exige só a camada Infrastructure.

### 4.2 Single Responsibility

Cada classe/fonte tem uma responsabilidade única:
- Domain = dados e contratos.
- Infrastructure = persistência.
- API = exposição e regras de autorização.
- Client = interface do usuário.

### 4.3 DTOs (Data Transfer Objects)

Usamos DTOs em `GestaoTalentos.API/AuthDtos.cs` para evitar expor informações sensíveis:
- `UserRegisterDto`, `UserLoginDto`, `RecordCreateDto`, etc.
- Garante que só as propriedades permitidas entram na API.

## 5. Mapeamento de Conformidade (Checklist do Enunciado)

### 5.1 Tabela de Conformidade

| Requisito do PDF | Status | Onde no Código |
|---|---|---|
| .NET 8 | ✅ | *.csproj* (net8.0) |
| Blazor | ✅ | `GestaoTalentos.Client` |
| Web API | ✅ | `GestaoTalentos.API/Program.cs` |
| PostgreSQL + EF | ✅ | `Program.cs` (UseNpgsql), `AppDbContext` |
| Auto-seed Admin | ✅ | `Program.cs` (admin/admin123) |
| 3 roles (User, UserManager, Admin) | ✅ | `UserRole` enum |
| Registo | ✅ | `POST /register` |
| Login | ✅ | `POST /login` |
| User pode ver own/partilhados | ✅ | `GET /records` + `GetVisibleForUserAsync` |
| UserManager gerir utilizadores | ✅ | `/users`, `/users` POST, `UsersManagerPolicy` |
| Admin controle total | ✅ | `/users/{id}/role`, policies e records edit/delete |

## 6. O que falta para a conclusão total do Tema A

- Criação de entidades associadas ao contexto do projeto:
  - Skills
  - TalentProfile (Perfis de Talento)
  - Experience (Experiência Profissional)
- Endpoint CRUD para estas entidades
- UI Blazor para gestão dos itens acima
- Validação robusta de dados (
  FluentValidation ou DataAnnotations)
- Testes unitários e de integração para todos os fluxos
- Logs estruturados e tratamento global de exceções
- Suporte a refresh tokens e política de bloqueio de login (security hardening)

## 7. Observações explicativas

- `Domain` existe para definir "o que temos" (entidades e regras).
- `Infrastructure` existe para guardar as coisas (base de dados). O professor deve ver aqui a camada com código SQL/EF.
- `API` é "a porta" da aplicação, onde chegam as requisições.
- `Client` é o que o utilizador final vê no navegador.
- `Tests` mostram como sistematicamente validar comportamento.

### Perguntas comuns respondidas
- *Por que usar Bcrypt?* Para que senhas sejam armazenadas como hash, não texto.
- *Por que JWT?* Para autenticar estado sem guardar sessão no servidor.
- *Por que Repository?* Para isolar dependências e facilitar mudança de tecnologia.

---
