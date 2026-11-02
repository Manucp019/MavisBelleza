namespace MavisBelleza.Models;

/// <summary>Resultado paginado genérico que devuelven los servicios de datos.</summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int TotalItems { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanioPagina { get; init; } = 6;

    public int TotalPaginas => TamanioPagina <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)TamanioPagina);
    public int Desde => TotalItems == 0 ? 0 : (Pagina - 1) * TamanioPagina + 1;
    public int Hasta => Math.Min(Pagina * TamanioPagina, TotalItems);
}
