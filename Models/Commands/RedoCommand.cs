using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class  RedoCommand : ICommand
{
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        sender.GetActions().Redo();
        return ActionResult.DontAddToStack;
    }

    public void Undo(CodeBoxViewModel sender)
    {
        
    }
}