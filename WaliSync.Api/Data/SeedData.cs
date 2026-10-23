using Microsoft.EntityFrameworkCore;
using WaliSync.Api.Models;

namespace WaliSync.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AcademicDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Dosens.AnyAsync())
        {
            return; // Data already seeded
        }

        var d1 = new Dosen { Nidn = "0512345678", NamaDosen = "Dr. Anita Wijaya", Email = "anita@kampus.ac.id", Prodi = "Sistem Informasi" };
        var d2 = new Dosen { Nidn = "0587654321", NamaDosen = "Budi Santoso, M.Kom", Email = "budi@kampus.ac.id", Prodi = "Informatika" };
        var d3 = new Dosen { Nidn = "0534567890", NamaDosen = "Cynthia Tan, S.T., M.T.", Email = "cynthia@kampus.ac.id", Prodi = "Sistem Informasi" };
        var d4 = new Dosen { Nidn = "0545678901", NamaDosen = "Dr. Hendra Gunawan", Email = "hendra@kampus.ac.id", Prodi = "Kedokteran" };
        var d5 = new Dosen { Nidn = "0556789012", NamaDosen = "Siti Rahmawati, M.Ars", Email = "siti@kampus.ac.id", Prodi = "Arsitektur" };
        var d6 = new Dosen { Nidn = "0567890123", NamaDosen = "Agus Pratama, S.E., M.M.", Email = "agus@kampus.ac.id", Prodi = "Manajemen" };

        await context.Dosens.AddRangeAsync(d1, d2, d3, d4, d5, d6);
        await context.SaveChangesAsync();

        var m1 = new Mahasiswa { Nim = "72240675", NamaMahasiswa = "Jimmy Nugraha Putra", Email = "jimmy@kampus.ac.id", Angkatan = 2024, DosenId = d1.DosenId };
        var m2 = new Mahasiswa { Nim = "71230012", NamaMahasiswa = "Ahmad Fauzi", Email = "ahmad@kampus.ac.id", Angkatan = 2023, DosenId = d1.DosenId };
        var m3 = new Mahasiswa { Nim = "71220045", NamaMahasiswa = "Dewi Lestari", Email = "dewi@kampus.ac.id", Angkatan = 2022, DosenId = d2.DosenId };
        var m4 = new Mahasiswa { Nim = "72210088", NamaMahasiswa = "Rian Hidayat", Email = "rian@kampus.ac.id", Angkatan = 2021, DosenId = d2.DosenId };
        var m5 = new Mahasiswa { Nim = "71200034", NamaMahasiswa = "Sarah Amelia", Email = "sarah@kampus.ac.id", Angkatan = 2020, DosenId = d1.DosenId };
        var m6 = new Mahasiswa { Nim = "73250005", NamaMahasiswa = "Kevin Wijaya", Email = "kevin@kampus.ac.id", Angkatan = 2025, DosenId = d4.DosenId };
        var m7 = new Mahasiswa { Nim = "74260010", NamaMahasiswa = "Nabila Putri", Email = "nabila@kampus.ac.id", Angkatan = 2026, DosenId = d5.DosenId };

        await context.Mahasiswas.AddRangeAsync(m1, m2, m3, m4, m5, m6, m7);
        await context.SaveChangesAsync();
    }
}

