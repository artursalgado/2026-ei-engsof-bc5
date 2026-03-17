using System;
using System.Net.Http.Json;
namespace GestaoTalentos.Shared.DTOs;

public class UpdateSkillDto
{
    public string Nome { get; set; } = string.Empty;
    public int AreaId { get; set; }
}