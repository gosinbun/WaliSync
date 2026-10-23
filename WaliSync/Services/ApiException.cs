namespace WaliSync.Services;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public string Detail { get; }

    public ApiException(int statusCode, string message, string detail = "") : base(message)
    {
        StatusCode = statusCode;
        Detail = detail;
    }
}

