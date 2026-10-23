namespace WaliSync.Models;

public record DashboardStatsDto
{
    public int TotalDosen { get; init; }
    public int TotalMahasiswa { get; init; }
    public int DosenWaliAktif { get; init; }
    public IReadOnlyDictionary<int, int> DistribusiAngkatan { get; init; } = new Dictionary<int, int>();
    public IReadOnlyList<BebanPerwalianItemDto> BebanPerwalian { get; init; } = Array.Empty<BebanPerwalianItemDto>();
}

public record BebanPerwalianItemDto
{
    public int DosenId { get; init; }
    public string NamaDosen { get; init; } = string.Empty;
    public string Prodi { get; init; } = string.Empty;
    public int JumlahBimbingan { get; init; }
    public string Inisial { get; init; } = string.Empty;
}

