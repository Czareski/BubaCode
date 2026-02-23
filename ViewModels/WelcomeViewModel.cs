using System;
using System.IO;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BubaCode.ViewModels;

public partial class WelcomeViewModel : ViewModelBase
{
    [ObservableProperty] private bool _showWelcomeView = true;
    [ObservableProperty] private bool _showButton = true;
    [ObservableProperty] private string _text = "Witaj w BubaCode! Wybierz projekt do edycji...";
    private FilesService _fileService;

    public WelcomeViewModel(FilesService fileService)
    {
        _fileService = fileService;
        _fileService.FolderOpened += OnFolderOpened;
        _fileService.FileImported += OnFileOpened;
    }

    private void OnFolderOpened(DirectoryInfo info)
    {
        Text = "Miłego tworzenia!";
        ShowButton = false;
        _fileService.FolderOpened -= OnFolderOpened;
    }
    private void OnFileOpened(Uri uri)
    {
        ShowWelcomeView = false;
        _fileService.FileImported -= OnFileOpened;
    }
    [RelayCommand]
    public async void OpenProject(Window window)
    {
        await _fileService.PickFileAsync(window);
    }
    
}
