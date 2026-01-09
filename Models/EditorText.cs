using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.Models;

public partial class EditorText : ObservableObject
{
    public int LinesCount => Lines.Count;
    [ObservableProperty]
    private ObservableCollection<EditorLine> _lines;
    private Caret _caret;
    private EditorLine CurrentLine => Lines[_caret.Line]; // ?
    
    public EditorText(ref Caret caret)
    {
        _lines = [new EditorLine("")];
        _caret = caret;
    }

    public void InitializeEmpty()
    {
        Lines.Add(new EditorLine(""));
        _caret.Line = 0;
        _caret.Column = 0;
    }

    public void HandleTextKey(KeyEventArgs eventArgs)
    {
        if (eventArgs.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            CurrentLine.Insert(_caret.Column, eventArgs.KeySymbol.ToUpper());
        }
        else
        {
            CurrentLine.Insert(_caret.Column, eventArgs.KeySymbol);
        }
        _caret.Column += 1;
    }

    public void InsertLine(string line)
    {
        Lines.Add(new EditorLine(line));
    }

    public void InsertText(string text)
    {
        CurrentLine.Insert(_caret.Column, text);
    }
    public void HandleEnter()
    {
        string shiftedFragment = CurrentLine.Text.Substring(_caret.Column);
        CurrentLine.Remove(_caret.Column, shiftedFragment.Length);
        Lines.Insert(_caret.Line + 1, new EditorLine(shiftedFragment));

        _caret.Column = 0;
        _caret.Line++;
    }

    public void HandleBackspace()
    {
        if (_caret.Column == 0)
        {
            if (Lines.Count > 1)
            {
                Lines.RemoveAt(_caret.Line);
                _caret.Line--;
                _caret.Column = CurrentLine.Length;
            }
        } else
        {
            CurrentLine.Remove(CurrentLine.Length - 1, 1);
            _caret.Column -= 1;
        }
    }

    public void HandleTab()
    {
        CurrentLine.Insert(_caret.Column, "\t");
        _caret.Column += 1;
    }

    public void Clear()
    {
        Lines.Clear();
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