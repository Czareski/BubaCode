using System;
using System.Collections.Generic;

namespace BubaCode.Models.Syntax;

public class HtmlLexer : ILexer
{
    public List<Token> Tokenize(ITextStorage text) {
        var tokens = new List<Token>();
        string fullText = GetFullText(text);

        int offset = 0;
        while (offset < fullText.Length)
        {
            char c = fullText[offset];

            // Whitespace
            if (char.IsWhiteSpace(c))
            {
                int start = offset;
                while (offset < fullText.Length && char.IsWhiteSpace(fullText[offset]))
                    offset++;
                tokens.Add(new Token(start, offset - start, TokenType.Whitespace));
                continue;
            }

            // HTML comment: <!-- ... -->
            if (c == '<' && StartsWith(fullText, offset, "<!--"))
            {
                int start = offset;
                offset += 4;
                while (offset < fullText.Length && !StartsWith(fullText, offset, "-->"))
                    offset++;

                if (offset < fullText.Length) offset += 3; // consume -->
                tokens.Add(new Token(start, offset - start, TokenType.Comment));
                continue;
            }

            // Doctype: <!DOCTYPE ...>
            if (c == '<' && StartsWithIgnoreCase(fullText, offset, "<!doctype"))
            {
                int start = offset;
                offset += 2; // "<!"
                while (offset < fullText.Length && fullText[offset] != '>')
                    offset++;
                if (offset < fullText.Length) offset++; // '>'
                tokens.Add(new Token(start, offset - start, TokenType.Keyword));
                continue;
            }

            // Tag open "<" or "</"
            if (c == '<')
            {
                // '<'
                tokens.Add(new Token(offset, 1, TokenType.Operator));
                offset++;

                // optional '/'
                if (offset < fullText.Length && fullText[offset] == '/')
                {
                    tokens.Add(new Token(offset, 1, TokenType.Operator));
                    offset++;
                }

                // tag name
                int tagStart = offset;
                while (offset < fullText.Length && IsNameChar(fullText[offset]))
                    offset++;

                if (offset > tagStart)
                    tokens.Add(new Token(tagStart, offset - tagStart, TokenType.Keyword));

                // inside tag: attributes until '>'
                while (offset < fullText.Length)
                {
                    c = fullText[offset];

                    if (char.IsWhiteSpace(c))
                    {
                        int wsStart = offset;
                        while (offset < fullText.Length && char.IsWhiteSpace(fullText[offset]))
                            offset++;
                        tokens.Add(new Token(wsStart, offset - wsStart, TokenType.Whitespace));
                        continue;
                    }

                    if (c == '>')
                    {
                        tokens.Add(new Token(offset, 1, TokenType.Operator));
                        offset++;
                        break;
                    }

                    if (c == '/' && offset + 1 < fullText.Length && fullText[offset + 1] == '>')
                    {
                        tokens.Add(new Token(offset, 1, TokenType.Operator));     // '/'
                        tokens.Add(new Token(offset + 1, 1, TokenType.Operator)); // '>'
                        offset += 2;
                        break;
                    }

                    // '=' or other punctuation
                    if (c is '=' or ':' or '.' or '-' )
                    {
                        tokens.Add(new Token(offset, 1, TokenType.Operator));
                        offset++;
                        continue;
                    }

                    // quoted attribute value
                    if (c is '"' or '\'')
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
                        if (offset < fullText.Length) offset++; // closing quote
                        tokens.Add(new Token(start, offset - start, TokenType.String));
                        continue;
                    }

                    // entity like &amp; or &#123; or &#x1F600;
                    if (c == '&')
                    {
                        int start = offset;
                        offset++;
                        while (offset < fullText.Length && fullText[offset] != ';' && !char.IsWhiteSpace(fullText[offset]) &&
                               fullText[offset] != '<' && fullText[offset] != '>')
                        {
                            offset++;
                        }
                        if (offset < fullText.Length && fullText[offset] == ';') offset++;
                        tokens.Add(new Token(start, offset - start, TokenType.Number));
                        continue;
                    }

                    // attribute name or unquoted value
                    if (IsNameStartChar(c))
                    {
                        int start = offset;
                        offset++;
                        while (offset < fullText.Length && IsNameChar(fullText[offset]))
                            offset++;
                        tokens.Add(new Token(start, offset - start, TokenType.Identifier));
                        continue;
                    }

                    // fallback single char
                    tokens.Add(new Token(offset, 1, TokenType.Operator));
                    offset++;
                }

                continue;
            }

            // Text node until next '<'
            {
                int start = offset;
                while (offset < fullText.Length && fullText[offset] != '<')
                    offset++;
                tokens.Add(new Token(start, offset - start, TokenType.Text));
            }
        }

        return tokens;
    }

    private static bool IsNameStartChar(char c) =>
        char.IsLetter(c) || c == '_' || c == ':'; // allow namespaces like x:Something

    private static bool IsNameChar(char c) =>
        IsNameStartChar(c) || char.IsDigit(c) || c is '-' or '.';

    private static bool StartsWith(string s, int index, string value)
    {
        if (index < 0 || index + value.Length > s.Length) return false;
        return string.CompareOrdinal(s, index, value, 0, value.Length) == 0;
    }

    private static bool StartsWithIgnoreCase(string s, int index, string value)
    {
        if (index < 0 || index + value.Length > s.Length) return false;
        return string.Compare(s, index, value, 0, value.Length, StringComparison.OrdinalIgnoreCase) == 0;
    }

    private static string GetFullText(ITextStorage text)
    {
        var lines = new List<string>(text.LinesCount);
        for (int i = 0; i < text.LinesCount; i++)
            lines.Add(text.GetLine(i));
        return string.Join("\n", lines);
    }
}