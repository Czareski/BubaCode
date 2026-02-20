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
        _scrollViewer = this.FindAncestorOfType<ScrollViewer>();
        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged += OnScrollChanged;
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
        if (_vm?.Text == null) return;

        int firstVisibleLine = 0;
        int visibleLinesCount = _vm.Text.LinesCount;

        if (_scrollViewer != null)
        {
            firstVisibleLine = (int)(_scrollViewer.Offset.Y / _metrics.LineHeight);
            visibleLinesCount = (int)Math.Ceiling(_scrollViewer.Viewport.Height / _metrics.LineHeight) + 1;
        }

        int lastLine = Math.Min(_vm.Text.LinesCount, firstVisibleLine + visibleLinesCount);

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
        if (_vm?.Text == null) return new Size(40, 0);
        
        double height = _vm.Text.LinesCount * _metrics.LineHeight;

        string maxLineStr = _vm.Text.LinesCount.ToString();
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
