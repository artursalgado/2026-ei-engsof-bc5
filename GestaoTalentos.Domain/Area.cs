using System;
using System.Collections.Generic;

namespace GestaoTalentos.Domain;

public class Area
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
}