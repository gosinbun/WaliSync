using System.ComponentModel.DataAnnotations;

namespace WaliSync.Api.DTOs;

public record DosenResponseDto
{
    public int DosenId { get; init; }
    public string Nidn { get; init; } = string.Empty;
    public string NamaDosen { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Prodi { get; init; } = string.Empty;
    public int JumlahMahasiswaBimbingan { get; init; }
}

public record MahasiswaResponseDto
{
    public int MahasiswaId { get; init; }
    public string Nim { get; init; } = string.Empty;
    public string NamaMahasiswa { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int Angkatan { get; init; }
    public int DosenId { get; init; }
    public string NamaDosenWali { get; init; } = string.Empty;
}

public record DosenLookupDto
{
    public int DosenId { get; init; }
    public string Nidn { get; init; } = string.Empty;
    public string NamaDosen { get; init; } = string.Empty;
    public string Prodi { get; init; } = string.Empty;
}

public class DosenCreateUpdateDto
{
    [Required(ErrorMessage = "NIDN wajib diisi.")]
    [StringLength(20, ErrorMessage = "NIDN maksimal 20 karakter.")]
    public string Nidn { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nama Dosen wajib diisi.")]
    [StringLength(100, ErrorMessage = "Nama Dosen maksimal 100 karakter.")]
    public string NamaDosen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    [StringLength(100, ErrorMessage = "Email maksimal 100 karakter.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Program Studi wajib dipilih.")]
    [StringLength(100, ErrorMessage = "Program Studi maksimal 100 karakter.")]
    public string Prodi { get; set; } = string.Empty;
}

public class MahasiswaCreateUpdateDto
{
    [Required(ErrorMessage = "NIM wajib diisi.")]
    [StringLength(20, ErrorMessage = "NIM maksimal 20 karakter.")]
    public string Nim { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nama Mahasiswa wajib diisi.")]
    [StringLength(100, ErrorMessage = "Nama Mahasiswa maksimal 100 karakter.")]
    public string NamaMahasiswa { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email Mahasiswa wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    [StringLength(100, ErrorMessage = "Email maksimal 100 karakter.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Angkatan wajib dipilih.")]
    [Range(2020, 2026, ErrorMessage = "Angkatan harus berada dalam rentang tahun 2020 - 2026.")]
    public int Angkatan { get; set; } = 2024;

    [Required(ErrorMessage = "Dosen Wali wajib dipilih.")]
    [Range(1, int.MaxValue, ErrorMessage = "Silakan pilih Dosen Wali yang valid.")]
    public int DosenId { get; set; }
}

public record PagedResultDto<T>
{
    public int TotalCount { get; init; }
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

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

public record ApiErrorResponseDto
{
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; init; }
}

