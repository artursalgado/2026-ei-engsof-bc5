namespace DefaultNamespace;

public record PesquisaTalentosPorSkillsDto
{

}

//
// =========================
// PESQUISA DTOs
// =========================
//

public record TalentoPesquisaResultDto(
    int Id,
    int OwnerId,
    string Nome,
    string Email,
    string Pais,
    decimal PrecoPorHora,
    bool IsShared,
    List<SkillResumoDto> Skills
);

public record SkillResumoDto(
    int SkillId,
    string Nome,
    int AnosExperiencia
);

public record TalentoElegivelPropostaDto(
    int TalentoElegivelId,
    int PerfilId,
    string Nome,
    string Email,
    string Pais,
    decimal PrecoPorHora,
    decimal ValorEstimado,
    int NumeroTotalHoras,
    decimal PrecoHoraMedioProposta
);