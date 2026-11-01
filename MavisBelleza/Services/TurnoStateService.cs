namespace MavisBelleza.Services;

public class TurnoStateService
{
    public event Action? OnChange;

    public void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}