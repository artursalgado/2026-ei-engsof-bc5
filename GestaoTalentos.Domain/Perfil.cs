namespace GestaoTalentos.Domain;

public class Perfil
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public decimal PrecoPorHora { get; set; }
    
    public int OwnerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsShared { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int PaisId { get; set; }
    
    // Relacionamentos
    public Pais? Pais { get; set; }

    public ICollection<ExperienciaProfissional> Experiencias { get; set; } = new List<ExperienciaProfissional>();
    public ICollection<PerfilSkill> PerfilSkills { get; set; } = new List<PerfilSkill>();
}