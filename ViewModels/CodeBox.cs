using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;


namespace BubaCode.ViewModels
{
    public class CodeBox : Control
    {
        public static readonly StyledProperty<IBrush?> BackgroundProperty =
            Border.BackgroundProperty.AddOwner<Panel>();
        
        public IBrush? Background
        {
            get => GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        
        // writing logic
        private List<StringBuilder> _lines;
        private int _caretIndex;
        private int _caretLine;
        
        // showing carret
        private DispatcherTimer _caretTimer;
        private bool _showCaret = false;
        public CodeBox()
        {
            Focusable = true;
            _lines = [new StringBuilder()];
            _caretIndex = 0;
            _caretTimer = new DispatcherTimer();
            _caretTimer.Interval = new TimeSpan(5000000);
            _caretTimer.Start();
            _caretTimer.Tick += (_, _) =>
            {
                _showCaret = !_showCaret;
                InvalidateVisual();
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            var currentLine = _lines[_caretLine];
            
            switch (e.Key)
            {
                case Key.Up:
                    MoveCaretY(-1);
                    break;
                case Key.Down:
                    MoveCaretY(1);
                    break;
                case Key.Left:
                    MoveCaretX(-1);
                    break;
                case Key.Right:
                    MoveCaretX(1);
                    break;
                case Key.Enter:
                    // 
                    string shiftedFragment = currentLine.ToString().Substring(_caretIndex);
                    currentLine.Remove(_caretIndex, shiftedFragment.Length);
                    _lines.Insert(_caretLine + 1, new StringBuilder(shiftedFragment));
                    _caretIndex = 0;
                    _caretLine++;
                    break;
                case Key.Back:
                    if (_caretIndex == 0)
                    {
                        if (_lines.Count > 1)
                        {
                            _lines.RemoveAt(_caretLine);
                            _caretLine--;
                            currentLine = _lines[_caretLine];
                            _caretIndex = currentLine.Length;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        currentLine.Remove(currentLine.Length - 1, 1);
                        _caretIndex -= 1;
                    }
                    break;
                default:
                    if (e.KeySymbol == null)
                    {
                        return;
                    }
                    
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                    {
                        currentLine.Insert(_caretIndex, e.KeySymbol.ToUpper());
                    }
                    else
                    {
                        currentLine.Insert(_caretIndex, e.KeySymbol);
                    }

                    _caretIndex += 1;
                    break;
            }
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
            
            var typeface = new Typeface("Consolas");
            var brush = Brushes.Black;

            double renderedLinesHeight = 0;
            // time consuming - to change
            for (int i = 0; i < _lines.Count; i++)
            {
                string line = _lines[i].ToString() + "\r\n";
                var lineText = new FormattedText(
                    line,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    16,
                    brush);
                context.DrawText(lineText, new Point(0, renderedLinesHeight));
                if (i == _caretLine && _showCaret)
                {
                    double lineHeight = new TextMetrics(typeface.GlyphTypeface, 16).LineHeight;
                    double carretX = _lines[i].Length > 0 ? _caretIndex * lineText.Width / _lines[i].Length : 1;
                    context.DrawLine(new Pen(brush), new Point(carretX, renderedLinesHeight), new Point(carretX, renderedLinesHeight + lineHeight));
                }
                renderedLinesHeight += lineText.Height;
            }
            
            
            
        }

        private void MoveCaretX(int xOffset)
        {
            if (xOffset == 0) return;

            int lineLength = _lines[_caretLine].Length;

            if (_caretIndex + xOffset <= lineLength && _caretIndex + xOffset >= 0)
            {
                _caretIndex += xOffset;
            }
        }
        private void MoveCaretY(int yOffset)
        {
            if (yOffset == 0) return;

            int linesCount = _lines.Count;

            if (_caretLine + yOffset < linesCount && _caretLine + yOffset >= 0)
            {
                if (_caretIndex >= _lines[_caretLine + yOffset].Length)
                {
                    _caretIndex = _lines[_caretLine + yOffset].Length;
                }
                _caretLine += yOffset;
            }
        }

        public string Export()
        {
            StringBuilder result = new();
            result.AppendJoin("\r\n", _lines);
            return result.ToString();
        }

        public void Import(IEnumerable<String> lines)
        {
            _lines.Clear();
            _caretLine = 0;
            foreach (var line in lines)
            {
                _lines.Add(new StringBuilder(line));
            }
            _caretLine = _lines.Count - 1;
            _caretIndex = _lines[_caretLine].Length;
            
        }
    }
}
