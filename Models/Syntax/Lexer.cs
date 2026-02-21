using System;
using System.Collections.Generic;

namespace BubaCode.Models.Syntax;

public class Lexer
{
    private static readonly HashSet<string> Keywords = new()
    {
        "if", "else", "while", "for", "return", "class", "public", "private", "static",
        "void", "int", "string", "bool", "var", "const", "let", "function", "new"
    };

    public List<Token> Tokenize(ITextStorage text)
    {
        var tokens = new List<Token>();

        string fullText = GetFullText(text);

        int offset = 0;
        while (offset < fullText.Length)
        {
            char c = fullText[offset];

            if (char.IsWhiteSpace(c))
            {
                int start = offset;
                while (offset < fullText.Length && char.IsWhiteSpace(fullText[offset]))
                    offset++;
                tokens.Add(new Token(start, offset - start, TokenType.Whitespace));
                continue;
            }

            if (c == '/' && offset + 1 < fullText.Length && fullText[offset + 1] == '/')
            {
                int start = offset;
                while (offset < fullText.Length && fullText[offset] != '\n')
                    offset++;
                tokens.Add(new Token(start, offset - start, TokenType.Comment));
                continue;
            }

            if (c == '"' || c == '\'')
            {
                int start = offset;
                char quote = c;
                offset++;
                while (offset < fullText.Length && fullText[offset] != quote)
                {
                    if (fullText[offset] == '\\' && offset + 1 < fullText.Length)
                        offset += 2;
                    else
                        offset++;
                }
                if (offset < fullText.Length)
                    offset++; 
                tokens.Add(new Token(start, offset - start, TokenType.String));
                continue;
            }

            if (char.IsDigit(c))
            {
                int start = offset;
                while (offset < fullText.Length && (char.IsDigit(fullText[offset]) || fullText[offset] == '.'))
                    offset++;
                tokens.Add(new Token(start, offset - start, TokenType.Number));
                continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                int start = offset;
                while (offset < fullText.Length && (char.IsLetterOrDigit(fullText[offset]) || fullText[offset] == '_'))
                    offset++;

                string word = fullText.Substring(start, offset - start);
                TokenType type;

                if (Keywords.Contains(word))
                {
                    type = TokenType.Keyword;
                }
                else
                {
                    type = DetermineIdentifierType(fullText, tokens, start, offset, word);
                }

                tokens.Add(new Token(start, offset - start, type));
                continue;
            }

            if (IsOperator(c))
            {
                int start = offset;
                offset++;
                tokens.Add(new Token(start, 1, TokenType.Operator));
                continue;
            }

            tokens.Add(new Token(offset, 1, TokenType.Text));
            offset++;
        }

        return tokens;
    }

    private string GetFullText(ITextStorage text)
    {
        var lines = new List<string>();
        for (int i = 0; i < text.LinesCount; i++)
        {
            lines.Add(text.GetLine(i));
        }
        return string.Join("\n", lines);
    }

    private TokenType DetermineIdentifierType(string fullText, List<Token> tokens, int start, int end, string word)
    {
        int nextPos = end;
        while (nextPos < fullText.Length && char.IsWhiteSpace(fullText[nextPos]))
            nextPos++;

        if (nextPos < fullText.Length && fullText[nextPos] == '(')
        {
            return TokenType.FunctionName;
        }

        if (tokens.Count > 0)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                var prevToken = tokens[i];
                if (prevToken.Type == TokenType.Whitespace)
                    continue;

                if (prevToken.Type == TokenType.Keyword)
                {
                    string prevWord = fullText.Substring(prevToken.StartOffset, prevToken.Length);
                    if (prevWord == "class" || prevWord == "interface" || prevWord == "enum" || prevWord == "struct")
                    {
                        return TokenType.ClassName;
                    }
                }
                break;
            }
        }

        if (char.IsUpper(word[0]))
        {
            return TokenType.TypeName;
        }

        return TokenType.Identifier;
    }

    private bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == '*' || c == '/' || c == '=' ||
               c == '<' || c == '>' || c == '!' || c == '&' || c == '|' ||
               c == '(' || c == ')' || c == '{' || c == '}' || c == '[' || c == ']' ||
               c == ';' || c == ',' || c == '.' || c == ':';
    }
}
