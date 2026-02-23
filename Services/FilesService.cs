using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.ViewModels;

public class FilesService
{
     
    public Uri CurrentFile;
    public event Action<Uri>? FileImported;
    public event Action<DirectoryInfo>? FolderOpened;
    public Action<bool>? SetFileDirty;
    public Func<string>? GetSourceToExport;
    private IErrorService _errorService;

    public FilesService(IErrorService errorService)
    {
        _errorService = errorService;
    }

    public void Import(Uri importedFile)
    {
        CurrentFile = importedFile;
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

        if (CurrentFile == null)
        {
            _errorService.ReportError("No file selected");
            return;
        }
        FileStream stream = File.Open(CurrentFile.LocalPath, FileMode.OpenOrCreate);
        foreach (byte character in text.ToString())
        {
            stream.WriteByte(character);
        }
        stream.Close();
        SetFileDirty?.Invoke(false);
    }

    public async Task OpenProject(Window window)
    {
        var folders = await window.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                AllowMultiple = false
            });
        try
        {
            var path = folders.FirstOrDefault().Path;
            DirectoryInfo directory = new DirectoryInfo(path.AbsolutePath);
            FolderOpened?.Invoke(directory);           
        }
        catch (Exception ex)
        {
            _errorService.ReportError("No folder selected");
            Debug.WriteLine(ex.Message);
        }
    }
    
    

}