using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Controls.Shapes;
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
    private EditorLine CurrentLine =>Lines[_vm.Caret.Line];
    private string _newLine = "\r\n";
    
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

    public void ImportLine(string line)
    {
        Lines.Add(new EditorLine(line));
        _vm.Caret.Line += 1;
        CurrentLine.Insert(CurrentLine.Length, '\n');
    }
    
    public void InsertText(string text)
    {
        foreach (char c in text)
        {
            InsertChar(c);
        }
    }
    public void InsertChar(char c)
    {
        CurrentLine.Insert(_vm.Caret.Column, c);
        _vm.Caret.Column += 1;
        if (c == '\n')
        {
            Lines.Insert(_vm.Caret.Line + 1, new EditorLine(""));
            _vm.Caret.Line++;
            _vm.Caret.Column = 0;
        }
    }
    
    public void HandleEnter()
    {
        string shiftedFragment = CurrentLine.Text.Substring(_vm.Caret.Column);
        CurrentLine.Remove(_vm.Caret.Column, shiftedFragment.Length);
        CurrentLine.Insert(CurrentLine.Length, '\n');
        Lines.Insert(_vm.Caret.Line + 1, new EditorLine(shiftedFragment));

        _vm.Caret.Column = 0;
        _vm.Caret.Line++;
    }

    public char? HandleBackspace()
    {
        if (_vm.Caret.Column == 0)
        {
            if (Lines.Count > 1)
            {
                Lines.RemoveAt(_vm.Caret.Line);
                _vm.Caret.Line--;
                _vm.Caret.Column = CurrentLine.Length;
                return '\n';
            }
            return null;
        }
        var removed = CurrentLine.Text[_vm.Caret.Column - 1]; 
        CurrentLine.Remove(_vm.Caret.Column - 1, 1);
        _vm.Caret.Column -= 1;
        return removed;
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

    public string RemoveSelected(Selection selection)
    {
        string result = "";
        for (int line = selection.StartPosition.X; line <= selection.EndPosition.X; line++)
        {
            var start = line == selection.StartPosition.X ? selection.StartPosition.Y : 0;
            var end = line == selection.EndPosition.X ? selection.EndPosition.Y : GetLineLength(line);
            
            result += Lines[line].Text.Substring(start, end - start);
            var removed = Lines[line].Text.Remove(start, end - start);
            Lines[line].Set(new StringBuilder(removed)); 
        }
        _vm.Caret.Line = selection.StartPosition.X;
        _vm.Caret.Column = selection.StartPosition.Y;
        return result;
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
        foreach (EditorLine line in Lines)
        {
            
            result.Append(line.Text.Replace("\n", _newLine));
        }
        return result.ToString();
    }
}