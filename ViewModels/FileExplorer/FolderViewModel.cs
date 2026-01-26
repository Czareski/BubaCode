using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using BubaCode.Models.FilesExplorer;
using BubaCode.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

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
            foreach (var item in items)
            {
                if (item is DirectoryInfo)
                {
                    FolderViewModel folderViewModel = new FolderViewModel((DirectoryInfo)item, fileExplorer);
                    Children.Add(folderViewModel);
                    fileExplorer.AddItem(folderViewModel);
                }

                if (item is FileInfo)
                {
                    FileViewModel fileViewModel = new FileViewModel((FileInfo)item);
                    Children.Add(fileViewModel);
                    fileExplorer.AddItem(fileViewModel);
                }

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
}