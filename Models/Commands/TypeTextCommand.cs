using System;
using System.Drawing;
using System.Timers;
using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class TypeTextCommand : ITypeCommand
{
    private Timer _timer;
    private string value;
    public TypeTextCommand(string concatedValue, TimeSpan timeLeftToConcat)
    {
        value = concatedValue;
        _timer = new Timer(timeLeftToConcat.TotalMilliseconds);
    }
    
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        sender.Text.InsertText(GetTypedValue());
        return ActionResult.AddToStack;
    }

    public void Undo(CodeBoxViewModel sender)
    {
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
        throw new System.NotImplementedException();
    }

    public void Concat(CodeBoxViewModel vm, ITypeCommand other)
    {
        vm.GetActions().RemoveFromTop(); // itself
        
        string concatedValue =  GetTypedValue() + other.GetTypedValue();
        TypeTextCommand concatedCommand = new TypeTextCommand(concatedValue);
        
        vm.GetActions().Do(concatedCommand);
    }
}