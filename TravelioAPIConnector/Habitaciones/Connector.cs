using System;
using System.ServiceModel;
using TravelioSOAP.Habitaciones.BuscarDatosReserva;
using TravelioSOAP.Habitaciones.BuscarHabitaciones;
using TravelioSOAP.Habitaciones.CrearPrerreserva;
using TravelioSOAP.Habitaciones.CrearUsuarioExterno;
using TravelioSOAP.Habitaciones.EmitirFacturaHotel;
using TravelioSOAP.Habitaciones.ReservarHabitacion;
using TravelioSOAP.Habitaciones.ValidarDisponibilidad;
using static TravelioAPIConnector.Global;

namespace TravelioAPIConnector.Habitaciones;

#pragma warning disable CS0162
public static class Connector
{
    public static async Task<Habitacion[]> BuscarHabitacionesAsync(
        string uri,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        string? tipoHabitacion = null,
        int? capacidad = null,
        decimal? precioMin = null,
        decimal? precioMax = null)
    {
        if (IsREST)
        {
            throw new NotImplementedException("La integracion REST para habitaciones aun no esta disponible.");
        }

        var soapClient = new BuscarHabitacionesWSSoapClient(GetBinding(uri), new EndpointAddress(uri));
        var response = await soapClient.buscarHabitacionesAsync(fechaInicio, fechaFin, tipoHabitacion, capacidad, precioMin, precioMax);
        var habitacionesDto = response.Body.buscarHabitacionesResult ?? [];

        return Array.ConvertAll(habitacionesDto, MapHabitacion);
    }

    public static async Task<bool> ValidarDisponibilidadAsync(string uri, string idHabitacion, DateTime fechaInicio, DateTime fechaFin)
    {
        if (IsREST)
        {
            throw new NotImplementedException("La integracion REST para habitaciones aun no esta disponible.");
        }

        var soapClient = new ValidarDisponibilidadWSSoapClient(GetBinding(uri), new EndpointAddress(uri));
        var response = await soapClient.validarDisponibilidadHabitacionAsync(idHabitacion, fechaInicio, fechaFin);
        return response.Body.validarDisponibilidadHabitacionResult.Disponible;
    }

    public static async Task<string> CrearPrerreservaAsync(
        string uri,
        string idHabitacion,
        DateTime fechaInicio,
        DateTime fechaFin,
        int numeroHuespedes,
        int? duracionHoldSegundos = null,
        decimal? precioActual = null)
    {
        if (IsREST)
        {
            throw new NotImplementedException("La integracion REST para habitaciones aun no esta disponible.");
        }

        var soapClient = new CrearPreReservaWSSoapClient(GetBinding(uri), new EndpointAddress(uri));
        var response = await soapClient.crearPreReservaHabitacionAsync(idHabitacion, fechaInicio, fechaFin, numeroHuespedes, duracionHoldSegundos, precioActual);
        var prerreserva = response.Body.crearPreReservaHabitacionResult;
        return prerreserva?.IdHold ?? throw new InvalidOperationException("No se pudo crear la prerreserva de habitacion.");
    }

    public static async Task<int> CrearUsuarioExternoAsync(string uri, string correo, string nombre, string apellido)
    {
        if (IsREST)
        {
            throw new NotImplementedException("La integracion REST para habitaciones aun no esta disponible.");
        }

        var soapClient = new CrearUsuarioExternoWSSoapClient(GetBinding(uri), new EndpointAddress(uri));
        var response = await soapClient.crearUsuarioExternoAsync(correo, nombre, apellido);
        return response.Body.crearUsuarioExternoResult?.Id ?? throw new InvalidOperationException("No se pudo crear el usuario externo.");
    }

    public static async Task<int> CrearReservaAsync(
        string uri,
        string idHabitacion,
        string idHold,
        string nombre,
        string apellido,
        string correo,
        string tipoDocumento,
        string documento,
        DateTime fechaInicio,
        DateTime fechaFin,
        int numeroHuespedes)
    {
        if (IsREST)
        {
            throw new NotImplementedException("La integracion REST para habitaciones aun no esta disponible.");
        }

        var soapClient = new ReservarHabitacionWSSoapClient(GetBinding(uri), new EndpointAddress(uri));
        var response = await soapClient.reservarHabitacionAsync(idHabitacion, idHold, nombre, apellido, correo, tipoDocumento, documento, fechaInicio, fechaFin, numeroHuespedes);
        return response.Body.reservarHabitacionResult?.IdReserva ?? throw new InvalidOperationException("No se pudo crear la reserva de habitacion.");
    }

    public static async Task<string> EmitirFacturaAsync(
        string uri,
        int idReserva,
        string nombre,
        string apellido,
        string tipoDocumento,
        string documento,
        string correo)
    {
        if (IsREST)
        {
            throw new NotImplementedException("La integracion REST para habitaciones aun no esta disponible.");
        }

        var soapClient = new EmitirFacturaHotelWSSoapClient(GetBinding(uri), new EndpointAddress(uri));
        var response = await soapClient.emitirFacturaHotelAsync(idReserva, nombre, apellido, tipoDocumento, documento, correo);
        return response.Body.emitirFacturaHotelResult?.UrlPdf ?? throw new InvalidOperationException("No se pudo emitir la factura.");
    }

    public static async Task<Reserva> ObtenerDatosReservaAsync(string uri, int idReserva)
    {
        if (IsREST)
        {
            throw new NotImplementedException("La integracion REST para habitaciones aun no esta disponible.");
        }

        var soapClient = new BuscarDatosReservaWSSoapClient(GetBinding(uri), new EndpointAddress(uri));
        var response = await soapClient.buscarDatosReservaAsync(idReserva);
        var datos = response.Body.buscarDatosReservaResult ?? throw new InvalidOperationException("No se pudieron obtener los datos de la reserva.");

        return new Reserva(
            datos.IdReserva,
            datos.CostoTotal ?? 0m,
            datos.FechaRegistro ?? DateTime.MinValue,
            datos.Inicio ?? DateTime.MinValue,
            datos.Fin ?? DateTime.MinValue,
            datos.EstadoGeneral ?? string.Empty,
            datos.Nombre ?? string.Empty,
            datos.Apellido ?? string.Empty,
            datos.Correo ?? string.Empty,
            datos.IdHabitacion ?? string.Empty,
            datos.NombreHabitacion ?? string.Empty,
            datos.TipoHabitacion ?? string.Empty,
            datos.Hotel ?? string.Empty,
            datos.Ciudad ?? string.Empty,
            datos.Pais ?? string.Empty,
            datos.CapacidadReserva ?? 0,
            datos.CostoCalculado ?? 0m,
            datos.Descuento ?? 0m,
            datos.Impuestos ?? 0m,
            datos.IdHold ?? string.Empty,
            datos.Amenidades ?? string.Empty,
            datos.Imagenes?.Split('|') ?? [],
            datos.UrlPdf ?? string.Empty);
    }

    private static Habitacion MapHabitacion(HabitacionListItemDto dto)
    {
        var precioActual = dto.PrecioActual ?? dto.PrecioVigente ?? dto.PrecioNormal ?? 0m;
        var precioVigente = dto.PrecioVigente ?? dto.PrecioActual ?? dto.PrecioNormal ?? 0m;

        return new Habitacion(
            dto.IdHabitacion ?? string.Empty,
            dto.NombreHabitacion ?? string.Empty,
            dto.TipoHabitacion ?? string.Empty,
            dto.NombreHotel ?? string.Empty,
            dto.NombreCiudad ?? string.Empty,
            dto.NombrePais ?? string.Empty,
            dto.Capacidad ?? 0,
            dto.PrecioNormal ?? 0m,
            precioActual,
            precioVigente,
            dto.Amenidades ?? string.Empty,
            dto.Imagenes?.Split('|') ?? []);
    }
}
#pragma warning restore CS0162
