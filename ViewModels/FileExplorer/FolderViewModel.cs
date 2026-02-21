using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using BubaCode.Models.FilesExplorer;
using BubaCode.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BubaCode.ViewModels.FileExplorer;

public partial class FolderViewModel : ViewModelBase, IFileExplorerItem
{
    private DirectoryInfo _folderInfo;
    [ObservableProperty]
    private bool _isExpanded = true;
    [ObservableProperty]
    private ObservableCollection<IFileExplorerItem> _children = new();
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _fontWeight = "Normal";
    private bool _isRoot;

    public string FolderName => Name;

    public FolderViewModel(DirectoryInfo directoryInfo, FileExplorerService fileExplorer, bool isRoot = false)
    {
        _folderInfo = directoryInfo;
        _name =  directoryInfo.Name;
        _isRoot = isRoot;
        PopulateChildren(fileExplorer);

        if (isRoot)
        {
            _fontWeight = "Bold";
            _name = _name.ToUpper();
        }
    }
    
    private void PopulateChildren(FileExplorerService fileExplorer)
    {
        _folderInfo.Refresh();
        if (!_folderInfo.Exists)
        {
            return;
        }
        var items = _folderInfo.GetFileSystemInfos();
        try
        {
            Children.Clear();

            var directories = items.OfType<DirectoryInfo>().OrderBy(d => d.Name);
            foreach (var directory in directories)
            {
                FolderViewModel folderViewModel = new FolderViewModel(directory, fileExplorer);
                Children.Add(folderViewModel);
                fileExplorer.AddItem(folderViewModel);
            }

            var files = items.OfType<FileInfo>().OrderBy(f => f.Name);
            foreach (var file in files)
            {
                FileViewModel fileViewModel = new FileViewModel(file);
                Children.Add(fileViewModel);
                fileExplorer.AddItem(fileViewModel);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public void UpdateUri(string newPath, string name)
    {
        _folderInfo = new DirectoryInfo(Path.Combine(newPath, name));     
        Name = _folderInfo.Name;
        if (_isRoot)
        {
            Name = Name.ToUpper();
        }
    }

    public string GetName()
    {
        return Name;
    }

    public string GetPath()
    {
        return PathUtils.NormalizePath(_folderInfo.FullName);
    }

    public string GetParentPath()
    {
        return PathUtils.NormalizePath(_folderInfo!.Parent!.FullName);
    }

    [RelayCommand]
    public async void Rename()
    {
        string newName = await DialogService.Instance.ShowInputDialogAsync("Rename Directory", "Enter new name:", GetName());

        if (!String.IsNullOrWhiteSpace(newName))
        {
            Directory.Move(GetPath(), GetPath().Replace(GetName(), newName));
        }
    }

    [RelayCommand]
    public async void Delete()
    {
        bool result = await DialogService.Instance.ShowAreYouSureDialogAsync("Delete Directory", $"Are you sure you want to delete {GetName()}?");
        if (result)
        {
            
            Directory.Delete(GetPath(), true);
        }
    }

    [RelayCommand]
    public async void AddNewFile()
    {
        string fileName = await DialogService.Instance.ShowInputDialogAsync("Create file", "Enter a new file name:");
        if (!String.IsNullOrWhiteSpace(fileName))
        {
            File.Create(Path.Combine(GetPath(), fileName)).Dispose();
        }
    }
    [RelayCommand]
    public async void AddNewFolder()
    {
        string directoryName = await DialogService.Instance.ShowInputDialogAsync("Create directory", "Enter a new directory name:");
        if (!String.IsNullOrWhiteSpace(directoryName))
        {
            Directory.CreateDirectory(Path.Combine(GetPath(), directoryName));
        }
    }
}