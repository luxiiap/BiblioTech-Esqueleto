using BiblioTech.Api.Exceptions;
using BiblioTech2.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Host;
using Microsoft.EntityFrameworkCore;
namespace BiblioTech2.Api.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private static readonly Dictionary<string, string> MensajeRestricciones = new()
    {
        ["FK_LibroAutor_Autores"] = "El autor libros asociados",
        ["UQ_Socios_Documento"] = "Ya existe un socio con este documento",
        ["UQ_Socios_Email"] = "Ya existe un socio con este correo",
        ["FK_Prestamos_Librros"] = "El libro tiene prestamos asociados",
        ["FK_Prestamos_Socios"] = "El socio tiene prestamos asociados",
        ["CK_Libros_Disponibles"] = "Los ejemplares no pueden ser negativos ni mayores que los totales",
        ["UQ_Libros_ISBN"] = "Ya existe un libro con el mismo ISBN",
        ["CK_Socios_Email"] = "El correo no tiene un formato valido",
    };

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await next(contexto);
        }
        catch (Exception ex) when (!contexto.Response.HasStarted)
        {
            await ResponderAsync(contexto, ex);
        }
    }

    private async Task ResponderAsync(HttpContext contexto, Exception ex)
    {
        var (codigo, titulo, detalle) = ex switch
        {
            NoEncontradoException e => (StatusCodes.Status404NotFound, "Recurso no encontrado", e.Message),
            ReglaNegocioException e => (StatusCodes.Status409Conflict, "Regla de negocio", e.Message),
            DbUpdateException e => (StatusCodes.Status409Conflict, "Conflicto con los datos", TraducirErrorBaseDatos(e)),
            _ => (StatusCodes.Status500InternalServerError, "Error interno",
                  "Ocurrió un error inesperado. Intenta de nuevo más tarde.")
        };

        if (codigo == StatusCodes.Status500InternalServerError)
            logger.LogError(ex, "Error no controlado en {Ruta}", contexto.Request.Path);
        else
            logger.LogWarning("{Codigo} en {Ruta}: {Mensaje}", codigo, contexto.Request.Path, detalle);

        var problema = new ProblemDetails
        {
            Status = codigo,
            Title = titulo,
            Detail = detalle,
            Instance = contexto.Request.Path
        };

        contexto.Response.StatusCode = codigo;
        await contexto.Response.WriteAsJsonAsync(problema, options: null, contentType:
            "application/problem+json");
    }

    private static string TraducirErrorBaseDatos(DbUpdateException ex)
    {
        var mensajeSql = ex.InnerException?.Message ?? ex.Message;
        foreach (var (restriccion, mensaje) in MensajeRestricciones)
        {
            if (mensajeSql.Contains(restriccion, StringComparison.OrdinalIgnoreCase))
                return mensaje;
        }
        return "La operacion viola una restriccion de la base de datos";
    }
}