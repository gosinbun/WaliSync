using System.Net.Http.Json;
using System.Text.Json;
using WaliSync.Models;

namespace WaliSync.Services;

public class HttpDosenService : IDosenService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public HttpDosenService(HttpClient http)
    {
        _http = http;
    }

    public async Task<PagedResultDto<DosenResponseDto>> GetDosensAsync(string? search = null, int page = 1, int pageSize = 10)
    {
        try
        {
            var url = $"api/dosen?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
            {
                url += $"&search={Uri.EscapeDataString(search.Trim())}";
            }

            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResultDto<DosenResponseDto>>(_jsonOptions);
                return result ?? new PagedResultDto<DosenResponseDto>();
            }

            await HandleErrorResponseAsync(response);
            return new PagedResultDto<DosenResponseDto>();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(503, "Gagal terhubung ke server API. Pastikan backend aktif.", ex.Message);
        }
        catch (Exception ex)
        {
            throw new ApiException(500, $"Terjadi kesalahan saat memuat data: {ex.Message}");
        }
    }

    public async Task<DosenResponseDto?> GetDosenByIdAsync(int id)
    {
        try
        {
            var response = await _http.GetAsync($"api/dosen/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DosenResponseDto>(_jsonOptions);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            await HandleErrorResponseAsync(response);
            return null;
        }
        catch (ApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(503, "Gagal terhubung ke server API.", ex.Message);
        }
    }

    public async Task<List<DosenLookupDto>> GetDosenLookupAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/dosen/lookup");
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadFromJsonAsync<List<DosenLookupDto>>(_jsonOptions);
                return list ?? new List<DosenLookupDto>();
            }

            await HandleErrorResponseAsync(response);
            return new List<DosenLookupDto>();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(503, "Gagal terhubung ke server API saat memuat dosen.", ex.Message);
        }
    }

    public async Task<DosenResponseDto> CreateDosenAsync(DosenCreateUpdateDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/dosen", dto);
            if (response.IsSuccessStatusCode)
            {
                var created = await response.Content.ReadFromJsonAsync<DosenResponseDto>(_jsonOptions);
                return created!;
            }

            await HandleErrorResponseAsync(response);
            throw new ApiException((int)response.StatusCode, "Gagal menambahkan dosen.");
        }
        catch (ApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(503, "Gagal terhubung ke server API.", ex.Message);
        }
    }

    public async Task<DosenResponseDto> UpdateDosenAsync(int id, DosenCreateUpdateDto dto)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"api/dosen/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                var updated = await response.Content.ReadFromJsonAsync<DosenResponseDto>(_jsonOptions);
                return updated!;
            }

            await HandleErrorResponseAsync(response);
            throw new ApiException((int)response.StatusCode, "Gagal memperbarui dosen.");
        }
        catch (ApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(503, "Gagal terhubung ke server API.", ex.Message);
        }
    }

    public async Task DeleteDosenAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/dosen/{id}");
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            await HandleErrorResponseAsync(response);
        }
        catch (ApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(503, "Gagal terhubung ke server API.", ex.Message);
        }
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response)
    {
        try
        {
            var errorDto = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(_jsonOptions);
            if (errorDto != null && !string.IsNullOrWhiteSpace(errorDto.Message))
            {
                throw new ApiException((int)response.StatusCode, errorDto.Message);
            }
        }
        catch (ApiException)
        {
            throw;
        }
        catch
        {
            // If response body is not ApiErrorResponseDto
        }

        throw new ApiException((int)response.StatusCode, $"Permintaan gagal dengan kode HTTP {(int)response.StatusCode}.");
    }
}

