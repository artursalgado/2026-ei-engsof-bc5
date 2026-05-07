namespace GestaoTalentos.Shared.DTOs;

//
// =========================
// AUTH DTOs
// =========================
//

public record UserRegisterDto(string Username, string Password, int RoleId);
public record UserLoginDto(string Username, string Password);
public record UserRoleUpdateDto(int UserId, string Role);
public record UserCreateDto(string Username, string Password, int RoleId);

//
// =========================
// PERFIL DTOs
// =========================
//

public record PerfilDto(
    int Id,
    int OwnerId,
    string Content,
    int PaisId,
    bool IsShared,
    DateTime CreatedAt
);

public record PerfilCreateDto(
    string Content,
    int PaisId,
    bool IsShared
);

public record PerfilUpdateDto(
    string Content,
    int PaisId,
    bool IsShared
);

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

public record CreateSkillDto(string Nome, int AreaId);

public record UpdateSkillDto(int Id, string Nome, int AreaId);

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

public record ExperienciaCreateDto(
    string Empresa,
    string Cargo,
    DateTime DataInicio,
    DateTime? DataFim
);

public record PerfilSkillDto(
    int SkillId,
    int Nivel
);

//
// =========================
// AREA DTOs
// =========================
//

public record AreaDto(int Id, string Nome, DateTime CriadoEm);
public record AreaCreateDto(string Nome);

//
// =========================
// ROLE DTOs
// =========================
//

public record RoleDto(int Id, string Nome, DateTime CriadoEm);
public record RoleCreateDto(string Nome);

//
// =========================
// PAIS DTOs
// =========================
//

public record PaisDto(int Id, string Nome, DateTime CriadoEm);
public record PaisCreateDto(string Nome);