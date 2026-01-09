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
        sender.Text.InsertText(clipboardText);
        sender.Caret.Column += clipboardText.Length;
        sender.ResetSelection();
    }
}