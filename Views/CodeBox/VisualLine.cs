using System.Collections.Generic;
using Avalonia.Media.TextFormatting;

namespace BubaCode.Views;

public class VisualLine
{
    private LinkedList<TextRun> _textRuns = new LinkedList<TextRun>();
    private int _lineIndex;
    private double _yOffset;
    
    public LinkedList<TextRun> TextRuns => _textRuns;
    public int Index => _lineIndex; 
    public double Y => _yOffset;
    public double Width;
    public VisualLine(int lineLength, double yOffset, int lineIndex)
    {
        _lineIndex = lineIndex;
        _textRuns.AddFirst(new TextRun(yOffset, lineLength));
        _yOffset = yOffset;
    }

}