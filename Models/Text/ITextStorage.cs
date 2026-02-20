using System;

namespace BubaCode.Models;

public interface ITextStorage
{
    public int LinesCount { get; }
    event Action? LinesCountChanged;
    void InsertText(string text);
    void InsertChar(char c);
    void HandleEnter();
    void UndoHandleEnter();
    char? HandleBackspace();
    void HandleTab();
    int GetLineLength(int line);
    string GetText(Selection range);
    string Remove(Selection range);
    string GetLine(int line);

}
