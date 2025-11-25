using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace TravelioREST.Autos;


public sealed class AutoReservaRequest
{
    public required string id_auto { get; set; }
    public required string id_hold { get; set; }
    public required string nombre { get; set; }
    public required string apellido { get; set; }
    public required string tipo_identificacion { get; set; }
    public required string identificacion { get; set; }
    public required string correo { get; set; }
    public DateTime fecha_inicio { get; set; }
    public DateTime fecha_fin { get; set; }
}

/*
 {
  "datos": {
    "mensaje": "Reserva creada correctamente",
    "id_reserva": 19,
    "id_hold": "14",
    "id_auto": "21",
    "nombre_titular": "Juan Pérez",
    "tipo_identificacion": "ID",
    "identificacion": "12",
    "correo": "jperez@correo.com",
    "vehiculo": "Mazda CX-5",
    "fecha_inicio": "2025-11-25T15:04:06.751Z",
    "fecha_fin": "2025-11-25T18:04:06.751Z",
    "total": 9.375,
    "estado": "Confirmada",
    "fecha_reserva": "2025-11-24T16:18:30.8342272+01:00"
  },
  "_links": [
    {
      "rel": "self",
      "href": "http://cuencautosinte.runasp.net/api/ReservaIntegracion",
      "method": "POST"
    },
    {
      "rel": "detalle_reserva",
      "href": "http://cuencautosinte.runasp.net/api/Reserva/19",
      "method": "GET"
    },
    {
      "rel": "detalle_hold",
      "href": "http://cuencautosinte.runasp.net/api/IntegracionHold?id_hold=14",
      "method": "GET"
    },
    {
      "rel": "detalle_auto",
      "href": "http://cuencautosinte.runasp.net/api/Autos?id_auto=21",
      "method": "GET"
    },
    {
      "rel": "emitir_factura",
      "href": "http://cuencautosinte.runasp.net/api/EmitirFactura",
      "method": "POST"
    }
  ]
}
 */

public sealed class ReservaResponse
{
    public required DatosReserva datos { get; set; }
    public required _Links[] _links { get; set; }
}

public sealed class DatosReserva
{
    public required string mensaje { get; set; }
    public required int id_reserva { get; set; }
    public required string id_hold { get; set; }
    public required string id_auto { get; set; }
    public required string nombre_titular { get; set; }
    public required string tipo_identificacion { get; set; }
    public required string identificacion { get; set; }
    public required string correo { get; set; }
    public required string vehiculo { get; set; }
    public DateTime fecha_inicio { get; set; }
    public DateTime fecha_fin { get; set; }
    public float total { get; set; }
    public required string estado { get; set; }
    public DateTime fecha_reserva { get; set; }
}

public sealed class _Links
{
    public required string rel { get; set; }
    public required string href { get; set; }
    public required string method { get; set; }
}

public static class AutosReservaCreador
{
    public static async Task<ReservaResponse> CrearReservaAsync(string url,
        string idAuto,
        string idHold,
        string nombre,
        string apellido,
        string tipoIdentificacion,
        string identificacion,
        string correo,
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        var request = new AutoReservaRequest()
        {
            id_auto = idAuto,
            id_hold = idHold,
            nombre = nombre,
            apellido = apellido,
            tipo_identificacion = tipoIdentificacion,
            identificacion = identificacion,
            correo = correo,
            fecha_inicio = fechaInicio,
            fecha_fin = fechaFin
        };
        var response = await Global.CachedHttpClient.PostAsJsonAsync(url, request);
        var reserva = await response.Content.ReadFromJsonAsync<ReservaResponse>();
        return reserva ?? throw new InvalidOperationException("No se pudo crear la reserva.");
    }
}
