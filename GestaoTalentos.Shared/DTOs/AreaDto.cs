using System;

namespace GestaoTalentos.Shared.DTOs;

public class AreaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
    public int TotalSkills { get; set; }
}