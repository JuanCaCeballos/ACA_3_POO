using System.Diagnostics.CodeAnalysis;
using EntradasEspectaculo.Modelos;

namespace EntradasEspectaculo.Servicios;

/// <summary>
/// Administra la taquilla actual y el historial de ventas de todos los espectáculos.
/// </summary>
public class TaquillaServicio
{
    private readonly List<CompraEntradas> historialVentas = new();

    /// <summary>Nombre del espectáculo de la taquilla actual, o null si no hay taquilla abierta.</summary>
    public string? NombreEspectaculo { get; private set; }

    /// <summary>Precio de una entrada en la taquilla actual.</summary>
    public decimal PrecioEntrada { get; private set; }

    /// <summary>Compras registradas en la taquilla actual.</summary>
    public int ComprasTaquillaActual { get; private set; }

    /// <summary>Indica si hay una taquilla abierta.</summary>
    public bool EstaAbierta => NombreEspectaculo is not null;

    /// <summary>
    /// Abre la taquilla para un espectáculo. El historial de ventas se conserva.
    /// </summary>
    /// <returns>La tabla de precios de 1 a 4 entradas.</returns>
    /// <exception cref="ArgumentException">Si el nombre está vacío o el precio no es válido.</exception>
    public List<CompraEntradas> AbrirTaquilla(string? nombre, string? precioTexto)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("Escriba el nombre del espectáculo.");
        }

        if (!ValidadorPrecio.TryLeer(precioTexto, out decimal precio))
        {
            throw new ArgumentException("El precio debe ser un número mayor que cero, sin letras ni decimales.");
        }

        NombreEspectaculo = nombre.Trim();
        PrecioEntrada = precio;
        ComprasTaquillaActual = 0;

        return GenerarTablaPrecios();
    }

    /// <summary>
    /// Registra una compra en la taquilla actual y la agrega al historial.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si no hay taquilla abierta.</exception>
    /// <exception cref="ArgumentException">Si la cantidad está fuera de rango.</exception>
    public CompraEntradas RegistrarCompra(int cantidad)
    {
        VerificarAbierta();

        var compra = new CompraEntradas(NombreEspectaculo, PrecioEntrada, cantidad);
        historialVentas.Add(compra);
        ComprasTaquillaActual++;
        return compra;
    }

    /// <summary>
    /// Cierra la taquilla actual sin modificar el historial de ventas.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si no hay taquilla abierta.</exception>
    public void FinalizarCompra()
    {
        VerificarAbierta();

        NombreEspectaculo = null;
        PrecioEntrada = 0;
        ComprasTaquillaActual = 0;
    }

    /// <summary>
    /// Genera el valor a pagar por cada cantidad posible de entradas en la taquilla actual.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si no hay taquilla abierta.</exception>
    public List<CompraEntradas> GenerarTablaPrecios()
    {
        VerificarAbierta();

        var tabla = new List<CompraEntradas>();
        for (int cantidad = 1; cantidad <= CompraEntradas.MaximoEntradas; cantidad++)
        {
            tabla.Add(new CompraEntradas(NombreEspectaculo, PrecioEntrada, cantidad));
        }
        return tabla;
    }

    /// <summary>Devuelve todas las ventas registradas, de todos los espectáculos.</summary>
    public IReadOnlyList<CompraEntradas> ObtenerHistorial()
    {
        return historialVentas;
    }

    /// <summary>Suma las entradas de todo el historial.</summary>
    public int CalcularEntradasVendidas()
    {
        int total = 0;
        for (int i = 0; i < historialVentas.Count; i++)
        {
            total += historialVentas[i].Cantidad;
        }
        return total;
    }

    /// <summary>Suma el valor pagado en todo el historial.</summary>
    public decimal CalcularTotalRecaudado()
    {
        decimal total = 0;
        for (int i = 0; i < historialVentas.Count; i++)
        {
            total += historialVentas[i].Total;
        }
        return total;
    }

    [MemberNotNull(nameof(NombreEspectaculo))]
    private void VerificarAbierta()
    {
        if (NombreEspectaculo is null)
        {
            throw new InvalidOperationException("No hay una taquilla abierta. Registre primero el espectáculo y el precio.");
        }
    }
}
