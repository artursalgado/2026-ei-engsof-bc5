namespace GestaoTalentos.API;


public record UserRegisterDto(string Username, string Password);/// DTO para registro de novo utilizador.
public record UserLoginDto(string Username, string Password);/// DTO para login de utilizador.

public record UserRoleUpdateDto(int UserId, string Role);// DTO para atualização da role de um utilizador.
public record UserCreateDto(string Username, string Password, string Role);// DTO para criação de novo utilizador (por admin).

// DTO para representação de um perfil na resposta da API.
public record PerfilDto(int Id, int OwnerId, string Content, int PaisId, bool IsShared, DateTime CreatedAt);
public record PerfilCreateDto(string Content, int PaisId, bool IsShared);
public record PerfilUpdateDto(string Content, int PaisId, bool IsShared);

/// DTO para representação de uma area na resposta da API.
public record AreaDto(int Id, string Nome, DateTime CriadoEm);
public record AreaCreateDto(string Nome);// DTO para criação de uma nova area.

/// DTO para representação de um role na resposta da API.
public record RoleDto(int Id, string Nome, DateTime CriadoEm);
public record RoleCreateDto(string Nome);// DTO para criação de um novo role.

/// DTO para representação de um pais na resposta da API.
public record PaisDto(int Id, string Nome, DateTime CriadoEm);
public record PaisCreateDto(string Nome);// DTO para criação de um novo pais.

