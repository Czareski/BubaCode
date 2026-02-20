using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using BubaCode.ViewModels;

namespace BubaCode.Models.Shortcut_Commands;

public class PasteCommand : ICommand
{
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        string? clipboardText = ClipboardService.Instance?.GetTextAsync().Result;
        if (clipboardText == null) return ActionResult.DontAddToStack;
        if (sender.Selection != null && sender.Selection.HasSelectedFragmentOfText())
        {
            sender.Text.Remove(sender.Selection);
        }
        sender.Text.InsertText(clipboardText);
        sender.ResetSelection();
        
        return ActionResult.DontAddToStack;
    }
    public void Undo(CodeBoxViewModel sender)
    {
        throw new System.NotImplementedException();
    }
}