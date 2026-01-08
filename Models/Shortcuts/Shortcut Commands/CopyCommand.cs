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
        Selection selection = sender.Selection;
        
        StringBuilder copiedText = new();
        for (int line = selection.StartPosition.X; line <= selection.EndPosition.X; line++)
        {
            var start = line == selection.StartPosition.X ? selection.StartPosition.Y : 0;
            var end = line == selection.EndPosition.X ? selection.EndPosition.Y : sender.Lines[line].Length;
            
            copiedText.Append(sender.Lines[line].Text.Substring(start, end - start));
        }

        ClipboardService.Instance?.SetTextAsync(copiedText.ToString());
    }
}