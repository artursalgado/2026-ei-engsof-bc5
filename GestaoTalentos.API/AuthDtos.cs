namespace GestaoTalentos.API;

/// DTO para registro de novo utilizador.
public record UserRegisterDto(string Username, string Password);

/// DTO para login de utilizador.
public record UserLoginDto(string Username, string Password);

/// DTO para atualização da role de um utilizador.
public record UserRoleUpdateDto(int UserId, string Role);

/// DTO para criação de novo utilizador (por administradores).
public record UserCreateDto(string Username, string Password, string Role);

/// DTO para representação de um registro (record) na resposta da API.
public record RecordDto(int Id, int OwnerId, string Content, bool IsShared, DateTime CreatedAt);

/// DTO para criação de um novo registro.
public record RecordCreateDto(string Content, bool IsShared);

/// DTO para atualização de um registro existente.
public record RecordUpdateDto(string Content, bool IsShared);

/// DTO para representação de um cliente na resposta da API.
public record ClienteDto(int Id, string Nome, string Email, int IdCriador, int? IdMinhaConta);

/// DTO para criação de um novo cliente.
public record ClienteCreateDto(string Nome, string Email, int? IdCriador, int? IdMinhaConta);
/// DTO para atualização de um cliente existente.
public record ClienteUpdateDto(string Nome, string Email, int? IdCriador, int? IdMinhaConta);

/// DTO para representação de uma apresentação na resposta da API.
//public record ApresentacaoDto(int IdCliente, DateTime DataApresentacao);

/// DTO para criação de uma nova apresentação.
//public record ApresentacaoCreateDto(int IdCliente, DateTime DataApresentacao);
/// DTO para atualização de uma apresentação existente.
//public record ApresentacaoUpdateDto(int IdCliente, ;
