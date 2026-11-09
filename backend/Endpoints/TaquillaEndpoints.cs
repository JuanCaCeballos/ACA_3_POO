using EntradasEspectaculo.Modelos;
using EntradasEspectaculo.Servicios;

namespace EntradasEspectaculo.Endpoints;

public static class TaquillaEndpoints
{
    public static void MapTaquillaEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api");

        api.MapPost("/taquilla", (SolicitudApertura solicitud, TaquillaServicio servicio) =>
            Responder(() =>
            {
                var tablaPrecios = servicio.AbrirTaquilla(solicitud.Nombre, solicitud.Precio);
                return DatosTaquilla(servicio, tablaPrecios);
            }));

        api.MapGet("/taquilla", (TaquillaServicio servicio) =>
            Responder(() => DatosTaquilla(servicio, servicio.GenerarTablaPrecios())));

        api.MapPost("/taquilla/finalizar", (TaquillaServicio servicio) =>
            Responder(() =>
            {
                servicio.FinalizarCompra();
                return new { mensaje = "Taquilla cerrada." };
            }));

        api.MapPost("/compras", (SolicitudCompra solicitud, TaquillaServicio servicio) =>
            Responder(() => servicio.RegistrarCompra(solicitud.Cantidad)));

        api.MapGet("/ventas", (TaquillaServicio servicio) =>
            Results.Ok(new
            {
                ventas = servicio.ObtenerHistorial(),
                entradasVendidas = servicio.CalcularEntradasVendidas(),
                totalRecaudado = servicio.CalcularTotalRecaudado()
            }));
    }

    private static object DatosTaquilla(TaquillaServicio servicio, List<CompraEntradas> tablaPrecios)
    {
        return new
        {
            espectaculo = servicio.NombreEspectaculo,
            precioEntrada = servicio.PrecioEntrada,
            comprasRegistradas = servicio.ComprasTaquillaActual,
            tablaPrecios
        };
    }

    private static IResult Responder(Func<object> accion)
    {
        try
        {
            return Results.Ok(accion());
        }
        catch (Exception error) when (error is ArgumentException or InvalidOperationException)
        {
            return Results.BadRequest(new { mensaje = error.Message });
        }
    }
}
