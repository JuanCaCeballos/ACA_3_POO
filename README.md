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

### Requisitos

- [SDK de .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0). Para comprobarlo, ejecute
  `dotnet --list-sdks` y verifique que aparezca una versión `10.x`.
- Git, para clonar el repositorio.
- Conexión a internet (opcional): solo para cargar la fuente Montserrat; sin ella se usa la fuente del sistema.

### Pasos

```
git clone https://github.com/JuanCaCeballos/ACA_3_POO.git
cd ACA_3_POO
dotnet run --project backend
```

El comando se ejecuta desde la carpeta raíz del repositorio. Al iniciar, la página se abre automáticamente
en el navegador en http://localhost:5080. Para detener la aplicación, presione `Ctrl + C` en la terminal.

### Solución de problemas

- **"No se pudo copiar ... EntradasEspectaculo.exe" o "being used by another process":** hay otra ejecución
  de la aplicación abierta. Deténgala con `Ctrl + C` en su terminal o cierre el proceso
  `EntradasEspectaculo` desde el Administrador de tareas, y vuelva a ejecutar el comando.
- **"address already in use" en el puerto 5080:** otro programa usa ese puerto. Ciérrelo o ejecute la
  aplicación en otro puerto con `dotnet run --project backend --urls http://localhost:5081`.

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
