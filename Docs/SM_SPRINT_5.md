# Relatório de Scrum Master — Sprint 5
**Autor:** Artur Salgado (EI33385)  
**Data início:** 28 de abril de 2026  
**Data fim:** 11 de maio de 2026  
**Scrum Master:** Artur Salgado  
**Product Owner:** Maria José Conceição

---

## Introdução ao Sprint

Este é o quinto de seis sprints no total. O principal objetivo era fechar a US-05 (Propostas de Trabalho) e avançar com o dashboard e o motor de pesquisa. Neste sprint também se confirmou a existência de um ciclo adicional, pelo que a equipa fez uma sessão de brainstorming para identificar funcionalidades para o Sprint 6.

Um problema que marcou este sprint foi a falta de participação de dois membros da equipa. Apesar de vários lembretes, Carlos e Cristiano não responderam nem cumpriram as tarefas atribuídas. Esta situação está documentada na secção de comunicação.

---

## Objetivos

- Concluir e fazer merge da US-05 (Propostas de Trabalho)
- Implementar US-08 (Motor de Pesquisa de Talentos)
- Implementar US-09 (Dashboard e Relatórios)
- Corrigir BUG-36 (problemas na US-05) e BUG-37 (DTOs)
- Resolver conflitos do PR #35 (back-end/logs)
- Merge develop → main
- Preparar relatório e vídeo de entrega
- Brainstorming de funcionalidades para o Sprint 6
- Escolha de Scrum Master para o Sprint 6

---

## Sprint Backlog

| ID | Tarefa | Responsável | Estimativa | Estado |
|---|---|---|---|---|
| T01 | [BUG-36] Corrigir lógica de matching em `PropostaMatchingService` | Maria José | S | ✅ Concluído |
| T02 | [BUG-36] Frontend: mostrar talentos elegíveis por proposta | Maria José | S | ✅ Concluído |
| T03 | [BUG-36] Adicionar formulário de edição de proposta | Maria José | XS | ✅ Concluído |
| T04 | [BUG-36] Corrigir registos duplicados no DI (`Program.cs`) | Maria José | XS | ✅ Concluído |
| T05 | [BUG-36] PR #38: US-05 → develop | Maria José | XS | ✅ Concluído |
| T06 | [PR#35] Resolver conflitos de merge e integrar `back-end/logs` → develop | Ricardo Silva | S | ✅ Concluído |
| T07 | [US-09] Dashboard com estatísticas da plataforma (PR #39) | Ricardo Silva | M | ✅ Concluído |
| T08 | [US-08] Motor de pesquisa de talentos por skills | Maria José + Carlos | L | 🔄 Em curso |
| T09 | [BUG-37] Criar `UserDto` e `LoginResponseDto` | Carlos | S | ❌ Não iniciado |
| T10 | [BUG-37] Substituir objetos anónimos por DTOs nos endpoints | Carlos | S | ❌ Não iniciado |
| T11 | Revisão geral do codebase | Cristiano | S | ⚠️ Iniciado, não concluído |
| T12 | Merge develop → main | Artur Salgado (SM) | XS | ✅ Concluído |
| T13 | Brainstorming de funcionalidades para o Sprint 6 | Equipa | XS | ✅ Concluído |
| T14 | Redação do Relatório Sprint 5 | Artur Salgado (SM) | M | ✅ Concluído |

---

## Trabalho Realizado

### BUG-36 — Correções na US-05 (Maria José Conceição)

A branch `feature/US-05-Propostas_Trabalho` tinha vários problemas que impediam o merge. A Maria corrigiu:

- `PropostaMatchingService` — implementação da lógica real de verificação de skills e anos mínimos de experiência
- Página `Propostas.razor` atualizada para mostrar talentos elegíveis por proposta
- Formulário de edição de proposta adicionado
- Registos duplicados no DI em `Program.cs` removidos

Durante o merge surgiu um conflito no ficheiro `AppDbContextModelSnapshot.cs`, que foi resolvido com orientação do Scrum Master (Artur Salgado) através do comando `dotnet ef migrations add`. O PR #38 foi revisto e merged para develop sem problemas adicionais.

### PR #35 — Back-end/Logs (Ricardo Silva)

O Ricardo Silva resolveu os conflitos do PR #35 (`back-end/Logs`) e integrou a branch em develop. Esta branch continha correções de queries N+1 no endpoint `GET /areas` e a implementação de logging.

### US-09 — Dashboard e Relatórios (Ricardo Silva)

Foi implementado um dashboard com estatísticas visíveis para Admin e UserManager:

- Total de perfis de talento registados
- Total de propostas ativas
- Taxa de aprovação de talentos elegíveis
- Valorização média mensal dos talentos (1 mês = 176h)

O PR #39 foi criado e submetido a review. Durante a revisão, foi identificado que as migrações tinham sido removidas da branch, tendo sido pedido esclarecimento ao autor via comentário no PR.

### US-08 — Motor de Pesquisa de Talentos

O desenvolvimento foi iniciado pela Maria José Conceição. Carlos, que era co-responsável, não participou. O trabalho está em curso com base no esforço da Maria José.

### Merge develop → main

No final do sprint foi feito o merge de develop para main, incluindo US-05, US-09 e todas as correções do sprint.

### Brainstorming para o Sprint 6

Com um sprint adicional confirmado, a equipa reuniu brevemente para identificar funcionalidades a desenvolver:

- Exportação de talentos elegíveis para CSV
- Filtros avançados de perfis por país e preço
- Histórico de propostas por cliente
- Correção de DTOs inconsistentes (BUG-37)
- Conclusão da revisão do codebase

---

## Comunicação e Acompanhamento da Equipa

| Data | Membro | Mensagem |
|---|---|---|
| 05/05 17:14 | Artur Salgado (SM) | Atribuição de tarefas e pedido de daily standup |
| 05/05 17:15 | Artur Salgado (SM) | Mensagem fixada no #general |
| 05/05 17:24 | Artur Salgado (SM) | Pedido de confirmação a todos os membros |
| 05/05 17:25 | Maria José | "Devo terminar hoje" |
| 05/05 20:55 | Maria José | "Já terminei e fiz o PR para a develop" |
| 06/05 13:11 | Artur Salgado (SM) | Lembrete a Ricardo Silva para PR #35 |
| 06/05 13:11 | Artur Salgado (SM) | Lembrete a Carlos para BUG-37 e US-08 |
| 06/05 13:11 | Artur Salgado (SM) | Lembrete a Cristiano para tarefa #28 |
| 06/05 14:03 | Maria José | Reportou conflito em `AppDbContextModelSnapshot.cs` |
| 06/05 14:09 | Artur Salgado (SM) | Orientou resolução do conflito de migração EF |
| 06/05 22:12 | Maria José | Correção de erros adicionais pós-merge |
| 06/05 23:25 | Ricardo Silva | US-09 concluída, PR #39 criado e à espera de review |
| 06/05 23:42 | Ricardo Silva | Conflitos do PR #35 resolvidos e merged para develop |
| 07/05 11:43 | Artur Salgado (SM) | Lembrete a Cristiano e Carlos pela ausência |
| 07/05 11:43 | Artur Salgado (SM) | Review PR #39: pedido de esclarecimento sobre remoção das migrações |
| 08/05 | Artur Salgado (SM) | Segundo lembrete a Carlos |
| 08/05 | Artur Salgado (SM) | Segundo lembrete a Cristiano |
| — | Cristiano | Sem resposta durante todo o sprint |
| — | Carlos | Sem resposta nas tarefas BUG-37 e US-08 |

### Impedimentos Identificados

1. **Falta de participação** — Cristiano e Carlos não responderam a nenhuma comunicação ao longo do sprint, apesar de alertas presenciais e múltiplos lembretes. A revisão do codebase ficou por concluir e o BUG-37 não foi sequer iniciado.
2. **Conflito de migração EF** — Ocorreu durante o merge da US-05. Resolvido com orientação do Scrum Master via `dotnet ef migrations add`.
3. **Migrações removidas no PR #39** — Identificado durante a review. Esclarecimentos pedidos ao autor.

---

## Estado Atual da Aplicação

### Funcionalidades Implementadas

- Autenticação JWT com 3 roles, seed automático do Admin
- Gestão de Skills com validação de eliminação
- Gestão de Áreas de competência
- Gestão de Perfis com experiências profissionais e skills associadas
- Gestão de Clientes com controlo de acesso por role
- Propostas de Trabalho com matching automático de elegíveis
- Dashboard com estatísticas (Admin e UserManager)

### Em Desenvolvimento

- US-08 Motor de Pesquisa de Talentos

### Planeado para Sprint 6

- Funcionalidades definidas na sessão de brainstorming

### Stack Tecnológica

- **Framework:** .NET Core 8
- **Backend:** ASP.NET Core Web API (Minimal APIs)
- **Frontend:** Blazor WebAssembly
- **Base de Dados:** PostgreSQL
- **ORM:** Entity Framework Core
- **Arquitetura:** Repository Pattern (GoF Estrutural)
- **Autenticação:** JWT com BCrypt

### Estado das Branches

| Branch | Estado |
|---|---|
| main | Atualizada (até US-09) |
| develop | Atualizada, merged para main |
| front-end/US-01-Login-Auth | Merged-Fechada |
| feature/US-02-skills | Merged-Fechada |
| feature/US-03-PerfisTalento | Merged-Fechada |
| feature/US04-Clientes | Merged-Fechada |
| front-end-refactor | Merged-Fechada |
| feature/US-05-Propostas_Trabalho | Merged (PR #38) |
| back-end/US-06-Enums-MERGED-OLD | Merged (2 Ahead but trash) |
| feature/US-08-Pesquisa | Em desenvolvimento |
| feature/US09/#23 | Merged (PR #39) |
| back-end/Logs | Merged (PR #35) |

---

## Retrospetiva

### O que correu bem

- A Maria José demonstrou grande autonomia, resolvendo alguns conflitos e concluindo a US-05 dentro do prazo
- O Ricardo Silva contribuiu de forma significativa com a US-09 e a resolução do PR #35, após não ter contribuído nos últimos sprints
- A comunicação foi mais estruturada do que em sprints anteriores, com mensagens fixadas e lembretes documentados
- O papel de SM foi desempenhado sem produzir código, focado em coordenação e documentação
- O brainstorming garantiu continuidade para o sprint seguinte

### O que correu mal

- Dois membros não responderam durante todo o sprint, sobrecarregando os restantes, o que cria alguma revolta dentro do grupo entre elementos ativos e inativos
- A revisão do codebase ficou por concluir
- O BUG-37 não foi iniciado

### Lições para o Próximo SM

- A responsabilização individual precisa de ser reforçada logo no início de cada sprint
- Os standups assíncronos mostraram-se eficazes para os membros ativos

---

## Scrum Master do Sprint 6

Após brainstorming e decisão da equipa no final do sprint, o **Ricardo Silva** foi escolhido como Scrum Master para o Sprint 6 (12/05 – 25/05/2026).

---

## Próximos Passos (Sprint 6)

- Conclusão da US-08 (Motor de Pesquisa)
- Correção de DTOs (BUG-37)
- Conclusão da revisão do codebase (#28)
- Exportação de elegíveis para CSV
- Filtros avançados de perfis
- Merge final develop → main
