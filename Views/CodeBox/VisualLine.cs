using System.Collections.Generic;

namespace BubaCode.Views;

public class VisualLine
{
    private List<TextRun> _textRuns = new();
    private int _lineIndex;
    private double _yOffset;

    public List<TextRun> TextRuns => _textRuns;
    public int Index => _lineIndex;
    public double Y => _yOffset;
    public double Width;

    public VisualLine(int lineIndex, double yOffset)
    {
        _lineIndex = lineIndex;
        _yOffset = yOffset;
    }

    public void AddTextRun(TextRun run)
    {
        _textRuns.Add(run);
    }
}