using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media.TextFormatting;

namespace BubaCode.Models;

public enum BufferType
{
    Original,
    Added
}

public class Piece(int start, int length, BufferType type)
{
    public int Start = start;
    public int Length = length;
    public BufferType Type = type;
}

public class PieceTableText
{
    public string original;
    public StringBuilder added = new();
    private LinkedList<Piece> pieces = new();
    private TextLines lines = new();

    public int Length
    {
        get
        {
            int len = 0;
            foreach (var p in pieces)
                len += p.Length;
            return len;
        }
    }
    
    public PieceTableText(string text)
    {
        text = text.Replace("\r", "");
        original = text;
        
        lines.OnInsert(0, text);
        
        pieces.AddFirst(new Piece(0, original.Length, BufferType.Original));
    }

    public void Insert(string text, int index)
    {
        lines.OnInsert(index, text);
        int offset = 0;
        var node = pieces.First;
        while (node != null)
        {
            var piece = node.Value;
            // at the end of piece (no split)
            if (index == offset + piece.Length)
            {
                pieces.AddAfter(node, new Piece(added.Length, text.Length, BufferType.Added));
                added.Append(text);
                break;
            }
            // in the part of piece (split)
            if (index < offset + piece.Length)
            {
                int localIndex = index - offset;
                int secondLength = piece.Length - localIndex;

                piece.Length = localIndex;

                var inserted = pieces.AddAfter(
                    node,
                    new Piece(added.Length, text.Length, BufferType.Added)
                );
                added.Append(text);

                if (secondLength > 0)
                {
                    pieces.AddAfter(
                        inserted,
                        new Piece(piece.Start + localIndex, secondLength, piece.Type)
                    );
                }
                break;
            }

            offset += piece.Length;
            node = node.Next;
        }
    }
    
    public void Insert(char c, int index)
    {
        lines.OnInsert(index, c);
        int offset = 0;
        var node = pieces.First;
        while (node != null)
        {
            var piece = node.Value;
            // at the end of piece (no split)
            if (index == offset + piece.Length)
            {
                pieces.AddAfter(node, new Piece(added.Length, 1, BufferType.Added));
                added.Append(c);
                break;
            }
            // in the part of piece (split)
            if (index < offset + piece.Length)
            {
                int localIndex = index - offset;
                int secondLength = piece.Length - localIndex;

                piece.Length = localIndex;

                var inserted = pieces.AddAfter(
                    node,
                    new Piece(added.Length, 1, BufferType.Added)
                );
                added.Append(c);

                if (secondLength > 0)
                {
                    pieces.AddAfter(
                        inserted,
                        new Piece(piece.Start + localIndex, secondLength, piece.Type)
                    );
                }
                break;
            }

            offset += piece.Length;
            node = node.Next;
        }
    }
    
    public string Delete(int index, int length)
    {
        if (length <= 0)
            return "";
        lines.OnDelete(index, length);
        
        int offset = 0;
        int remaining = length;
        bool firstPiece = true;

        var node = pieces.First;
        
        StringBuilder removed = new(); 
        
        while (node != null && remaining > 0)
        {
            var piece = node.Value;
            int pieceStart = offset;
            int pieceEnd = offset + piece.Length;

            // całkiem przed pierwszym cięciem
            if (firstPiece && pieceEnd <= index)
            {
                offset += piece.Length;
                node = node.Next;
                continue;
            }

            int localStart = firstPiece
                ? Math.Max(0, index - pieceStart)
                : 0;

            int localLen = Math.Min(
                piece.Length - localStart,
                remaining
            );

            var next = node.Next;

            // cały piece do usuniecia
            if (localStart == 0 && localLen == piece.Length)
            {
                pieces.Remove(node);
                removed.Append(piece.Type == BufferType.Added ? added.ToString(pieceStart, piece.Length) : original.Substring(pieceStart, piece.Length));
            }
            // początek
            else if (localStart == 0)
            {
                piece.Start += localLen;
                piece.Length -= localLen;
                removed.Append(piece.Type == BufferType.Added ? added.ToString(pieceStart, localLen) : original.Substring(pieceStart, localLen));
            }
            // koniec
            else if (localStart + localLen == piece.Length)
            {
                piece.Length -= localLen;
                removed.Append(piece.Type == BufferType.Added ? added.ToString(pieceEnd - localLen, localLen) : original.Substring(pieceEnd - localLen, localLen));
            }
            // środek
            else
            {
                int rightLen = piece.Length - (localStart + localLen);

                var right = new Piece(
                    piece.Start + localStart + localLen,
                    rightLen,
                    piece.Type
                );

                piece.Length = localStart;
                pieces.AddAfter(node, right);
                removed.Append(piece.Type == BufferType.Added ? added.ToString(pieceStart, localStart + localLen) : original.Substring(pieceStart, localStart + localLen));
            }

            remaining -= localLen;
            firstPiece = false;
            node = next;
        }

        return removed.ToString();
    }

    public char GetCharacter(int index)
    {
        var node = pieces.First;
        var offset = 0;
        while (node != null)
        {
            Piece piece = node.Value;
            if (index < offset + piece.Length)
            {
                int localIndex = index - offset;
                return (piece.Type == BufferType.Added) ? added[piece.Start + localIndex] : original[piece.Start + localIndex];
            }
            
            offset += node.Value.Length;
            node = node.Next;
        }

        throw new ArgumentOutOfRangeException(nameof(index));
    }

    public string GetText(int offset, int length)
    {
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length));
        if (length == 0)
            return string.Empty;

        var result = new StringBuilder(length);

        int globalPos = 0;
        int remaining = length;

        var node = pieces.First;
        while (node != null && remaining > 0)
        {
            var piece = node.Value;

            int pieceGlobalStart = globalPos;
            int pieceGlobalEnd = globalPos + piece.Length;

            // piece fully before requested range
            if (pieceGlobalEnd <= offset)
            {
                globalPos = pieceGlobalEnd;
                node = node.Next;
                continue;
            }

            // piece starts after requested range ends
            if (pieceGlobalStart >= offset + length)
                break;

            int takeFromPieceStart = Math.Max(0, offset - pieceGlobalStart);
            int canTake = piece.Length - takeFromPieceStart;
            int takeLen = Math.Min(canTake, remaining);

            int sourceStart = piece.Start + takeFromPieceStart;

            if (piece.Type == BufferType.Added)
                result.Append(added.ToString(sourceStart, takeLen));
            else
                result.Append(original.AsSpan(sourceStart, takeLen));

            remaining -= takeLen;

            globalPos = pieceGlobalEnd;
            node = node.Next;
        }

        if (remaining > 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Requested range exceeds the document length.");

        return result.ToString();
    }

    public string Export()
    {
        string result = "";
        string addedParsed = added.ToString();
        foreach (var piece in pieces)
        {
            if (piece.Type == BufferType.Added)
            {
                result += addedParsed.Substring(piece.Start, piece.Length);
            }
            else
            {
                result += original.Substring(piece.Start, piece.Length);
            }
        }

        return result;
    }

    public TextLines GetLines() => lines;
}