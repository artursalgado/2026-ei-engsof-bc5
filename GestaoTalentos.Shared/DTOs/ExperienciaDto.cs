namespace GestaoTalentos.Shared.DTOs;

// DTO para exibir uma experiência profissional existente
public class ExperienciaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public int AnoInicio { get; set; }
    public int? AnoFim { get; set; }
}

// DTO para criar/editar uma experiência (sem Id, o Id fica na BD)
public class ExperienciaCreateDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public int AnoInicio { get; set; }
    public int? AnoFim { get; set; }
}
