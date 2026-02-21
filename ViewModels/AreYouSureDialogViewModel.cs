using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BubaCode.ViewModels;

public partial class AreYouSureDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = "Input";

    [ObservableProperty]
    private string _prompt = "Enter value:";

    public bool DialogResult { get; private set; }

    [RelayCommand]
    private void Ok()
    {
        DialogResult = true;
        CloseRequested?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        CloseRequested?.Invoke();
    }

    public event Action? CloseRequested;
}
