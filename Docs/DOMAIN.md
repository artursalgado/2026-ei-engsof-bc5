# Domínio da Plataforma — Gestão de Talentos

Este documento explica a lógica de negócio e os tipos de utilizador da plataforma. O objetivo é evitar confusões entre o que é um "Cliente", um "Talento" e um "Utilizador do sistema".

---

## Contexto

A plataforma é uma ferramenta **interna** de uma empresa de recrutamento/consultoria.
Os funcionários da empresa usam a plataforma para gerir propostas de trabalho de clientes externos e encontrar os melhores talentos (candidatos) para as preencher.

---

## Tipos de Utilizador (Accounts)

Existem 3 roles no sistema:

| Role | Quem é | O que pode fazer |
|------|--------|-----------------|
| `User` | Funcionário base da empresa | Regista-se via `/register`. Pode criar perfis de talento, consultar skills e áreas, ver propostas. |
| `UserManager` | Gestor de equipa / recrutador sénior | Tudo o que o `User` faz, mais: criar/editar/apagar propostas, criar skills e áreas, ver dashboard e relatórios, partilhar propostas. |
| `Admin` | Administrador do sistema | Tudo. Gere contas de utilizadores (suspender, promover, criar funcionários). |

### Como se cria um utilizador?

- **Auto-registo** → qualquer pessoa acede a `/register` e cria uma conta com role `User`. A conta fica **ativa imediatamente** (sem aprovação necessária). Isto permite que novos funcionários da empresa comecem a usar a plataforma sem demoras.
- **Criação pelo Admin** → o administrador pode criar contas diretamente no painel `/admin`, com escolha de role (`User` ou `UserManager`). Útil para criar contas de gestores.
- **Promoção** → o administrador pode promover um `User` para `UserManager` e vice-versa a qualquer momento.
- **Suspensão** → o administrador pode suspender (revogar acesso) de qualquer conta. Uma conta suspensa não consegue fazer login.

---

## Entidades de Dados (não são contas)

### Perfil de Talento (`Perfil`)

Um **Perfil** representa um candidato/talento externo à empresa. **Não é uma conta de utilizador.**

- É criado por um funcionário (`User` ou superior) dentro da plataforma.
- Contém: nome, email, país, preço por hora, experiências profissionais, skills.
- Campo `IsShared`: quando `true`, o perfil é visível a todos os utilizadores da plataforma (não apenas ao seu criador).
- Campo `OwnerId`: FK para o `User` que criou o perfil.

### Cliente (`Cliente`)

Um **Cliente** representa uma empresa ou pessoa externa que faz pedidos de trabalho. **Não é uma conta de utilizador.**

- É criado por um funcionário dentro da plataforma (como registo de contacto).
- Contém: nome, email, referência ao criador.
- Campo `IdMinhaConta`: FK opcional para um `User` — permite ligar um cliente a uma conta caso esse cliente seja também funcionário.

### Proposta de Trabalho (`Proposta`)

Uma **Proposta** representa um pedido de trabalho de um cliente, que precisa de ser preenchido por um talento.

- Tem: nome, área, descrição, total de horas, preço por hora médio estimado.
- **Skills necessárias**: lista de skills com anos de experiência mínimos.
- **Talentos Elegíveis** (calculados automaticamente pelo `PropostaMatchingService`): perfis que têm as skills necessárias, com o valor estimado calculado.
- **Estado**: `Aberta` → `EmNegociacao` → `Fechada`.
- **TalentoSelecionadoId**: quando a proposta é fechada, guarda o `PerfilId` do talento escolhido.

---

## Fluxo Típico de Trabalho

```
1. Cliente externo contacta a empresa e pede um perfil de talento
2. UserManager cria uma Proposta com skills necessárias
3. Sistema calcula automaticamente os Talentos Elegíveis (matching)
4. UserManager abre o drawer da proposta → vê os talentos por ordem de fit
5. (Opcional) UserManager gera um link de partilha → envia ao cliente
6. Cliente vê a proposta e os talentos via link público (sem login)
7. UserManager seleciona o talento → proposta fica "Fechada"
```

---

## Partilha com Cliente (US-17)

Para facilitar a comunicação com clientes externos:

- Em qualquer proposta aberta, o `UserManager` pode clicar em **Partilhar**.
- O sistema gera um `PartilhaToken` (UUID de 30 caracteres, válido 30 dias).
- O link público é: `{base_url}/partilha/{token}`
- A página pública **não requer autenticação** — qualquer pessoa com o link pode ver a proposta e os talentos elegíveis.
- O link é copiado automaticamente para o clipboard.

---

## Estados de uma Proposta

| Estado | Significado |
|--------|-------------|
| `Aberta` | Proposta criada, à procura de talento. |
| `EmNegociacao` | Contacto iniciado com um ou mais talentos (estado manual). |
| `Fechada` | Talento selecionado. A proposta está concluída. |

---

## Matching de Talentos

O `PropostaMatchingService` corre automaticamente após a criação ou edição de uma proposta.

**Critérios de match:**
- O perfil de talento tem todas as skills necessárias da proposta.
- Os anos de experiência em cada skill ≥ o mínimo exigido pela proposta.

O **Valor Estimado** por talento = `PrecoPorHora do talento × NumeroTotalHoras da proposta`.

O **Fit de Preço** mostrado na UI = `PrecoHoraMedio da proposta ÷ PrecoPorHora do talento × 100%` (capped a 100%).
Um fit de 100% significa que o talento cabe exactamente no orçamento.

---

## Resumo Visual

```
[Empresa]
    └── Utilizadores (accounts)
            ├── Admin
            ├── UserManager
            └── User

[Dados geridos pelos utilizadores]
    ├── Clientes (empresas externas, registo de contacto)
    ├── Perfis de Talento (candidatos externos)
    ├── Skills / Áreas
    └── Propostas de Trabalho
            ├── Skills Necessárias
            ├── Talentos Elegíveis (auto-calculados)
            └── Token de Partilha (link público)
```
