using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using BubaCode.ViewModels.FileExplorer;

namespace BubaCode.Views;

public partial class FileView : UserControl
{
    private Point _dragStartPoint;
    private bool _isDragging;

    public FileView()
    {
        InitializeComponent();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _dragStartPoint = e.GetPosition(this);
            _isDragging = false;
        }
    }

    private async void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && !_isDragging)
        {
            var currentPoint = e.GetPosition(this);
            var distance = currentPoint - _dragStartPoint;

            if (Math.Abs(distance.X) > 5 || Math.Abs(distance.Y) > 5)
            {
                _isDragging = true;
                if (DataContext is FileViewModel fileViewModel)
                {
                    var data = new DataObject();
                    data.Set("FileExplorerItem", fileViewModel);
                    await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
                }
            }
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
    }
}