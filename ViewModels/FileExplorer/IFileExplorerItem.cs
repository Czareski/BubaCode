using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.Models.FilesExplorer;

public interface IFileExplorerItem
{
    public void UpdateUri(string newPath, string name);
    public string GetName();
    public string GetPath();
    public string GetParentPath();
}