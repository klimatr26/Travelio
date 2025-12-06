namespace TravelioDatabaseConnector.Models;

public class Cliente
{
    public int Id { get; set; }
    public string CorreoElectronico { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string Apellido { get; set; } = default!;
    public string? Pais { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public string? Telefono { get; set; }
    public string TipoIdentificacion { get; set; } = default!;
    public string DocumentoIdentidad { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string PasswordSalt { get; set; } = default!;

    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
