using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BubaCode.Models.FilesExplorer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.ViewModels.FileExplorer;

public partial class FileExplorerViewModel : ViewModelBase
{
    private FilesService _fileService;
    [ObservableProperty] private FolderViewModel? _rootFolderViewModel;

    [ObservableProperty] private IEnumerable<FolderViewModel> _rootAsList;
    private MainWindowViewModel _mainWindow;
    private Dictionary<Uri, CodeBoxViewModel> _fileCodeBoxesDictionary = new();
    private Dictionary<Uri, FileViewModel> _filesDictionary = new();
    private IFileExplorerItem _selectedItem;
    public IFileExplorerItem SelectedItem
    {
        get => _selectedItem;
        set 
        {
            if (SetProperty(ref _selectedItem, value))
            {
                if (value is FileViewModel fileViewModel)
                {
                    Uri uri = fileViewModel.GetUri();
                    _fileService.CurrentFile = uri;
                    _mainWindow.CodeBoxViewModel.UnsubscribeToFileImported();
                    _fileCodeBoxesDictionary[uri] = new CodeBoxViewModel(_fileService);
                    _filesDictionary[uri] = fileViewModel;
                    _fileService.Import(uri);
                    _mainWindow.CodeBoxViewModel = _fileCodeBoxesDictionary[uri];
                }
            }
        }
    }

    
    public FileExplorerViewModel(FilesService filesService, MainWindowViewModel mainWindowViewModel)
    {
        _mainWindow = mainWindowViewModel;
        _fileService = filesService;
        _fileService.FolderOpened += InitializeTree;
        _fileService.SetFileDirty += SetFileDirty;
    }

    public void InitializeTree(DirectoryInfo rootFolder)
    {
        RootFolderViewModel = new FolderViewModel(rootFolder);
        RootAsList = new[] { RootFolderViewModel };
    }

    public void SetFileDirty(bool value)
    {
        if (_fileService.CurrentFile == null) return;
        _filesDictionary.TryGetValue(_fileService.CurrentFile, out var fileViewModel);
        if (fileViewModel != null)
        {
            fileViewModel.Dirty = value;
        }
    }
}