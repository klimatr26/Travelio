using System;
using System.Collections.Generic;
using System.Text;

namespace TravelioAPIConnector.Aerolinea;

public record struct Reserva(
    string Origen,
    string Destino,
    string CorreoAsignado,
    DateTime FechaSalida,
    string TipoCabina,
    (string nombre, string apellido, string tipoIdentificacion, string identificacion)[] Pasajeros,
    decimal ValorPagado,
    string UriFactura
    );
