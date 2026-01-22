using System;
using System.IO;
using BubaCode.Models.FilesExplorer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.ViewModels.FileExplorer;

public partial class FileViewModel : ViewModelBase, IFileExplorerItem
{
    [ObservableProperty]
    private FileInfo _fileInfo;

    [ObservableProperty] private bool _dirty = false;
    public FileViewModel(FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
    }

    public Uri GetUri()
    {
        return new Uri(_fileInfo.FullName);
    }
}