using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.VisualTree;
using BubaCode.ViewModels;

namespace BubaCode.Views;

public partial class LineNumberBox : Control
{
    private CodeBoxViewModel? _vm;
    private ScrollViewer? _scrollViewer;
    private readonly Typeface _typeface = new("Consolas");
    private readonly TextMetrics _metrics;

    public LineNumberBox()
    {
        _metrics = new TextMetrics(_typeface.GlyphTypeface, 16);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (_vm?.Text != null)
        {
            _vm.Text.LinesCountChanged -= OnLinesCountChanged;
        }

        base.OnDataContextChanged(e);
        _vm = DataContext as CodeBoxViewModel;

        if (_vm?.Text != null)
        {
            _vm.Text.LinesCountChanged += OnLinesCountChanged;
        }

        InvalidateVisual();
        InvalidateMeasure();
    }

    private void OnLinesCountChanged()
    {
        InvalidateVisual();
        InvalidateMeasure();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var parent = this.Parent;
        if (parent is Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if (child is ScrollViewer sv)
                {
                    _scrollViewer = sv;
                    _scrollViewer.ScrollChanged += OnScrollChanged;
                    break;
                }
            }
        }
    }

    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        InvalidateVisual();
        InvalidateMeasure();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_vm?.Text == null)
        {
            return;
        }

        int totalLines = _vm.Text.LinesCount;
        if (totalLines == 0)
        {
            return;
        }

        int firstVisibleLine = 0;
        int visibleLinesCount = totalLines;

        if (_scrollViewer != null)
        {
            firstVisibleLine = (int)(_scrollViewer.Offset.Y / _metrics.LineHeight);
            visibleLinesCount = (int)Math.Ceiling(_scrollViewer.Viewport.Height / _metrics.LineHeight) + 1;
        }

        int lastLine = Math.Min(totalLines, firstVisibleLine + visibleLinesCount);

        for (int i = firstVisibleLine; i < lastLine; i++)
        {
            var lineText = (i + 1).ToString();
            var layout = new TextLayout(
                lineText,
                _typeface,
                16,
                Brushes.Gray,
                TextAlignment.Right,
                TextWrapping.NoWrap,
                TextTrimming.None
            );

            double y = i * _metrics.LineHeight;
            layout.Draw(context, new Point(Bounds.Width - layout.Width - 5, y));
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_vm?.Text == null)
        {
            return new Size(40, availableSize.Height);
        }

        int linesCount = _vm.Text.LinesCount;
        if (linesCount == 0)
        {
            return new Size(40, availableSize.Height);
        }

        double height = linesCount * _metrics.LineHeight;

        string maxLineStr = linesCount.ToString();
        var layout = new TextLayout(
            maxLineStr,
            _typeface,
            16,
            Brushes.Gray,
            TextAlignment.Left,
            TextWrapping.NoWrap,
            TextTrimming.None
        );

        return new Size(layout.Width + 15, height);
    }
}
