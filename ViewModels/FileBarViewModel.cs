using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BubaCode.ViewModels;

public partial class FileBarViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _fileName = "some Name";

    [ObservableProperty] private string _filePath = "some Path";
    private FilesService _fileService;
    public FileBarViewModel(FilesService fileService)
    {
        _fileService = fileService;
        _fileService.FileImported += SetFilePath;
    }
    
    [RelayCommand]
    public void Save()
    {
        _fileService.Export();
    }

    [RelayCommand]
    public async void Open(Window window)
    {
        await _fileService.PickFileAsync(window);
    }
    
    public void SetFilePath(Uri importedFile)
    {
        FileName = Path.GetFileName(importedFile.LocalPath);
        FilePath = importedFile.LocalPath.Remove(importedFile.LocalPath.Length - FileName.Length).Replace('\\', '/');
    }
}