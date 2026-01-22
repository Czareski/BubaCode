using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BubaCode.Models.FilesExplorer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.ViewModels.FileExplorer;

public partial class FolderViewModel : ViewModelBase, IFileExplorerItem
{
    [ObservableProperty]
    private DirectoryInfo _directoryInfo;
    [ObservableProperty]
    private bool _isExpanded = true;
    [ObservableProperty]
    private ObservableCollection<IFileExplorerItem> _children = new ObservableCollection<IFileExplorerItem>();
    
    public FolderViewModel(DirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
        PopulateChildren();
    }

    public async Task PopulateChildren()
    {
        // Sprawdzaj pole prywatne, jeśli to w nie wpisujesz dane, 
        // lub upewnij się, że używasz generowanej właściwości.
        if (_directoryInfo == null) 
        {
            Debug.WriteLine("Pole _directoryInfo jest nullem!");
            return;
        }
        
        // Odśwież stan obiektu, aby upewnić się, że Exists jest aktualne
        _directoryInfo.Refresh(); 

        if (!_directoryInfo.Exists) 
        {
            Debug.WriteLine("Folder fizycznie nie istnieje na dysku!");
            return;
        }

        var items = _directoryInfo.GetFileSystemInfos();

        try 
        {
            // Czyścimy obecne dzieci przed dodaniem nowych, 
            // żeby uniknąć duplikatów przy ponownym wywołaniu
            Children.Clear();
            
            foreach (var item in items)
            {
                // Przekazujemy 'dir' (podfolder), a nie '_directoryInfo' (obecny folder)!
                if (item is DirectoryInfo)
                {
                    Children.Add(new FolderViewModel((DirectoryInfo)item));
                }

                if (item is FileInfo)
                {
                    Children.Add(new FileViewModel((FileInfo)item));
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Obsługa braku uprawnień do folderu
            Debug.WriteLine($"Brak dostępu do: {_directoryInfo.FullName}");
        }
    }
}