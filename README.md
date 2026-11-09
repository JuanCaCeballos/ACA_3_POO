# ACA_3_POO: Taquilla de Espectáculos

> Para ver mejor esta documentación en Visual Studio Code, se recomienda usar la extensión
> **Office Viewer**, que muestra el Markdown con formato, tablas y bloques de código.

Entrega final del proyecto de la materia PROGRAMACION ORIENTADA A OBJETOS 2/54492.

Autor: Juan Camilo Ceballos 

## Descripción

Aplicación web que calcula el pago de entradas a un espectáculo, con un máximo de 4 entradas por compra.
Toda la lógica está en C#; el frontend solo consume la API y muestra los resultados.

| Entradas | Descuento |
| -------- | --------- |
| 1        | 0%        |
| 2        | 10%       |
| 3        | 15%       |
| 4        | 20%       |

## Funcionamiento

1. **Abrir taquilla:** se escribe el nombre del espectáculo y el precio por entrada. Se muestra la tabla de
   precios de 1 a 4 entradas, con la fila de mejor precio resaltada.
2. **Registrar compra:** se elige la cantidad (1 a 4) y se genera una boleta con subtotal, descuento y total.
3. **Finalizar compra:** cierra la taquilla actual y deja los formularios listos para un nuevo espectáculo.
4. **Ventas:** historial de todas las compras de todos los espectáculos, con entradas vendidas y total
   recaudado acumulados. Abrir o finalizar una taquilla no lo borra.

El precio acepta solo dígitos y separadores de miles (`50000`, `50.000` o `50,000`); se rechazan el texto
vacío, las letras, los decimales y el cero.

Los datos se guardan en memoria: el historial se conserva mientras la aplicación esté en ejecución y se
pierde al detenerla.

## Conceptos de POO aplicados

| Concepto                    | Dónde                                                                                                 |
| --------------------------- | ------------------------------------------------------------------------------------------------------ |
| Clases y objetos            | `CompraEntradas`, `TaquillaServicio`, `ValidadorPrecio`                                          |
| Constructor con validación | `CompraEntradas(espectaculo, precio, cantidad)` lanza `ArgumentException` si un dato no es válido |
| Encapsulamiento             | Campos privados con propiedades de solo lectura en `CompraEntradas`                                  |
| Métodos                    | `AbrirTaquilla`, `RegistrarCompra`, `FinalizarCompra`, `GenerarTablaPrecios`, entre otros      |
| `switch`                  | Porcentaje de descuento según la cantidad (`CompraEntradas.PorcentajeDescuento`)                    |
| `for`                     | Subtotal, tabla de precios de 1 a 4, entradas vendidas y total recaudado                               |
| `while`                   | `ValidadorPrecio.TryLeer` recorre el texto del precio carácter por carácter                        |

## Estructura

```
ACA_3_POO/
│
├── backend/                         # API desarrollada en ASP.NET Core (.NET 10)
│   │
│   ├── EntradasEspectaculo.csproj   # Configuración del proyecto
│   ├── Program.cs                  # Configuración inicial y arranque de la API
│   │
│   ├── Properties/
│   │   └── launchSettings.json     # Perfil de ejecución (localhost:5080)
│   │
│   ├── Endpoints/                  # Definición de rutas HTTP
│   │   ├── TaquillaEndpoints.cs     # Endpoints agrupados bajo /api
│   │   └── Solicitudes.cs           # Estructuras de datos de entrada
│   │
│   ├── Servicios/                  # Lógica de negocio
│   │   ├── TaquillaServicio.cs     # Compras, taquilla e historial de ventas
│   │   └── ValidadorPrecio.cs      # Lectura y validación de precios
│   │
│   └── Modelos/                    # Modelos de datos
│       └── CompraEntradas.cs       # Compra, subtotal, descuento y total
│
└── frontend/                        # Interfaz web (HTML, CSS y JavaScript)
    │
    ├── index.html                   # Página principal
    │
    └── assets/
        │
        ├── css/
        │   └── styles.css           # Estilos visuales
        │
        ├── js/
        │   └── app.js               # Consumo de la API y visualización de resultados en COP
        │
        └── img/
            └── perfil.jpg           # Fotografía del cajero (opcional)
                                      # Si no existe, se muestran las iniciales
```

## Cómo ejecutar

Requisito: SDK de .NET 10.

```
dotnet run --project backend
```

La página se abre en http://localhost:5080.

## Endpoints

| Método | Ruta                        | Cuerpo                                     | Descripción                                                       |
| ------- | --------------------------- | ------------------------------------------ | ------------------------------------------------------------------ |
| POST    | `/api/taquilla`           | `{ "nombre": "...", "precio": "50000" }` | Abre la taquilla y devuelve la tabla de precios                    |
| GET     | `/api/taquilla`           |                                            | Taquilla actual: espectáculo, precio, tabla de precios y compras  |
| POST    | `/api/taquilla/finalizar` |                                            | Cierra la taquilla actual; el historial de ventas se conserva      |
| POST    | `/api/compras`            | `{ "cantidad": 2 }`                      | Registra una compra en la taquilla actual                          |
| GET     | `/api/ventas`             |                                            | Historial de todas las ventas, entradas vendidas y total recaudado |

El precio se envía como texto para que lo valide `ValidadorPrecio`. Los errores de validación y las
operaciones sobre la taquilla cuando no hay una abierta responden `400` con `{ "mensaje": "..." }`.
`GET /api/ventas` siempre responde `200`, aunque no haya ventas.

### Prueba rápida (PowerShell)

Con la aplicación en ejecución, en otra terminal:

```powershell
$api = "http://localhost:5080/api"
Invoke-RestMethod -Method Post "$api/taquilla" -ContentType "application/json" -Body '{"nombre":"Concierto","precio":"50000"}'
Invoke-RestMethod -Method Post "$api/compras" -ContentType "application/json" -Body '{"cantidad":2}'
Invoke-RestMethod "$api/ventas"
```

La última consulta muestra 2 entradas vendidas y un total recaudado de 90000.
