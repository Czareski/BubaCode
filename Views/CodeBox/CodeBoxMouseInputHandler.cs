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

        int line = (int)Math.Floor(y / _view.metrics.LineHeight);
        int column = 0;
        if (line >= _viewModel.Text.LinesCount)
        {
            line = _viewModel.Text.LinesCount - 1;
        }

        if (line < 0)
        {
            line = 0;
        }

        if (x > _view.GetLineWidth(line))
        {
            column = _viewModel.Text.GetLineLength(line);
        } else if (x < 0)
        {
            column = 0;
        }
        else
        {
            double charWidth = _view.GetLineWidth(line) / _viewModel.Text.GetLineLength(line);
            var ratio = x / charWidth;
            column = (int)Math.Round(ratio);
        }

        return new System.Drawing.Point(line, column);
    }
}