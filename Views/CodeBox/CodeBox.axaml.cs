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
using Avalonia.VisualTree;
using BubaCode.Models;
using BubaCode.Models.Syntax;
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

    private ScrollViewer _scrollViewer;
    private CodeBoxViewModel _vm;
    private CodeBoxMouseInputHandler _inputHandler;
    private readonly DispatcherTimer _caretTimer;
    private bool _showCaret;
    private readonly Typeface _typeface = new("Consolas");
    private double _scrollOffset = 0;
    private int _firstVisibleLine = 0;
    private int _visibleLinesCount = 1;

    private readonly List<VisualLine> _visualLines;
    private List<Token> _tokens = new();
    private readonly Lexer _lexer = new();

    private const int TabSize = 4;

    private static string ExpandTabs(string text, int tabSize, int startingVisualColumn = 0)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var sb = new StringBuilder(text.Length);
        int col = startingVisualColumn;

        foreach (char ch in text)
        {
            if (ch == '\t')
            {
                int spaces = tabSize - (col % tabSize);
                if (spaces == 0) spaces = tabSize;
                sb.Append(' ', spaces);
                col += spaces;
            }
            else
            {
                sb.Append(ch);
                col += 1;
            }
        }

        return sb.ToString();
    }

    public CodeBox()
    {
        Focusable = true;

        _caretTimer = new DispatcherTimer();
        _caretTimer.Interval = new TimeSpan(5000000);
        metrics = new TextMetrics(_typeface.GlyphTypeface, 16);
        _visualLines = new List<VisualLine>();
        AttachedToVisualTree += (_, _) =>
        {
            _vm = DataContext as CodeBoxViewModel;
            _inputHandler = new CodeBoxMouseInputHandler(_vm, this);
        };
        InitCaretTimer();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        _vm = DataContext as CodeBoxViewModel;
        _inputHandler = new CodeBoxMouseInputHandler(_vm, this);

        _tokens.Clear();

        InvalidateVisual();
        InvalidateMeasure();
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
        _scrollViewer = this.FindAncestorOfType<ScrollViewer>();
        _scrollViewer.ScrollChanged += OnScrollChanged;
        Focus();
    }

    private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        _scrollOffset = Math.Max(0, e.OffsetDelta.Y);

        _firstVisibleLine = (int)(_scrollOffset / metrics.LineHeight);
        _visibleLinesCount = Math.Max(
            1,
            (int)Math.Ceiling(_scrollViewer.Viewport.Height / metrics.LineHeight) + 1
        );

        InvalidateVisual();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        ((CodeBoxViewModel)DataContext!).OnKeyDown(e);

        if (_vm?.Text != null)
        {
            _tokens = _lexer.Tokenize(_vm.Text);
        }

        InvalidateVisual();
        InvalidateMeasure();
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

        if (_scrollViewer != null)
        {
            _firstVisibleLine = (int)(_scrollViewer.Offset.Y / metrics.LineHeight);
            _visibleLinesCount = Math.Max(1, (int)Math.Ceiling(_scrollViewer.Viewport.Height / metrics.LineHeight) + 1);
        }

        if (_scrollViewer != null)
        {
            _firstVisibleLine = (int)(_scrollViewer.Offset.Y / metrics.LineHeight);
            _visibleLinesCount = Math.Max(1, (int)Math.Ceiling(_scrollViewer.Viewport.Height / metrics.LineHeight) + 1);
        }

        if (_vm?.Text == null)
            return;

        if (_tokens.Count == 0)
        {
            _tokens = _lexer.Tokenize(_vm.Text);
        }

        VisualLinesBuilder builder = new VisualLinesBuilder(_vm.Text, _tokens);
        _visualLines.Clear();
        foreach (VisualLine line in builder.BuildLines(_firstVisibleLine, _visibleLinesCount, metrics.LineHeight))
        {
            MeasureLineWidth(line);
            _visualLines.Add(line);
        }

        RenderSelectionBackground(context);

        foreach (var line in _visualLines)
        {
            RenderLine(context, line);

            if (line.Index == CaretLine && _showCaret)
            {
                RenderCaret(context, line.Index);
            }
        }
    }

    private bool TryGetVisualLine(int absoluteLineIndex, out VisualLine visualLine)
    {
        int relative = absoluteLineIndex - _firstVisibleLine;
        if (relative < 0 || relative >= _visualLines.Count)
        {
            visualLine = default!;
            return false;
        }

        visualLine = _visualLines[relative];
        return true;
    }

    private void MeasureLineWidth(VisualLine line)
    {
        if (_vm.Text is not PieceTableTextAdapter pieceTable)
            throw new NotImplementedException();

        double width = 0;

        foreach (TextRun run in line.TextRuns)
        {
            string text = pieceTable.GetText(run.StartOffset, run.Length);
            text = ExpandTabs(text, TabSize);

            var layout = new TextLayout(
                text,
                _typeface,
                16,
                run.Brush,
                TextAlignment.Left,
                TextWrapping.NoWrap,
                TextTrimming.None
            );

            width += layout.WidthIncludingTrailingWhitespace;
        }

        line.Width = width;
    }

    private void RenderSelectionBackground(DrawingContext context)
    {
        Selection? selection = _vm.Selection;

        if (selection == null) return;
        if (selection.HasSelectedFragmentOfText() == false) return;

        int startLine = selection.StartPosition.X;
        int endLine = selection.EndPosition.X;

        if (endLine < startLine)
            (startLine, endLine) = (endLine, startLine);

        int startColumn = selection.StartPosition.Y;

        for (int lineIndex = startLine; lineIndex <= endLine; lineIndex++)
        {
            if (!TryGetVisualLine(lineIndex, out var vLine))
            {
                startColumn = 0;
                continue;
            }

            int lineLength = _vm.Text.GetLineLength(lineIndex);
            double width = vLine.Width;

            double left;
            if (lineLength <= 0)
            {
                left = 0;
            }
            else
            {
                left = startColumn * width / lineLength;
                if (left < 0) left = 0;
                if (left > width) left = width;
            }

            double right;
            if (lineIndex == endLine)
            {
                int endColumn = selection.EndPosition.Y;
                if (lineLength <= 0)
                {
                    right = left;
                }
                else
                {
                    right = endColumn * width / lineLength;
                    if (right < left) right = left;
                    if (right > width) right = width;
                }
            }
            else
            {
                right = width;
            }

            double top = vLine.Y;
            double bottom = top + metrics.LineHeight;

            context.FillRectangle(Brushes.Crimson, new Rect(left, top, Math.Max(0, right - left), bottom - top));

            startColumn = 0;
        }
    }

    private void RenderLine(DrawingContext context, VisualLine line)
    {
        double xOffset = 0;

        if (_vm.Text is not PieceTableTextAdapter pieceTable)
            throw new NotImplementedException();

        foreach (TextRun run in line.TextRuns)
        {
            string text = pieceTable.GetText(run.StartOffset, run.Length);
            text = ExpandTabs(text, TabSize);

            var layout = new TextLayout(
                text,
                _typeface,
                16,
                run.Brush,
                TextAlignment.Left,
                TextWrapping.NoWrap,
                TextTrimming.None
            );
            layout.Draw(context, new Point(xOffset, line.Y));
            xOffset += layout.WidthIncludingTrailingWhitespace;
        }

        line.Width = xOffset;
    }

    private void RenderCaret(DrawingContext context, int lineIndex)
    {
        if (!TryGetVisualLine(lineIndex, out var vLine))
            return;

        if (_vm.Text is not PieceTableTextAdapter pieceTable)
            throw new NotImplementedException();

        int lineOffset = pieceTable.Lines.GetOffset(lineIndex);


        int safeColumn = Math.Clamp(CaretColumn, 0, _vm.Text.GetLineLength(lineIndex));
        string beforeCaret = safeColumn > 0
            ? pieceTable.GetText(lineOffset, safeColumn)
            : string.Empty;

        beforeCaret = ExpandTabs(beforeCaret, TabSize);

        double caretX;
        if (beforeCaret.Length == 0)
        {
            caretX = 0;
        }
        else
        {
            var layout = new TextLayout(
                beforeCaret,
                _typeface,
                16,
                Brushes.White,
                TextAlignment.Left,
                TextWrapping.NoWrap,
                TextTrimming.None
            );
            caretX = layout.WidthIncludingTrailingWhitespace;
        }

        context.DrawRectangle(
            new Pen(Brushes.Red),
            new Rect(
                new Point(caretX, vLine.Y),
                new Point(caretX + 1, vLine.Y + metrics.LineHeight)
            )
        );
    }

    public double GetLineWidth(int index)
    {
        if (!TryGetVisualLine(index, out var vLine))
            return 0;

        return vLine.Width;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_vm?.Text == null) return new Size(500, 0);
        int totalLines = _vm.Text.LinesCount;
        double height = totalLines * metrics.LineHeight;

        double maxWidth = 500;
        foreach (var line in _visualLines)
        {
            maxWidth = Math.Max(maxWidth, line.Width);
        }

        return new Size(maxWidth, height);
    }
}