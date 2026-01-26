using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using BubaCode.Models.FilesExplorer;
using BubaCode.ViewModels.FileExplorer;

namespace BubaCode.ViewModels;

public class FileExplorerService
{
    private Dictionary<string, IFileExplorerItem> _items = new();
    private FileSystemWatcher _watcher;

    public FileExplorerService(DirectoryInfo folder)
    {
        _watcher = new FileSystemWatcher(folder.FullName);
        _watcher.IncludeSubdirectories = true;
        _watcher.EnableRaisingEvents = true;
        _watcher.Renamed += OnRename;
        _watcher.Deleted += OnDelete;
        _watcher.Created += OnCreate;
        _watcher.Changed += OnChange;
    }

    public void AddItem(IFileExplorerItem item)
    {
        _items[item.GetPath()] = item;
    }

    private void OnRename(object sender, RenamedEventArgs e)
    {
        string oldPath = e.OldFullPath;
        string newPath = e.FullPath;
        IFileExplorerItem renamedItem = _items[oldPath];

        _items.Remove(oldPath);
        _items.Add(newPath, renamedItem);
        renamedItem.UpdateUri(newPath, "");
        if (renamedItem is FolderViewModel)
        {
            FolderViewModel folder = (FolderViewModel)renamedItem;
            foreach (var child in folder.Children)
            {
                _items.Remove(child.GetPath());
                child.UpdateUri(newPath, child.GetName());
                _items.Add(child.GetPath(), child);
            }
        }
        
    }
    private void OnDelete(object sender, FileSystemEventArgs e)
    {
        string path = e.FullPath;
        IFileExplorerItem deletedItem = _items[path];
        FolderViewModel parent = (FolderViewModel)_items[deletedItem.GetParentPath()];
        parent.Children.Remove(deletedItem);
        _items.Remove(path);
        
        if (deletedItem is FolderViewModel folder)
        {
           DeleteChildren(folder);
        }
    }

    private void DeleteChildren(FolderViewModel folder)
    {
        foreach (var child in folder.Children)
        {
            _items.Remove(child.GetPath());
            if (child is FolderViewModel folderChild)
            {
                DeleteChildren(folderChild);
            }
        }
    }
    private void OnChange(object sender, FileSystemEventArgs e)
    {
    }

    private void OnCreate(object sender, FileSystemEventArgs e)
    {
        IFileExplorerItem item = null;
        if (Directory.Exists(e.FullPath))
        {
            DirectoryInfo info = new DirectoryInfo(e.FullPath);
            item = new FolderViewModel(info, this);
            
        }
        else if (File.Exists(e.FullPath))
        {
            FileInfo info = new FileInfo(e.FullPath);
            item = new FileViewModel(info);
        }
        AddItem(item!);
        FolderViewModel parent =  (FolderViewModel)_items[item!.GetParentPath()];
        parent.Children.Add(item);
    }

    public void TryGetValue(Uri uri, out IFileExplorerItem? item)
    {
        _items.TryGetValue(uri.AbsolutePath, out item);
    }
}