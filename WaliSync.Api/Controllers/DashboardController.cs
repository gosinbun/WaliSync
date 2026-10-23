using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaliSync.Api.Data;
using WaliSync.Api.DTOs;

namespace WaliSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AcademicDbContext _context;

    public DashboardController(AcademicDbContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        var totalDosen = await _context.Dosens.CountAsync();
        var totalMahasiswa = await _context.Mahasiswas.CountAsync();

        // Dosen wali aktif: dosen yang DosenId-nya terdapat pada tabel Mahasiswas
        var activeDosenCount = await _context.Mahasiswas
            .Select(m => m.DosenId)
            .Distinct()
            .CountAsync();

        // Distribusi Angkatan 2020 - 2026
        var angkatanCounts = await _context.Mahasiswas
            .GroupBy(m => m.Angkatan)
            .Select(g => new { Angkatan = g.Key, Count = g.Count() })
            .ToListAsync();

        var distDict = new Dictionary<int, int>();
        for (int y = 2020; y <= 2026; y++)
        {
            var match = angkatanCounts.FirstOrDefault(a => a.Angkatan == y);
            distDict[y] = match?.Count ?? 0;
        }

        // Beban Perwalian per Dosen
        var dosens = await _context.Dosens
            .AsNoTracking()
            .Select(d => new
            {
                d.DosenId,
                d.NamaDosen,
                d.Prodi,
                JumlahBimbingan = d.Mahasiswas.Count
            })
            .OrderByDescending(d => d.JumlahBimbingan)
            .ThenBy(d => d.NamaDosen)
            .ToListAsync();

        var bebanList = dosens.Select(d => new BebanPerwalianItemDto
        {
            DosenId = d.DosenId,
            NamaDosen = d.NamaDosen,
            Prodi = d.Prodi,
            JumlahBimbingan = d.JumlahBimbingan,
            Inisial = GetInitials(d.NamaDosen)
        }).ToList();

        var stats = new DashboardStatsDto
        {
            TotalDosen = totalDosen,
            TotalMahasiswa = totalMahasiswa,
            DosenWaliAktif = activeDosenCount,
            DistribusiAngkatan = distDict,
            BebanPerwalian = bebanList
        };

        return Ok(stats);
    }

    private static string GetInitials(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return "DS";
        var cleaned = fullName.Replace("Dr.", "").Replace("Prof.", "").Replace("M.Kom", "").Replace("S.T.", "").Replace("M.T.", "").Replace("M.Ars", "").Replace("S.E.", "").Replace("M.M.", "").Replace(",", "");
        var parts = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2) return $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[1][0])}";
        if (parts.Length == 1 && parts[0].Length >= 2) return parts[0].Substring(0, 2).ToUpper();
        return "DS";
    }
}

