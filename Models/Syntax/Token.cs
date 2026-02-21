namespace BubaCode.Models.Syntax;

public struct Token
{
    public int StartOffset;
    public int Length;
    public TokenType Type;

    public Token(int startOffset, int length, TokenType type)
    {
        StartOffset = startOffset;
        Length = length;
        Type = type;
    }

    public int EndOffset => StartOffset + Length;
}
