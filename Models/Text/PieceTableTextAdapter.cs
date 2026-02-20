using System;
using BubaCode.ViewModels;

namespace BubaCode.Models;

public class PieceTableTextAdapter : ITextStorage
{
    private PieceTableText _text;
    private CodeBoxViewModel _vm;
    public int LinesCount => _text.GetLines().Count;
    public event Action? LinesCountChanged;

    
    public PieceTableTextAdapter(CodeBoxViewModel vm, PieceTableText text)
    {
        _text = text;
        _vm = vm;
    }

    private int GetCurrentOffset()
    {
        return GetOffset(_vm.Caret.Line, _vm.Caret.Column);
    }

    private int GetOffset(int line, int column)
    {
        return _text.GetLines().GetOffset(line) + column;
    }

    private CaretPosition GetCaretPosition(int offset)
    {
        int line = _text.GetLines().GetLine(offset);
        return new CaretPosition(line, offset - GetOffset(line, 0));
    }

    public void InsertText(string text)
    {
        int oldLines = LinesCount;
        _text.Insert(text, GetCurrentOffset());
        if (oldLines != LinesCount)
            LinesCountChanged?.Invoke();
        CaretPosition position = GetCaretPosition(GetCurrentOffset() + text.Length);
        _vm.Caret.SetPosition(position);
    }

    public void InsertChar(char c)
    {
        int oldLines = LinesCount;
        _text.Insert(c, GetCurrentOffset());
        if (oldLines != LinesCount)
            LinesCountChanged?.Invoke();
        _vm.Caret.Column++;
    }

    public void HandleEnter()
    {
        _text.Insert('\n', GetCurrentOffset());
        LinesCountChanged?.Invoke();
        _vm.Caret.Line++;
        _vm.Caret.Column = 0;
    }

    public void UndoHandleEnter()
    {
        HandleBackspace();
    }

    public char? HandleBackspace()
    {
        int currentOffset = GetCurrentOffset();
        if (_text.Length == 0 || currentOffset == 0)
            return null;

        int deleteOffset = currentOffset - 1;

        int oldLines = LinesCount;
        string removed = _text.Delete(deleteOffset, 1);
        if (oldLines != LinesCount)
            LinesCountChanged?.Invoke();
        _vm.Caret.SetPosition(GetCaretPosition(deleteOffset));

        return removed.Length > 0 ? removed[0] : null;
    }

    public void HandleTab()
    {
        _text.Insert('\t', GetCurrentOffset());
        _vm.Caret.Column += 1;
    }

    public string GetText(Selection range)
    {
        int startOffset = GetOffset(range.StartPosition.X, range.StartPosition.Y);
        int endOffset = GetOffset(range.EndPosition.X, range.EndPosition.Y);
        return _text.GetText(startOffset, endOffset - startOffset);
    }
    public string GetText(int offset, int length) => _text.GetText(offset, length);

    public string Remove(Selection range)
    {
        int startOffset = GetOffset(range.StartPosition.X, range.StartPosition.Y);
        int endOffset = GetOffset(range.EndPosition.X, range.EndPosition.Y);
        CaretPosition position = GetCaretPosition(startOffset);
        _vm.Caret.SetPosition(position);
        int oldLines = LinesCount;
        var removed = _text.Delete(startOffset, endOffset - startOffset);
        if (oldLines != LinesCount)
            LinesCountChanged?.Invoke();
        return removed;
    }

    public string GetLine(int line)
    {
        int offset = _text.GetLines().GetOffset(line);

        return _text.GetText(offset, GetLineLength(line));
    }

    public int GetLineLength(int line)
    {
        if (line < 0 || line >= LinesCount)
            return 0;

        int startOffset = _text.GetLines().GetOffset(line);

        bool hasNextLine = (line + 1 < _text.GetLines().Count);
        int endOffset = hasNextLine
            ? _text.GetLines().GetOffset(line + 1)
            : _text.Length;

        int len = endOffset - startOffset;

        if (hasNextLine && len > 0)
            len -= 1;

        return len;
    }
    public TextLines Lines => _text.GetLines();
    public override string ToString()
    {
        return _text.Export();
    }
}