namespace BiblioTech2.Api.DTOs.Autores
{
    public class AutorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Nacionalidad { get; set; }
        public int CantidadLibros { get; set; }

    }
}