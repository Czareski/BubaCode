using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using BubaCode.Models.FilesExplorer;
using BubaCode.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BubaCode.ViewModels.FileExplorer;

public partial class FileViewModel : ViewModelBase, IFileExplorerItem
{
    [ObservableProperty]
    private FileInfo _fileInfo;
    [ObservableProperty]
    private string _name;
    [ObservableProperty] private bool _dirty = false;

    public string FileName => Name;

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

    [RelayCommand]
    public async void Rename()
    {
        string newName = await DialogService.Instance.ShowInputDialogAsync("Rename file", "Enter new name:", GetName());

        if (!String.IsNullOrWhiteSpace(newName))
        {
            File.Move(GetPath(), GetPath().Replace(GetName(), newName));
        }
    }

    [RelayCommand]
    public async void Delete()
    {
        bool result = await DialogService.Instance.ShowAreYouSureDialogAsync("Delete file", $"Are you sure you want to delete {GetName()}?");
        if (result)
        {
            File.Delete(GetPath());
        }
    }
}