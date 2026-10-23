using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaliSync.Api.Models;

[Table("Mahasiswas")]
public class Mahasiswa
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MahasiswaId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Nim { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string NamaMahasiswa { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Range(2020, 2026)]
    public int Angkatan { get; set; }

    [Required]
    public int DosenId { get; set; }

    // Navigation property
    [ForeignKey(nameof(DosenId))]
    public Dosen? Dosen { get; set; }
}

