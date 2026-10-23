namespace WaliSync.Models;

public record DosenResponseDto
{
    public int DosenId { get; init; }
    public string Nidn { get; init; } = string.Empty;
    public string NamaDosen { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Prodi { get; init; } = string.Empty;

    // Properti kalkulasi dari relasi (baca-saja)
    public int JumlahMahasiswaBimbingan { get; init; }
}

