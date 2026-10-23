using WaliSync.Models;

namespace WaliSync.Services;

public class StubMahasiswaService : IMahasiswaService
{
    private readonly StubDataStore _dataStore;

    public StubMahasiswaService(StubDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<PagedResultDto<MahasiswaResponseDto>> GetMahasiswasAsync(string? search = null, int page = 1, int pageSize = 10)
    {
        var rawMahasiswas = _dataStore.GetMahasiswas();
        var rawDosens = _dataStore.GetDosens();

        var query = rawMahasiswas.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(m =>
                m.NamaMahasiswa.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                m.Nim.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                m.Angkatan.ToString().Contains(s)
            );
        }

        var total = query.Count();

        var items = query
            .OrderByDescending(m => m.Angkatan)
            .ThenBy(m => m.NamaMahasiswa)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m =>
            {
                var dosen = rawDosens.FirstOrDefault(d => d.DosenId == m.DosenId);
                return new MahasiswaResponseDto
                {
                    MahasiswaId = m.MahasiswaId,
                    Nim = m.Nim,
                    NamaMahasiswa = m.NamaMahasiswa,
                    Email = m.Email,
                    Angkatan = m.Angkatan,
                    DosenId = m.DosenId,
                    NamaDosenWali = dosen?.NamaDosen ?? "-"
                };
            })
            .ToList();

        return Task.FromResult(new PagedResultDto<MahasiswaResponseDto>
        {
            TotalCount = total,
            Items = items,
            Page = page,
            PageSize = pageSize
        });
    }

    public Task<MahasiswaResponseDto?> GetMahasiswaByIdAsync(int id)
    {
        var m = _dataStore.FindMahasiswaById(id);
        if (m == null) return Task.FromResult<MahasiswaResponseDto?>(null);

        var dosen = _dataStore.FindDosenById(m.DosenId);
        return Task.FromResult<MahasiswaResponseDto?>(new MahasiswaResponseDto
        {
            MahasiswaId = m.MahasiswaId,
            Nim = m.Nim,
            NamaMahasiswa = m.NamaMahasiswa,
            Email = m.Email,
            Angkatan = m.Angkatan,
            DosenId = m.DosenId,
            NamaDosenWali = dosen?.NamaDosen ?? "-"
        });
    }

    public Task<MahasiswaResponseDto> CreateMahasiswaAsync(MahasiswaCreateUpdateDto dto)
    {
        var created = _dataStore.AddMahasiswa(dto);
        var dosen = _dataStore.FindDosenById(created.DosenId);
        return Task.FromResult(new MahasiswaResponseDto
        {
            MahasiswaId = created.MahasiswaId,
            Nim = created.Nim,
            NamaMahasiswa = created.NamaMahasiswa,
            Email = created.Email,
            Angkatan = created.Angkatan,
            DosenId = created.DosenId,
            NamaDosenWali = dosen?.NamaDosen ?? "-"
        });
    }

    public Task<MahasiswaResponseDto> UpdateMahasiswaAsync(int id, MahasiswaCreateUpdateDto dto)
    {
        var updated = _dataStore.UpdateMahasiswa(id, dto);
        var dosen = _dataStore.FindDosenById(updated.DosenId);
        return Task.FromResult(new MahasiswaResponseDto
        {
            MahasiswaId = updated.MahasiswaId,
            Nim = updated.Nim,
            NamaMahasiswa = updated.NamaMahasiswa,
            Email = updated.Email,
            Angkatan = updated.Angkatan,
            DosenId = updated.DosenId,
            NamaDosenWali = dosen?.NamaDosen ?? "-"
        });
    }

    public Task DeleteMahasiswaAsync(int id)
    {
        _dataStore.DeleteMahasiswa(id);
        return Task.CompletedTask;
    }
}

