using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Media;

namespace BubaCode.Models;

public class EditorLine : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private StringBuilder _line;
    public string Text => _line.ToString();
    public int Length => _line.Length;
    public int LengthWithoutNewLine
    {
        get
        {
            if (_line[^1] == '\n')
            {
                return _line.Length - 1;
            }

            return Length;
        }
    }
    
    public EditorLine(string value)
    {
        _line = new StringBuilder(value);
    }

    public void Insert(int index, string value)
    {
        _line.Insert(index, value);
        OnPropertyChanged(nameof(Text));
    }
    public void Insert(int index, char value)
    {
        _line.Insert(index, value);
        OnPropertyChanged(nameof(Text));
    }

    public StringBuilder Remove(int index, int count)
    {
        var result = _line.Remove(index, count);
        OnPropertyChanged(nameof(Text));
        return result;
    }

    public void Set(StringBuilder newStringBuilder)
    {
        _line = newStringBuilder;
        OnPropertyChanged(nameof(Text));
    }
    
}