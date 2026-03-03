# 🚀 IT Talent Management Platform (Tema A)

> **UC:** Engenharia de Software II | **Grupo:** 2026-ei-engsof-bc5  
> **Instituição:** IPVC - ESTG

Esta plataforma é uma solução moderna para a gestão de talentos de TI e propostas de trabalho, permitindo a gestão de perfis, competências (skills) e o matchmaking entre candidatos e oportunidades.

---

## 👥 Equipa e Gestão (Scrum)

O projeto é desenvolvido seguindo a metodologia **Scrum**, com rotação do papel de **Scrum Master** a cada Sprint.

| Sprint | Scrum Master | Estado |
| :--- | :--- | :--- |
| **Sprint 1** | Cristiano| 🏗️ Em curso |
| **Sprint 2** | Carlos| 📅 Agendado |
| **Sprint 3** | Maria | 📅 Agendado |
| **Sprint 4** | Ricardo | 📅 Agendado |
| **Sprint 5** | Artur | 📅 Agendado |

📌 *Consultar o ficheiro [SPRINTS.md](./SPRINTS.md) para o cronograma detalhado.*

---

## 🛠️ Stack Tecnológica

* **Framework:** .NET Core 8/9
* **Backend:** ASP.NET Core Web API / MVC (C#)
* **Frontend:** Blazor / Razor
* **Base de Dados:** PostgreSQL
* **ORM:** Entity Framework Core
* **Arquitetura:** Repository Pattern & Design Patterns

---

## 📋 Requisitos do Sistema

### 🔐 Autenticação e Autorização (RBAC)
Conforme o enunciado, o sistema possui três níveis de acesso:
* **User:** Acede aos seus registos e partilhados.
* **UserManager:** Gere utilizadores e permissões.
* **Admin:** Controlo total do sistema e logs.
* *Nota: O sistema realiza Seed automático de um utilizador Admin no primeiro arranque.*

### 🏗️ Funcionalidades Core
- [ ] **Gestão de Skills:** CRUD completo e catálogo de competências.
- [ ] **Perfis de Talento:** Gestão de CVs, associação de múltiplas skills, localização e pesquisa avançada.
- [ ] **Gestão de Clientes e Propostas:** Registo de propostas de trabalho e filtragem inteligente de talentos elegíveis.
- [ ] **Relatórios:** Visualização de métricas de ocupação e propostas (conforme Requisitos Gerais).

---

## ⚙️ Configuração e Instalação

### Pré-requisitos
* .NET SDK (v8.0+)
* PostgreSQL

### Passos para Execução
1.  **Clonar o repositório:**
    ```bash
    git clone [https://github.com/teu-utilizador/2026-ei-engsof-bc5.git](https://github.com/teu-utilizador/2026-ei-engsof-bc5.git)
    ```
2.  **Configurar a Base de Dados:**
    Atualize a `ConnectionString` no ficheiro `appsettings.json`.
3.  **Aplicar Migrations:**
    ```bash
    dotnet ef database update
    ```
4.  **Executar a aplicação:**
    ```bash
    dotnet run
    ```

---

## 💬 Comunicação
* **Discord do Projeto:** [Aceder aqui](https://discord.gg/XNEQsZvg)

---
© 2026 - Grupo bc5 - Engenharia de Software II
