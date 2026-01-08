using Avalonia.Input;

namespace BubaCode.Models;

public record struct KeyCombination
{
    Key key;
    bool ctrl;
    bool shift;
    bool alt;

    public KeyCombination(KeyEventArgs e)
    {
        key = e.Key;
        shift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
        ctrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        alt = e.KeyModifiers.HasFlag(KeyModifiers.Alt);
    }
    public KeyCombination(Key key)
    {
        this.key = key;
    }

    public KeyCombination Shift(bool shift)
    {
        this.shift = shift;
        return this;
    }

    public KeyCombination Ctrl(bool ctrl)
    {
        this.ctrl = ctrl;
        return this;
    }

    public KeyCombination Alt(bool alt)
    {
        this.alt = alt;
        return this;
    }

    public override string ToString()
    {
        return key.ToString() + ((ctrl) ? "ctrl" : "") + ((alt) ? "alt" : "") + (shift ? "shift" : ""); 
    }
}