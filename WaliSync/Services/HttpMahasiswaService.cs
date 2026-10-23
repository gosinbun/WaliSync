using System.Net.Http.Json;
using System.Text.Json;
using WaliSync.Models;

namespace WaliSync.Services;

public class HttpMahasiswaService : IMahasiswaService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public HttpMahasiswaService(HttpClient http)
    {
        _http = http;
    }

    public async Task<PagedResultDto<MahasiswaResponseDto>> GetMahasiswasAsync(string? search = null, int page = 1, int pageSize = 10)
    {
        try
        {
            var url = $"api/mahasiswa?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
            {
                url += $"&search={Uri.EscapeDataString(search.Trim())}";
            }

            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResultDto<MahasiswaResponseDto>>(_jsonOptions);
                return result ?? new PagedResultDto<MahasiswaResponseDto>();
            }

            await HandleErrorResponseAsync(response);
            return new PagedResultDto<MahasiswaResponseDto>();
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

    public async Task<MahasiswaResponseDto?> GetMahasiswaByIdAsync(int id)
    {
        try
        {
            var response = await _http.GetAsync($"api/mahasiswa/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MahasiswaResponseDto>(_jsonOptions);
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

    public async Task<MahasiswaResponseDto> CreateMahasiswaAsync(MahasiswaCreateUpdateDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/mahasiswa", dto);
            if (response.IsSuccessStatusCode)
            {
                var created = await response.Content.ReadFromJsonAsync<MahasiswaResponseDto>(_jsonOptions);
                return created!;
            }

            await HandleErrorResponseAsync(response);
            throw new ApiException((int)response.StatusCode, "Gagal menambahkan mahasiswa.");
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

    public async Task<MahasiswaResponseDto> UpdateMahasiswaAsync(int id, MahasiswaCreateUpdateDto dto)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"api/mahasiswa/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                var updated = await response.Content.ReadFromJsonAsync<MahasiswaResponseDto>(_jsonOptions);
                return updated!;
            }

            await HandleErrorResponseAsync(response);
            throw new ApiException((int)response.StatusCode, "Gagal memperbarui mahasiswa.");
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

    public async Task DeleteMahasiswaAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/mahasiswa/{id}");
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
            // Fallback
        }

        throw new ApiException((int)response.StatusCode, $"Permintaan gagal dengan kode HTTP {(int)response.StatusCode}.");
    }
}

