using GestaoTalentos.Domain;
using GestaoTalentos.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.API;

public static class SeedData
{
    /// <summary>
    /// Popula a BD com dados de demonstração.
    /// Só executa se ainda não existirem áreas (BD nova/limpa).
    /// </summary>
    public static async Task SeedAsync(AppDbContext context)
    {
        // Guarda para não repetir em re-arranques
        // (a migration já cria áreas base; verificamos skills que só o seed cria)
        if (await context.Skills.AnyAsync()) return;

        // =====================
        // ÁREAS
        // =====================
        var areaDevWeb   = new Area { Nome = "Desenvolvimento Web",  CriadoEm = DateTime.UtcNow };
        var areaDesign   = new Area { Nome = "Design & UX",          CriadoEm = DateTime.UtcNow };
        var areaGestao   = new Area { Nome = "Gestão de Projeto",    CriadoEm = DateTime.UtcNow };
        var areaDados    = new Area { Nome = "Dados & IA",           CriadoEm = DateTime.UtcNow };

        await context.Areas.AddRangeAsync(areaDevWeb, areaDesign, areaGestao, areaDados);
        await context.SaveChangesAsync();

        // =====================
        // SKILLS
        // =====================
        var now = DateTime.UtcNow;
        var skills = new List<Skill>
        {
            new Skill { Nome = "React",              AreaId = areaDevWeb.Id,  CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "Node.js",            AreaId = areaDevWeb.Id,  CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "C#",                 AreaId = areaDevWeb.Id,  CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "TypeScript",         AreaId = areaDevWeb.Id,  CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "PostgreSQL",         AreaId = areaDevWeb.Id,  CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "Figma",              AreaId = areaDesign.Id,  CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "UI Design",          AreaId = areaDesign.Id,  CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "Adobe XD",           AreaId = areaDesign.Id,  CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "Scrum",              AreaId = areaGestao.Id,  CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "Gestão de Equipas", AreaId = areaGestao.Id,  CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "Python",             AreaId = areaDados.Id,   CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "Machine Learning",   AreaId = areaDados.Id,   CriadoEm = now, AtualizadoEm = now },
            new Skill { Nome = "Power BI",           AreaId = areaDados.Id,   CriadoEm = now, AtualizadoEm = now },
        };
        await context.Skills.AddRangeAsync(skills);
        await context.SaveChangesAsync();

        // Referências rápidas por nome
        Skill S(string nome) => skills.First(s => s.Nome == nome);
        var react    = S("React");
        var nodejs   = S("Node.js");
        var csharp   = S("C#");
        var ts       = S("TypeScript");
        var pg       = S("PostgreSQL");
        var figma    = S("Figma");
        var uidesign = S("UI Design");
        var scrum    = S("Scrum");
        var geqip    = S("Gestão de Equipas");
        var python   = S("Python");
        var ml       = S("Machine Learning");
        var powerbi  = S("Power BI");

        // =====================
        // UTILIZADORES EXTRA
        // =====================
        var manager = new User
        {
            Username = "manager",
            Password = BCrypt.Net.BCrypt.HashPassword("manager123"),
            Role = UserRole.UserManager
        };
        var joao = new User
        {
            Username = "joao",
            Password = BCrypt.Net.BCrypt.HashPassword("joao123"),
            Role = UserRole.User
        };
        var maria = new User
        {
            Username = "maria",
            Password = BCrypt.Net.BCrypt.HashPassword("maria123"),
            Role = UserRole.User
        };
        await context.Users.AddRangeAsync(manager, joao, maria);
        await context.SaveChangesAsync();

        // =====================
        // CLIENTES
        // =====================
        var clientes = new List<Cliente>
        {
            new Cliente { Nome = "TechCorp Lda",     Email = "contacto@techcorp.pt",  IdCriador = joao.Id    },
            new Cliente { Nome = "Inovação SA",      Email = "geral@inovacao.pt",     IdCriador = maria.Id   },
            new Cliente { Nome = "StartUp Digital",  Email = "info@startupdigital.pt",IdCriador = manager.Id },
        };
        await context.Clientes.AddRangeAsync(clientes);
        await context.SaveChangesAsync();

        // =====================
        // PERFIS DE TALENTO
        // =====================
        var perfis = new List<Perfil>
        {
            new Perfil
            {
                Nome = "João Silva", Email = "joao.silva@email.pt", Pais = "Portugal",
                PrecoPorHora = 35, IsShared = true, OwnerId = joao.Id, CreatedAt = now,
                PerfilSkills = new List<PerfilSkill>
                {
                    new PerfilSkill { SkillId = react.Id,  AnosExperiencia = 3 },
                    new PerfilSkill { SkillId = nodejs.Id, AnosExperiencia = 2 },
                    new PerfilSkill { SkillId = csharp.Id, AnosExperiencia = 4 },
                },
                Experiencias = new List<ExperienciaProfissional>
                {
                    new ExperienciaProfissional { Titulo = "Desenvolvedor Frontend", Empresa = "WebSolutions Lda",  AnoInicio = 2020, AnoFim = 2022 },
                    new ExperienciaProfissional { Titulo = "Full Stack Developer",   Empresa = "TechCorp Lda",     AnoInicio = 2023, AnoFim = null  },
                }
            },
            new Perfil
            {
                Nome = "Maria Santos", Email = "maria.santos@email.pt", Pais = "Portugal",
                PrecoPorHora = 42, IsShared = true, OwnerId = maria.Id, CreatedAt = now,
                PerfilSkills = new List<PerfilSkill>
                {
                    new PerfilSkill { SkillId = react.Id, AnosExperiencia = 5 },
                    new PerfilSkill { SkillId = ts.Id,    AnosExperiencia = 3 },
                    new PerfilSkill { SkillId = pg.Id,    AnosExperiencia = 2 },
                },
                Experiencias = new List<ExperienciaProfissional>
                {
                    new ExperienciaProfissional { Titulo = "Frontend Developer", Empresa = "Inovação SA",     AnoInicio = 2019, AnoFim = 2022 },
                    new ExperienciaProfissional { Titulo = "Lead Developer",     Empresa = "StartUp Digital", AnoInicio = 2023, AnoFim = null  },
                }
            },
            new Perfil
            {
                Nome = "Carlos Ferreira", Email = "carlos.ferreira@email.pt", Pais = "Portugal",
                PrecoPorHora = 30, IsShared = true, OwnerId = manager.Id, CreatedAt = now,
                PerfilSkills = new List<PerfilSkill>
                {
                    new PerfilSkill { SkillId = figma.Id,    AnosExperiencia = 4 },
                    new PerfilSkill { SkillId = uidesign.Id, AnosExperiencia = 5 },
                    new PerfilSkill { SkillId = react.Id,    AnosExperiencia = 2 },
                },
                Experiencias = new List<ExperienciaProfissional>
                {
                    new ExperienciaProfissional { Titulo = "UI/UX Designer",  Empresa = "Design Studio Lda", AnoInicio = 2019, AnoFim = 2022 },
                    new ExperienciaProfissional { Titulo = "Product Designer", Empresa = "StartUp Digital",  AnoInicio = 2023, AnoFim = null  },
                }
            },
            new Perfil
            {
                Nome = "Ana Pereira", Email = "ana.pereira@email.pt", Pais = "Espanha",
                PrecoPorHora = 48, IsShared = true, OwnerId = manager.Id, CreatedAt = now,
                PerfilSkills = new List<PerfilSkill>
                {
                    new PerfilSkill { SkillId = python.Id,  AnosExperiencia = 5 },
                    new PerfilSkill { SkillId = ml.Id,      AnosExperiencia = 3 },
                    new PerfilSkill { SkillId = pg.Id,      AnosExperiencia = 2 },
                },
                Experiencias = new List<ExperienciaProfissional>
                {
                    new ExperienciaProfissional { Titulo = "Data Scientist",     Empresa = "DataLab ES",      AnoInicio = 2020, AnoFim = 2022 },
                    new ExperienciaProfissional { Titulo = "ML Engineer",        Empresa = "AI Solutions SA", AnoInicio = 2023, AnoFim = null  },
                }
            },
            new Perfil
            {
                Nome = "Pedro Costa", Email = "pedro.costa@email.pt", Pais = "Portugal",
                PrecoPorHora = 38, IsShared = true, OwnerId = manager.Id, CreatedAt = now,
                PerfilSkills = new List<PerfilSkill>
                {
                    new PerfilSkill { SkillId = scrum.Id,  AnosExperiencia = 6 },
                    new PerfilSkill { SkillId = geqip.Id,  AnosExperiencia = 4 },
                    new PerfilSkill { SkillId = react.Id,  AnosExperiencia = 1 },
                },
                Experiencias = new List<ExperienciaProfissional>
                {
                    new ExperienciaProfissional { Titulo = "Scrum Master",    Empresa = "AgileTeam",        AnoInicio = 2018, AnoFim = 2021 },
                    new ExperienciaProfissional { Titulo = "Project Manager", Empresa = "Consulting Group", AnoInicio = 2022, AnoFim = null  },
                }
            },
        };
        await context.Perfis.AddRangeAsync(perfis);
        await context.SaveChangesAsync();

        // =====================
        // PROPOSTAS + TALENTOS ELEGÍVEIS (matching automático)
        // =====================
        var matching = new PropostaMatchingService(context);

        async Task CriarProposta(Proposta p, List<(Skill skill, int nivel)> skillsReq)
        {
            await context.Propostas.AddAsync(p);
            await context.SaveChangesAsync();

            foreach (var (skill, nivel) in skillsReq)
            {
                await context.SkillsNecessarias.AddAsync(new SkillNecessaria
                {
                    SkillId = skill.Id, PropostaId = p.Id,
                    NivelMinimoRequerido = nivel, CriadoEm = now
                });
            }
            await context.SaveChangesAsync();

            var talentos = await matching.IdentificarTalentosElegiveisAsync(p.Id, p.PrecoHoraMedio);
            if (talentos.Any())
            {
                await context.TalentosElegiveis.AddRangeAsync(talentos);
                await context.SaveChangesAsync();
            }
        }

        await CriarProposta(
            new Proposta
            {
                Nome = "Sistema de Gestão de RH",
                AreaId = areaDevWeb.Id,
                DescricaoTrabalho = "Plataforma web para gestão de recursos humanos com dashboard analítico e relatórios.",
                NumeroTotalHoras = 500, PrecoHoraMedio = 35,
                CriadoEm = now, AtualizadoEm = now
            },
            new List<(Skill, int)> { (react, 2), (csharp, 3) }
        );

        await CriarProposta(
            new Proposta
            {
                Nome = "App Mobile E-commerce",
                AreaId = areaDevWeb.Id,
                DescricaoTrabalho = "Aplicação mobile para e-commerce com integração de pagamentos e notificações push.",
                NumeroTotalHoras = 300, PrecoHoraMedio = 40,
                CriadoEm = now, AtualizadoEm = now
            },
            new List<(Skill, int)> { (react, 3), (ts, 1) }
        );

        await CriarProposta(
            new Proposta
            {
                Nome = "Redesign Plataforma SaaS",
                AreaId = areaDesign.Id,
                DescricaoTrabalho = "Redesign completo da interface e experiência de utilizador de uma plataforma SaaS B2B.",
                NumeroTotalHoras = 200, PrecoHoraMedio = 32,
                CriadoEm = now, AtualizadoEm = now
            },
            new List<(Skill, int)> { (figma, 2), (uidesign, 2) }
        );

        await CriarProposta(
            new Proposta
            {
                Nome = "Dashboard de Análise de Dados",
                AreaId = areaDados.Id,
                DescricaoTrabalho = "Criação de dashboard interativo para análise de dados de vendas com modelos preditivos.",
                NumeroTotalHoras = 400, PrecoHoraMedio = 45,
                CriadoEm = now, AtualizadoEm = now
            },
            new List<(Skill, int)> { (python, 3), (ml, 2) }
        );
    }
}
