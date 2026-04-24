using GestaoTalentos.API.Services;
using GestaoTalentos.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GestaoTalentos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropostasController : ControllerBase
{
    private readonly IPropostaService _service;

    public PropostasController(IPropostaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Proposta>>> GetAll()
    {
        var propostas = await _service.GetAllPropostasAsync();
        return Ok(propostas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Proposta>> GetById(int id)
    {
        var proposta = await _service.GetPropostaByIdAsync(id);
        if (proposta == null)
            return NotFound();
        return Ok(proposta);
    }

    [HttpPost]
    public async Task<ActionResult<Proposta>> Create([FromBody] Proposta proposta)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.CreatePropostaAsync(proposta);
        return CreatedAtAction(nameof(GetById), new { id = proposta.Id }, proposta);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Proposta proposta)
    {
        if (id != proposta.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _service.GetPropostaByIdAsync(id);
        if (existing == null)
            return NotFound();

        existing.Nome = proposta.Nome;
        existing.AreaId = proposta.AreaId;
        existing.DescricaoTrabalho = proposta.DescricaoTrabalho;
        existing.NumeroTotalHoras = proposta.NumeroTotalHoras;
        existing.PrecoHoraMedio = proposta.PrecoHoraMedio;
        existing.AtualizadoEm = DateTime.UtcNow;
        existing.SkillsNecessarias = proposta.SkillsNecessarias;

        await _service.UpdatePropostaAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var proposta = await _service.GetPropostaByIdAsync(id);
        if (proposta == null)
            return NotFound();

        await _service.DeletePropostaAsync(id);
        return NoContent();
    }
}