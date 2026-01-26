using BubaCode.Models.Commands;
using BubaCode.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.Models;

public partial class Caret : ObservableObject
{
    [ObservableProperty]
    private int _line;
    [ObservableProperty]
    private int _column;
    private readonly CodeBoxViewModel _vm;

    public Caret(CodeBoxViewModel vm)
    {
        _line = 0;
        _column = 0;
        _vm = vm;
    }

    partial void OnLineChanged(int value)
    {
        if (value < 0)
        {
            Line = 0;
            return;
        }
        if (value >= _vm.Text.LinesCount)
        {
            Line = _vm.Text.LinesCount - 1;
            return;
        }

        Line = value;
        OnColumnChanged(Column);
    }


    partial void OnColumnChanged(int value)
    {
        if (value < 0)
        {
            Column = 0;
            return;
        }
        if (value >= _vm.Text.GetLineLength(Line))
        {
            Column = _vm.Text.GetLineLength(Line);
            return;
        }

        Column = value;
    }

    public void SetPosition(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public void SetPosition(CaretPosition position)
    {
        Line = position.Line;
        Column = position.Column;
    }
}