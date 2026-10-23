using WaliSync.Models;

namespace WaliSync.Services;

public interface IMahasiswaService
{
    Task<PagedResultDto<MahasiswaResponseDto>> GetMahasiswasAsync(string? search = null, int page = 1, int pageSize = 10);
    Task<MahasiswaResponseDto?> GetMahasiswaByIdAsync(int id);
    Task<MahasiswaResponseDto> CreateMahasiswaAsync(MahasiswaCreateUpdateDto dto);
    Task<MahasiswaResponseDto> UpdateMahasiswaAsync(int id, MahasiswaCreateUpdateDto dto);
    Task DeleteMahasiswaAsync(int id);
}

