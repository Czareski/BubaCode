using System;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using Avalonia.Input;
using Avalonia.Threading;
using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class TypeTextCommand : TextEditingCommand, ITypeCommand
{
    private DispatcherTimer _timer;
    private string? _value = null;
    public TypeTextCommand(KeyEventArgs args)
    {
        
        _value = GetValueFromArgs(args);
        _timer = new DispatcherTimer();
        _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        _timer.Tick += (_, _) =>
        {
            _timer.Stop();
        };
        _timer.Start();
    }
    
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        if (_value == null) return ActionResult.DontAddToStack;
        OnExecute(sender);
        
        sender.Text.InsertText(GetTypedValue());
        if (sender.GetActions().LastCommand is TypeTextCommand typeTextCommand)
        {
            if (typeTextCommand.CanBeConcated())
            {
                typeTextCommand.Concat(sender, this);
                return ActionResult.DontAddToStack;
            }
        }
        MakeCaretAfterSnapshot(sender);

        return ActionResult.AddToStack;
    }

    public void Undo(CodeBoxViewModel sender)
    {
        OnUndo(sender);
    }

    public void Redo(CodeBoxViewModel sender)
    {
        OnRedo(sender);
    }

    public string GetValueFromArgs(KeyEventArgs args)
    {
        if (args.KeySymbol == null) return null;
        if (args!.KeyModifiers.HasFlag(KeyModifiers.Shift))
            return args.KeySymbol!.ToUpper();
        return args.KeySymbol!;
    }

    public string GetTypedValue()
    {
        return _value;
    }

    public bool CanBeConcated()
    {
        return _timer.IsEnabled;
    }

    public void Concat(CodeBoxViewModel vm, ITypeCommand other)
    {
        _value = GetTypedValue() + other.GetTypedValue();
        MakeCaretAfterSnapshot(vm);
        _timer.Start();
    }
}