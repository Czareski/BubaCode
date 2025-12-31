using Avalonia.Controls;
using BubaCode.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.Views;

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}