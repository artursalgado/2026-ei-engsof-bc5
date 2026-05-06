namespace GestaoTalentos.API;

using GestaoTalentos.Shared.DTOs;

public record UserRegisterDto(string Username, string Password, GestaoTalentos.Domain.TipoUtilizador TipoUtilizador);
public record UserLoginDto(string Username, string Password);
public record UserRoleUpdateDto(int UserId, string Role);
public record UserCreateDto(string Username, string Password, string Role);

public record ClienteDto(int Id, string Nome, string Email, int IdCriador, int? IdMinhaConta);
public record ClienteCreateDto(string Nome, string Email, int? IdMinhaConta);
public record ClienteUpdateDto(string Nome, string Email, int? IdMinhaConta);

// DTO novo para criar um Perfil de Talento completo (US03)
public class PerfilCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public decimal PrecoPorHora { get; set; }
    public bool IsShared { get; set; }
    public List<ExperienciaCreateDto> Experiencias { get; set; } = new();
    public List<PerfilSkillDto> Skills { get; set; } = new();
}

// DTO para editar um Perfil de Talento (dados identicos ao criar)
public class PerfilUpdateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public decimal PrecoPorHora { get; set; }
    public bool IsShared { get; set; }
    public List<ExperienciaCreateDto> Experiencias { get; set; } = new();
    public List<PerfilSkillDto> Skills { get; set; } = new();
}