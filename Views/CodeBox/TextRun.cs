using Avalonia.Media;
using BubaCode.Models.Syntax;

namespace BubaCode.Views;

public class TextRun
{
    public int StartOffset;
    public int Length;
    public IBrush Brush;

    public TextRun(int startOffset, int length, IBrush brush)
    {
        StartOffset = startOffset;
        Length = length;
        Brush = brush;
    }
}