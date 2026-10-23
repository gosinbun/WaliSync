using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaliSync.Api.Models;

[Table("Dosens")]
public class Dosen
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DosenId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Nidn { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string NamaDosen { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Prodi { get; set; } = string.Empty;

    // Relasi One-to-Many ke Mahasiswa
    public ICollection<Mahasiswa> Mahasiswas { get; set; } = new List<Mahasiswa>();
}

