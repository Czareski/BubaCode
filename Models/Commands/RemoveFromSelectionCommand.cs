using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class RemoveFromSelectionCommand : ICommand
{
    private string removedText;
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        if (sender.Selection == null) return ActionResult.DontAddToStack;
        removedText = sender.Text.Remove(sender.Selection);
        return ActionResult.AddToStack;
    }

    public void Undo(CodeBoxViewModel sender)
    {
        sender.Text.InsertText(removedText);
    }
}