using System;

namespace BubaCode.ViewModels;

public class ErrorService : IErrorService
{
    public event Action<string>? ErrorReceived;
    public void ReportError(string message)
    {
        ErrorReceived?.Invoke(message);
    }
}