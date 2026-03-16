using System;
namespace GestaoTalentos.API;

public class AreaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public int TotalSkills { get; set; }
}