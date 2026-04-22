namespace GestaoTalentos.Domain;

public class ExperienciaProfissional
{
    public int Id { get; set; }
    public int PerfilId { get; set; }
    public Perfil? Perfil { get; set; }
    
    public string Titulo { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public int AnoInicio { get; set; }
    public int? AnoFim { get; set; }
}
