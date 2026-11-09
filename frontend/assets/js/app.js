const formatoCop = new Intl.NumberFormat("es-CO", {
  style: "currency",
  currency: "COP",
  maximumFractionDigits: 0
});

const elementos = {
  formTaquilla: document.getElementById("formTaquilla"),
  nombre: document.getElementById("nombre"),
  precio: document.getElementById("precio"),
  errorTaquilla: document.getElementById("errorTaquilla"),
  estadoTaquilla: document.getElementById("estadoTaquilla"),
  formCompra: document.getElementById("formCompra"),
  chips: document.querySelectorAll(".chip"),
  botonComprar: document.getElementById("botonComprar"),
  botonFinalizar: document.getElementById("botonFinalizar"),
  errorCompra: document.getElementById("errorCompra"),
  boleta: document.getElementById("boleta"),
  boletaNumero: document.getElementById("boletaNumero"),
  boletaEspectaculo: document.getElementById("boletaEspectaculo"),
  boletaCantidad: document.getElementById("boletaCantidad"),
  boletaSubtotal: document.getElementById("boletaSubtotal"),
  boletaDescuento: document.getElementById("boletaDescuento"),
  boletaTotal: document.getElementById("boletaTotal"),
  tablaPrecios: document.getElementById("tablaPrecios"),
  tablaVentas: document.getElementById("tablaVentas"),
  entradasVendidas: document.getElementById("entradasVendidas"),
  totalRecaudado: document.getElementById("totalRecaudado")
};

const fotoPerfil = document.getElementById("fotoPerfil");
const inicialesPerfil = document.getElementById("inicialesPerfil");

function mostrarIniciales() {
  fotoPerfil.hidden = true;
  inicialesPerfil.hidden = false;
}

fotoPerfil.addEventListener("error", mostrarIniciales);
if (fotoPerfil.complete && fotoPerfil.naturalWidth === 0) {
  mostrarIniciales();
}

let cantidadSeleccionada = 1;

async function llamarApi(ruta, metodo, datos) {
  const opciones = { method: metodo, headers: {} };
  if (datos !== undefined) {
    opciones.headers["Content-Type"] = "application/json";
    opciones.body = JSON.stringify(datos);
  }

  const respuesta = await fetch(ruta, opciones);
  const contenido = await respuesta.json().catch(() => ({}));

  if (!respuesta.ok) {
    throw new Error(contenido.mensaje || "No fue posible completar la solicitud.");
  }
  return contenido;
}

function mostrarError(elemento, mensaje) {
  elemento.textContent = mensaje;
  elemento.hidden = !mensaje;
}

function textoEntradas(cantidad) {
  return cantidad === 1 ? "1 entrada" : `${cantidad} entradas`;
}

function textoDescuento(compra) {
  return `${compra.porcentajeDescuento}% (${formatoCop.format(compra.descuento)})`;
}

function crearFila(celdas) {
  const fila = document.createElement("tr");
  for (const contenido of celdas) {
    const celda = document.createElement("td");
    celda.append(contenido);
    fila.appendChild(celda);
  }
  return fila;
}

function crearFilaVacia(mensaje, columnas) {
  const fila = crearFila([mensaje]);
  fila.firstChild.colSpan = columnas;
  fila.firstChild.className = "vacio";
  return fila;
}

function seleccionarCantidad(cantidad) {
  cantidadSeleccionada = cantidad;
  for (const chip of elementos.chips) {
    chip.setAttribute("aria-pressed", String(Number(chip.dataset.cantidad) === cantidad));
  }
}

function habilitarCompra(habilitado) {
  for (const chip of elementos.chips) {
    chip.disabled = !habilitado;
  }
  elementos.botonComprar.disabled = !habilitado;
}

function mostrarTablaPrecios(tablaPrecios) {
  let mayorDescuento = 0;
  for (const compra of tablaPrecios) {
    mayorDescuento = Math.max(mayorDescuento, compra.porcentajeDescuento);
  }

  elementos.tablaPrecios.replaceChildren(
    ...tablaPrecios.map((compra) => {
      const esMejorPrecio = mayorDescuento > 0 && compra.porcentajeDescuento === mayorDescuento;
      const primeraCelda = document.createDocumentFragment();
      primeraCelda.append(String(compra.cantidad));

      if (esMejorPrecio) {
        const etiqueta = document.createElement("span");
        etiqueta.className = "etiqueta-mejor";
        etiqueta.textContent = "Mejor precio";
        primeraCelda.append(etiqueta);
      }

      const fila = crearFila([
        primeraCelda,
        formatoCop.format(compra.subtotal),
        textoDescuento(compra),
        formatoCop.format(compra.total)
      ]);
      if (esMejorPrecio) {
        fila.className = "mejor-precio";
      }
      return fila;
    })
  );

  for (const compra of tablaPrecios) {
    const descuento = document.querySelector(`[data-descuento="${compra.cantidad}"]`);
    if (descuento) {
      descuento.textContent = `${compra.porcentajeDescuento}%`;
    }
  }
}

function mostrarHistorial(historial) {
  if (historial.ventas.length === 0) {
    elementos.tablaVentas.replaceChildren(crearFilaVacia("Aún no hay ventas", 5));
  } else {
    elementos.tablaVentas.replaceChildren(
      ...historial.ventas.map((compra, indice) => {
        // En celular el nombre se muestra debajo del número y se oculta su columna.
        const numero = document.createDocumentFragment();
        const espectaculoMovil = document.createElement("span");
        espectaculoMovil.className = "espectaculo-movil";
        espectaculoMovil.textContent = compra.espectaculo;
        numero.append(`N.º ${indice + 1}`, espectaculoMovil);

        const fila = crearFila([
          numero,
          compra.espectaculo,
          String(compra.cantidad),
          formatoCop.format(compra.descuento),
          formatoCop.format(compra.total)
        ]);
        fila.children[1].className = "espectaculo";
        return fila;
      })
    );
  }

  elementos.entradasVendidas.textContent = historial.entradasVendidas;
  elementos.totalRecaudado.textContent = formatoCop.format(historial.totalRecaudado);
}

async function actualizarHistorial() {
  const historial = await llamarApi("/api/ventas", "GET");
  mostrarHistorial(historial);
  return historial;
}

function mostrarTaquilla(taquilla) {
  const nombre = document.createElement("strong");
  nombre.textContent = taquilla.espectaculo;
  elementos.estadoTaquilla.replaceChildren(
    nombre,
    ` · ${formatoCop.format(taquilla.precioEntrada)} por entrada`
  );

  habilitarCompra(true);
  elementos.botonFinalizar.disabled = taquilla.comprasRegistradas === 0;
  mostrarTablaPrecios(taquilla.tablaPrecios);
}

function reiniciarTaquilla() {
  elementos.nombre.value = "";
  elementos.precio.value = "";
  elementos.estadoTaquilla.textContent = "Abre la taquilla para registrar compras.";
  elementos.tablaPrecios.replaceChildren(crearFilaVacia("Abre la taquilla para ver los precios", 4));
  seleccionarCantidad(1);
  habilitarCompra(false);
  elementos.botonFinalizar.disabled = true;
  elementos.boleta.hidden = true;
  mostrarError(elementos.errorTaquilla, "");
  mostrarError(elementos.errorCompra, "");
}

function mostrarBoleta(compra, numero) {
  elementos.boletaNumero.textContent = `N.º ${numero}`;
  elementos.boletaEspectaculo.textContent = compra.espectaculo;
  elementos.boletaCantidad.textContent = textoEntradas(compra.cantidad);
  elementos.boletaSubtotal.textContent = formatoCop.format(compra.subtotal);
  elementos.boletaDescuento.textContent = textoDescuento(compra);
  elementos.boletaTotal.textContent = formatoCop.format(compra.total);
  elementos.boleta.hidden = false;
}

for (const chip of elementos.chips) {
  chip.addEventListener("click", () => seleccionarCantidad(Number(chip.dataset.cantidad)));
}

elementos.formTaquilla.addEventListener("submit", async (evento) => {
  evento.preventDefault();
  mostrarError(elementos.errorTaquilla, "");

  try {
    const taquilla = await llamarApi("/api/taquilla", "POST", {
      nombre: elementos.nombre.value,
      precio: elementos.precio.value
    });
    mostrarTaquilla(taquilla);
    elementos.boleta.hidden = true;
    mostrarError(elementos.errorCompra, "");
  } catch (error) {
    mostrarError(elementos.errorTaquilla, error.message);
  }
});

elementos.formCompra.addEventListener("submit", async (evento) => {
  evento.preventDefault();
  mostrarError(elementos.errorCompra, "");

  try {
    const compra = await llamarApi("/api/compras", "POST", { cantidad: cantidadSeleccionada });
    const historial = await actualizarHistorial();
    mostrarBoleta(compra, historial.ventas.length);
    elementos.botonFinalizar.disabled = false;
  } catch (error) {
    mostrarError(elementos.errorCompra, error.message);
  }
});

elementos.botonFinalizar.addEventListener("click", async () => {
  mostrarError(elementos.errorCompra, "");

  try {
    await llamarApi("/api/taquilla/finalizar", "POST");
    reiniciarTaquilla();
  } catch (error) {
    mostrarError(elementos.errorCompra, error.message);
  }
});

actualizarHistorial().catch(() => {
  // Si el historial no responde, la tabla de ventas queda en su estado inicial.
});

llamarApi("/api/taquilla", "GET")
  .then((taquilla) => {
    elementos.nombre.value = taquilla.espectaculo;
    elementos.precio.value = taquilla.precioEntrada;
    mostrarTaquilla(taquilla);
  })
  .catch(() => {
    // No hay taquilla abierta: la página queda lista para abrir una nueva.
  });
