using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Input;
using BubaCode.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.Models;

public partial class EditorText : ObservableObject
{
    public int LinesCount => Lines.Count;
    [ObservableProperty]
    private ObservableCollection<EditorLine> _lines;
    private CodeBoxViewModel _vm;
    private EditorLine CurrentLine =>Lines[_vm.Caret.Line]; // ?
    
    public EditorText(CodeBoxViewModel vm)
    {
        _lines = [new EditorLine("")];
        _vm = vm;
    }

    public void InitializeEmpty()
    {
        Lines.Add(new EditorLine(""));
        _vm.Caret.Line = 0;
        _vm.Caret.Column = 0;
    }

    public void HandleTextKey(KeyEventArgs eventArgs)
    {
        if (eventArgs.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            CurrentLine.Insert(_vm.Caret.Column, eventArgs.KeySymbol.ToUpper());
        }
        else
        {
            CurrentLine.Insert(_vm.Caret.Column, eventArgs.KeySymbol);
        }
        _vm.Caret.Column += 1;
    }

    public void InsertLine(string line)
    {
        Lines.Add(new EditorLine(line));
    }

    public void InsertText(string text)
    {
        CurrentLine.Insert(_vm.Caret.Column, text);
    }
    
    public void HandleEnter()
    {
        string shiftedFragment = CurrentLine.Text.Substring(_vm.Caret.Column);
        CurrentLine.Remove(_vm.Caret.Column, shiftedFragment.Length);
        Lines.Insert(_vm.Caret.Line + 1, new EditorLine(shiftedFragment));

        _vm.Caret.Column = 0;
        _vm.Caret.Line++;
    }

    public void HandleBackspace()
    {
        if (_vm.Selection != null)
        {
            RemoveFromSelection(_vm.Selection);
            _vm.Selection = null;
            return;
        }
        if (_vm.Caret.Column == 0)
        {
            if (Lines.Count > 1)
            {
                Lines.RemoveAt(_vm.Caret.Line);
                _vm.Caret.Line--;
                _vm.Caret.Column = CurrentLine.Length;
            }
        } else
        {
            CurrentLine.Remove(CurrentLine.Length - 1, 1);
            _vm.Caret.Column -= 1;
        }
    }

    public void HandleTab()
    {
        CurrentLine.Insert(_vm.Caret.Column, "\t");
        _vm.Caret.Column += 1;
    }

    public void Clear()
    {
        Lines.Clear();
    }

    public string GetTextFromSelection(Selection selection)
    {
        StringBuilder result = new();
        for (int line = selection.StartPosition.X; line <= selection.EndPosition.X; line++)
        {
            var start = line == selection.StartPosition.X ? selection.StartPosition.Y : 0;
            var end = line == selection.EndPosition.X ? selection.EndPosition.Y : GetLineLength(line);
            
            result.Append(Lines[line].Text.Substring(start, end - start));
        }

        return result.ToString();
    }

    public void RemoveFromSelection(Selection selection)
    {
        for (int line = selection.StartPosition.X; line <= selection.EndPosition.X; line++)
        {
            var start = line == selection.StartPosition.X ? selection.StartPosition.Y : 0;
            var end = line == selection.EndPosition.X ? selection.EndPosition.Y : GetLineLength(line);
            
            var removed = Lines[line].Text.Remove(start, end - start);
            Lines[line] = new EditorLine(removed); 
        }
        _vm.Caret.Line = selection.StartPosition.X;
        _vm.Caret.Column = selection.StartPosition.Y;
    }
    
    public int GetLineLength(int line)
    {
        if (line < 0 || line >= Lines.Count)
        {
            return 0;
        } 
        return Lines[line].Length;
    }

    public override string ToString()
    {
        StringBuilder result = new();
        result.AppendJoin("\r\n", Lines);
        return result.ToString();
    }
}