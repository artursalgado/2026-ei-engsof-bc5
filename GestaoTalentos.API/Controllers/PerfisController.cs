using Microsoft.AspNetCore.Mvc;
using GestaoTalentos.Domain;
using GestaoTalentos.API.Services;

namespace GestaoTalentos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerfisController : ControllerBase
{
    private readonly IPerfilService _service;

    public PerfisController(IPerfilService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Perfil>>> GetAll()
    {
        var perfis = await _service.GetAllPerfisAsync();
        return Ok(perfis);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Perfil>> GetById(int id)
    {
        var perfil = await _service.GetPerfilByIdAsync(id);
        if (perfil == null)
            return NotFound();
        return Ok(perfil);
    }

    [HttpPost]
    public async Task<ActionResult<Perfil>> Create([FromBody] Perfil perfil)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.CreatePerfilAsync(perfil);
        return CreatedAtAction(nameof(GetById), new { id = perfil.Id }, perfil);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Perfil perfil)
    {
        if (id != perfil.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _service.GetPerfilByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _service.UpdatePerfilAsync(perfil);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var perfil = await _service.GetPerfilByIdAsync(id);
        if (perfil == null)
            return NotFound();

        await _service.DeletePerfilAsync(id);
        return NoContent();
    }
}