using System.Collections.Generic;

namespace BubaCode.Models.Syntax;

public interface ILexer
{
    public abstract List<Token> Tokenize(ITextStorage text);
}