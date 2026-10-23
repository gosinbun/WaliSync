namespace WaliSync.Services;

public enum ToastType
{
    Success,
    Error
}

public class ToastMessage
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Message { get; init; } = string.Empty;
    public ToastType Type { get; init; } = ToastType.Success;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}

public class ToastService
{
    public event Action? OnChange;
    private readonly List<ToastMessage> _toasts = new();

    public IReadOnlyList<ToastMessage> Toasts
    {
        get
        {
            lock (_toasts)
            {
                return _toasts.ToList();
            }
        }
    }

    public void ShowSuccess(string message)
    {
        AddToast(message, ToastType.Success);
    }

    public void ShowError(string message)
    {
        AddToast(message, ToastType.Error);
    }

    private void AddToast(string message, ToastType type)
    {
        var toast = new ToastMessage { Message = message, Type = type };
        lock (_toasts)
        {
            _toasts.Add(toast);
        }
        OnChange?.Invoke();

        // Auto dismiss after 3.5 seconds
        _ = Task.Run(async () =>
        {
            await Task.Delay(3500);
            RemoveToast(toast.Id);
        });
    }

    public void RemoveToast(Guid id)
    {
        lock (_toasts)
        {
            var item = _toasts.FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                _toasts.Remove(item);
                OnChange?.Invoke();
            }
        }
    }
}

