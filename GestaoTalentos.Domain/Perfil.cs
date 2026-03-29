namespace GestaoTalentos.Domain;

public class Perfil
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsShared { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}