using GestaoTalentos.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestaoTalentos.Infrastructure;

public class PropostaMatchingService
{
    private readonly AppDbContext _context;

    public PropostaMatchingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TalentoElegivel>> IdentificarTalentosElegiveisAsync(int propostaId, decimal precoHoraMedio)
    {
        var proposta = await _context.Propostas
            .Include(p => p.SkillsNecessarias)
            .ThenInclude(sn => sn.Skill)
            .FirstOrDefaultAsync(p => p.Id == propostaId);

        if (proposta == null)
            throw new Exception("Proposta não encontrada");

        var skillsNecessarias = proposta.SkillsNecessarias.ToList();

        if (!skillsNecessarias.Any())
            return new List<TalentoElegivel>();

        var perfis = await _context.Set<Perfil>().ToListAsync();

        var talentosElegiveis = new List<TalentoElegivel>();

        foreach (var perfil in perfis)
        {
            if (VerificarSePerfilAtendeRequisitos(perfil, skillsNecessarias))
            {
                talentosElegiveis.Add(new TalentoElegivel
                {
                    PropostaId = propostaId,
                    PerfilId = perfil.Id,
                    ValorEstimado = proposta.NumeroTotalHoras * precoHoraMedio,
                    CriadoEm = DateTime.UtcNow
                });
            }
        }

        return talentosElegiveis.OrderBy(te => te.ValorEstimado).ToList();
    }

    
    private static bool VerificarSePerfilAtendeRequisitos(Perfil perfil, List<SkillNecessaria> skillsNecessarias)
    {
        var content = perfil.Content ?? string.Empty;

        foreach (var skillNecessaria in skillsNecessarias)
        {
            var nomeSkill = skillNecessaria.Skill?.Nome;

            // Skill sem nome carregado — não é possível validar, rejeitar o perfil
            if (string.IsNullOrWhiteSpace(nomeSkill))
                return false;

            // O Content do perfil tem de mencionar a skill (case-insensitive)
            if (!content.Contains(nomeSkill, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }
}
