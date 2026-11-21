using BiblioTech2.Api.Data;
using BiblioTech2.Api.DTOs.Autores;
using BiblioTech2.Api.Exceptions;
using BiblioTech2.Api.Models;
using BiblioTech2.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace BiblioTech2.Api.Services;


public class AutorService(BibliotecaContext db) : IAutorService
{
    public async Task<List<AutorDto>> ListarAsync()
    {
        return await db.Autores
            .OrderBy(a => a.Nombre)
            .Select(a => new AutorDto
            {
                Id = a.Id,
                Nombre = a.Nombre,
                Nacionalidad = a.Nacionalidad,
                CantidadLibros = a.Libros.Count
            })
            .ToListAsync();
    }
    public async Task<AutorDto> ObtenerAsync(int id)
    {
        return await db.Autores
            .Where(a => a.Id == id)
            .Select(a => new AutorDto
            {
                Id = a.Id,
                Nombre = a.Nombre,
                Nacionalidad = a.Nacionalidad,
                CantidadLibros = a.Libros.Count
            })
            .FirstOrDefaultAsync()
            ?? throw new NoEncontradoException($"El autor {id} no exite.");
    }

    public async Task<AutorDto> CrearAsync(AutorGuardarDto dto)
    {
        var autor = new Autor
        {
            Nombre = dto.Nombre.Trim(),
            Nacionalidad = string.IsNullOrWhiteSpace(dto.Nacionalidad) ? null : dto.Nacionalidad.Trim()
        };
        db.Autores.Add(autor);
        await db.SaveChangesAsync();
        return new AutorDto { Id = autor.Id, Nombre = autor.Nombre, Nacionalidad = autor.Nacionalidad };
    }

    public async Task ActualizarAsync(int id, AutorGuardarDto dto)
    {
        var autor = await db.Autores.FindAsync(id)
            ?? throw new NoEncontradoException($"El autor {id} no existe");

        autor.Nombre = dto.Nombre.Trim();
        autor.Nacionalidad = string.IsNullOrWhiteSpace(dto.Nacionalidad) ? null : dto.Nacionalidad.Trim();

        await db.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var autor = await db.Autores.FindAsync(id)
           ?? throw new NoEncontradoException($"El autor {id} no existe");

        var cantidadLibros = await db.Libros.CountAsync(l => l.Autores.Any(a => a.Id == id));
        if (cantidadLibros > 0)

            db.Autores.Remove(autor);
        await db.SaveChangesAsync();
    }
}