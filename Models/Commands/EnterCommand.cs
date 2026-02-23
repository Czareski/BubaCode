using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class EnterCommand : TextEditingCommand, ICommand
{
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        OnExecute(sender);
        sender.Text.HandleEnter();
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
}