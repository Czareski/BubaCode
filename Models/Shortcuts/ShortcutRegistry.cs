using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Styling;
using BubaCode.Models.Shortcut_Commands;
using BubaCode.ViewModels;
using Tmds.DBus.Protocol;

namespace BubaCode.Models;

public class ShortcutRegistry
{
    private readonly Dictionary<KeyCombination, IShortcutCommand> _shortcuts = new();

    public ShortcutRegistry()
    {
        Register(new KeyCombination(Key.C).Ctrl(true), new CopyCommand());
        Register(new KeyCombination(Key.V).Ctrl(true), new PasteCommand());
        Register(new KeyCombination(Key.Left).Shift(true), new SelectLeftCommand());
        Register(new KeyCombination(Key.Right).Shift(true), new SelectRightCommand());
    }
    
    public void Register(KeyCombination combination, IShortcutCommand shortcut)
    {
        _shortcuts.TryAdd(combination, shortcut);
    }

    public bool Execute(KeyCombination combination, CodeBoxViewModel sender)
    {
        _shortcuts.TryGetValue(combination, out IShortcutCommand? shortcut);
        if (shortcut == null)
        {
            return false;
        }
        shortcut?.Execute(sender);
        return true;
    }
    
    
    
}