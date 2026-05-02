namespace GestaoTalentos.Domain;

public class Log
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string CommandName { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public string? OldValues { get; set; }
    public string? NewValues { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;
}