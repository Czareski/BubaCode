using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using BubaCode.Models;
using BubaCode.ViewModels;

namespace BubaCode.Views;

public partial class CodeBox : Control
{
    public static readonly StyledProperty<IBrush?> BackgroundProperty = Border.BackgroundProperty.AddOwner<Panel>();

    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public static readonly StyledProperty<int> CaretLineProperty =
        AvaloniaProperty.Register<CodeBox, int>(nameof(CaretLine));

    public int CaretLine
    {
        get => GetValue(CaretLineProperty);
        set => SetValue(CaretLineProperty, value);
    }

    public static readonly StyledProperty<int> CaretColumnProperty =
        AvaloniaProperty.Register<CodeBox, int>(nameof(CaretColumn));

    public int CaretColumn
    {
        get => GetValue(CaretColumnProperty);
        set => SetValue(CaretColumnProperty, value);
    }

    public TextMetrics metrics;

    private CodeBoxViewModel _vm;
    private CodeBoxMouseInputHandler _inputHandler;
    private readonly DispatcherTimer _caretTimer;
    private bool _showCaret;
    private readonly Typeface _typeface = new("Consolas");
    private List<TextLayout> _formattedLines;

    public CodeBox()
    {
        Focusable = true;

        _caretTimer = new DispatcherTimer();
        _caretTimer.Interval = new TimeSpan(5000000);
        metrics = new TextMetrics(_typeface.GlyphTypeface, 16);
        _formattedLines = new List<TextLayout>([CreateTextLayout(" ")]);

        AttachedToVisualTree += (_, _) =>
        {
            _vm = DataContext as CodeBoxViewModel;
            _inputHandler = new CodeBoxMouseInputHandler(_vm, this);
        };
        InitCaretTimer();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (_vm != null)
            _vm.PropertyChanged -= VmOnPropertyChanged;

        base.OnDataContextChanged(e);

        _vm = DataContext as CodeBoxViewModel;
        _inputHandler = new CodeBoxMouseInputHandler(_vm, this);

        if (_vm != null)
            _vm.PropertyChanged += VmOnPropertyChanged;

        RefreshFormattedLines();
    }

    private void VmOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Gdy import podmienia cały Text (np. new PieceTableTextAdapter(...))
        if (e.PropertyName == nameof(CodeBoxViewModel.Text))
        {
            RefreshFormattedLines();
            InvalidateVisual();
        }
    }

    private void RefreshFormattedLines()
    {
        if (_vm?.Text == null)
            return;

        _formattedLines.Clear();

        for (int line = 0; line < _vm.Text.LinesCount; line++)
            _formattedLines.Add(CreateTextLayout(_vm.Text.GetLine(line)));

        if (_formattedLines.Count == 0)
            _formattedLines.Add(CreateTextLayout(" "));

        InvalidateMeasure();
    }

    private TextLayout CreateTextLayout(string text)
    {

        return new TextLayout(
            text,
            _typeface,
            16,
            Brushes.White,
            TextAlignment.Left,
            TextWrapping.NoWrap,
            TextTrimming.None
            );
    }

    private void InitCaretTimer()
    {
        _caretTimer.Start();
        _caretTimer.Tick += (_, _) =>
        {
            _showCaret = !_showCaret;
            InvalidateVisual();
        };
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Focus();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        ((CodeBoxViewModel)DataContext!).OnKeyDown(e);

        // Tekst mutuje się "w środku" adaptera, więc musimy odświeżyć layouty ręcznie
        RefreshFormattedLines();

        InvalidateVisual();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Focus();
        _inputHandler.OnPointerPressed(e.GetPosition(this));
        InvalidateVisual();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        _inputHandler.OnPointerRealesed(e.GetPosition(this));
        InvalidateVisual();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        _inputHandler.OnPointerMoved(e.GetPosition(this));
        InvalidateVisual();
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        Cursor = new Cursor(StandardCursorType.Ibeam);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        _caretTimer.Stop();
        _showCaret = false;
        InvalidateVisual();
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        _showCaret = true;
        _caretTimer.Start();
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var background = Background;


        if (background != null)
        {
            var renderSize = Bounds.Size;
            context.FillRectangle(background, new Rect(renderSize));
        }


        RenderSelectionBackground(context);


        for (int i = 0; i < _formattedLines.Count; i++)
        {
            RenderLine(context, i);

            if (i == CaretLine && _showCaret)
            {
                RenderCaret(context, i);
            }
        }
    }

    private void RenderSelectionBackground(DrawingContext context)
    {
        Selection? selection = _vm.Selection;

        if (selection == null) return;
        if (selection.HasSelectedFragmentOfText() == false) return;
        int column = selection.StartPosition.Y;
        for (int i = 0; i <= selection.EndPosition.X - selection.StartPosition.X; i++)
        {
            var line = selection.StartPosition.X + i;
            var left = column * _formattedLines[line].Width / _vm.Text.GetLineLength(line) ;
            left = (left < 0) ? 0 : left;
            var right = (selection.EndPosition.X == line)
                ? selection.EndPosition.Y * _formattedLines[line].Width / _vm.Text.GetLineLength(line)
                : _formattedLines[line].Width;
            var top = metrics.LineHeight * line;
            var bottom = top + metrics.LineHeight;

            column = 0;
            context.FillRectangle(Brushes.Crimson, new Rect(left, top, right - left, bottom - top));
        }
    }

    private void RenderLine(DrawingContext context, int index)
    {
        _formattedLines[index].Draw(context, new Point(0, index * metrics.LineHeight));
    }

    private void RenderCaret(DrawingContext context, int lineIndex)
    {
        double carretX = _vm.Text.GetLineLength(lineIndex) > 0
            ? CaretColumn * _formattedLines[lineIndex].WidthIncludingTrailingWhitespace / _vm.Text.GetLineLength(lineIndex) // bez \n powinno byc
            : 1;
        context.DrawRectangle(new Pen(Brushes.Red),
            new Rect(new Point(carretX, lineIndex * metrics.LineHeight),
                new Point(carretX + 1, lineIndex * metrics.LineHeight + metrics.LineHeight)));
    }

    public double GetLineWidth(int index)
    {
        if (index >= _formattedLines.Count || index < 0)
        {
            return 0;
        }

        return _formattedLines[index].Width;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_formattedLines == null || _formattedLines.Count == 0)
            return new Size(0, 0);

        double maxWidth = 0;

        foreach (var line in _formattedLines)
            maxWidth = Math.Max(maxWidth, line.WidthIncludingTrailingWhitespace);

        double height = _formattedLines.Count * metrics.LineHeight;

        return new Size(maxWidth, height);
    }
}