namespace WaliSync.Models;

public record MahasiswaResponseDto
{
    public int MahasiswaId { get; init; }
    public string Nim { get; init; } = string.Empty;
    public string NamaMahasiswa { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int Angkatan { get; init; }
    public int DosenId { get; init; }

    // Proyeksi relasi
    public string NamaDosenWali { get; init; } = string.Empty;
}

