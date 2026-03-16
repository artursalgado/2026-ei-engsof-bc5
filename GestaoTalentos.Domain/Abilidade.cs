using System;

using System.ComponentModel.DataAnnotations;

namespace GestaoTalentos.Domain;

public class Abilidade
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da abilidade é obrigatório")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição não pode ter mais de 500 caracteres")]
    public string? Descricao { get; set; }

    [Range(1, 10, ErrorMessage = "O nível deve estar entre 1 e 10")]
    public int? NivelComplexidade { get; set; }

    public int SkillId { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public Skill? Skill { get; set; }
}