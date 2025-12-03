namespace TravelioTestConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TravelioTestConsoleApp.Aerolinea.ConnectionTest.TestBasicGetConnection().GetAwaiter().GetResult();
            TravelioTestConsoleApp.Autos.ConnectionTest.TestBasicGetConnection().GetAwaiter().GetResult();
            //TravelioTestConsoleApp.Habitaciones.ConnectionTest.TestBasicConnectionAsync().GetAwaiter().GetResult();

            //TravelioTestConsoleApp.Banco.BancoTest.RunTransferTest().GetAwaiter().GetResult();
        }
    }
}
