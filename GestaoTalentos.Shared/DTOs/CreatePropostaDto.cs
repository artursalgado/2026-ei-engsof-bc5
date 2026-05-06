using System.ComponentModel.DataAnnotations;

namespace GestaoTalentos.Shared.DTOs;

public class CreatePropostaDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A área é obrigatória")]
    public int AreaId { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória")]
    public string DescricaoTrabalho { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "Horas deve estar entre 1 e 10000")]
    public int NumeroTotalHoras { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "Preço deve ser maior que 0")]
    public decimal PrecoHoraMedio { get; set; }

    public List<SkillNecessariaDto> SkillsNecessarias { get; set; } = new();
}