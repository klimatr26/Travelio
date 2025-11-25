using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TravelioAPIConnector;
using TravelioAPIConnector.Aerolinea;
using TravelioREST.Aerolinea;

namespace TravelioTestConsoleApp.Aerolinea;

internal static class ConnectionTest
{
    public static async Task TestBasicGetConnection()
    {
        Console.WriteLine($"Probando {(Global.IsREST ? "REST" : "SOAP")}");

        const string restGetFlightsUri = @"http://skyandes.runasp.net/api/integracion/aerolinea/search";
        const string soapGetFlightsUri = @"https://skyandesintegracion.runasp.net/WS_Integracion.asmx?WSDL";

        const string getFlightsUri = Global.IsREST ? restGetFlightsUri : soapGetFlightsUri;

        var vuelos = await Connector.GetVuelosAsync(getFlightsUri);

        if (vuelos.Length == 0)
        {
            Console.WriteLine("No se encontraron vuelos.");
            return;
        }

        foreach (var v in vuelos) Console.WriteLine(v);

        Console.WriteLine("\nVuelos disponibles:");

        const string restGetAvailableUri = @"http://skyandes.runasp.net/api/integracion/aerolinea/availability";
        const string soapGetAvailableUri = @"https://skyandesintegracion.runasp.net/WS_Integracion.asmx?WSDL";

        const string getAvailableUri = Global.IsREST ? restGetAvailableUri : soapGetAvailableUri;

        Console.WriteLine($"Vuelo {vuelos[0].IdVuelo}: {await Connector.VerificarDisponibilidadVueloAsync(getAvailableUri, vuelos[0].IdVuelo, 2)}");

        Console.WriteLine($"Vuelo 123456: {await Connector.VerificarDisponibilidadVueloAsync(getAvailableUri, 123456, 2)}");

        const string restCreatePreReservationUri = @" http://skyandes.runasp.net/api/integracion/aerolinea/hold";
        const string soapCreatePreReservationUri = @"https://skyandesintegracion.runasp.net/WS_Integracion.asmx?WSDL";

        const string createPreReservationUri = Global.IsREST ? restCreatePreReservationUri : soapCreatePreReservationUri;

        var (holdId, holdExpiration) = await Connector.CrearPrerreservaVueloAsync(createPreReservationUri, vuelos[0].IdVuelo, 2);

        Console.WriteLine($"Creada prerreserva con id '{holdId}', que expira el {holdExpiration}");

        const string restCreateReservationUri = @"https://skyandes.runasp.net/api/integracion/aerolinea/book";
        const string soapCreateReservationUri = @"https://skyandesintegracion.runasp.net/WS_Integracion.asmx?WSDL";

        const string createReservationUri = Global.IsREST ? restCreateReservationUri : soapCreateReservationUri;

        var pasajeros = new (string nombre, string apellido, string tipoIdentificacion, string identificacion)[]
        {
            ("Juan", "Pérez", "DNI", "12345678"),
            ("María", "Gómez", "DNI", "87654321")
        };

        var (numeroReserva, codigoReserva, estado) = await Connector.CrearReservaAsync(createReservationUri, vuelos[0].IdVuelo, holdId, "correo@correo.com", pasajeros);

        Console.WriteLine($"Reserva creada: Número {numeroReserva}, Código {codigoReserva}, Estado {estado}");

        const string restCreateClientUri = @"http://skyandes.runasp.net/api/integracion/aerolinea/usuarios/externo";
        const string soapCreateClientUri = @"https://skyandesintegracion.runasp.net/WS_Integracion.asmx?WSDL";

        const string createClientUri = Global.IsREST ? restCreateClientUri : soapCreateClientUri;

        var clienteId = await Connector.CrearClienteExternoAsync(createClientUri, "Carlos", "López", "correo@correo.com");

        Console.WriteLine($"Cliente externo creado con ID: {clienteId}");

        const string restCreateInvoiceUri = @"http://skyandes.runasp.net/api/integracion/aerolinea/invoices";
        const string soapCreateInvoiceUri = @"https://skyandesintegracion.runasp.net/WS_Integracion.asmx?WSDL";

        const string createInvoiceUri = Global.IsREST ? restCreateInvoiceUri : soapCreateInvoiceUri;

        var facturaUrl = await Connector.GenerarFacturaAsync(createInvoiceUri, numeroReserva, 1000m, 120m, 1120m, ("Carlos López", "DNI 12345678", "correo@correo.com"));
        Console.WriteLine($"Factura generada. Puede descargarse en: {facturaUrl}");

        const string restGetReservationDataUri = @"http://skyandes.runasp.net/api/integracion/aerolinea/reserva";
        const string soapGetReservationDataUri = @"https://skyandesintegracion.runasp.net/WS_Integracion.asmx?WSDL";

        const string getReservationDataUri = Global.IsREST ? restGetReservationDataUri : soapGetReservationDataUri;

        var reserva = await Connector.GetDatosReservaAsync(getReservationDataUri, numeroReserva);
        Console.WriteLine($"Datos de la reserva {numeroReserva}: {reserva}");
    }
}
