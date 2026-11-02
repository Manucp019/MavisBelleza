using Microsoft.JSInterop;

namespace MavisBelleza.Services;

/// <summary>
/// Envoltorio del módulo JavaScript wwwroot/js/mavis.js (JS Interop con import de módulo ES).
/// El módulo se carga una sola vez por circuito (lazy) y se libera al cerrar.
/// Solo se puede usar después del primer render (no durante el prerender).
/// </summary>
public sealed class JsInteropService : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _modulo;

    public JsInteropService(IJSRuntime js)
    {
        _modulo = new(() => js.InvokeAsync<IJSObjectReference>("import", "./js/mavis.js").AsTask());
    }

    /// <summary>Descarga un archivo generado en el servidor (por ejemplo un CSV) desde el navegador.</summary>
    public async Task DescargarArchivoAsync(string nombreArchivo, string contenido, string tipoMime = "text/csv;charset=utf-8")
    {
        var m = await _modulo.Value;
        await m.InvokeVoidAsync("descargarArchivo", nombreArchivo, contenido, tipoMime);
    }

    /// <summary>Copia texto al portapapeles. Devuelve true si lo logró.</summary>
    public async Task<bool> CopiarAlPortapapelesAsync(string texto)
    {
        var m = await _modulo.Value;
        return await m.InvokeAsync<bool>("copiarAlPortapapeles", texto);
    }

    /// <summary>Hace scroll suave hasta arriba de la página (al cambiar de página en el paginador).</summary>
    public async Task ScrollArribaAsync()
    {
        var m = await _modulo.Value;
        await m.InvokeVoidAsync("scrollArriba");
    }

    /// <summary>Pone el foco en un elemento por id.</summary>
    public async Task EnfocarAsync(string elementId)
    {
        var m = await _modulo.Value;
        await m.InvokeVoidAsync("enfocar", elementId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_modulo.IsValueCreated)
        {
            try
            {
                var m = await _modulo.Value;
                await m.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // El usuario cerró la pestaña: no hay nada que liberar del lado del navegador
            }
        }
    }
}
