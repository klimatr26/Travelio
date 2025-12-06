using Microsoft.EntityFrameworkCore;
using TravelioDatabaseConnector.Enums;
using TravelioDatabaseConnector.Models;

namespace TravelioDatabaseConnector.Data;

public class TravelioDbContext : DbContext
{
    public TravelioDbContext(DbContextOptions<TravelioDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ReservaCompra> ReservasCompra => Set<ReservaCompra>();
    public DbSet<DetalleServicio> DetallesServicio => Set<DetalleServicio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureCliente(modelBuilder);
        ConfigureServicio(modelBuilder);
        ConfigureReserva(modelBuilder);
        ConfigureCompra(modelBuilder);
        ConfigureReservaCompra(modelBuilder);
        ConfigureDetalleServicio(modelBuilder);

        SeedServicios(modelBuilder);
        SeedDetallesServicio(modelBuilder);
    }

    private static void ConfigureCliente(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CorreoElectronico).IsRequired().HasMaxLength(256);
            builder.HasIndex(c => c.CorreoElectronico).IsUnique();
            builder.Property(c => c.Nombre).IsRequired().HasMaxLength(150);
            builder.Property(c => c.Apellido).IsRequired().HasMaxLength(150);
            builder.Property(c => c.Pais).HasMaxLength(100);
            builder.Property(c => c.FechaNacimiento).HasColumnType("date");
            builder.Property(c => c.Telefono).HasMaxLength(50);
            builder.Property(c => c.TipoIdentificacion).IsRequired().HasMaxLength(60);
            builder.Property(c => c.DocumentoIdentidad).IsRequired().HasMaxLength(120);
            builder.Property(c => c.PasswordHash).IsRequired().HasMaxLength(256);
            builder.Property(c => c.PasswordSalt).IsRequired().HasMaxLength(256);
        });
    }

    private static void ConfigureServicio(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Servicio>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Nombre).IsRequired().HasMaxLength(200);
            builder.Property(s => s.NumeroCuenta).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Activo).IsRequired();
            builder.Property(s => s.TipoServicio)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(60);
        });
    }

    private static void ConfigureReserva(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reserva>(builder =>
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.CodigoReserva).IsRequired().HasMaxLength(120);
            builder.Property(r => r.FacturaUrl).HasMaxLength(1024);
            builder.HasOne(r => r.Servicio)
                .WithMany(s => s.Reservas)
                .HasForeignKey(r => r.ServicioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureCompra(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Compra>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FechaCompra).IsRequired();
            builder.Property(c => c.ValorPagado).IsRequired().HasPrecision(18, 2);
            builder.Property(c => c.FacturaUrl).HasMaxLength(1024);
            builder.HasOne(c => c.Cliente)
                .WithMany(cl => cl.Compras)
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureReservaCompra(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReservaCompra>(builder =>
        {
            builder.HasKey(rc => new { rc.CompraId, rc.ReservaId });
            builder.HasOne(rc => rc.Compra)
                .WithMany(c => c.ReservasCompra)
                .HasForeignKey(rc => rc.CompraId);
            builder.HasOne(rc => rc.Reserva)
                .WithMany(r => r.ReservasCompra)
                .HasForeignKey(rc => rc.ReservaId);
        });
    }

    private static void ConfigureDetalleServicio(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DetalleServicio>(builder =>
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.UriBase).HasMaxLength(512);
            builder.Property(d => d.ObtenerProductosEndpoint).HasMaxLength(512);
            builder.Property(d => d.RegistrarClienteEndpoint).HasMaxLength(512);
            builder.Property(d => d.ConfirmarProductoEndpoint).HasMaxLength(512);
            builder.Property(d => d.CrearPrerreservaEndpoint).HasMaxLength(512);
            builder.Property(d => d.CrearReservaEndpoint).HasMaxLength(512);
            builder.Property(d => d.GenerarFacturaEndpoint).HasMaxLength(512);
            builder.Property(d => d.ObtenerReservaEndpoint).HasMaxLength(512);
            builder.Property(d => d.TipoProtocolo)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);
            builder.HasOne(d => d.Servicio)
                .WithOne(s => s.DetalleServicio)
                .HasForeignKey<DetalleServicio>(d => d.ServicioId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void SeedServicios(ModelBuilder modelBuilder)
    {
        var servicios = new[]
        {
            new Servicio
            {
                Id = 1,
                Nombre = "Travelio Airlines",
                TipoServicio = TipoServicio.Aerolinea,
                NumeroCuenta = "AL-1001",
                Activo = true
            },
            new Servicio
            {
                Id = 2,
                Nombre = "Travelio Hotels",
                TipoServicio = TipoServicio.Hotel,
                NumeroCuenta = "HT-2001",
                Activo = true
            },
            new Servicio
            {
                Id = 3,
                Nombre = "Travelio Cars",
                TipoServicio = TipoServicio.RentaVehiculos,
                NumeroCuenta = "CR-3001",
                Activo = true
            }
        };

        modelBuilder.Entity<Servicio>().HasData(servicios);
    }

    private static void SeedDetallesServicio(ModelBuilder modelBuilder)
    {
        var detalles = new[]
        {
            new DetalleServicio
            {
                Id = 1,
                ServicioId = 1,
                TipoProtocolo = TipoProtocolo.Rest,
                UriBase = "https://api.travelio-air.example.com/v1",
                ObtenerProductosEndpoint = "/flights",
                RegistrarClienteEndpoint = "/customers",
                ConfirmarProductoEndpoint = "/flights/confirm",
                CrearPrerreservaEndpoint = "/flights/prereservations",
                CrearReservaEndpoint = "/flights/reservations",
                GenerarFacturaEndpoint = "/billing/invoices",
                ObtenerReservaEndpoint = "/flights/reservations/{codigo}"
            },
            new DetalleServicio
            {
                Id = 2,
                ServicioId = 2,
                TipoProtocolo = TipoProtocolo.Soap,
                UriBase = "https://soap.travelio-hotels.example.com",
                ObtenerProductosEndpoint = "/Rooms/GetAvailable",
                RegistrarClienteEndpoint = "/Guests/Register",
                ConfirmarProductoEndpoint = "/Rooms/ConfirmAvailability",
                CrearPrerreservaEndpoint = "/Bookings/CreatePrebooking",
                CrearReservaEndpoint = "/Bookings/CreateReservation",
                GenerarFacturaEndpoint = "/Billing/CreateInvoice",
                ObtenerReservaEndpoint = "/Bookings/GetReservation"
            },
            new DetalleServicio
            {
                Id = 3,
                ServicioId = 3,
                TipoProtocolo = TipoProtocolo.Rest,
                UriBase = "https://api.travelio-cars.example.com",
                ObtenerProductosEndpoint = "/vehicles",
                RegistrarClienteEndpoint = "/clients",
                ConfirmarProductoEndpoint = "/vehicles/availability",
                CrearPrerreservaEndpoint = "/vehicles/prereservations",
                CrearReservaEndpoint = "/vehicles/reservations",
                GenerarFacturaEndpoint = "/invoices",
                ObtenerReservaEndpoint = "/vehicles/reservations/{codigo}"
            }
        };

        modelBuilder.Entity<DetalleServicio>().HasData(detalles);
    }
}
