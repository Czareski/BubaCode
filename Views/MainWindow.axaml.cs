using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using BubaCode.ViewModels;
using BubaCode.ViewModels.FileExplorer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ClipboardService clipboard = new ClipboardService(GetTopLevel(this));
        DataContext = new MainWindowViewModel();
    }
}