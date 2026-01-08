using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BubaCode.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BubaCode.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ErrorViewModel _errorViewModel;
    [ObservableProperty]
    private FileBarViewModel _fileBarViewModel;
    [ObservableProperty]
    private CodeBoxViewModel _codeBoxViewModel;
    private FilesService? _fileService;
    private IErrorService? _errorService;

    public MainWindowViewModel()
    {
        
        _errorService = new ErrorService();
        _errorViewModel = new ErrorViewModel(_errorService);
        
        _fileService = new FilesService(_errorService);
        _fileBarViewModel = new FileBarViewModel(_fileService);
        _codeBoxViewModel = new CodeBoxViewModel(_fileService);
    }
}
