using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoTalentos.Domain;

public class Proposta
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da proposta é obrigatório")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A categoria de talento é obrigatória")]
    public int AreaId { get; set; }

    [Required(ErrorMessage = "A descrição do trabalho é obrigatória")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 2000 caracteres")]
    public string DescricaoTrabalho { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "O número de horas deve estar entre 1 e 10000")]
    public int NumeroTotalHoras { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "O preço/hora deve ser maior que 0")]
    public decimal PrecoHoraMedio { get; set; } // Para calcular valor estimado

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    // Criador (recrutador que registou a proposta)
    public int? CriadorId { get; set; }
    public User? Criador { get; set; }

    // Relacionamentos
    public Area? Area { get; set; }
    public ICollection<SkillNecessaria> SkillsNecessarias { get; set; } = new List<SkillNecessaria>();
    public ICollection<TalentoElegivel> TalentosElegiveis { get; set; } = new List<TalentoElegivel>();
}