using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Avalonia.Input;
using BubaCode.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.ViewModels;

public partial class CodeBoxViewModel : ViewModelBase
{
    private FilesService _fileService;
    private ShortcutRegistry _registry;
    [ObservableProperty]
    public EditorText _text;
    public Selection? Selection;
    
    [ObservableProperty]
    private Caret _caret;

    public CodeBoxViewModel(FilesService fileService)
    {
        _fileService = fileService;
        _fileService.FileImported += Import;
        _fileService.GetSourceToExport = GetText;
        _registry = new ShortcutRegistry();
        Caret = new Caret(this);
        _text = new EditorText(ref _caret);
    }

    public void OnKeyDown(KeyEventArgs e)
    {
    // var currentLine = Lines[CaretLine];
    _registry.Execute(new KeyCombination(e), this);
        switch (e.Key)
            {
                case Key.Up:
                    Caret.Line -= 1;
                    break;
                case Key.Down:
                    Caret.Line += 1;
                    break;
                case Key.Left:
                    Caret.Column -= 1;
                    break;
                case Key.Right:
                    Caret.Column += 1;
                    break;
                case Key.Enter:
                    Text.HandleEnter();
                    break;
                case Key.Back:
                    Text.HandleBackspace();
                    break;
                case Key.Tab:
                    Text.HandleTab();
                    e.Handled = true;
                    break;
                default:
                    if (e.KeySymbol == null)
                    {
                        return;
                    }

                    Text.HandleTextKey(e);
                    break;
            }    
    }

    public void ResetSelection()
    {
        if (Selection == null) return;
        Selection.StartPosition = new Point(0, 0);
        Selection.EndPosition = new Point(0, 0);
        Selection = null;
    }
    public string GetText()
    {
        return Text.ToString();
    }
    public void Import(Uri file)
    {
        Text.Clear();
        Caret.Line = 0;
        
        IEnumerable<string> lines = File.ReadLines(file.LocalPath);
        foreach (string line in lines)
        {
            Text.InsertLine(line);
        }

        if (Text.LinesCount == 0)
        {
            Text.InitializeEmpty();
            return;
        }
        Caret.Line = Text.LinesCount - 1;
        Caret.Column = Text.GetLineLength(Caret.Line);

    }
}