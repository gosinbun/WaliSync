namespace WaliSync.Models;

public record ApiErrorResponseDto
{
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; init; }
}

