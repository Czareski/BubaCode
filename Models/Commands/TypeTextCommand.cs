using System;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using Avalonia.Threading;
using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class TypeTextCommand : TextEditingCommand, ITypeCommand
{
    private DispatcherTimer _timer;
    private string value;
    public TypeTextCommand(string concatedValue, CodeBoxViewModel vm)
    {
        value = concatedValue;
        _timer = new DispatcherTimer();
        _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        _timer.Tick += (_, _) =>
        {
            _timer.Stop();
        };
        _timer.Start();
        MakeCaretAfterSnapshot(vm);
    }
    
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        OnExecute(sender);
        
        sender.Text.InsertText(GetTypedValue());
        
        
        return ActionResult.AddToStack;
    }

    public void Undo(CodeBoxViewModel sender)
    {
        OnUndo(sender);
        var selection = new Selection(new Point(sender.Caret.Line, sender.Caret.Column));
        selection.Update(new Point(sender.Caret.Line, sender.Caret.Column - value.Length));
        sender.Text.RemoveSelected(selection);
    }

    public string GetTypedValue()
    {
        return value;
    }

    public bool CanBeConcated()
    {
        return _timer.IsEnabled;
    }

    public void Concat(CodeBoxViewModel vm, ITypeCommand other)
    {
        value = GetTypedValue() + other.GetTypedValue();
        MakeCaretAfterSnapshot(vm);
        _timer.Start();
    }
}