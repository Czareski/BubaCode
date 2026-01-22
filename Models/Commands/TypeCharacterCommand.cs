using System;
using Avalonia.Input;
using Avalonia.Threading;
using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class TypeCharacterCommand : ITypeCommand
{
    private KeyEventArgs? _keyEventArgs;
    private DispatcherTimer _timer;
    private bool _canBeConcated = true;
    public TypeCharacterCommand(KeyEventArgs keyEventArgs)
    {
        _keyEventArgs = keyEventArgs;
           
        _timer = new DispatcherTimer();
        _timer.Interval = new TimeSpan(0, 0, 0, 0, 700);
        _timer.Tick += (_, _) =>
        {
            _canBeConcated = false;
        };
    }
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        if (_keyEventArgs == null || _keyEventArgs.KeySymbol == null)
        {
            return ActionResult.DontAddToStack;
        }

        if (TryConcating(sender))
        {
            return ActionResult.DontAddToStack;
        }
        
        if (sender.Selection != null)
        {
            sender.Text.RemoveSelected(sender.Selection);
            sender.Selection = null;
        }
        sender.Text.InsertText(GetTypedValue());
        return ActionResult.AddToStack;
    }

    public void Undo(CodeBoxViewModel sender)
    {
        sender.Text.HandleBackspace();
    }

    public string GetTypedValue()
    {
        if (_keyEventArgs!.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            return _keyEventArgs.KeySymbol!.ToUpper();
        }
        return _keyEventArgs.KeySymbol!;
    }

    public bool CanBeConcated()
    {
        return _canBeConcated;
    }

    public void Concat(CodeBoxViewModel vm, ITypeCommand other)
    {
        _timer.Stop();
        string concatedValue =  GetTypedValue() + other.GetTypedValue();
        TypeTextCommand concatedCommand = new TypeTextCommand(concatedValue, _timer.Interval);
        
        vm.GetActions().Do(concatedCommand);
    }

    public bool TryConcating(CodeBoxViewModel vm)
    {
        if (vm.GetActions().LastCommand is ITypeCommand)
        {
            var typeCommand = vm.GetActions().RemoveFromTop() as ITypeCommand;
            if (typeCommand!.CanBeConcated())
            {
                typeCommand.Concat(vm, this);
                return true;
            }
        }
        return false;
    }


}