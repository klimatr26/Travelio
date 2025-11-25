using System;
using System.Collections.Generic;
using System.Text;

namespace TravelioAPIConnector.Aerolinea;

public record struct Vuelo(
    int IdVuelo,
    string Origen,
    string Destino,
    DateTime FechaSalida,
    DateTime FechaLlegada,
    string TipoCabina,
    string NombreAerolinea,
    decimal PrecioNormal,
    decimal PrecioActual,
    decimal DescuentoPorcentaje,
    int CapacidadDisponible
    );
    
