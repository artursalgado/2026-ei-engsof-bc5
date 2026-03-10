# Requisitos do Projeto - Tema A

## Requisitos Funcionais (RF)

### RF01 — Autenticação e Gestão de Acessos
O sistema deve permitir que os utilizadores se registem, façam login e logout na aplicação.
O sistema deve suportar três níveis de permissões:
* **User** – pode aceder e gerir os seus registos.
* **UserManager** – pode criar utilizadores, alterar permissões e visualizar todos os registos.
* **Admin** – tem acesso total ao sistema e pode realizar qualquer ação.

### RF02 — Gestão de Skills
O sistema deve permitir criar, editar, listar e remover skills (ex: C#, React, SQL).
Cada skill deve conter:
* nome
* categoria/área profissional (ex: developer, design, project manager).
Uma skill só pode ser removida se não estiver associada a nenhum talento.

> 📌 **Explicação:** as skills são partilhadas entre todos os utilizadores e servem para descrever competências dos talentos.

### RF03 — Gestão de Perfis de Talento
O sistema deve permitir criar, editar e remover perfis de talentos.
Cada perfil de talento deve incluir:
* nome
* país
* e-mail
* preço por hora
* visibilidade (público ou privado).
* 
> 📌 **Explicação:** talentos públicos podem ser vistos por todos os utilizadores, enquanto talentos privados apenas pelo utilizador que os criou.

### RF04 — Associação de Skills a Talentos
O sistema deve permitir associar múltiplas skills a um perfil de talento.
Para cada skill associada deve ser indicado:
* número de anos de experiência nessa skill.
* 


### RF05 — Gestão de Experiência Profissional
O sistema deve permitir adicionar experiências profissionais a um perfil de talento.
Cada experiência deve conter:
* nome da empresa
* ano de início
* ano de fim (opcional).
Não pode existir sobreposição de anos entre experiências profissionais do mesmo talento.


### RF06 — Pesquisa de Talentos
O sistema deve permitir pesquisar talentos por combinação de skills.
Os resultados da pesquisa devem ser ordenados alfabeticamente pelo nome do talento.

> 📌 **Explicação:** permite encontrar rapidamente profissionais com determinadas competências.

### RF07 — Gestão de Clientes
O sistema deve permitir criar, editar, listar e remover clientes (empresas).

> 📌 **Explicação:** os clientes representam empresas que procuram talentos e para quem podem ser criadas propostas de trabalho.

### RF08 — Gestão de Propostas de Trabalho
O sistema deve permitir criar, editar e remover propostas de trabalho associadas a clientes.
Cada proposta deve incluir:
* nome da proposta
* categoria de talento
* skills necessárias
* anos mínimos de experiência por skill
* número total de horas do projeto
* descrição do trabalho.


### RF09 — Matchmaking de Talentos
O sistema deve ser capaz de identificar automaticamente os talentos elegíveis para uma proposta de trabalho, com base nos requisitos definidos.
> 📌 **Explicação:** apenas talentos que possuam as skills e anos mínimos exigidos devem ser considerados elegíveis. (CONDIÇOES)

### RF10 — Listagem de Talentos Elegíveis
Para cada proposta de trabalho, o sistema deve permitir listar todos os talentos elegíveis.
Os resultados devem ser ordenados pelo valor total estimado do trabalho, calculado a partir:
* preço por hora do talento
* número de horas da proposta.
> 📌 **Explicação:** isto permite identificar os talentos mais económicos para determinada proposta.

### RF11 — Relatório de Preço Médio por Categoria e País
O sistema deve permitir gerar um relatório com o preço médio mensal dos talentos, considerando 176 horas de trabalho por mês.
O relatório deve ser apresentado:
* por categoria profissional
* por país.
> 📌 **Explicação:** 8 horas por dia × 22 dias de trabalho por mês = 176 horas

### RF12 — Relatório de Preço Médio por Skill
O sistema deve permitir gerar um relatório com o preço médio mensal por skill, considerando 176 horas de trabalho por mês.
> 📌 **Explicação:** permite perceber quanto custa em média um profissional com x ou y  competência.

---

## Requisitos Não Funcionais (RNF)

### RNF01 — Tecnologia
A aplicação deve ser desenvolvida utilizando:
* .NET Core (7, 8 ou 9) no backend
* Blazor ou Razor para a interface web.

### RNF02 — Persistência de Dados
Todos os dados da aplicação devem ser armazenados numa base de dados PostgreSQL.
O acesso à base de dados deve ser realizado utilizando Entity Framework Core.

### RNF03 — Arquitetura
O backend da aplicação deve ser estruturado utilizando o padrão Repository, garantindo separação entre lógica de negócio e acesso a dados.

### RNF04 — Validação de Dados
O sistema deve validar todos os dados introduzidos pelo utilizador antes de os guardar na base de dados.
Exemplo:
* datas inválidas
* campos obrigatórios
* valores negativos
