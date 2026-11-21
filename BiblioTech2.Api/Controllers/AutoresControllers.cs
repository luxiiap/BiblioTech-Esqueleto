using BiblioTech2.Api.DTOs.Autores;
using BiblioTech2.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BiblioTech2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AutoresController(IAutorService servicio) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AutorDto>>> Listar()
        => Ok(await servicio.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AutorDto>> obtener(int id)
    {
        var autor = await servicio.ObtenerAsync(id);
        return autor is null ? NotFound() : Ok(autor);
    }

    [HttpPost]
    public async Task<ActionResult<AutorDto>> CrearAsync(AutorGuardarDto dto)
    {
        var creado = await servicio.CrearAsync(dto);
        return CreatedAtAction(nameof(obtener), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, AutorGuardarDto dto)
    {
        await servicio.ActualizarAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await servicio.EliminarAsync(id);
        return NoContent();
    }
}
