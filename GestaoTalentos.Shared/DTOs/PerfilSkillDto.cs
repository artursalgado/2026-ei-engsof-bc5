namespace GestaoTalentos.Shared.DTOs;

// DTO para a associação Perfil <-> Skill (com anos de experiência)
public class PerfilSkillDto
{
    public int SkillId { get; set; }
    public string? SkillNome { get; set; }
    public int AnosExperiencia { get; set; }
}
