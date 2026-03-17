using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoTalentos.Shared.DTOs;

public class CreateSkillDto
{
    public string Nome { get; set; } = string.Empty;
    public int AreaId { get; set; }
}