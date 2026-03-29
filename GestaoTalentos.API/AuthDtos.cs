namespace GestaoTalentos.API;

public record UserRegisterDto(string Username, string Password);
public record UserLoginDto(string Username, string Password);
public record UserRoleUpdateDto(int UserId, string Role);
public record UserCreateDto(string Username, string Password, string Role);

public record PerfilDto(int Id, int OwnerId, string Content, bool IsShared, DateTime CreatedAt);
public record PerfilCreateDto(string Content, bool IsShared);
public record PerfilUpdateDto(string Content, bool IsShared);