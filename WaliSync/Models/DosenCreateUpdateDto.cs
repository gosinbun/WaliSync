using System.ComponentModel.DataAnnotations;

namespace WaliSync.Models;

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

