using System;
using System.Collections.Generic;
using Avalonia.Media;
using BubaCode.Models;
using BubaCode.Models.Syntax;

namespace BubaCode.Views;

public class VisualLinesBuilder
{
    private ITextStorage _text;
    private List<Token> _tokens;
    private PieceTableTextAdapter _adapter;

    public VisualLinesBuilder(ITextStorage document, List<Token> tokens)
    {
        _text = document;
        _tokens = tokens;
        _adapter = document as PieceTableTextAdapter ?? throw new InvalidOperationException("Expected PieceTableTextAdapter");
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

            double y = i * lineHeight;
            var visualLine = new VisualLine(i, y);

            int lineStart = _adapter.Lines.GetOffset(i);
            int lineLength = _text.GetLineLength(i);
            int lineEnd = lineStart + lineLength;

            foreach (var token in _tokens)
            {
                if (token.StartOffset >= lineEnd)
                    break;

                if (token.EndOffset <= lineStart)
                    continue; 

                int clippedStart = Math.Max(token.StartOffset, lineStart);
                int clippedEnd = Math.Min(token.EndOffset, lineEnd);
                int clippedLength = clippedEnd - clippedStart;

                if (clippedLength > 0)
                {
                    IBrush brush = GetBrushForTokenType(token.Type);
                    visualLine.AddTextRun(new TextRun(clippedStart, clippedLength, brush));
                }
            }

            if (visualLine.TextRuns.Count == 0 && lineLength > 0)
            {
                visualLine.AddTextRun(new TextRun(lineStart, lineLength, Brushes.White));
            }

            yield return visualLine;
        }
    }

    private IBrush GetBrushForTokenType(TokenType type)
    {
        return type switch
        {
            TokenType.Keyword => Brushes.CornflowerBlue,
            TokenType.String => Brushes.LightGreen,
            TokenType.Number => Brushes.LightCoral,
            TokenType.Comment => Brushes.Gray,
            TokenType.Operator => Brushes.Yellow,
            TokenType.FunctionName => Brushes.Chartreuse,
            TokenType.ClassName => Brushes.LightSeaGreen,
            TokenType.TypeName => Brushes.MediumTurquoise,
            TokenType.Identifier => Brushes.LightCyan,
            _ => Brushes.White
        };
    }
}