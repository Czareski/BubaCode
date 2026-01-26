using System;
using System.Diagnostics;
using System.IO;
using BubaCode.Models.FilesExplorer;
using BubaCode.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.ViewModels.FileExplorer;

public partial class FileViewModel : ViewModelBase, IFileExplorerItem
{
    [ObservableProperty]
    private FileInfo _fileInfo;
    [ObservableProperty]
    private string _name;
    [ObservableProperty] private bool _dirty = false;
    public FileViewModel(FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
        _name = fileInfo.Name;
        
    }
    public void UpdateUri(string newPath, string name)
    { 
        FileInfo = new FileInfo(Path.Combine(newPath, name));
        Name = FileInfo.Name;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetPath()
    {
        return PathUtils.NormalizePath(_fileInfo.FullName);
    }

    public string GetParentPath()
    {
        return PathUtils.NormalizePath(FileInfo!.Directory!.FullName);
    }
}