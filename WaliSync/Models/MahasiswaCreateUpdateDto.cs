using System.ComponentModel.DataAnnotations;

namespace WaliSync.Models;

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

