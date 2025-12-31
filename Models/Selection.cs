using System.Drawing;

namespace BubaCode.Models;

public class Selection
{
    // System.Drawing.Point defines a caret position. X variable is caret line, Y variable is caret column.
    private Point _firstPosition;
    private Point _secondPosition;

    public Point StartPosition;
    public Point EndPosition;
    
    public Selection(Point pressedPosition)
    {
        _firstPosition = pressedPosition;
        StartPosition = pressedPosition;
    }
    public void Update(Point position)
    {
        _secondPosition = position;
        if (this.HasSelectedFragmentOfText())
        {
            UpdateSelectionBounds();
        }
    }

    public bool HasSelectedFragmentOfText()
    {
        return _firstPosition != _secondPosition;
    }
    
    private void UpdateSelectionBounds()
    {
        if (_firstPosition.X < _secondPosition.X || (_firstPosition.X == _secondPosition.X && _firstPosition.Y <= _secondPosition.Y))
        {
            StartPosition = _firstPosition;
            EndPosition = _secondPosition;
        }
        else
        {
            StartPosition = _secondPosition;
            EndPosition = _firstPosition;
        }
    }
}