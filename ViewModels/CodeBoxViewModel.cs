using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Input;
using BubaCode.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.ViewModels;

public partial class CodeBoxViewModel : ViewModelBase
{
    private FilesService _fileService;
    [ObservableProperty]
    private ObservableCollection<EditorLine> _lines;
    [ObservableProperty]
    private int _caretColumn;
    [ObservableProperty]
    private int _caretLine;

    public CodeBoxViewModel(FilesService fileService)
    {
        Lines = [new EditorLine()];
        CaretColumn = 0;
        _fileService = fileService;
        _fileService.FileImported += Import;
        _fileService.GetSourceToExport = Export;
    }

    public void OnKeyDown(KeyEventArgs e)
    {
    var currentLine = Lines[CaretLine];
        switch (e.Key)
            {
                case Key.Up:
                    MoveCaretY(-1);
                    break;
                case Key.Down:
                    MoveCaretY(1);
                    break;
                case Key.Left:
                    MoveCaretX(-1);
                    break;
                case Key.Right:
                    MoveCaretX(1);
                    break;
                case Key.Enter:
                    // 
                    string shiftedFragment = currentLine.Text.Substring(CaretColumn);
                    currentLine.Remove(CaretColumn, shiftedFragment.Length);
                    Lines.Insert(CaretLine + 1, new EditorLine(shiftedFragment));

                    CaretColumn = 0;
                    CaretLine++;
                    break;
                case Key.Back:
                    if (CaretColumn == 0)
                    {
                        if (Lines.Count > 1)
                        {
                            Lines.RemoveAt(CaretLine);
                            CaretLine--;
                            currentLine = Lines[CaretLine];
                            CaretColumn = currentLine.Length;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        currentLine.Remove(currentLine.Length - 1, 1);
                        CaretColumn -= 1;
                    }
                    break;
                case Key.Tab:
                    currentLine.Insert(CaretColumn, e.KeySymbol);
                    CaretColumn += 1;
                    e.Handled = true;
                    break;
                default:
                    if (e.KeySymbol == null)
                    {
                        return;
                    }
                    
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                    {
                        currentLine.Insert(CaretColumn, e.KeySymbol.ToUpper());
                    }
                    else
                    {
                        currentLine.Insert(CaretColumn, e.KeySymbol);
                    }

                    CaretColumn += 1;
                    break;
            }    
    }
    
    public void SetCaret(int line, int column)
    {
        if (line >= Lines.Count)
        {
            return;
        }
        CaretLine = line;

        if (column > Lines[line].Length)
        {
            column = Lines[line].Length;
        }
        CaretColumn = column;
    }
    
    private void MoveCaretX(int xOffset)
    {
        if (xOffset == 0) return;

        int lineLength = Lines[CaretLine].Length;

        if (CaretColumn + xOffset <= lineLength && CaretColumn + xOffset >= 0)
        {
            CaretColumn += xOffset;
        }
    }
    private void MoveCaretY(int yOffset)
    {
        if (yOffset == 0) return;

        int linesCount = Lines.Count;

        if (CaretLine + yOffset < linesCount && CaretLine + yOffset >= 0)
        {
            if (CaretColumn >= Lines[CaretLine + yOffset].Length)
            {
                CaretColumn = Lines[CaretLine + yOffset].Length;
            }
            CaretLine += yOffset;
        }
    }
    public string Export()
    {
        StringBuilder result = new();
        result.AppendJoin("\r\n", Lines);
        return result.ToString();
    }
    public void Import(Uri file)
    {
        Lines.Clear();
        CaretLine = 0;
        
        IEnumerable<string> lines = File.ReadLines(file.LocalPath);
        foreach (var line in lines)
        {
            Lines.Add(new EditorLine(line));
        }

        if (Lines.Count == 0)
        {
            Lines.Add(new EditorLine(""));
            CaretLine = 0;
            CaretColumn = 0;
            return;
        }
        CaretLine = Lines.Count - 1;
        CaretColumn = Lines[CaretLine].Length;
            
    }
}