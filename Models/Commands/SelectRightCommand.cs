using System.Drawing;
using BubaCode.ViewModels;

namespace BubaCode.Models.Shortcut_Commands;

public class SelectRightCommand : ICommand
{
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        if (sender.Selection == null)
        {
            sender.Selection = new Selection(new Point(sender.Caret.Line, sender.Caret.Column));
        }
        sender.Caret.Column += 1;
        sender.Selection.Update(new Point(sender.Caret.Line, sender.Caret.Column));
        return ActionResult.DontAddToStack;
    }
    public void Undo(CodeBoxViewModel sender)
    {
        return;
    }

    public void Redo(CodeBoxViewModel sender)
    {
        return;
    }
}