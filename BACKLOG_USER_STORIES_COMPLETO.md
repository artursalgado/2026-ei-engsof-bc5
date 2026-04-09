# Backlog Oficial Completo - Gestão de Talentos (Tema A)

Aqui tens todas as User Stories (da 1 à 8) escritas exatamente com o mesmo padrão e nível de detalhe que partilhaste comigo. Isto garante que quer tu quer a tua equipa, e principalmente o professor, vejam uma coerência de 100%.

---

## [US01] Sistema de Autenticação e Perfis de Acesso
**User Story:** 
Como visitante, quero poder criar uma conta e efetuar login, para poder aceder ao sistema de forma segura.

**Critérios de Aceitação:**
- [ ] O sistema deve permitir o registo e o login de utilizadores.
- [ ] Devem existir exatamente 3 níveis de autenticação: User, UserManager e Admin.
- [ ] O nível 'User' apenas pode aceder aos seus registos ou aos partilhados consigo.
- [ ] O nível 'UserManager' pode criar utilizadores, editar permissões e ver todos os registos.
- [ ] O nível 'Admin' tem controlo total sobre o sistema (criar/editar todos, alterar permissões, ver tudo).
- [ ] Quando a aplicação arranca pela primeira vez, o sistema tem de criar automaticamente um utilizador Admin com credenciais conhecidas.

---

## [US02] Gestão do Catálogo de Skills
**User Story:** 
Como utilizador autenticado, quero poder criar e editar skills, para que estas fiquem disponíveis no catálogo partilhado do sistema.

**Critérios de Aceitação:**
- [ ] O formulário de criação/edição de uma skill tem de pedir um nome e associar uma área profissional (ex: developer, design, etc.).
- [ ] As skills criadas têm de ser partilhadas e visíveis entre todos os utilizadores do sistema.
- [ ] O sistema não pode permitir que uma skill seja apagada se ela já estiver associada a algum profissional de talento.
- [ ] Os dados inseridos devem ser validados no backend e frontend antes de serem guardados na base de dados.

---

## [US03] Gestão de Perfis de Talento
**User Story:** 
Como utilizador autenticado, quero poder criar e editar Perfis de Talento, para registar os seus currículos e competências no sistema.

**Critérios de Aceitação:**
- [ ] O formulário de criação/edição de um Perfil de Talento tem de pedir obrigatoriamente um nome, e-mail, país e um preço por hora.
- [ ] Os perfis podem ser listados como públicos ou privados.
- [ ] O perfil de talento deverá ter várias skills associadas, indicando o número de anos de experiência para cada skill.
- [ ] A cada perfil poderá ser adicionado o detalhe de uma determinada experiência profissional.
  - [ ] Para cada experiência é necessário indicar um título, nome da empresa, ano de começo e ano de término (opcional, caso o talento ainda trabalhe nessa empresa).
  - [ ] O sistema deverá fazer validações de cruzamento de datas para **garantir que não pode haver sobreposição de experiências no mesmo ano**.
- [ ] Os dados inseridos devem ser estritamente validados antes de serem guardados.

---

## [US04] Gestão de Clientes
**User Story:** 
Como utilizador autenticado, quero poder criar e editar Clientes (entidades patronais/agências), para que fiquem registadas no sistema.

**Critérios de Aceitação:**
- [ ] O formulário de criação/edição de um Cliente tem de pedir obrigatoriamente o seu nome e e-mail de contacto.
- [ ] A cada cliente deverá ser possível apresentar perfis de talentos e apresentar propostas de trabalho futuras.
- [ ] Cada cliente poderá também criar propostas de trabalho para perfis que lhe foram apresentados diretamente ou que este possa visualizar ativamente (perfis públicos do sistema).
- [ ] As validações de base de dados têm de estar ativas em todos os campos.

---

## [US05] Gestão de Propostas de Trabalho
**User Story:** 
Como utilizador autenticado, quero poder criar e gerir Propostas de Trabalho de forma a associá-las aos talentos com o perfil certo.

**Critérios de Aceitação:**
- [ ] O formulário de criação/edição de uma Proposta de Trabalho tem de pedir um nome, uma Categoria de talento, as Skills necessárias, o número mínimo de anos de experiência por skill, uma indicação do número total de horas a faturar do projeto, e validação/descrição do trabalho.
- [ ] O sistema deve possuir um algoritmo para identificar automaticamente e extrair numa lista todos os talentos elegíveis para a proposta.
- [ ] Os talentos e as listas de resultados devem vir ordenados no ecrã de acordo com o valor total estimado do trabalho. 
  - [ ] O cálculo desse valor final é: Preço/Hora pedido pelo talento x Número de horas estipuladas na proposta.

---

## [US06] Gestão de Enums (Bases do Sistema)
**User Story:** 
Como utilizador do tipo Admin autenticado, quero poder criar e editar os Enums globais, para não os ter listados no código (hardcoded).

**Critérios de Aceitação:**
- [ ] O formulário de criação/edição de um recurso "Enum" (seja de que tipo for) tem de pedir obrigatoriamente um "Nome".
- [ ] O projeto tem de base os seguintes Enums na Base de Dados: UserRoles (*User, UserManager, Admin*), País e Área Profissional.
- [ ] O C# / Banco de Dados deve validar de forma que, por se tratarem de "Categories/Enums", seja impossível haver repetição de nomes na base de dados.

---

## [US07] Motor de Pesquisa Livre de Talentos
**(Issue Adicionada após revermos os requisitos oficiais do projeto)**

**User Story:** 
Como recrutador, quero poder pesquisar por múltiplas skills em simultâneo para encontrar a pessoa exata para o cargo.

**Critérios de Aceitação:**
- [ ] O sistema deve apresentar um campo ou menu de pesquisa interativo visível na dashboard ou página de talentos.
- [ ] O recrutador pode selecionar múltiplas "Skills" num único filtro.
- [ ] O sistema devolve de imediato a listagem dos IDs/Nomes apenas dos Talentos que contenham *obrigatoriamente* toda a combinação daquelas Skills procuradas nas suas experiências ativas.

---

## [US08] Dashboard e Relatórios Estatísticos
**(Issue Adicionada após revermos os requisitos oficiais do projeto)**

**User Story:** 
Como Administração (Admin / UserManager), quero aceder a um painel de indicadores (Dashboard) para analisar dados vitais do desempenho da minha agência.

**Critérios de Aceitação:**
- [ ] Ao ter o nível de permissão adequado, exibe-se uma zona estatística.
- [ ] Deverá mostrar informações úteis para o negócio como a soma de inscrições do site nas últimas semanas, e um rácio de aprovação.
- [ ] Deverá possuir e realizar métricas de valorização de propostas.
- [ ] A matemática para os totais de estimativas laborais tem de utilizar a regra global (1 Mês de trabalho = 176 horas) nas fórmulas em C#.
