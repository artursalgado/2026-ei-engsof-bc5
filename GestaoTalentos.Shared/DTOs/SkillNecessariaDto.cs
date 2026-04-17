using System.ComponentModel.DataAnnotations;

namespace GestaoTalentos.Shared.DTOs;

public class SkillNecessariaDto
{
    [Required]
    public int SkillId { get; set; }

    [Range(1, 10)]
    public int AnosExperienciaMinimo { get; set; }
}