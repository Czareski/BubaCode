using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace BubaCode.ViewModels;

public class FileDialogService
{
    private Window? _window;
    private Uri currentFile;
    public FileDialogService(Window window)
    {
        _window = window;
    }

    public async Task<Uri> PickFileAsync()
    {
        if (_window is null)
        {
            throw new Exception("Window not initialized");
            return null;
        }
        var files = await _window.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                AllowMultiple = false
            });

        return files.FirstOrDefault()?.Path;
    }
}