using System.Collections.Generic;

namespace BubaCode.Models;

public class TextLines
{
    private List<int> newLineOffsets = new([0]);
    public int Count => newLineOffsets.Count;
    public void OnInsert(int offset, string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        int lineIndex = GetLine(offset);

        var insertedLines = new List<int>();

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
                insertedLines.Add(offset + i + 1);
        }

        newLineOffsets.InsertRange(lineIndex + 1, insertedLines);

        int additionalOffset = text.Length;
        int firstAffectedLine = lineIndex + 1 + insertedLines.Count;

        for (int i = firstAffectedLine; i < newLineOffsets.Count; i++)
        {
            newLineOffsets[i] += additionalOffset;
        }
    }
    public void OnInsert(int offset, char c)
    {

        int lineIndex = GetLine(offset);

        var insertedLines = new List<int>();

        
        if (c == '\n')
            insertedLines.Add(offset + 1);

        newLineOffsets.InsertRange(lineIndex + 1, insertedLines);

        int additionalOffset = 1;
        int firstAffectedLine = lineIndex + 1 + insertedLines.Count;

        for (int i = firstAffectedLine; i < newLineOffsets.Count; i++)
        {
            newLineOffsets[i] += additionalOffset;
        }
    }

    public void OnDelete(int offset, int length)
    {
        if (length <= 0)
            return;

        int end = offset + length;

        int lineIndex = GetLine(offset);

        int removeFrom = lineIndex + 1;
        int removeTo = removeFrom;

        while (removeTo < newLineOffsets.Count &&
               newLineOffsets[removeTo] <= end)
        {
            removeTo++;
        }

        int removed = removeTo - removeFrom;

        if (removed > 0)
            newLineOffsets.RemoveRange(removeFrom, removed);

        for (int i = removeFrom; i < newLineOffsets.Count; i++)
        {
            newLineOffsets[i] -= length;
        }
    }

    public int GetOffset(int line)
    {
        if (line >= newLineOffsets.Count)
        {
            return -1;
        }

        return newLineOffsets[line];
    }

    public int GetLine(int offset)
    {
        int lineIndex = newLineOffsets.BinarySearch(offset);
        if (lineIndex < 0)
            lineIndex = ~lineIndex - 1;
        return lineIndex;
    }
}