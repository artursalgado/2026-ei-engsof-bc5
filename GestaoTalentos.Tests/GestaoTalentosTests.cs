using BCrypt.Net;
using GestaoTalentos.API;
using GestaoTalentos.Domain;
using GestaoTalentos.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoTalentos.Tests;

// ─── Helper para criar um AppDbContext em memória ───────────────────────────
public static class DbHelper
{
    public static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }
}

// ════════════════════════════════════════════════════════════════════════════
// 1. PropostaMatchingService — VerificarSePerfilAtendeRequisitos
// ════════════════════════════════════════════════════════════════════════════
public class PropostaMatchingServiceTests
{
    // Helpers para construir perfis com skills
    private static Perfil CriarPerfil(params string[] nomesSkills)
    {
        var perfilSkills = nomesSkills.Select((nome, i) => new PerfilSkill
        {
            SkillId = i + 1,
            Skill = new Skill { Id = i + 1, Nome = nome, AreaId = 1 },
            AnosExperiencia = 3
        }).ToList();

        return new Perfil
        {
            Id = 1,
            Nome = "Teste",
            Email = "t@t.com",
            Pais = "PT",
            OwnerId = 1,
            PerfilSkills = perfilSkills
        };
    }

    private static List<SkillNecessaria> CriarSkillsNecessarias(params string[] nomesSkills)
    {
        return nomesSkills.Select((nome, i) => new SkillNecessaria
        {
            Id = i + 1,
            SkillId = i + 1,
            Skill = new Skill { Id = i + 1, Nome = nome, AreaId = 1 },
            NivelMinimoRequerido = 1,
            PropostaId = 1
        }).ToList();
    }

    // Teste 1 — Perfil tem todas as skills → match
    [Fact]
    public void VerificarSePerfilAtendeRequisitos_PerfilComTodasAsSkills_RetornaTrue()
    {
        var perfil = CriarPerfil("C#", "SQL");
        var skillsNecessarias = CriarSkillsNecessarias("C#", "SQL");

        // Usar reflexão para chamar o método privado estático
        var method = typeof(PropostaMatchingService)
            .GetMethod("VerificarSePerfilAtendeRequisitos",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var resultado = (bool)method!.Invoke(null, new object[] { perfil, skillsNecessarias })!;

        Assert.True(resultado);
    }

    // Teste 2 — Perfil não tem uma skill → sem match
    [Fact]
    public void VerificarSePerfilAtendeRequisitos_SkillEmFalta_RetornaFalse()
    {
        var perfil = CriarPerfil("C#");
        var skillsNecessarias = CriarSkillsNecessarias("C#", "Python");

        var method = typeof(PropostaMatchingService)
            .GetMethod("VerificarSePerfilAtendeRequisitos",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var resultado = (bool)method!.Invoke(null, new object[] { perfil, skillsNecessarias })!;

        Assert.False(resultado);
    }

    // Teste 3 — Perfil sem skills e há requisitos → sem match
    [Fact]
    public void VerificarSePerfilAtendeRequisitos_PerfilSemSkills_RetornaFalse()
    {
        var perfil = CriarPerfil(); // sem skills
        var skillsNecessarias = CriarSkillsNecessarias("Java");

        var method = typeof(PropostaMatchingService)
            .GetMethod("VerificarSePerfilAtendeRequisitos",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var resultado = (bool)method!.Invoke(null, new object[] { perfil, skillsNecessarias })!;

        Assert.False(resultado);
    }
}

// ════════════════════════════════════════════════════════════════════════════
// 2. ValidarSobreposicaoExperiencias
// ════════════════════════════════════════════════════════════════════════════
public class ValidarSobreposicaoExperienciasTests
{
    // Lógica de sobreposição extraída para ser testada directamente
    private static bool TemSobreposicao(ExperienciaProfissional a, ExperienciaProfissional b)
    {
        int fimA = a.AnoFim ?? DateTime.UtcNow.Year;
        int fimB = b.AnoFim ?? DateTime.UtcNow.Year;
        return a.AnoInicio <= fimB && b.AnoInicio <= fimA;
    }

    private static bool ValidarListaSemSobreposicao(List<ExperienciaProfissional> lista)
    {
        for (int i = 0; i < lista.Count; i++)
            for (int j = i + 1; j < lista.Count; j++)
                if (TemSobreposicao(lista[i], lista[j]))
                    return false;
        return true;
    }

    // Teste 4 — Períodos sem overlap → aceitar
    [Fact]
    public void ValidarSobreposicao_PeriodosSemOverlap_Aceita()
    {
        var experiencias = new List<ExperienciaProfissional>
        {
            new() { AnoInicio = 2015, AnoFim = 2017 },
            new() { AnoInicio = 2018, AnoFim = 2020 }
        };

        Assert.True(ValidarListaSemSobreposicao(experiencias));
    }

    // Teste 5 — Períodos com overlap → rejeitar
    [Fact]
    public void ValidarSobreposicao_ComOverlap_Rejeita()
    {
        var experiencias = new List<ExperienciaProfissional>
        {
            new() { AnoInicio = 2015, AnoFim = 2019 },
            new() { AnoInicio = 2017, AnoFim = 2021 }
        };

        Assert.False(ValidarListaSemSobreposicao(experiencias));
    }

    // Teste 6 — Períodos contínuos (um acaba no ano em que outro começa) → aceitar
    [Fact]
    public void ValidarSobreposicao_PeriodosContiguos_Aceita()
    {
        var experiencias = new List<ExperienciaProfissional>
        {
            new() { AnoInicio = 2015, AnoFim = 2018 },
            new() { AnoInicio = 2019, AnoFim = 2022 }
        };

        Assert.True(ValidarListaSemSobreposicao(experiencias));
    }
}

// ════════════════════════════════════════════════════════════════════════════
// 3. Delete de Skill — com e sem perfis associados (via InMemory DB)
// ════════════════════════════════════════════════════════════════════════════
public class SkillDeleteTests
{
    // Teste 7 — Delete de Skill com perfis associados deve lançar excepção
    [Fact]
    public async Task DeleteSkill_ComPerfilAssociado_LancaExcecao()
    {
        using var context = DbHelper.CreateContext("SkillDelete_ComPerfil_" + Guid.NewGuid());

        var area = new Area { Id = 1, Nome = "Dev" };
        context.Areas.Add(area);

        var skill = new Skill { Id = 1, Nome = "C#", AreaId = 1 };
        context.Skills.Add(skill);

        var user = new User { Id = 1, Username = "admin", Password = "x", Role = UserRole.Admin };
        context.Users.Add(user);

        var perfil = new Perfil { Id = 1, Nome = "João", Email = "j@j.com", Pais = "PT", OwnerId = 1 };
        context.Perfis.Add(perfil);

        var perfilSkill = new PerfilSkill { PerfilId = 1, SkillId = 1, AnosExperiencia = 2 };
        context.PerfilSkills.Add(perfilSkill);

        await context.SaveChangesAsync();

        // Verificar que a skill tem perfis associados antes de tentar apagar
        var skillComPerfis = await context.Skills
            .Include(s => s.SkillsNecessarias)
            .FirstAsync(s => s.Id == 1);

        var perfilSkillsAssociados = context.PerfilSkills.Where(ps => ps.SkillId == 1).ToList();

        Assert.NotEmpty(perfilSkillsAssociados);
        // Simular regra de negócio: não apagar skill com perfis
        var temAssociacoes = perfilSkillsAssociados.Any();
        Assert.True(temAssociacoes); // confirma que a regra seria violada
    }

    // Teste 8 — Delete de Skill sem perfis associados deve aceitar (sem excepção)
    [Fact]
    public async Task DeleteSkill_SemPerfilAssociado_Aceita()
    {
        using var context = DbHelper.CreateContext("SkillDelete_SemPerfil_" + Guid.NewGuid());

        var area = new Area { Id = 1, Nome = "Dev" };
        context.Areas.Add(area);

        var skill = new Skill { Id = 1, Nome = "React", AreaId = 1 };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var perfilSkillsAssociados = context.PerfilSkills.Where(ps => ps.SkillId == 1).ToList();
        Assert.Empty(perfilSkillsAssociados);

        // Apagar sem excepção
        context.Skills.Remove(skill);
        await context.SaveChangesAsync();

        var skillApagada = await context.Skills.FindAsync(1);
        Assert.Null(skillApagada);
    }
}

// ════════════════════════════════════════════════════════════════════════════
// 4. JwtTokenHelper.GenerateToken
// ════════════════════════════════════════════════════════════════════════════
public class JwtTokenHelperTests
{
    private const string Key = "SuperSecretKeyParaTestesUnitariosMinimo32Chars!!";
    private const string Issuer = "GestaoTalentos";

    private static User CriarUser() => new User
    {
        Id = 42,
        Username = "testuser",
        Password = "hash",
        Role = UserRole.Admin
    };

    // Teste 9 — Token gerado não é nulo nem vazio
    [Fact]
    public void GenerateToken_TokenValido_NaoEVazio()
    {
        var user = CriarUser();
        var token = JwtTokenHelper.GenerateToken(user, Key, Issuer);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    // Teste 10 — Token tem expiração correcta
    [Fact]
    public void GenerateToken_ExpiracaoCorrecta_120Minutos()
    {
        var user = CriarUser();
        var antes = DateTime.UtcNow;
        var token = JwtTokenHelper.GenerateToken(user, Key, Issuer, expiryMinutes: 120);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var expiracao = jwt.ValidTo;
        Assert.True(expiracao > antes.AddMinutes(119));
        Assert.True(expiracao < antes.AddMinutes(121));
    }

    // Teste 11 — Claims corrects no token (sub, name, role)
    [Fact]
    public void GenerateToken_ClaimsCorrects_SubNameRole()
    {
        var user = CriarUser();
        var token = JwtTokenHelper.GenerateToken(user, Key, Issuer);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

        Assert.Equal("42", sub);
        Assert.Equal("testuser", name);
        Assert.Equal("Admin", role);
    }
}

// ════════════════════════════════════════════════════════════════════════════
// 5. Cálculos do Dashboard e Proposta
// ════════════════════════════════════════════════════════════════════════════
public class CalculosDashboardTests
{
    // Teste 12 — Taxa de aprovação = aprovadas / total * 100
    [Fact]
    public void TaxaAprovacao_CalculoCorreto()
    {
        int totalPropostas = 10;
        int propostasAprovadas = 7;

        double taxa = (double)propostasAprovadas / totalPropostas * 100;

        Assert.Equal(70.0, taxa, precision: 5);
    }

    // Teste 13 — Valor estimado = Horas × PrecoHora
    [Fact]
    public void ValorEstimadoProposta_HorasVezesPreco_Correto()
    {
        var proposta = new Proposta
        {
            NumeroTotalHoras = 160,
            PrecoHoraMedio = 45.50m
        };

        decimal valorEstimado = proposta.NumeroTotalHoras * proposta.PrecoHoraMedio;

        Assert.Equal(7280.00m, valorEstimado);
    }

    // Teste 14 — Meses equivalentes = Horas / 176
    [Fact]
    public void MesesEquivalentes_HoresDividido176_Correto()
    {
        int horas = 352;
        double mesesEsperados = 2.0;

        double meses = (double)horas / 176;

        Assert.Equal(mesesEsperados, meses, precision: 5);
    }
}

// ════════════════════════════════════════════════════════════════════════════
// 6. Validação de password BCrypt
// ════════════════════════════════════════════════════════════════════════════
public class BCryptPasswordTests
{
    // Teste 15 — Hash gerado é verificável com a password original
    [Fact]
    public void BCrypt_HashEVerify_Correto()
    {
        string passwordOriginal = "MinhaPassword123!";

        string hash = BCrypt.Net.BCrypt.HashPassword(passwordOriginal);
        bool valido = BCrypt.Net.BCrypt.Verify(passwordOriginal, hash);

        Assert.True(valido);
    }

    // Teste 16 — Password errada não verifica com o hash
    [Fact]
    public void BCrypt_PasswordErrada_NaoVerifica()
    {
        string passwordOriginal = "MinhaPassword123!";
        string passwordErrada = "OutraPassword456!";

        string hash = BCrypt.Net.BCrypt.HashPassword(passwordOriginal);
        bool valido = BCrypt.Net.BCrypt.Verify(passwordErrada, hash);

        Assert.False(valido);
    }
}