using System;

namespace BubaCode.ViewModels;

public interface IErrorService
{
    event Action<string>? ErrorReceived;

    public abstract void ReportError(string message);
}