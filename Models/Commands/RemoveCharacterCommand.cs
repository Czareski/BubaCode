using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class RemoveCharacterCommand : TextEditingCommand, ICommand
{
    private char? removedCharacter;
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        OnExecute(sender);
        removedCharacter = sender.Text.HandleBackspace();
        MakeCaretAfterSnapshot(sender);
        if (removedCharacter == null)
        {
            return ActionResult.DontAddToStack;
        }
        return ActionResult.AddToStack;
    }

    public void Undo(CodeBoxViewModel sender)
    {
        if (removedCharacter == null) return;
        OnUndo(sender);
    }

    public void Redo(CodeBoxViewModel sender)
    {
        OnRedo(sender);
    }
}