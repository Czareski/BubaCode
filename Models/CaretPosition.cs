namespace BubaCode.Models;

public struct CaretPosition(int line, int column)
{
    public int Line = line;
    public int Column = column;
}