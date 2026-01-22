using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class RemoveCharacterCommand : ICommand
{
    private char? removedCharacter;
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        removedCharacter = sender.Text.HandleBackspace();
        if (removedCharacter == null)
        {
            return ActionResult.DontAddToStack;
        }
        return ActionResult.AddToStack;
    }

    public void Undo(CodeBoxViewModel sender)
    {
        if (removedCharacter == null) return;
        
        sender.Text.InsertChar((char)removedCharacter);
    }
}