using System;

using System.ComponentModel.DataAnnotations;

namespace GestaoTalentos.API;

public class SkillCreateDto
{
    [Required(ErrorMessage = "O nome da skill é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A área profissional é obrigatória")]
    public int AreaId { get; set; }
}