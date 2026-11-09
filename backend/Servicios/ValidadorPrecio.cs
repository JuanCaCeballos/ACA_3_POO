namespace EntradasEspectaculo.Servicios;

/// <summary>
/// Convierte el precio escrito por el usuario en un número.
/// </summary>
public static class ValidadorPrecio
{
    private const int LongitudMaxima = 15;

    /// <summary>
    /// Intenta leer un precio que solo contenga dígitos y separadores de miles (punto o coma).
    /// </summary>
    /// <returns>true si el texto es un precio válido mayor que cero.</returns>
    public static bool TryLeer(string? texto, out decimal precio)
    {
        precio = 0;

        if (string.IsNullOrWhiteSpace(texto))
        {
            return false;
        }

        texto = texto.Trim();
        if (texto.Length > LongitudMaxima)
        {
            return false;
        }

        decimal valor = 0;
        bool hayDigitos = false;
        int i = 0;

        while (i < texto.Length)
        {
            char caracter = texto[i];

            if (caracter >= '0' && caracter <= '9')
            {
                valor = valor * 10 + (caracter - '0');
                hayDigitos = true;
            }
            else if (caracter != '.' && caracter != ',')
            {
                return false;
            }

            i++;
        }

        if (!hayDigitos || valor == 0)
        {
            return false;
        }

        precio = valor;
        return true;
    }
}
