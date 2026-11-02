namespace MavisBelleza.Services;

public enum TipoToast { Exito, Error, Info }

public record ToastMensaje(Guid Id, string Texto, TipoToast Tipo);

/// <summary>
/// Notificaciones tipo "toast" para el usuario actual (Scoped = una instancia por circuito).
/// Cualquier página llama a Mostrar(...) y el componente &lt;Toasts /&gt; del layout las dibuja.
/// </summary>
public class ToastService
{
    private readonly List<ToastMensaje> _mensajes = new();
    private readonly object _lock = new();

    public event Action? OnChange;

    /// <summary>Copia de los mensajes actuales (se cierran desde otro hilo, por eso se copia con lock).</summary>
    public IReadOnlyList<ToastMensaje> Mensajes
    {
        get { lock (_lock) return _mensajes.ToArray(); }
    }

    public void Exito(string texto) => Mostrar(texto, TipoToast.Exito);
    public void Error(string texto) => Mostrar(texto, TipoToast.Error);
    public void Info(string texto) => Mostrar(texto, TipoToast.Info);

    public void Mostrar(string texto, TipoToast tipo)
    {
        var msg = new ToastMensaje(Guid.NewGuid(), texto, tipo);
        lock (_lock) _mensajes.Add(msg);
        OnChange?.Invoke();

        // Se cierra sola a los 4 segundos
        _ = Task.Delay(4000).ContinueWith(_ => Cerrar(msg.Id));
    }

    public void Cerrar(Guid id)
    {
        int quitados;
        lock (_lock) quitados = _mensajes.RemoveAll(m => m.Id == id);
        if (quitados > 0)
            OnChange?.Invoke();
    }
}
