using System;
using Avalonia;
using BubaCode.ViewModels;

namespace BubaCode.Views;

public class CodeBoxMouseInputHandler
{
    private Point pressedPoint;
    private CodeBoxViewModel _viewModel;
    private CodeBox _view;
    public CodeBoxMouseInputHandler(CodeBoxViewModel vm, CodeBox view)
    {
        _viewModel = vm;
        _view = view;
    }
    public void OnPointerPressed(Point clickedPoint)
    {
        System.Drawing.Point caretPosition = GetCaretPosition(clickedPoint.X, clickedPoint.Y);
        _viewModel.OnPointerPressed(caretPosition.X, caretPosition.Y);
    }
    
    private System.Drawing.Point GetCaretPosition(double x, double y)
    {
        int line = (int)Math.Floor(y / _view.metrics.LineHeight);
        int column = 0;
        if (line > _view.Lines.Count)
        {
            line = _view.Lines.Count - 1;
        }
        if (x > _view.GetLineWidth(line))
        {
            column = _view.Lines[line].Length;
        }
        else
        {
            double charWidth =  _view.GetLineWidth(line) / _view.Lines[line].Length;
            var ratio = x / charWidth;
            column = (int)Math.Round(ratio);
        }

        return new System.Drawing.Point(line, column);
    }
    
    
}