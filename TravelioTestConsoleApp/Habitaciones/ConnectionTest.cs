using System;
using TravelioAPIConnector;
using TravelioAPIConnector.Habitaciones;

namespace TravelioTestConsoleApp.Habitaciones;

internal static class ConnectionTest
{
    public static async Task TestBasicConnectionAsync()
    {
        Console.WriteLine($"Probando {(Global.IsREST ? "REST" : "SOAP")}");

        //const string soapBuscarHabitacionesUri = @"http://aureacuen.runasp.net/buscarHabitacionesWS.asmx?WSDL";
        //const string soapValidarDisponibilidadUri = @"http://aureacuen.runasp.net/ValidarDisponibilidadWS.asmx?WSDL";
        //const string soapCrearPreReservaUri = @"http://aureacuen.runasp.net/CrearPreReservaWS.asmx?WSDL";
        //const string soapCrearUsuarioExternoUri = @"http://aureacuen.runasp.net/CrearUsuarioExternoWS.asmx?WSDL";
        //const string soapReservarHabitacionUri = @"http://aureacuen.runasp.net/ReservarHabitacionWS.asmx?WSDL";
        //const string soapEmitirFacturaUri = @"http://aureacuen.runasp.net/EmitirFacturaHotelWS.asmx?WSDL";
        //const string soapBuscarDatosReservaUri = @"http://aureacuen.runasp.net/buscarDatosReservaWS.asmx?WSDL";

        const string soapBuscarHabitacionesUri = @"https://intehoca-eheqd8h6bvdyfqfy.canadacentral-01.azurewebsites.net/buscarHabitacionesWS.asmx?WSDL";
        const string soapValidarDisponibilidadUri = @"https://intehoca-eheqd8h6bvdyfqfy.canadacentral-01.azurewebsites.net/ValidarDisponibilidadWS.asmx?WSDL";
        const string soapCrearPreReservaUri = @"https://intehoca-eheqd8h6bvdyfqfy.canadacentral-01.azurewebsites.net/CrearPreReservaWS.asmx?WSDL";
        const string soapCrearUsuarioExternoUri = @"https://intehoca-eheqd8h6bvdyfqfy.canadacentral-01.azurewebsites.net/CrearUsuarioExternoWS.asmx?WSDL";
        const string soapReservarHabitacionUri = @"https://intehoca-eheqd8h6bvdyfqfy.canadacentral-01.azurewebsites.net/ReservarHabitacionWS.asmx?WSDL";
        const string soapEmitirFacturaUri = @"https://intehoca-eheqd8h6bvdyfqfy.canadacentral-01.azurewebsites.net/EmitirFacturaHotelWS.asmx?WSDL";
        const string soapBuscarDatosReservaUri = @"https://intehoca-eheqd8h6bvdyfqfy.canadacentral-01.azurewebsites.net/buscarDatosReservaWS.asmx?WSDL";

        var habitaciones = await Connector.BuscarHabitacionesAsync(soapBuscarHabitacionesUri);
        if (habitaciones.Length == 0)
        {
            Console.WriteLine("No se encontraron habitaciones.");
            return;
        }

        foreach (var hab in habitaciones)
        {
            Console.WriteLine(hab);
        }

        var habitacion = habitaciones[^13];
        Console.WriteLine($"Habitación seleccionada: {habitacion}");

        var fechaInicio = DateTime.Now.Date.AddMonths(1);
        var fechaFin = fechaInicio.AddDays(3);

        var disponible = await Connector.ValidarDisponibilidadAsync(soapValidarDisponibilidadUri, habitacion.IdHabitacion, fechaInicio, fechaFin);
        Console.WriteLine($"La habitación {habitacion.IdHabitacion} {(disponible ? "está" : "no está")} disponible entre {fechaInicio:d} y {fechaFin:d}.");

        if (!disponible)
        {
            return;
        }

        var holdId = await Connector.CrearPrerreservaAsync(
            soapCrearPreReservaUri,
            habitacion.IdHabitacion,
            fechaInicio,
            fechaFin,
            numeroHuespedes: habitacion.Capacidad,
            precioActual: habitacion.PrecioActual);
        Console.WriteLine($"Hold creado: {holdId}");

        var usuarioId = await Connector.CrearUsuarioExternoAsync(soapCrearUsuarioExternoUri, "jperez123456@travelio.com", "Juan", "Pérez");
        Console.WriteLine($"Usuario externo creado: {usuarioId}");

        var reservaId = await Connector.CrearReservaAsync(
            soapReservarHabitacionUri,
            habitacion.IdHabitacion,
            holdId,
            "Juan",
            "Pérez",
            "juan@correo.com",
            "DNI",
            "1234567890",
            fechaInicio,
            fechaFin,
            numeroHuespedes: habitacion.Capacidad);
        Console.WriteLine($"Reserva creada con Id: {reservaId}");

        var facturaUrl = await Connector.EmitirFacturaAsync(
            soapEmitirFacturaUri,
            reservaId,
            "Juan",
            "Pérez",
            "DNI",
            "1234567890",
            "juan@correo.com");
        Console.WriteLine($"Factura emitida: {facturaUrl}");

        var datosReserva = await Connector.ObtenerDatosReservaAsync(soapBuscarDatosReservaUri, reservaId);
        Console.WriteLine($"Datos de la reserva: {datosReserva}");
    }
}
