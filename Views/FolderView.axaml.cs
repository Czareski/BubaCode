using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using BubaCode.Models.FilesExplorer;
using BubaCode.ViewModels.FileExplorer;

namespace BubaCode.Views;

public partial class FolderView : UserControl
{
    private Point _dragStartPoint;
    private bool _isDragging;

    public FolderView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            Debug.WriteLine(DataContext);
        };

        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
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
                if (DataContext is FolderViewModel folderViewModel)
                {
                    var data = new DataObject();
                    data.Set("FileExplorerItem", folderViewModel);
                    await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
                }
            }
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains("FileExplorerItem"))
        {
            e.DragEffects = DragDropEffects.Move;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains("FileExplorerItem") && DataContext is FolderViewModel targetFolder)
        {
            var item = e.Data.Get("FileExplorerItem") as IFileExplorerItem;
            if (item != null && item != targetFolder)
            {
                try
                {
                    string sourcePath = item.GetPath();
                    string targetPath = Path.Combine(targetFolder.GetPath(), item.GetName());

                    if (targetPath.StartsWith(sourcePath + Path.DirectorySeparatorChar))
                    {
                        return;
                    }

                    if (item is FolderViewModel)
                    {
                        Directory.Move(sourcePath, targetPath);
                    }
                    else if (item is FileViewModel)
                    {
                        File.Move(sourcePath, targetPath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error moving item: {ex.Message}");
                }
            }
        }
    }
}