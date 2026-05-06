namespace GestaoTalentos.Domain;

public class PerfilSkill
{
    public int PerfilId { get; set; }
    public Perfil? Perfil { get; set; }

    public int SkillId { get; set; }
    public Skill? Skill { get; set; }

    public int AnosExperiencia { get; set; }
}
