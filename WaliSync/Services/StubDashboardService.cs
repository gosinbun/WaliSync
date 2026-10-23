using WaliSync.Models;

namespace WaliSync.Services;

public class StubDashboardService : IDashboardService
{
    private readonly StubDataStore _dataStore;

    public StubDashboardService(StubDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var dosens = _dataStore.GetDosens();
        var mahasiswas = _dataStore.GetMahasiswas();

        var totalDosen = dosens.Count;
        var totalMahasiswa = mahasiswas.Count;

        // Dosen wali aktif = dosen yang memiliki minimal 1 mahasiswa bimbingan
        var activeDosenIds = mahasiswas.Select(m => m.DosenId).Distinct().ToHashSet();
        var activeDosenCount = dosens.Count(d => activeDosenIds.Contains(d.DosenId));

        // Distribusi Angkatan 2020-2026
        var angkatanDist = new Dictionary<int, int>();
        for (int year = 2020; year <= 2026; year++)
        {
            angkatanDist[year] = mahasiswas.Count(m => m.Angkatan == year);
        }

        // Beban Perwalian
        var bebanList = dosens
            .Select(d =>
            {
                var count = mahasiswas.Count(m => m.DosenId == d.DosenId);
                var initials = GetInitials(d.NamaDosen);
                return new BebanPerwalianItemDto
                {
                    DosenId = d.DosenId,
                    NamaDosen = d.NamaDosen,
                    Prodi = d.Prodi,
                    JumlahBimbingan = count,
                    Inisial = initials
                };
            })
            .OrderByDescending(b => b.JumlahBimbingan)
            .ThenBy(b => b.NamaDosen)
            .ToList();

        var stats = new DashboardStatsDto
        {
            TotalDosen = totalDosen,
            TotalMahasiswa = totalMahasiswa,
            DosenWaliAktif = activeDosenCount,
            DistribusiAngkatan = angkatanDist,
            BebanPerwalian = bebanList
        };

        return Task.FromResult(stats);
    }

    private static string GetInitials(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return "DS";
        var parts = fullName
            .Replace("Dr.", "")
            .Replace("Prof.", "")
            .Replace("M.Kom", "")
            .Replace("S.T.", "")
            .Replace("M.T.", "")
            .Replace("M.Ars", "")
            .Replace("S.E.", "")
            .Replace("M.M.", "")
            .Replace(",", "")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length >= 2)
        {
            return $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[1][0])}";
        }
        else if (parts.Length == 1 && parts[0].Length >= 2)
        {
            return parts[0].Substring(0, 2).ToUpper();
        }
        return "DS";
    }
}

