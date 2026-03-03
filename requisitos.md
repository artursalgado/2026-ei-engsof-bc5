Requisitos Funcionais (RF)

RF01 - Autenticação e Gestão de Acessos: O sistema deve permitir registo, login, logout e suportar 3 perfis: User (vê os seus dados), UserManager (gere utilizadores e vê tudo) e Admin (controlo total).

RF02 - Gestão de Skills (Competências): O sistema deve permitir criar, editar, apagar e listar skills (ex: C#, React, SQL).

RF03 - Gestão de Perfis de Talento: O sistema deve permitir criar, editar e apagar perfis de talento, bem como associar/remover múltiplas skills a cada perfil.

RF04 - Pesquisa e Filtragem: O sistema deve permitir pesquisar talentos por nome e filtrá-los por skill e localização.

RF05 - Gestão de Clientes: O sistema deve permitir o registo e gestão de empresas/clientes que procuram talentos.

RF06 - Gestão de Propostas de Trabalho: O sistema deve permitir criar propostas de trabalho associadas a clientes.

RF07 - Matchmaking: O sistema deve filtrar automaticamente os talentos elegíveis para uma determinada proposta de trabalho (com base nas skills pedidas).

Requisitos Não Funcionais (RNF)

RNF01: A aplicação deve ser desenvolvida em .NET Core (7, 8 ou 9) com interface em Blazor ou Razor.

RNF02: Os dados devem ser persistidos em PostgreSQL utilizando a Entity Framework Core.

RNF03: A arquitetura do backend tem de implementar o padrão Repository.

RNF04: O sistema tem de validar os dados de input antes de os enviar para a base de dados.

RNF05: O sistema deve criar um utilizador Admin por defeito (Seed) no primeiro arranque.
