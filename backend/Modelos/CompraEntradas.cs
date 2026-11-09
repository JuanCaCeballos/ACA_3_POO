namespace EntradasEspectaculo.Modelos;

/// <summary>
/// Representa una compra de entradas y calcula su subtotal, descuento y total.
/// </summary>
public class CompraEntradas
{
    /// <summary>Cantidad máxima de entradas permitida por compra.</summary>
    public const int MaximoEntradas = 4;

    private readonly string espectaculo;
    private readonly decimal precio;
    private readonly int cantidad;

    /// <summary>
    /// Crea una compra validando el espectáculo, el precio por entrada y la cantidad.
    /// </summary>
    /// <exception cref="ArgumentException">Si falta el espectáculo, el precio no es positivo o la cantidad está fuera de rango.</exception>
    public CompraEntradas(string espectaculo, decimal precio, int cantidad)
    {
        if (string.IsNullOrWhiteSpace(espectaculo))
        {
            throw new ArgumentException("La compra debe indicar el espectáculo.");
        }

        if (precio <= 0)
        {
            throw new ArgumentException("El precio de la entrada debe ser mayor que cero.");
        }

        if (cantidad < 1 || cantidad > MaximoEntradas)
        {
            throw new ArgumentException($"La cantidad debe estar entre 1 y {MaximoEntradas} entradas.");
        }

        this.espectaculo = espectaculo;
        this.precio = precio;
        this.cantidad = cantidad;
    }

    /// <summary>Nombre del espectáculo al que pertenece la compra.</summary>
    public string Espectaculo => espectaculo;

    /// <summary>Precio de una entrada.</summary>
    public decimal Precio => precio;

    /// <summary>Número de entradas de la compra.</summary>
    public int Cantidad => cantidad;

    /// <summary>Porcentaje de descuento según la cantidad: 2 = 10, 3 = 15, 4 = 20.</summary>
    public int PorcentajeDescuento
    {
        get
        {
            switch (cantidad)
            {
                case 2:
                    return 10;
                case 3:
                    return 15;
                case 4:
                    return 20;
                default:
                    return 0;
            }
        }
    }

    /// <summary>Suma del precio de cada entrada, sin descuento.</summary>
    public decimal Subtotal
    {
        get
        {
            decimal subtotal = 0;
            for (int i = 0; i < cantidad; i++)
            {
                subtotal += precio;
            }
            return subtotal;
        }
    }

    /// <summary>Valor descontado del subtotal.</summary>
    public decimal Descuento => Subtotal * PorcentajeDescuento / 100;

    /// <summary>Valor a pagar: subtotal menos descuento.</summary>
    public decimal Total => Subtotal - Descuento;
}
