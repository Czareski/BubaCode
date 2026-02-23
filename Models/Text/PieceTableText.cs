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
    public int Start { get; } = start;
    public int Length { get; } = length;
    public BufferType Type { get; } = type;
}

public class PieceTableText
{
    private string _original;
    private StringBuilder _added = new();
    private TextLines _lines = new();
    public LinkedList<Piece> Pieces = new();
    public TextLines GetLines() => _lines;
    public string GetOriginal() => _original;
    public string GetAdded() => _added.ToString();
    public int Length
    {
        get
        {
            int len = 0;
            foreach (var p in Pieces)
                len += p.Length;
            return len;
        }
    }
    
    public PieceTableText(string text)
    {
        text = text.Replace("\r", "");
        _original = text;
        
        _lines.OnInsert(0, text);
        
        Pieces.AddFirst(new Piece(0, _original.Length, BufferType.Original));
    }

    public void Insert(string text, int index)
    {
        _lines.OnInsert(index, text);
        int offset = 0;
        var node = Pieces.First;
        while (node != null)
        {
            var piece = node.Value;
            // at the end of piece (no split)
            if (index == offset + piece.Length)
            {
                Pieces.AddAfter(node, new Piece(_added.Length, text.Length, BufferType.Added));
                _added.Append(text);
                break;
            }
            // in the part of piece (split)
            if (index < offset + piece.Length)
            {
                int localIndex = index - offset;
                int secondLength = piece.Length - localIndex;

                // Replace current piece with shortened version
                var newFirstPiece = new Piece(piece.Start, localIndex, piece.Type);
                node.Value = newFirstPiece;

                var inserted = Pieces.AddAfter(
                    node,
                    new Piece(_added.Length, text.Length, BufferType.Added)
                );
                _added.Append(text);

                if (secondLength > 0)
                {
                    Pieces.AddAfter(
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
        _lines.OnInsert(index, c);
        int offset = 0;
        var node = Pieces.First;
        while (node != null)
        {
            var piece = node.Value;
            // at the end of piece (no split)
            if (index == offset + piece.Length)
            {
                Pieces.AddAfter(node, new Piece(_added.Length, 1, BufferType.Added));
                _added.Append(c);
                break;
            }
            // in the part of piece (split)
            if (index < offset + piece.Length)
            {
                int localIndex = index - offset;
                int secondLength = piece.Length - localIndex;

                // Replace current piece with shortened version
                var newFirstPiece = new Piece(piece.Start, localIndex, piece.Type);
                node.Value = newFirstPiece;

                var inserted = Pieces.AddAfter(
                    node,
                    new Piece(_added.Length, 1, BufferType.Added)
                );
                _added.Append(c);

                if (secondLength > 0)
                {
                    Pieces.AddAfter(
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
        _lines.OnDelete(index, length);
        
        int offset = 0;
        int remaining = length;
        bool firstPiece = true;

        var node = Pieces.First;
        
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
                Pieces.Remove(node);

                int bufferStart = piece.Start;

                removed.Append(
                    piece.Type == BufferType.Added
                        ? _added.ToString(bufferStart, piece.Length)
                        : _original.Substring(bufferStart, piece.Length)
                );
            }
            // początek
            else if (localStart == 0)
            {
                var newPiece = new Piece(piece.Start + localLen, piece.Length - localLen, piece.Type);
                node.Value = newPiece;
                removed.Append(piece.Type == BufferType.Added ? _added.ToString(piece.Start, localLen) : _original.Substring(piece.Start, localLen));
            }
            // koniec
            else if (localStart + localLen == piece.Length)
            {
                var newPiece = new Piece(piece.Start, piece.Length - localLen, piece.Type);
                node.Value = newPiece;
                removed.Append(piece.Type == BufferType.Added ? _added.ToString(piece.Start + piece.Length - localLen, localLen) : _original.Substring(piece.Start + piece.Length - localLen, localLen));
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

                var left = new Piece(piece.Start, localStart, piece.Type);
                node.Value = left;
                Pieces.AddAfter(node, right);
                removed.Append(piece.Type == BufferType.Added ? _added.ToString(piece.Start + localStart, localLen) : _original.Substring(piece.Start + localStart, localLen));
            }

            remaining -= localLen;
            firstPiece = false;
            node = next;
        }

        return removed.ToString();
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

        var node = Pieces.First;
        while (node != null && remaining > 0)
        {
            var piece = node.Value;

            int pieceGlobalStart = globalPos;
            int pieceGlobalEnd = globalPos + piece.Length;

            if (pieceGlobalEnd <= offset)
            {
                globalPos = pieceGlobalEnd;
                node = node.Next;
                continue;
            }

            if (pieceGlobalStart >= offset + length)
                break;

            int takeFromPieceStart = Math.Max(0, offset - pieceGlobalStart);
            int canTake = piece.Length - takeFromPieceStart;
            int takeLen = Math.Min(canTake, remaining);

            int sourceStart = piece.Start + takeFromPieceStart;

            if (piece.Type == BufferType.Added)
                result.Append(_added.ToString(sourceStart, takeLen));
            else
                result.Append(_original.AsSpan(sourceStart, takeLen));

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
        string addedParsed = _added.ToString();
        foreach (var piece in Pieces)
        {
            if (piece.Type == BufferType.Added)
            {
                result += addedParsed.Substring(piece.Start, piece.Length);
            }
            else
            {
                result += _original.Substring(piece.Start, piece.Length);
            }
        }

        return result;
    }
    
    public void MergePieces()
    {
        var node = Pieces.First;
        while (node != null && node.Next != null)
        {
            var current = node.Value;
            var next = node.Next.Value;

            // Check if pieces are adjacent in the same buffer and can be merged
            if (current.Type == next.Type &&
                current.Start + current.Length == next.Start)
            {
                // Create new merged piece
                var merged = new Piece(current.Start, current.Length + next.Length, current.Type);
                node.Value = merged;

                // Remove the next piece
                var toRemove = node.Next;
                Pieces.Remove(toRemove);
                // Don't advance node, check if we can merge with the new next
            }
            else
            {
                node = node.Next;
            }
        }
    }

    public void RestoreSnapshot(TextSnapshot snapshot)
    {
        Pieces = new LinkedList<Piece>(snapshot.Pieces);
        _lines = snapshot.Lines.Clone();
        _original = snapshot.Original;
        _added = new StringBuilder(snapshot.Added);
    }
}