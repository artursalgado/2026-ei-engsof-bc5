using System.ComponentModel.DataAnnotations;

namespace GestaoTalentos.Shared.DTOs;

public class UpdatePropostaDto
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public int AreaId { get; set; }

    [Required]
    public string DescricaoTrabalho { get; set; } = string.Empty;

    [Range(1, 10000)]
    public int NumeroTotalHoras { get; set; }

    [Range(1, double.MaxValue)]
    public decimal PrecoHoraMedio { get; set; }

    public List<SkillNecessariaDto> SkillsNecessarias { get; set; } = new();
}