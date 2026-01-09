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
    private Selection? _selection;
    public CodeBoxMouseInputHandler(CodeBoxViewModel vm, CodeBox view)
    {
        _viewModel = vm;
        _view = view;
    }
    public void OnPointerPressed(Point pointerPosition)
    {
        _isHolding = true;
        System.Drawing.Point caretPosition = GetCaretPosition(pointerPosition.X, pointerPosition.Y);
        _selection = new Selection(caretPosition);
        _viewModel.Caret.SetPosition(caretPosition.X, caretPosition.Y);
        _viewModel.Selection = _selection;
    }
    public void OnPointerRealesed(Point pointerPosition)
    {
        _isHolding = false;
        System.Drawing.Point caretPosition = GetCaretPosition(pointerPosition.X, pointerPosition.Y);
        Debug.WriteLine("----START----- \r\n - line: {0} \r\n - column {1}", _selection.StartPosition.X, _selection.StartPosition.Y);
        Debug.WriteLine("-----END------ \r\n - line: {0} \r\n - column {1}", _selection.EndPosition.X, _selection.EndPosition.Y);
    }
    
    public void OnPointerMoved(Point pointerPosition)
    {
        if (_isHolding)
        {
            System.Drawing.Point caretPosition = GetCaretPosition(pointerPosition.X, pointerPosition.Y);
            _viewModel.Caret.SetPosition(caretPosition.X, caretPosition.Y);
            _selection?.Update(caretPosition);
            _view.InvalidateVisual();
        }
        
    }

    private System.Drawing.Point GetCaretPosition(double x, double y)
    {
        if (y < 0 || x < 0) return new System.Drawing.Point(_viewModel.Caret.Line,  _viewModel.Caret.Column);
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
            double charWidth = _view.GetLineWidth(line) / _view.Lines[line].Length;
            var ratio = x / charWidth;
            column = (int)Math.Round(ratio);
        }

        return new System.Drawing.Point(line, column);
    }

    public Selection GetSelection()
    {
        
        if (_selection != null && _selection.StartPosition == _selection.EndPosition)
        {
            return null;
        }
        return _selection;
    }
    
}