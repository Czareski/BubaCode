using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class RemoveFromSelectionCommand : TextEditingCommand, ICommand
{
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        if (sender.Selection == null) return ActionResult.DontAddToStack;
        OnExecute(sender);
        sender.Text.Remove(sender.Selection);
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