using System;
using System.Drawing;
using Avalonia.Input;
using Avalonia.Threading;
using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class TypeCharacterCommand : TextEditingCommand, ITypeCommand
{
   
    private KeyEventArgs? _keyEventArgs;
    private DispatcherTimer _timer;
    public TypeCharacterCommand(KeyEventArgs keyEventArgs)
    {
        _keyEventArgs = keyEventArgs;
           
        _timer = new DispatcherTimer();
        _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        _timer.Tick += (_, _) =>
        {
            _timer.Stop();
        };
    }
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        OnExecute(sender);
        if (_keyEventArgs == null || _keyEventArgs.KeySymbol == null)
        {
            return ActionResult.DontAddToStack;
        }
        if (sender.Selection != null)
        {
            sender.Text.RemoveSelected(sender.Selection);
            sender.Selection = null;
        }
        sender.Text.InsertText(GetTypedValue());
        
        MakeCaretAfterSnapshot(sender);
        
        if (TryConcating(sender))
        {
            return ActionResult.DontAddToStack;
        }
        
        _timer.Start();
        return ActionResult.AddToStack;
        
    }

    public void Undo(CodeBoxViewModel sender)
    {
        OnUndo(sender);
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
        return _timer.IsEnabled;
    }
    // this class is concating
    public void Concat(CodeBoxViewModel vm, ITypeCommand other)
    {
        _timer.Stop();
        vm.GetActions().RemoveFromTop(); // itself
        string concatedValue = GetTypedValue() + other.GetTypedValue();
        TypeTextCommand concatedCommand = new TypeTextCommand(concatedValue, vm);
        
        vm.GetActions().PushWithoutDoing(concatedCommand);
    }
    
    // last command is concating
    public bool TryConcating(CodeBoxViewModel vm)
    {
        if (vm.GetActions().LastCommand != null && vm.GetActions().LastCommand is ITypeCommand)
        {
            ITypeCommand typeCommand = (ITypeCommand) vm.GetActions().LastCommand;
            if (typeCommand.CanBeConcated())
            {
                typeCommand.Concat(vm, this);
                return true;
            }
        }
        return false;
    }


}