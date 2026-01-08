using Avalonia.Controls;
using BubaCode.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.Views;

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
        ClipboardService clipboard = new ClipboardService(TopLevel.GetTopLevel(this));
        DataContext = new MainWindowViewModel();
    }
}