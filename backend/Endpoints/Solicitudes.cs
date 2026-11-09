namespace EntradasEspectaculo.Endpoints;

public class SolicitudApertura
{
    public string? Nombre { get; set; }
    public string? Precio { get; set; }
}

public class SolicitudCompra
{
    public int Cantidad { get; set; }
}
