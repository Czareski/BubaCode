using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Avalonia.Input.Platform;
using BubaCode.ViewModels;

namespace BubaCode.Models.Shortcut_Commands;

public class CopyCommand : IShortcutCommand
{
    public void Execute(CodeBoxViewModel sender)
    {
        Selection? selection = sender.Selection;
        if (selection == null || !selection.HasSelectedFragmentOfText()) return;
        
        string copiedText = sender.Text.GetTextFromSelection(selection);
        ClipboardService.Instance?.SetTextAsync(copiedText);
    }
}