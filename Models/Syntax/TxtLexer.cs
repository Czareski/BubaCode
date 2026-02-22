using System.Collections.Generic;

namespace BubaCode.Models.Syntax;

public class TxtLexer : ILexer
{
    public List<Token> Tokenize(ITextStorage text)
    {
        List<Token> tokens = new List<Token>();
        tokens.Add(new Token(0, text.ToString().Length, TokenType.Text));
        return tokens;
    }
}