using TravelioDatabaseConnector.Enums;

namespace TravelioDatabaseConnector.Models;

public class Servicio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public TipoServicio TipoServicio { get; set; }
    public string NumeroCuenta { get; set; } = default!;
    public bool Activo { get; set; }

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public DetalleServicio? DetalleServicio { get; set; }
}
