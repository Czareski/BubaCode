using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BubaCode.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    
    [ObservableProperty] public string _code = "";
    private FileDialogService? _fileService;

    public MainWindowViewModel(FileDialogService service)
    {
        _fileService = service;
    }
    [RelayCommand]
    public void Save(CodeBox cb)
    {
        Debug.WriteLine(cb.Export());
    }
    [RelayCommand]
    public async void Open(CodeBox cb)
    {

        Uri file = await _fileService.PickFileAsync();
        if (file != null)
        {
            try
            {
                IEnumerable<string> lines = File.ReadLines(file.AbsolutePath);
                cb.Import(lines);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
                return;
            }
        }
    }
}
