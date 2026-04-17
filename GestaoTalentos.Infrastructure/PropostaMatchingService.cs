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

    /// <summary>
    /// Identifica automaticamente todos os talentos elegíveis para uma proposta
    /// </summary>
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

        // Obter todos os perfis com suas skills
        var perfis = await _context.Set<Perfil>()
            .ToListAsync();

        var talentosElegiveis = new List<TalentoElegivel>();

        // Este é um exemplo simplificado - você precisará expandir conforme sua estrutura de Perfil
        // Aqui assumimos que Perfil tem relacionamento com Skill
        // NOTA: Você precisa adicionar um relacionamento Perfil -> Skills no seu modelo

        foreach (var perfil in perfis)
        {
            // Verificar se o perfil tem todas as skills necessárias com experiência mínima
            bool atendeTodosRequsitos = await VerificarSePerfilAtendeRequisitosAsync(perfil.Id, skillsNecessarias);

            if (atendeTodosRequsitos)
            {
                var talentoElegivel = new TalentoElegivel
                {
                    PropostaId = propostaId,
                    PerfilId = perfil.Id,
                    ValorEstimado = proposta.NumeroTotalHoras * precoHoraMedio,
                    CriadoEm = DateTime.UtcNow
                };

                talentosElegiveis.Add(talentoElegivel);
            }
        }

        // Ordenar por valor estimado (crescente)
        return talentosElegiveis.OrderBy(te => te.ValorEstimado).ToList();
    }

    private async Task<bool> VerificarSePerfilAtendeRequisitosAsync(int perfilId, List<SkillNecessaria> skillsNecessarias)
    {
        // TODO: Implementar verificação conforme sua estrutura de Perfil
        // Por agora, retorna true para todos (você precisa expandir isto)
        
        // PSEUDOCÓDIGO:
        // Para cada skill necessária:
        //   - Verificar se o perfil tem essa skill
        //   - Verificar se o perfil tem >= anos de experiência necessários
        //   - Se alguma skill faltar, retornar false
        // Se todas as skills forem atendidas, retornar true

        return await Task.FromResult(true); // Placeholder
    }
}