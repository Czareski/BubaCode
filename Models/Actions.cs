using System;
using System.Collections.Generic;
using Avalonia.Threading;
using BubaCode.Models.Commands;
using BubaCode.ViewModels;

namespace BubaCode.Models;

public class Actions
{
    private Stack<ICommand> _actions;
    private Stack<ICommand> _undoCommands;
    private readonly DispatcherTimer _typingTimer;
    private CodeBoxViewModel _vm;
    public ICommand? LastCommand => _actions.Count > 0 ? _actions.Peek() : null;
    
    public Actions(CodeBoxViewModel vm)
    {
        _actions = new Stack<ICommand>();
        _undoCommands = new Stack<ICommand>();
        _vm = vm;
        _typingTimer = new DispatcherTimer();
        _typingTimer.Interval = new TimeSpan(5000000);
        
    }

    public void Do(ICommand command)
    {
        ActionResult result = command.Execute(_vm);
        if (result == ActionResult.AddToStack)
        {
            _actions.Push(command);
            _undoCommands.Clear();
        }
    }
    public void Undo()
    {
        if (_actions.Count == 0) { return; }
        ICommand command = _actions.Pop();
        command.Undo(_vm);

        _undoCommands.Push(command);
    }

    public void Redo()
    {
        if (_undoCommands.Count == 0)
        {
            return;
        }
        ICommand command = _undoCommands.Pop();
        command.Execute(_vm);
        _actions.Push(command);
    }

    public ICommand? RemoveFromTop()
    {
        if (_actions.Count == 0) { return null; }
        return _actions.Pop();
    }

    public void PushWithoutDoing(ICommand command)
    {
        _actions.Push(command);
    }
}