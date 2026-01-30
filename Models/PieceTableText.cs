using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media.TextFormatting;
using PieceTableReal;

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
    
    public void Delete(int index, int length)
    {
        if (length <= 0)
            return;
        lines.OnDelete(index, length);
        
        int offset = 0;
        int remaining = length;
        bool firstPiece = true;

        var node = pieces.First;

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
            }
            // początek
            else if (localStart == 0)
            {
                piece.Start += localLen;
                piece.Length -= localLen;
            }
            // koniec
            else if (localStart + localLen == piece.Length)
            {
                piece.Length -= localLen;
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
            }

            remaining -= localLen;
            firstPiece = false;
            node = next;
        }
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

    public void Import()
    {
        
    }
}