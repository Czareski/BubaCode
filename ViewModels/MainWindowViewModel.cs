using System.Diagnostics;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;

namespace BubaCode.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string CodeSample { get; } = "public partial class MainWindowViewModel : ViewModelBase \r\n{ \r\n\t public int example = -1; \r\n} \r\n";
    public int GetLinesCount()
    {
        return CodeSample.Split("\r\n").Length;
    }
    [RelayCommand]
    public void Save()
    {
        Debug.WriteLine("Save");
    }
}
