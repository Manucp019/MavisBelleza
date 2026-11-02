// =====================================================================
// Mavis Belleza - módulo de JS Interop
// Se importa desde C# con IJSRuntime.InvokeAsync("import", "./js/mavis.js")
// (ver Services/JsInteropService.cs). Cada función exportada se llama
// desde Blazor con InvokeVoidAsync / InvokeAsync<T>.
// =====================================================================

/**
 * Genera un archivo en el navegador y lo descarga (Blob + enlace temporal).
 * Blazor Server corre en el servidor, así que no puede "bajar" un archivo
 * al disco del usuario sin ayuda de JavaScript.
 */
export function descargarArchivo(nombreArchivo, contenido, tipoMime) {
    // BOM UTF-8 para que Excel muestre bien los acentos y la ñ
    const blob = new Blob(["﻿" + contenido], { type: tipoMime });
    const url = URL.createObjectURL(blob);

    const enlace = document.createElement("a");
    enlace.href = url;
    enlace.download = nombreArchivo;
    enlace.style.display = "none";
    document.body.appendChild(enlace);
    enlace.click();

    document.body.removeChild(enlace);
    URL.revokeObjectURL(url);
}

/**
 * Copia texto al portapapeles. Usa la Clipboard API y, si no está disponible
 * (HTTP sin TLS o navegador viejo), un textarea oculto como alternativa.
 * @returns {Promise<boolean>} true si se copió.
 */
export async function copiarAlPortapapeles(texto) {
    try {
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(texto);
            return true;
        }
        const area = document.createElement("textarea");
        area.value = texto;
        area.setAttribute("readonly", "");
        area.style.position = "fixed";
        area.style.opacity = "0";
        document.body.appendChild(area);
        area.select();
        const ok = document.execCommand("copy");
        document.body.removeChild(area);
        return ok;
    } catch {
        return false;
    }
}

/** Scroll suave hasta arriba (se usa al cambiar de página en los listados). */
export function scrollArriba() {
    window.scrollTo({ top: 0, behavior: "smooth" });
}

/** Pone el foco en un elemento por id (accesibilidad en formularios). */
export function enfocar(id) {
    const el = document.getElementById(id);
    if (el) el.focus();
}
