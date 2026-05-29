namespace GestaoTalentos.Shared.DTOs;

//
// =========================
// AUTH DTOs
// =========================
//

public record UserRegisterDto(string Username, string Password);
public record UserLoginDto(string Username, string Password);
public record UserRoleUpdateDto(int UserId, string Role);
public record UserCreateDto(string Username, string Password, string Role);

//
// =========================
// PERFIL DTOs
// =========================
//

public record PerfilDto(
    int Id,
    int OwnerId,
    string Nome,
    string Email,
    string Pais,
    decimal PrecoPorHora,
    bool IsShared,
    DateTime CreatedAt
);

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

//
// =========================
// CLIENTE DTOs
// =========================
//

public record ClienteDto(int Id, string Nome, string Email, int IdCriador, int? IdMinhaConta);
public record ClienteCreateDto(string Nome, string Email, int? IdMinhaConta);
public record ClienteUpdateDto(string Nome, string Email, int? IdMinhaConta);

//
// =========================
// SKILL DTOs (CORRIGIDO)
// =========================
//
public record SkillDto(int Id, string Nome, int AreaId, string AreaNome, DateTime CriadoEm);

public class CreateSkillDto
{
    public string Nome { get; set; } = string.Empty;
    public int AreaId { get; set; }
}

public class UpdateSkillDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int AreaId { get; set; }
}

//
// =========================
// PERFIL TALENTO
// =========================
//

public class PerfilTalentoCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PaisId { get; set; }
    public decimal PrecoPorHora { get; set; }
    public bool IsShared { get; set; }

    public List<ExperienciaCreateDto> Experiencias { get; set; } = new();
    public List<PerfilSkillDto> Skills { get; set; } = new();
}

public class PerfilTalentoUpdateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PaisId { get; set; }
    public decimal PrecoPorHora { get; set; }
    public bool IsShared { get; set; }

    public List<ExperienciaCreateDto> Experiencias { get; set; } = new();
    public List<PerfilSkillDto> Skills { get; set; } = new();
}

//
// =========================
// AUXILIARES
// =========================
//

public class ExperienciaCreateDto
{
    public string Empresa { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public int AnoInicio { get; set; }
    public int? AnoFim { get; set; }
}

public class PerfilSkillDto
{
    public int SkillId { get; set; }
    public int AnosExperiencia { get; set; }
}

//
// =========================
// AREA DTOs
// =========================
//

public record AreaDto(int Id, string Nome, DateTime CriadoEm);
public class AreaCreateDto
{
    public string Nome { get; set; } = string.Empty;
}

//
// =========================
// ROLE DTOs
// =========================
//

public record RoleDto(int Id, string Nome, DateTime CriadoEm);
public record RoleCreateDto(string Nome);

//
// =========================
// PARTILHA / US-17
// =========================
//

public class SelecionarTalentoDto
{
    public int PerfilId { get; set; }
}

//
// =========================
// PAIS DTOs
// =========================
//

public record PaisDto(int Id, string Nome, DateTime CriadoEm);
public record PaisCreateDto(string Nome);