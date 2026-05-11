using Microsoft.AspNetCore.Mvc;
using GestaoTalentos.Domain;
using GestaoTalentos.API.Services;

namespace GestaoTalentos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TalentosElegiveisController : ControllerBase
{
    private readonly ITalentoElegiveService _service;

    public TalentosElegiveisController(ITalentoElegiveService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TalentoElegivel>>> GetAll()
    {
        var talentos = await _service.GetAllTalentosElegiveisAsync();
        return Ok(talentos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TalentoElegivel>> GetById(int id)
    {
        var talento = await _service.GetTalentoElegiveByIdAsync(id);
        if (talento == null)
            return NotFound();
        return Ok(talento);
    }

    [HttpPost]
    public async Task<ActionResult<TalentoElegivel>> Create([FromBody] TalentoElegivel talento)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.CreateTalentoElegiveAsync(talento);
        return CreatedAtAction(nameof(GetById), new { id = talento.Id }, talento);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TalentoElegivel talento)
    {
        if (id != talento.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _service.GetTalentoElegiveByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _service.UpdateTalentoElegiveAsync(talento);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var talento = await _service.GetTalentoElegiveByIdAsync(id);
        if (talento == null)
            return NotFound();

        await _service.DeleteTalentoElegiveAsync(id);
        return NoContent();
    }
}