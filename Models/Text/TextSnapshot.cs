using System.Collections.Generic;

namespace BubaCode.Models;
public class TextSnapshot
{
    public LinkedList<Piece> Pieces { get; }
    public TextLines Lines { get; }
    public string Original { get; }
    public string Added { get; }

    public TextSnapshot(LinkedList<Piece> pieces, TextLines lines, string original, string added)
    {
        Pieces = new LinkedList<Piece>(pieces);
        Lines = lines.Clone();
        Original = original;
        Added = added;
    }
}