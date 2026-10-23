using WaliSync.Models;

namespace WaliSync.Services;

public class InternalDosen
{
    public int DosenId { get; set; }
    public string Nidn { get; set; } = string.Empty;
    public string NamaDosen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Prodi { get; set; } = string.Empty;
}

public class InternalMahasiswa
{
    public int MahasiswaId { get; set; }
    public string Nim { get; set; } = string.Empty;
    public string NamaMahasiswa { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Angkatan { get; set; }
    public int DosenId { get; set; }
}

public class StubDataStore
{
    private readonly object _lock = new();
    private readonly List<InternalDosen> _dosens = new();
    private readonly List<InternalMahasiswa> _mahasiswas = new();
    private int _nextDosenId = 1;
    private int _nextMahasiswaId = 1;

    public StubDataStore()
    {
        SeedInitialData();
    }

    private void SeedInitialData()
    {
        lock (_lock)
        {
            var d1 = new InternalDosen { DosenId = _nextDosenId++, Nidn = "0512345678", NamaDosen = "Dr. Anita Wijaya", Email = "anita@kampus.ac.id", Prodi = "Sistem Informasi" };
            var d2 = new InternalDosen { DosenId = _nextDosenId++, Nidn = "0587654321", NamaDosen = "Budi Santoso, M.Kom", Email = "budi@kampus.ac.id", Prodi = "Informatika" };
            var d3 = new InternalDosen { DosenId = _nextDosenId++, Nidn = "0534567890", NamaDosen = "Cynthia Tan, S.T., M.T.", Email = "cynthia@kampus.ac.id", Prodi = "Sistem Informasi" };
            var d4 = new InternalDosen { DosenId = _nextDosenId++, Nidn = "0545678901", NamaDosen = "Dr. Hendra Gunawan", Email = "hendra@kampus.ac.id", Prodi = "Kedokteran" };
            var d5 = new InternalDosen { DosenId = _nextDosenId++, Nidn = "0556789012", NamaDosen = "Siti Rahmawati, M.Ars", Email = "siti@kampus.ac.id", Prodi = "Arsitektur" };
            var d6 = new InternalDosen { DosenId = _nextDosenId++, Nidn = "0567890123", NamaDosen = "Agus Pratama, S.E., M.M.", Email = "agus@kampus.ac.id", Prodi = "Manajemen" };

            _dosens.AddRange(new[] { d1, d2, d3, d4, d5, d6 });

            var m1 = new InternalMahasiswa { MahasiswaId = _nextMahasiswaId++, Nim = "72240675", NamaMahasiswa = "Jimmy Nugraha Putra", Email = "jimmy@kampus.ac.id", Angkatan = 2024, DosenId = d1.DosenId };
            var m2 = new InternalMahasiswa { MahasiswaId = _nextMahasiswaId++, Nim = "71230012", NamaMahasiswa = "Ahmad Fauzi", Email = "ahmad@kampus.ac.id", Angkatan = 2023, DosenId = d1.DosenId };
            var m3 = new InternalMahasiswa { MahasiswaId = _nextMahasiswaId++, Nim = "71220045", NamaMahasiswa = "Dewi Lestari", Email = "dewi@kampus.ac.id", Angkatan = 2022, DosenId = d2.DosenId };
            var m4 = new InternalMahasiswa { MahasiswaId = _nextMahasiswaId++, Nim = "72210088", NamaMahasiswa = "Rian Hidayat", Email = "rian@kampus.ac.id", Angkatan = 2021, DosenId = d2.DosenId };
            var m5 = new InternalMahasiswa { MahasiswaId = _nextMahasiswaId++, Nim = "71200034", NamaMahasiswa = "Sarah Amelia", Email = "sarah@kampus.ac.id", Angkatan = 2020, DosenId = d1.DosenId };
            var m6 = new InternalMahasiswa { MahasiswaId = _nextMahasiswaId++, Nim = "73250005", NamaMahasiswa = "Kevin Wijaya", Email = "kevin@kampus.ac.id", Angkatan = 2025, DosenId = d4.DosenId };
            var m7 = new InternalMahasiswa { MahasiswaId = _nextMahasiswaId++, Nim = "74260010", NamaMahasiswa = "Nabila Putri", Email = "nabila@kampus.ac.id", Angkatan = 2026, DosenId = d5.DosenId };

            _mahasiswas.AddRange(new[] { m1, m2, m3, m4, m5, m6, m7 });
        }
    }

    public List<InternalDosen> GetDosens() { lock (_lock) { return _dosens.ToList(); } }
    public List<InternalMahasiswa> GetMahasiswas() { lock (_lock) { return _mahasiswas.ToList(); } }

    public InternalDosen? FindDosenById(int id) { lock (_lock) { return _dosens.FirstOrDefault(d => d.DosenId == id); } }
    public InternalMahasiswa? FindMahasiswaById(int id) { lock (_lock) { return _mahasiswas.FirstOrDefault(m => m.MahasiswaId == id); } }

    public InternalDosen AddDosen(DosenCreateUpdateDto dto)
    {
        lock (_lock)
        {
            var cleanNidn = dto.Nidn.Trim();
            if (_dosens.Any(d => d.Nidn.Equals(cleanNidn, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiException(409, $"NIDN '{cleanNidn}' sudah terdaftar dalam sistem.");
            }

            var cleanEmail = dto.Email.Trim().ToLowerInvariant();
            if (_dosens.Any(d => d.Email.Equals(cleanEmail, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiException(409, $"Email '{cleanEmail}' sudah terdaftar dalam sistem.");
            }

            var entity = new InternalDosen
            {
                DosenId = _nextDosenId++,
                Nidn = cleanNidn,
                NamaDosen = dto.NamaDosen.Trim(),
                Email = cleanEmail,
                Prodi = dto.Prodi.Trim()
            };
            _dosens.Add(entity);
            return entity;
        }
    }

    public InternalDosen UpdateDosen(int id, DosenCreateUpdateDto dto)
    {
        lock (_lock)
        {
            var entity = _dosens.FirstOrDefault(d => d.DosenId == id)
                ?? throw new ApiException(404, $"Dosen dengan ID {id} tidak ditemukan.");

            var cleanNidn = dto.Nidn.Trim();
            if (_dosens.Any(d => d.DosenId != id && d.Nidn.Equals(cleanNidn, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiException(409, $"NIDN '{cleanNidn}' sudah digunakan oleh dosen lain.");
            }

            var cleanEmail = dto.Email.Trim().ToLowerInvariant();
            if (_dosens.Any(d => d.DosenId != id && d.Email.Equals(cleanEmail, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiException(409, $"Email '{cleanEmail}' sudah digunakan oleh dosen lain.");
            }

            entity.Nidn = cleanNidn;
            entity.NamaDosen = dto.NamaDosen.Trim();
            entity.Email = cleanEmail;
            entity.Prodi = dto.Prodi.Trim();
            return entity;
        }
    }

    public void DeleteDosen(int id)
    {
        lock (_lock)
        {
            var entity = _dosens.FirstOrDefault(d => d.DosenId == id)
                ?? throw new ApiException(404, $"Dosen dengan ID {id} tidak ditemukan.");

            // Relational Integrity Constraint: Check if Dosen is still actively assigned to any student
            var bimbinganCount = _mahasiswas.Count(m => m.DosenId == id);
            if (bimbinganCount > 0)
            {
                throw new ApiException(409, $"Operasi ditolak. Dosen masih menjadi wali aktif bagi {bimbinganCount} mahasiswa.");
            }

            _dosens.Remove(entity);
        }
    }

    public InternalMahasiswa AddMahasiswa(MahasiswaCreateUpdateDto dto)
    {
        lock (_lock)
        {
            var cleanNim = dto.Nim.Trim();
            if (_mahasiswas.Any(m => m.Nim.Equals(cleanNim, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiException(409, $"NIM '{cleanNim}' sudah terdaftar dalam sistem.");
            }

            var cleanEmail = dto.Email.Trim().ToLowerInvariant();
            if (_mahasiswas.Any(m => m.Email.Equals(cleanEmail, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiException(409, $"Email '{cleanEmail}' sudah terdaftar dalam sistem.");
            }

            if (!_dosens.Any(d => d.DosenId == dto.DosenId))
            {
                throw new ApiException(400, "Dosen Wali yang dipilih tidak valid.");
            }

            var entity = new InternalMahasiswa
            {
                MahasiswaId = _nextMahasiswaId++,
                Nim = cleanNim,
                NamaMahasiswa = dto.NamaMahasiswa.Trim(),
                Email = cleanEmail,
                Angkatan = dto.Angkatan,
                DosenId = dto.DosenId
            };
            _mahasiswas.Add(entity);
            return entity;
        }
    }

    public InternalMahasiswa UpdateMahasiswa(int id, MahasiswaCreateUpdateDto dto)
    {
        lock (_lock)
        {
            var entity = _mahasiswas.FirstOrDefault(m => m.MahasiswaId == id)
                ?? throw new ApiException(404, $"Mahasiswa dengan ID {id} tidak ditemukan.");

            var cleanNim = dto.Nim.Trim();
            if (_mahasiswas.Any(m => m.MahasiswaId != id && m.Nim.Equals(cleanNim, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiException(409, $"NIM '{cleanNim}' sudah digunakan oleh mahasiswa lain.");
            }

            var cleanEmail = dto.Email.Trim().ToLowerInvariant();
            if (_mahasiswas.Any(m => m.MahasiswaId != id && m.Email.Equals(cleanEmail, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiException(409, $"Email '{cleanEmail}' sudah digunakan oleh mahasiswa lain.");
            }

            if (!_dosens.Any(d => d.DosenId == dto.DosenId))
            {
                throw new ApiException(400, "Dosen Wali yang dipilih tidak valid.");
            }

            entity.Nim = cleanNim;
            entity.NamaMahasiswa = dto.NamaMahasiswa.Trim();
            entity.Email = cleanEmail;
            entity.Angkatan = dto.Angkatan;
            entity.DosenId = dto.DosenId;
            return entity;
        }
    }

    public void DeleteMahasiswa(int id)
    {
        lock (_lock)
        {
            var entity = _mahasiswas.FirstOrDefault(m => m.MahasiswaId == id)
                ?? throw new ApiException(404, $"Mahasiswa dengan ID {id} tidak ditemukan.");

            _mahasiswas.Remove(entity);
        }
    }
}

