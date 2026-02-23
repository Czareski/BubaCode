using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using BubaCode.Models.Commands;
using BubaCode.ViewModels;

namespace BubaCode.Models.Shortcut_Commands;

public class PasteCommand : TextEditingCommand, ICommand
{
    public ActionResult Execute(CodeBoxViewModel sender)
    {
        string? clipboardText = ClipboardService.Instance?.GetTextAsync().Result;
        if (clipboardText == null) return ActionResult.DontAddToStack;
        
        OnExecuteForce(sender);
        
        if (sender.Selection != null && sender.Selection.HasSelectedFragmentOfText())
        {
            sender.Text.Remove(sender.Selection);
        }
        sender.Text.InsertText(clipboardText);
        
        MakeCaretAfterSnapshot(sender);
        sender.ResetSelection();
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