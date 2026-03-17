using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoTalentos.API;

public class CreateSkillDto
{
    [Required(ErrorMessage = "O nome da skill é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A área profissional é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Área inválida")]
    public int AreaId { get; set; }
}