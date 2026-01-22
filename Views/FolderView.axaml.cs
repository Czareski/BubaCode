using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BubaCode.Views;

public partial class FolderView : UserControl
{
    public FolderView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            Debug.WriteLine(DataContext);
        };
    }
}