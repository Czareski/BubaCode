using Avalonia.Controls;
using BubaCode.ViewModels;

namespace BubaCode.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        FileDialogService service = new FileDialogService(this);
        DataContext = new MainWindowViewModel(service);
        InitializeComponent();
    }
}