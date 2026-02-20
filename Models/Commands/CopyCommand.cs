using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Avalonia.Input.Platform;
using BubaCode.ViewModels;

namespace BubaCode.Models.Shortcut_Commands;

public class CopyCommand : ICommand
{
    ActionResult ICommand.Execute(CodeBoxViewModel sender)
    {
        Selection? selection = sender.Selection;
        if (selection == null || !selection.HasSelectedFragmentOfText()) return ActionResult.DontAddToStack;
        
        string copiedText = sender.Text.GetText(selection);
        ClipboardService.Instance?.SetTextAsync(copiedText);
        return ActionResult.DontAddToStack;
    }
    
    public void Undo(CodeBoxViewModel sender)
    {
        return;
    }

    
}