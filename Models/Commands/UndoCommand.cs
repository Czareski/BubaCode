using System.Diagnostics.CodeAnalysis;
using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class UndoCommand : ICommand
{
    private ICommand? _recalledCommand;
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        sender.GetActions().Undo();
        return ActionResult.DontAddToStack;
    }

    public void Undo(CodeBoxViewModel sender)
    {
        return;
    }
}