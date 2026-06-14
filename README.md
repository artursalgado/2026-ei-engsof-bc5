# 🚀 IT Talent Management Platform (Tema A)

> **UC:** Engenharia de Software II | **Grupo:** 2026-ei-engsof-bc5  
> **Instituição:** IPVC - ESTG

Esta plataforma é uma solução moderna para a gestão de talentos de TI e propostas de trabalho, permitindo a gestão de perfis, competências (skills) e o matchmaking entre candidatos e oportunidades.

---

## 👥 Equipa e Gestão (Scrum)

O projeto é desenvolvido seguindo a metodologia **Scrum**, com rotação do papel de **Scrum Master** a cada Sprint.

| Sprint | Data Início | Data Fim | Scrum Master | Product Owner | Estado |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Sprint 1** | 03/03 | 16/03 | Cristiano | Artur | ✅ Concluido |
| **Sprint 2** | 17/03 | 30/03 | Carlos | Ricardo | ✅ Concluido |
| **Sprint 3** | 31/03 | 13/04 | Maria | Carlos |  ✅ Concluido |
| **Sprint 4** | 14/04 | 27/04 | Ricardo | Cristiano |  ✅ Concluido |
| **Sprint 5** | 28/04 | 11/05 | Artur | Maria| 📅 Agendado |

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

### Setup manual da Base de Dados

A base de dados pode ser criada de duas formas:

**Opção 1 — Migrations EF Core (recomendado para desenvolvimento):**

```bash
dotnet ef database update --project GestaoTalentos.Infrastructure --startup-project GestaoTalentos.API
```

**Opção 2 — Scripts SQL standalone:**

Os scripts SQL estão disponíveis na pasta `database/` para quem prefere criar a BD sem depender do Entity Framework.

```bash
# Criar a base de dados (uma vez)
createdb TalentosDB

# Aplicar o schema (tabelas + índices + chaves estrangeiras)
psql -d TalentosDB -f database/schema.sql

# Inserir dados iniciais (4 Áreas + utilizador Admin)
psql -d TalentosDB -f database/seed.sql
```

Após executar os scripts existirá:
- **Utilizador Admin** — username `admin`, password `admin123`
- **4 Áreas profissionais** — Developer, Designer, Product Manager, Project Manager

Ambos os scripts são idempotentes — podem ser corridos várias vezes sem erro nem duplicação de registos.

---

## 💬 Comunicação
* **Discord do Projeto:** [Aceder aqui](https://discord.gg/XNEQsZvg)

---
© 2026 - Grupo bc5 - Engenharia de Software II
