using Microsoft.EntityFrameworkCore;
using WaliSync.Api.Models;

namespace WaliSync.Api.Data;

public class AcademicDbContext : DbContext
{
    public AcademicDbContext(DbContextOptions<AcademicDbContext> options) : base(options)
    {
    }

    public DbSet<Dosen> Dosens => Set<Dosen>();
    public DbSet<Mahasiswa> Mahasiswas => Set<Mahasiswa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique Index for NIDN
        modelBuilder.Entity<Dosen>(entity =>
        {
            entity.HasIndex(e => e.Nidn).IsUnique();
        });

        // Unique Index for NIM
        modelBuilder.Entity<Mahasiswa>(entity =>
        {
            entity.HasIndex(e => e.Nim).IsUnique();

            // Strict Restrict Delete Behavior on Dosen -> Mahasiswa
            entity.HasOne(m => m.Dosen)
                  .WithMany(d => d.Mahasiswas)
                  .HasForeignKey(m => m.DosenId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

