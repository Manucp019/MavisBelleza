namespace MavisBelleza.Services;

public class TurnoStateService
{
    public event Action? OnChange;
    public int TotalTurnos { get; private set; }

    public void ActualizarTotal(int total)
    {
        TotalTurnos = total;
        OnChange?.Invoke();
    }
}