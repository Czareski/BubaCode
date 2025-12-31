using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace BubaCode.ViewModels;

public class FilesService
{
    
    private Uri currentFile;
    public event Action<Uri>? FileImported;
    public Func<string>? GetSourceToExport;
    private IErrorService _errorService;

    public FilesService(IErrorService errorService)
    {
        _errorService = errorService;
    }
    public async Task PickFileAsync(Window window)
    {
        var files = await window.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                AllowMultiple = false
            });

        try
        {
            currentFile = files.FirstOrDefault().Path;
        }
        catch (NullReferenceException ex)
        {
            _errorService.ReportError("No file selected");
            return;
        }

    Import(currentFile);
    }

    public void Import(Uri importedFile)
    {
        FileImported?.Invoke(importedFile);
    }


    public void Export()
    {
        string text = GetSourceToExport?.Invoke();
        if (text == null)
        {
            _errorService.ReportError("No source to export");
            return;
        }

        if (currentFile == null)
        {
            _errorService.ReportError("No file selected");
            return;
        }
        FileStream stream = File.Open(currentFile.LocalPath, FileMode.OpenOrCreate);
        foreach (byte character in text.ToString())
        {
            stream.WriteByte(character);
        }
        stream.Close();
    }
    
    
}