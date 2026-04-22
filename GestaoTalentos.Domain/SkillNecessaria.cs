using System;

using System.ComponentModel.DataAnnotations;

namespace GestaoTalentos.Domain;

public class SkillNecessaria
{
    public int Id { get; set; }

    [Range(1, 10, ErrorMessage = "O nível mínimo requerido deve estar entre 1 e 10")]
    public int NivelMinimoRequerido { get; set; }

    public int SkillId { get; set; }
    public int PropostaId { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public Skill? Skill { get; set; }
}