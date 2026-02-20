using System.Collections;
using System.Collections.Generic;
using Avalonia.Media.TextFormatting;
using BubaCode.Models;

namespace BubaCode.Views;

public class VisualLinesBuilder
{
    private ITextStorage _text;

    public VisualLinesBuilder(ITextStorage document)
    {
        _text = document;
    }
    
    public IEnumerable<VisualLine> BuildLines(int firstLine, int lineCount, double lineHeight)
    {
        int linesCount = _text.LinesCount; 
        for (int i = firstLine; i < firstLine + lineCount; i++)
        {
            if (i >= linesCount)
            {
                break;
            }
            
            int lineLength = _text.GetLineLength(i);
            double y = i * lineHeight;
            yield return new VisualLine(lineLength, y, i);
        }
    }
}