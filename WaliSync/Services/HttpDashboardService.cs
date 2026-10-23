using System.Net.Http.Json;
using System.Text.Json;
using WaliSync.Models;

namespace WaliSync.Services;

public class HttpDashboardService : IDashboardService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public HttpDashboardService(HttpClient http)
    {
        _http = http;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/dashboard/stats");
            if (response.IsSuccessStatusCode)
            {
                var stats = await response.Content.ReadFromJsonAsync<DashboardStatsDto>(_jsonOptions);
                return stats ?? new DashboardStatsDto();
            }

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

            throw new ApiException((int)response.StatusCode, "Gagal mengambil data statistik dashboard.");
        }
        catch (ApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(503, "Gagal terhubung ke server API. Pastikan server backend sedang aktif.", ex.Message);
        }
        catch (Exception ex)
        {
            throw new ApiException(500, $"Terjadi kesalahan: {ex.Message}");
        }
    }
}

