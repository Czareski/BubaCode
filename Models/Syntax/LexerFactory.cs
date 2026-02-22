namespace BubaCode.Models.Syntax;

public class LexerFactory
{
    public static ILexer Create(string extension)
    {
        extension = extension.ToLower();
        return extension switch
        {
            ".cs" => new Lexer(),
            ".js" => new Lexer(),
            ".py" => new Lexer(),
            ".css" => new Lexer(),
            ".java" => new Lexer(),
            ".json" => new Lexer(),
            ".html" => new HtmlLexer(),
            ".htm" => new HtmlLexer(),
            _ => new TxtLexer(),
        };
    }
}