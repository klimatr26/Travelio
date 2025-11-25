using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Web;
using TravelioSOAP.Aerolinea;
using TravelioREST.Aerolinea;
using static TravelioAPIConnector.Global;
using System.Runtime.InteropServices;

namespace TravelioAPIConnector.Aerolinea;

#pragma warning disable CS0162 // Se detectó código inaccesible
public static class Connector
{
    public static async Task<Vuelo[]> GetVuelosAsync(
        string uri,
        string? origin = null,
        string? destination = null,
        DateTime? dateFrom = null,
        string? cabin = null,
        int? passengers = null,
        decimal? priceMin = null,
        decimal? priceMax = null)
    {
        if (IsREST)
        {
            var uriBuilder = new UriBuilder(uri);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (origin is not null)
                query["origin"] = origin;

            if (destination is not null)
                query["destination"] = destination;

            if (dateFrom is not null)
                query["dateFrom"] = dateFrom.ToString();

            if (cabin is not null)
                query["cabin"] = cabin;

            if (passengers is not null)
                query["passengers"] = passengers.ToString();

            if (priceMin is not null)
                query["priceMin"] = priceMin.ToString();

            if (priceMax is not null)
                query["priceMax"] = priceMin.ToString();

            uriBuilder.Query = query.ToString();

            var vuelos = await VuelosGetter.GetVuelosAsync(uriBuilder.ToString());

            if (vuelos is not null)
            {
                var vuelosResult = new Vuelo[vuelos.Length];
                for (int i = 0; i < vuelos.Length; i++)
                {
                    var v = vuelos[i];
                    vuelosResult[i] = new Vuelo
                    {
                        IdVuelo = v.IdVuelo,
                        Origen = v.Origen,
                        Destino = v.Destino,
                        FechaSalida = v.FechaSalida,
                        FechaLlegada = v.FechaLlegada,
                        TipoCabina = v.TipoCabina,
                        NombreAerolinea = v.NombreAerolinea,
                        PrecioNormal = v.PrecioNormal,
                        PrecioActual = v.PrecioActual,
                        DescuentoPorcentaje = (1.0m - v.PrecioActual / v.PrecioNormal) * 100.0m,
                        CapacidadDisponible = v.CapacidadDisponible
                    };
                }
                return vuelosResult;
            }
            return [];
        }
        else
        {
            var soapClient = new WS_IntegracionSoapClient(GetBinding(uri), new EndpointAddress(uri));
            var vueloSoap = await soapClient!.buscarVuelosAsync(origin, destination, dateFrom, cabin,
                                                               passengers, priceMin, priceMax, null);
            var vuelosResult = new Vuelo[vueloSoap.Body.buscarVuelosResult.Length];
            for (int i = 0; i < vueloSoap.Body.buscarVuelosResult.Length; i++)
            {
                var v = vueloSoap.Body.buscarVuelosResult[i];
                vuelosResult[i] = new Vuelo
                {
                    IdVuelo = v.FlightId,
                    Origen = v.OriginName,
                    Destino = v.DestinationName,
                    FechaSalida = v.DepartureTime,
                    FechaLlegada = v.ArrivalTime,
                    TipoCabina = v.CabinClass,
                    NombreAerolinea = v.Airline,
                    PrecioNormal = v.Price,
                    PrecioActual = v.Price,
                    DescuentoPorcentaje = 0.0m,
                    CapacidadDisponible = v.SeatsAvailable
                };
            }
            return vuelosResult;
        }
    }

    public static async Task<bool> VerificarDisponibilidadVueloAsync(string uri, int idVuelo, int numPasajeros)
    {
        if (IsREST)
        {
            return await VueloCheckAvailable.GetDisponibilidadAsync(uri, idVuelo, numPasajeros);
        }
        else
        {
            var soapClient = new WS_IntegracionSoapClient(GetBinding(uri), new EndpointAddress(uri));
            var disponibilidadResponse = await soapClient.validarDisponibilidadVueloAsync(idVuelo, numPasajeros);
            return disponibilidadResponse;
        }
    }

    public static async Task<(string holdId, DateTime holdExpiration)> CrearPrerreservaVueloAsync(
        string uri,
        int idVuelo,
        int numPasajeros,
        int duracionHold = 300)
    {
        string[] asientos = new string[numPasajeros];

        for (int i = 0; i < numPasajeros; i++)
        {
            asientos[i] = $"E{i + 1}";
        }

        return await CrearPrerreservaVueloAsync(uri, idVuelo, asientos, duracionHold);
    }

    public static async Task<(string holdId, DateTime holdExpiration)> CrearPrerreservaVueloAsync(string uri,
        int idVuelo,
        string[] asientos,
        int duracionHold = 300)
    {
        if (IsREST)
        {
            var prerreserva = await HoldCreator.CreateHoldAsync(uri, idVuelo, asientos, duracionHold);

            return (prerreserva.holdId, DateTime.Parse(prerreserva.expiraEn));
        }
        else
        {
            var soapClient = new WS_IntegracionSoapClient(GetBinding(uri), new EndpointAddress(uri));
            var strings = new ArrayOfString();
            strings.AddRange(asientos);
            var prereservaResponse = await soapClient!.crearPreReservaVueloAsync(idVuelo, strings, duracionHold);
            return (prereservaResponse.Body.crearPreReservaVueloResult.HoldId, prereservaResponse.Body.crearPreReservaVueloResult.ExpiraEn);
        }
    }

    public static async Task<(int numeroReserva, string codigoReserva, string estado)> CrearReservaAsync(string uri,
        int idVuelo,
        string holdId,
        string correo,
        (string nombre, string apellido, string tipoIdentificacion, string identificacion)[] pasajeros)
    {
        if (IsREST)
        {
            var reservaResponse = await ReservationCreator.CreateReservationAsync(
                uri,
                idVuelo,
                holdId,
                correo,
                pasajeros);

            return (reservaResponse.IdReserva,
                    reservaResponse.CodigoReserva,
                    reservaResponse.Estado);
        }
        else
        {
            var soapClient = new WS_IntegracionSoapClient(GetBinding(uri), new EndpointAddress(uri));

            var pasajerosDTO = new DTOPasajero[pasajeros.Length];
            for (int i = 0; i < pasajeros.Length; i++)
            {
                var p = pasajeros[i];
                pasajerosDTO[i] = new DTOPasajero
                {
                    Nombre = p.nombre,
                    Apellido = p.apellido,
                    TipoIdentificacion = p.tipoIdentificacion,
                    Identificacion = p.identificacion
                };
            }

            var reservaResponse = await soapClient.reservarVueloAsync(
                idVuelo,
                holdId,
                pasajerosDTO,
                correo);

            return (reservaResponse.Body.reservarVueloResult.IdReserva,
                    reservaResponse.Body.reservarVueloResult.CodigoReserva,
                    reservaResponse.Body.reservarVueloResult.Estado);
        }
    }

    public static async Task<string> GenerarFacturaAsync(string uri,
        int reservaId,
        decimal subtotal,
        decimal iva,
        decimal total,
        (string nombre, string documento, string correo) cliente)
    {
        if (IsREST)
        {
            var facturaResponse = await InvoiceGenerator.GenerateInvoiceAsync(
                uri,
                reservaId,
                subtotal,
                iva,
                total,
                cliente);
            return facturaResponse.UriFactura;
        }
        else
        {
            var soapClient = new WS_IntegracionSoapClient(GetBinding(uri), new EndpointAddress(uri));
            var dtoFacturaRequest = new DTOFacturaRequest
            {
                ReservaId = reservaId,
                Subtotal = subtotal,
                Iva = iva,
                Total = total,
                Cliente = new DTOClienteFactura
                {
                    Nombre = cliente.nombre,
                    Documento = cliente.documento,
                    Correo = cliente.correo
                }
            };
            var facturaResponse = await soapClient.emitirFacturaVueloAsync(dtoFacturaRequest);
            return facturaResponse.Body.emitirFacturaVueloResult.UriFactura;
        }
    }

    public static async Task<int> CrearClienteExternoAsync(string uri, string nombre, string apellido, string correo)
    {
        if (IsREST)
        {
            var cliente = await ExternalClientCreator.CreateExternalClientAsync(uri, nombre, apellido, correo);
            return cliente.IdUsuario;
        }
        else
        {
            var soapClient = new WS_IntegracionSoapClient(GetBinding(uri), new EndpointAddress(uri));
            var dtoUsuario = new DTOUsuarioExterno
            {
                Nombre = nombre,
                Apellido = apellido,
                Correo = correo
            };
            var clienteResponse = await soapClient.crearUsuarioExternoAsync(dtoUsuario);
            return clienteResponse.Body.crearUsuarioExternoResult.IdUsuario;
        }
    }

    public static async Task<Reserva> GetDatosReservaAsync(string uri, int reservaId)
    {
        if (IsREST)
        {
            var uriBuilder = new UriBuilder(uri);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query["idReserva"] = reservaId.ToString();

            uriBuilder.Query = query.ToString();

            string result = await ReservationDataGetter.GetBookingDataContent(uriBuilder.ToString());

            Reserva resultReserva = default;

            return resultReserva with { Origen = result };

            /*

            var reservaResponse = await ReservationDataGetter.GetReservationDataAsync(uri, reservaId);
            return new Reserva
            {
                Origen = reservaResponse.Origen,
                Destino = reservaResponse.Destino,
                CorreoAsignado = reservaResponse.CorreoAsignado,
                FechaSalida = reservaResponse.FechaSalida,
                TipoCabina = reservaResponse.TipoCabina,
                Pasajeros = Array.ConvertAll(reservaResponse.Pasajeros, p => (p.Nombre, p.Apellido, p.TipoIdentificacion, p.Identificacion)),
                ValorPagado = reservaResponse.ValorPagado,
                UriFactura = reservaResponse.UriFactura
            };
            */
        }
        else
        {
            var soapClient = new WS_IntegracionSoapClient(GetBinding(uri), new EndpointAddress(uri));
            var reservaResponse = await soapClient.buscarDatosReservaAsync(reservaId);
            var pasajeros = Array.ConvertAll(reservaResponse.Body.buscarDatosReservaResult.Pasajeros, p => (p.Nombre, p.Apellido, p.TipoIdentificacion, p.Identificacion));
            return new Reserva
            {
                Origen = reservaResponse.Body.buscarDatosReservaResult.Origen,
                Destino = reservaResponse.Body.buscarDatosReservaResult.Destino,
                CorreoAsignado = "",
                FechaSalida = reservaResponse.Body.buscarDatosReservaResult.Fecha,
                TipoCabina = reservaResponse.Body.buscarDatosReservaResult.TipoCabina,
                Pasajeros = pasajeros,
                ValorPagado = 0,
                UriFactura = ""
            };
        }
    }
}

#pragma warning restore CS0162 // Se detectó código inaccesible
