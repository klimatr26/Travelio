# TravelioDatabaseConnector

Código de acceso a datos con Entity Framework Core para SQL Server. Incluye tablas requeridas, hash de contraseñas con SHA-256 + sal y datos seed para Servicio/DetalleServicio.

## Dónde colocar DbContextOptionsBuilder
- Usa `Data/SqlServerContextFactory.cs` para instanciar `TravelioDbContext` con SQL Server local (cadena por defecto: `Server=localhost;Database=TravelioDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;`).
- Para crear la base programáticamente:
  ```csharp
  await SqlServerContextFactory.EnsureDatabaseAsync(); // opcional: pasa tu cadena de conexión
  ```
- Para usar el contexto en tu app:
  ```csharp
  await using var ctx = SqlServerContextFactory.CreateContext();
  // ctx.Clientes.Add(...); await ctx.SaveChangesAsync();
  ```

## Crear la base de datos y migraciones (SQL Server local)
1) Instala la CLI de EF Core (si no la tienes):  
   `dotnet tool install --global dotnet-ef`
2) (Opcional) Exporta una cadena de conexión propia:  
   `set TRAVELIO_SQLSERVER_CONNECTION=Server=localhost;Database=TravelioDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;`
3) Genera la migración inicial:  
   `dotnet ef migrations add InitialCreate --project TravelioDatabaseConnector --startup-project TravelioDatabaseConnector`
4) Aplica la migración a SQL Server:  
   `dotnet ef database update --project TravelioDatabaseConnector --startup-project TravelioDatabaseConnector`

El seeding de datos de ejemplo para `Servicio` y `DetalleServicio` se ejecutará al aplicar las migraciones.

## Diagrama ER
```mermaid
erDiagram
    Cliente {
        int Id
        string CorreoElectronico
        string Nombre
        string Apellido
        string Pais
        date FechaNacimiento
        string Telefono
        string TipoIdentificacion
        string DocumentoIdentidad
        string PasswordHash
        string PasswordSalt
    }
    Servicio {
        int Id
        string Nombre
        string TipoServicio
        string NumeroCuenta
        bool Activo
    }
    DetalleServicio {
        int Id
        int ServicioId
        string TipoProtocolo
        string UriBase
        string ObtenerProductosEndpoint
        string RegistrarClienteEndpoint
        string ConfirmarProductoEndpoint
        string CrearPrerreservaEndpoint
        string CrearReservaEndpoint
        string GenerarFacturaEndpoint
        string ObtenerReservaEndpoint
    }
    Reserva {
        int Id
        int ServicioId
        string CodigoReserva
        string FacturaUrl
    }
    Compra {
        int Id
        int ClienteId
        datetime FechaCompra
        decimal ValorPagado
        string FacturaUrl
    }
    ReservaCompra {
        int CompraId
        int ReservaId
    }

    Cliente ||--o{ Compra : realiza
    Compra ||--o{ ReservaCompra : vincula
    Reserva ||--o{ ReservaCompra : pertenece
    Servicio ||--o{ Reserva : provee
    Servicio ||--|| DetalleServicio : detalle
```
