using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;

namespace BubaCode.ViewModels;

public class ClipboardService
{
    private IClipboard _clipboard;
    public static ClipboardService? Instance;
    public ClipboardService(TopLevel topLevel)
    {
        _clipboard = topLevel.Clipboard;
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public Task SetTextAsync(string text)
        => _clipboard.SetTextAsync(text);

    public Task<string?> GetTextAsync()
        => _clipboard.TryGetTextAsync();
}