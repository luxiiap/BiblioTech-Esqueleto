using BiblioTech2.Api.DTOs.Autores;
using BiblioTech2.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace BiblioTech2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutoresControllers(IAutorService servicio) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AutorDto>>>Listar()
    
        => Ok(await servicio.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AutorDto>> Obtener(int id)
    {
        var autor = await servicio.ObtenerAsync(id);
        return autor is null ? NotFound() : Ok(autor);
    }

    [HttpPost]
    public async Task<ActionResult<AutorDto>> Crear(AutorGuardarDto dto)
    {
        var creado = await servicio.CrearAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, AutorGuardarDto dto)
    
         => await servicio.ActualizarAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)

        => await servicio.EliminarAsync(id) ? NoContent() : NotFound();
}
