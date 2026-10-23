using WaliSync.Models;

namespace WaliSync.Services;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}

