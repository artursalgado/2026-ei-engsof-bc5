using System;

using System.ComponentModel.DataAnnotations;

namespace GestaoTalentos.Domain;

public class Skill
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da skill é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A área profissional é obrigatória")]
    public int AreaId { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public Area? Area { get; set; }
    public ICollection<Abilidade> Abilidades { get; set; } = new List<Abilidade>();
    public ICollection<SkillNecessaria> SkillsNecessarias { get; set; } = new List<SkillNecessaria>();
}