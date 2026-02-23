using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class SaveCommand : ICommand
{
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        sender.Export();
        return ActionResult.DontAddToStack;
    }

    public void Undo(CodeBoxViewModel sender)
    {
    }

    public void Redo(CodeBoxViewModel sender)
    {

    }
}