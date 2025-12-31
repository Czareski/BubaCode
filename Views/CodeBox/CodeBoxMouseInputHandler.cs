using System;
using System.Diagnostics;
using Avalonia;
using BubaCode.Models;
using BubaCode.ViewModels;

namespace BubaCode.Views;

public class CodeBoxMouseInputHandler
{
    private CodeBoxViewModel _viewModel;
    private CodeBox _view;
    private bool _isHolding = false;
    private Selection selection;
    public CodeBoxMouseInputHandler(CodeBoxViewModel vm, CodeBox view)
    {
        _viewModel = vm;
        _view = view;
    }
    public void OnPointerPressed(Point pointerPosition)
    {
        _isHolding = true;
        System.Drawing.Point caretPosition = GetCaretPosition(pointerPosition.X, pointerPosition.Y);
        selection = new Selection(caretPosition);
        _viewModel.SetCaret(caretPosition.X, caretPosition.Y);
    }
    public void OnPointerRealesed(Point pointerPosition)
    {
        _isHolding = false;
        System.Drawing.Point caretPosition = GetCaretPosition(pointerPosition.X, pointerPosition.Y);
        Debug.WriteLine("----START----- \r\n - line: {0} \r\n - column {1}", selection.StartPosition.X, selection.StartPosition.Y);
        Debug.WriteLine("-----END------ \r\n - line: {0} \r\n - column {1}", selection.EndPosition.X, selection.EndPosition.Y);
        
    }
    
    public void OnPointerMoved(Point pointerPosition)
    {
        if (_isHolding)
        {
            System.Drawing.Point caretPosition = GetCaretPosition(pointerPosition.X, pointerPosition.Y);
            _viewModel.SetCaret(caretPosition.X, caretPosition.Y);
            selection.Update(caretPosition);
        }
        
    }
    
    
    private System.Drawing.Point GetCaretPosition(double x, double y)
    {
        int line = (int)Math.Floor(y / _view.metrics.LineHeight);
        int column = 0;
        if (line >= _view.Lines.Count)
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