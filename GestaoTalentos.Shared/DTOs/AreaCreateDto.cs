using System;

using System.ComponentModel.DataAnnotations;

namespace GestaoTalentos.Shared.DTOs;

public class AreaCreateDto
{
    [Required(ErrorMessage = "O nome da área é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
}