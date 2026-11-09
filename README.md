# ACA_3_POO: Taquilla de Espectáculos

Entrega final del proyecto de la materia PROGRAMACION ORIENTADA A OBJETOS 2/54492.

Autor: Juan Camilo Ceballos

## Descripción

Aplicación web que calcula el pago de entradas a un espectáculo, con un máximo de 4 entradas por compra.
Toda la lógica está en C# (clases, constructores, métodos, `while` y `for`); el frontend solo consume la API.

| Entradas | Descuento |
|----------|-----------|
| 1        | 0%        |
| 2        | 10%       |
| 3        | 15%       |
| 4        | 20%       |

## Estructura

```
backend/                         API en ASP.NET Core (.NET 10, Minimal API)
  Program.cs                     configuración y arranque; sirve la carpeta frontend
  Properties/launchSettings.json perfil http en http://localhost:5080
  Endpoints/                     rutas HTTP y clases de solicitud
  Servicios/                     lógica de la taquilla y validación del precio
  Modelos/                       clase CompraEntradas
frontend/                        HTML, CSS y JavaScript sin frameworks
  index.html
  assets/
    css/styles.css
    js/app.js
    img/perfil.jpg               foto del cajero (opcional; sin ella se muestran las iniciales)
```

## Cómo ejecutar

Requisito: SDK de .NET 10.

```
dotnet run --project backend
```

La página se abre en http://localhost:5080.

## Endpoints

| Método | Ruta                      | Cuerpo                                   | Descripción                                                        |
|--------|---------------------------|------------------------------------------|--------------------------------------------------------------------|
| POST   | `/api/taquilla`           | `{ "nombre": "...", "precio": "50000" }` | Abre la taquilla y devuelve la tabla de precios                    |
| GET    | `/api/taquilla`           |                                          | Taquilla actual: espectáculo, precio, tabla de precios y compras   |
| POST   | `/api/taquilla/finalizar` |                                          | Cierra la taquilla actual; el historial de ventas se conserva      |
| POST   | `/api/compras`            | `{ "cantidad": 2 }`                      | Registra una compra en la taquilla actual                          |
| GET    | `/api/ventas`             |                                          | Historial de todas las ventas, entradas vendidas y total recaudado |

Los errores de validación y las operaciones sobre la taquilla cuando no hay una abierta responden `400` con
`{ "mensaje": "..." }`. `GET /api/ventas` siempre responde `200`, aunque no haya ventas.
