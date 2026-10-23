namespace WaliSync.Models;

public record DosenLookupDto
{
    public int DosenId { get; init; }
    public string Nidn { get; init; } = string.Empty;
    public string NamaDosen { get; init; } = string.Empty;
    public string Prodi { get; init; } = string.Empty;
}

