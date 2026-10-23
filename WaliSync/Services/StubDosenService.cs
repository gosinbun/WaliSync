using WaliSync.Models;

namespace WaliSync.Services;

public class StubDosenService : IDosenService
{
    private readonly StubDataStore _dataStore;

    public StubDosenService(StubDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<PagedResultDto<DosenResponseDto>> GetDosensAsync(string? search = null, int page = 1, int pageSize = 10)
    {
        var rawDosens = _dataStore.GetDosens();
        var rawMahasiswas = _dataStore.GetMahasiswas();

        var query = rawDosens.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(d =>
                d.NamaDosen.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                d.Nidn.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                d.Prodi.Contains(s, StringComparison.OrdinalIgnoreCase)
            );
        }

        var total = query.Count();

        var items = query
            .OrderBy(d => d.NamaDosen)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DosenResponseDto
            {
                DosenId = d.DosenId,
                Nidn = d.Nidn,
                NamaDosen = d.NamaDosen,
                Email = d.Email,
                Prodi = d.Prodi,
                JumlahMahasiswaBimbingan = rawMahasiswas.Count(m => m.DosenId == d.DosenId)
            })
            .ToList();

        return Task.FromResult(new PagedResultDto<DosenResponseDto>
        {
            TotalCount = total,
            Items = items,
            Page = page,
            PageSize = pageSize
        });
    }

    public Task<DosenResponseDto?> GetDosenByIdAsync(int id)
    {
        var d = _dataStore.FindDosenById(id);
        if (d == null) return Task.FromResult<DosenResponseDto?>(null);

        var rawMahasiswas = _dataStore.GetMahasiswas();
        var dto = new DosenResponseDto
        {
            DosenId = d.DosenId,
            Nidn = d.Nidn,
            NamaDosen = d.NamaDosen,
            Email = d.Email,
            Prodi = d.Prodi,
            JumlahMahasiswaBimbingan = rawMahasiswas.Count(m => m.DosenId == d.DosenId)
        };
        return Task.FromResult<DosenResponseDto?>(dto);
    }

    public Task<List<DosenLookupDto>> GetDosenLookupAsync()
    {
        var list = _dataStore.GetDosens()
            .OrderBy(d => d.NamaDosen)
            .Select(d => new DosenLookupDto
            {
                DosenId = d.DosenId,
                Nidn = d.Nidn,
                NamaDosen = d.NamaDosen,
                Prodi = d.Prodi
            })
            .ToList();

        return Task.FromResult(list);
    }

    public Task<DosenResponseDto> CreateDosenAsync(DosenCreateUpdateDto dto)
    {
        var created = _dataStore.AddDosen(dto);
        return Task.FromResult(new DosenResponseDto
        {
            DosenId = created.DosenId,
            Nidn = created.Nidn,
            NamaDosen = created.NamaDosen,
            Email = created.Email,
            Prodi = created.Prodi,
            JumlahMahasiswaBimbingan = 0
        });
    }

    public Task<DosenResponseDto> UpdateDosenAsync(int id, DosenCreateUpdateDto dto)
    {
        var updated = _dataStore.UpdateDosen(id, dto);
        var rawMahasiswas = _dataStore.GetMahasiswas();
        return Task.FromResult(new DosenResponseDto
        {
            DosenId = updated.DosenId,
            Nidn = updated.Nidn,
            NamaDosen = updated.NamaDosen,
            Email = updated.Email,
            Prodi = updated.Prodi,
            JumlahMahasiswaBimbingan = rawMahasiswas.Count(m => m.DosenId == updated.DosenId)
        });
    }

    public Task DeleteDosenAsync(int id)
    {
        _dataStore.DeleteDosen(id);
        return Task.CompletedTask;
    }
}

