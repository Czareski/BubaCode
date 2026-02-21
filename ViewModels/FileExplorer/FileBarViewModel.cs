using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using BubaCode.ViewModels.FileExplorer;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BubaCode.ViewModels;

public partial class FileBarViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _fileName = "New File";

    [ObservableProperty] private string _filePath = "";
    private FilesService _fileService;
    private FileExplorerViewModel _fileExplorerViewModel;

    public FileBarViewModel(FilesService fileService, FileExplorerViewModel fileExplorerViewModel)
    {
        _fileService = fileService;
        _fileExplorerViewModel = fileExplorerViewModel;
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