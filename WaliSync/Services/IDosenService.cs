using WaliSync.Models;

namespace WaliSync.Services;

public interface IDosenService
{
    Task<PagedResultDto<DosenResponseDto>> GetDosensAsync(string? search = null, int page = 1, int pageSize = 10);
    Task<DosenResponseDto?> GetDosenByIdAsync(int id);
    Task<List<DosenLookupDto>> GetDosenLookupAsync();
    Task<DosenResponseDto> CreateDosenAsync(DosenCreateUpdateDto dto);
    Task<DosenResponseDto> UpdateDosenAsync(int id, DosenCreateUpdateDto dto);
    Task DeleteDosenAsync(int id);
}

