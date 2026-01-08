using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using BubaCode.ViewModels;

namespace BubaCode.Models.Shortcut_Commands;

public class PasteCommand : IShortcutCommand
{
    public void Execute(CodeBoxViewModel sender)
    {
        string? clipboardText = ClipboardService.Instance?.GetTextAsync().Result;
        if (clipboardText == null) return;
        sender.Lines[sender.CaretLine].Insert(sender.CaretColumn, clipboardText);
        sender.SetCaret(sender.CaretLine, sender.CaretColumn + clipboardText.Length);
        
    }
}