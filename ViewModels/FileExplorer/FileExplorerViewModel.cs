using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BubaCode.Models.FilesExplorer;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BubaCode.ViewModels.FileExplorer;

public partial class FileExplorerViewModel : ViewModelBase
{
    public Action<Uri> FileOpened; 
    [ObservableProperty] 
    private FolderViewModel? _rootFolderViewModel;
    [ObservableProperty] 
    private IEnumerable<FolderViewModel> _rootAsList = null!;
    private FilesService _fileService;
    private FileExplorerService? _fileExplorer;
    private IFileExplorerItem _selectedItem = null!;
    public IFileExplorerItem SelectedItem
    {
        get => _selectedItem;
        set 
        {
            if (SetProperty(ref _selectedItem, value))
            {
                if (value is FileViewModel fileViewModel)
                {
                    string uri = fileViewModel.GetPath();
                    FileOpened?.Invoke(new Uri(uri));
                }
            }
        }
    }
    

    
    public FileExplorerViewModel(FilesService filesService, MainWindowViewModel mainWindowViewModel)
    {
        _fileService = filesService;
        _fileService.FolderOpened += InitializeTree;
        _fileService.SetFileDirty += SetFileDirty;
        
        FileOpened += mainWindowViewModel.SetCodeBoxViewModel;
        FileOpened += filesService.Import;
    }

    public void InitializeTree(DirectoryInfo rootFolder)
    {
        _fileExplorer = new FileExplorerService(rootFolder);
        RootFolderViewModel = new FolderViewModel(rootFolder, _fileExplorer, true);
        _fileExplorer.AddItem(RootFolderViewModel);
        RootAsList = new[] { RootFolderViewModel };
    }

    public void SetFileDirty(bool value)
    {
        if (_fileService.CurrentFile == null) return;
        _fileExplorer.TryGetValue(_fileService.CurrentFile, out var fileViewModel);
        if (fileViewModel != null)
        {
            FileViewModel vm = (FileViewModel)fileViewModel;
            vm.Dirty = value;
        }
    }
    
}