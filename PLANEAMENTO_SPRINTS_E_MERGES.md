# Planeamento Oficial do Projeto - Gestão de Talentos (Tema A)

Este documento dita as regras do jogo para o Grupo 05 em relação à organização dos User Stories ao longo de 5 Sprints e como a vossa equipa deve gerir as branches (os "Merges") para o código não se destruir sozinho na reta final.

---

## PARTE 1: Correção do Backlog (As User Stories que faltam)

Ao analisarmos as imagens, vocês têm mapeado de **US01 a US06**. Para os requisitos do Professor estarem 100% cumpridos, vocês **têm obrigatoriamente de adicionar mais duas Issues ao vosso Github**:

*   **[US07] Motor de Pesquisa Livre de Talentos** *(Ponto 7 do Enunciado Mestre)*
    *   **Critério de Aceitação:** O sistema deve permitir que um recrutador insira uma lista de "Skills" que procura. O sistema deve devolver automaticamente todos os Perfis de Talento que detenham o histórico dessas skills em modo de lista.
*   **[US08] Dashboard e Relatórios Estatísticos** *(Pontos 10 e 11 do Enunciado Mestre)*
    *   **Critério de Aceitação:** O Admin ou UserManager pode ver um Dashboard com contadores, como por exemplo, a "taxa de conversão e aprovação", "novos talentos adicionados na semana passada" e o "orçamento mensal previsto face às propostas ativas" *(nota: a regra estrita de negócio exige que usem exatamente 1 mês = 176 horas laborais nos cálculos)*.

Atenção especial à **US03**: Vi nos critérios de aceitação da vossa foto a regra de que "*não pode haver sobreposição de experiências no mesmo ano*". Relembra o grupo que o backend tem imperativamente de ter uma validação matemática de datas a rejeitar anos conflituantes, antes de gravar na base de dados.

---

## PARTE 2: ROADMAP DOS 5 SPRINTS

Como o vosso projeto tem 5 Sprints e já terminaram a US01 no Front-end (e quase finalizaram a US02), esta é a distribuição inteligente (e segura de dependências lógicas) em que têm de trabalhar daqui para a frente:

### SPRINT 1: As Bases de Estrutura (Já executado)
*   **[US06] (Gestão de Enums):** A fundação total! É crítico existirem os tipos de *UserRoles*, *Países*, e *Áreas* na API antes de todo o resto arrancar, caso contrário os talentos ficam órfãos de localidade e setor.
*   **[US01] (Autenticação):** Toda a arquitetura do JWT e perfis (User, UserManager, Admin).

### SPRINT 2: Definições de Catálogos (A terminar agora)
*   **[US02] (Catálogo de Skills):** Depende obrigatoriamente de a US06 estar fechada para ter a categoria de "Área Profissional" injetada aqui neste catálogo mestre.

### SPRINT 3: O Motor Central da Aplicação 
*   **[US03] (Gestão de Perfis de Talento):** É a vossa tarefa colossal e a mais importante. Este currículo agrega: o utilizador logado (US01), a localização dele num país (US06) e junta as competências listadas globalmente (US02).

### SPRINT 4: A Lógica de Agências & Dinheiro 
*   **[US04] (Gestão de Clientes):** Criação das frentes comerciais (as empresas que compram talento).
*   **[US05] (Propostas de Trabalho):** Matchmaking matemático. Atribuição das vagas aos talentos já existentes. Calculam os honorários usando os dados que vieram do Sprint 3 e do catálogo do Sprint 2.

### SPRINT 5: Apresentação Final Analítica
*   **[US07] (Pesquisa Dinâmica):** É uma query complexa que faz scan à vossa base de dados madura. 
*   **[US08] (Analytics Gerais):** Usa LINQ/EntityFramework para contabilizar de forma agrupada todas as Propostas, Horas, $$ e Clientes ativos no vosso software. Fecha o semestre!

---

## PARTE 3: O GRAFO MESTRE DE MERGES DO VOSSO GRUPO

Uma aplicação com esta robustez de tabelas não se liga "à sorte". As peças de Lego da vossa base de dados exigem o seguinte ritual estrito:

1. **A US06** é concluída pelas mãos de alguém -> **Dá MERGE com a `develop`.**  *(A partir daqui, os 'Países e Áreas' já moram na gaveta principal).*
2. **A US01 (A tua)** foi concluída com o Front-End inteiro -> **Já deu MERGE maravilhoso com a `develop`.**  *(O projeto passou a estar trancado à porta e tem um AuthState ativo).*
3. A branch da Maria (US02-Skills) está dependente do ponto 1. Ela faz os acertos em casa -> **Dá MERGE com a `develop`.**  *(Passámos a ter um catálogo global de profissões na empresa).*

### 🚫 AGORA CUIDADO: COMO TU VAIS ARRANCAR A US03!
Chegou o teu dia de liderar e arrancar o Sistema de Currículos.
**Tu NÃO podes carregar num único botão de código C# sem antes**:
No teu Visual Studio abrires a consola e correres a lei magna:
1. `git fetch` (Ver O que ocorreu no mundo)
2. `git checkout develop` (Viajar para a base mestra)
3. `git pull origin develop` (Puxar Tudo das branches anteriores 1, 2 e 3 para a tua secretária)
4. Só aqui fazes `git checkout -b feature/US03-talentos`

Ao seguires este fluxo de injetar a `develop` atualizada, o teu *AppDbContext* vai herdar a tabela das Skills da Maria, as restrições e os Países.
Ao criares o Formulário do Talento na tua US03 nova, já podes ir buscar os ID's reais do País ou os ID's Reais das Skills, sem nunca desfragmentar ou partir a base de dados em pedaços perdidos! 

5. Terminas a US03 cheia de validações -> **Dás MERGE fantástico para a `develop`.**
6. Informas logo o rapaz/rapariga que tem a incumbência da **US04 (Clientes)** e da **US05 (Propostas)** que as tuas Tabelas de Perfil (UserIds e Experiências) aterraram na base-lugar, e dás-lhes ordens para eles sacarem a Develop atual por ti para, só então, iniciarem os "Merges" mágicos da reta final.
