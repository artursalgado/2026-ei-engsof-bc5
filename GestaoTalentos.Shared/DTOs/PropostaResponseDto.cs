namespace GestaoTalentos.Shared.DTOs;

public record PropostaAreaDto(int Id, string Nome);

public record PropostaSkillInfoDto(int Id, string Nome);

public record SkillNecessariaResponseDto(
    int Id,
    int SkillId,
    int NivelMinimoRequerido,
    PropostaSkillInfoDto? Skill
);

public record PerfilResumoDto(int Id, int OwnerId, string Nome, string Pais, decimal PrecoPorHora);

public record TalentoElegivelResponseDto(
    int Id,
    int PerfilId,
    decimal ValorEstimado,
    PerfilResumoDto? Perfil
);

public class PropostaResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public PropostaAreaDto? Area { get; set; }
    public string DescricaoTrabalho { get; set; } = string.Empty;
    public int NumeroTotalHoras { get; set; }
    public decimal PrecoHoraMedio { get; set; }
    public decimal ValorEstimadoTotal => NumeroTotalHoras * PrecoHoraMedio;
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
    public List<SkillNecessariaResponseDto> SkillsNecessarias { get; set; } = new();
    public List<TalentoElegivelResponseDto>? TalentosElegiveis { get; set; }
}

public record PropostaCreatedResponseDto(int Id, string Nome);