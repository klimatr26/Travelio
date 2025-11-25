# Travelio

Conjunto de proyectos que construyen el bus de integracion de Travelio. El codigo esta en desarrollo y algunas capas estan incompletas.

## Proyectos de la solucion
- **TravelioREST**: Conecta el bus de integracion con cada API REST indicada por URL.
- **TravelioSOAP**: Conecta el bus de integracion con servicios SOAP segun la URL configurada.
- **TravelioAPIConnector**: Abstrae la conexion a APIs, ya sean SOAP o REST, y deja abierta la posibilidad de soportar gRPC o GraphQL en el futuro.
- **TravelioBankConnector**: Conecta el bus de integracion con la API REST del banco.
- **TravelioDatabaseConnector**: Manipula la base de datos del bus de integracion usando Entity Framework Core.
- **TravelioIntegrator**: Orquesta las capas anteriores; se conecta con la base de datos, el banco y las APIs para automatizar cargas de datos hacia la base de datos.
- **TravelioDBAdministrator**: Provee una interfaz ASP.NET Core para administrar la base de datos a traves de TravelioDatabaseConnector. Esta capa podria eliminarse o cambiarse segun la estrategia de front-end.

## Diagrama de relacion

```mermaid
flowchart TD
  Bus[Bus de integracion] --> Integrator[TravelioIntegrator]
  Integrator --> DatabaseConnector[TravelioDatabaseConnector]
  Integrator --> BankConnector[TravelioBankConnector]
  Integrator --> APIConnector[TravelioAPIConnector]
  APIConnector --> REST[TravelioREST (APIs REST)]
  APIConnector --> SOAP[TravelioSOAP (Servicios SOAP)]
  DatabaseConnector --> DB[(Base de datos)]
  DBAdmin[TravelioDBAdministrator (UI)] --> DatabaseConnector
  BankConnector --> BankAPI[(API REST del banco)]
```

## Estado
El codigo aun no esta completo; varias capas pueden cambiar conforme avance el desarrollo.
