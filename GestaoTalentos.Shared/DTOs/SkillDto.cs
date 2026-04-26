using System;

namespace GestaoTalentos.Shared.DTOs;

public class SkillDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string? AreaNome { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}