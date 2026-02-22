using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Avalonia.Input;
using BubaCode.Models;
using BubaCode.Models.Commands;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BubaCode.ViewModels;

public partial class CodeBoxViewModel : ViewModelBase
{
    [ObservableProperty]
    private ITextStorage _text;
    [ObservableProperty]
    private Caret _caret;
    [ObservableProperty]
    private string _extension = "";
    public Selection? Selection;
    private Actions _actions;
    private FilesService _fileService;
    private ShortcutRegistry _registry;
    private bool _imported;
    public CodeBoxViewModel(FilesService fileService)
    {
        _fileService = fileService;
        _fileService.FileImported += Import;
        _fileService.GetSourceToExport = GetText;
        _registry = new ShortcutRegistry();
        Caret = new Caret(this);
        _text = new PieceTableTextAdapter(this, new PieceTableText(""));
        _actions =  new Actions(this);
    }

    public void OnKeyDown(KeyEventArgs e)
    {
        // var currentLine = Lines[CaretLine];
        bool exectuedShortcut = _registry.Execute(new KeyCombination(e), this);
        if (exectuedShortcut)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Up:
                Caret.Line -= 1;
                break;
            case Key.Down:
                Caret.Line += 1;
                break;
            case Key.Left:
                if (Selection != null)
                {
                    Caret.Line = Selection.StartPosition.X;
                    Caret.Column = Selection.StartPosition.Y;
                    break;
                }
                Caret.Column -= 1;
                break;
            case Key.Right:
                if (Selection != null)
                {
                    Caret.Line = Selection.EndPosition.X;
                    Caret.Column = Selection.EndPosition.Y;
                    break;
                }
                Caret.Column += 1;
                break;
            case Key.Enter:
                _actions.Do(new EnterCommand());
                break;
            case Key.Back:
                if (Selection != null)
                {
                    _actions.Do(new RemoveFromSelectionCommand());
                    Selection = null;
                    return;
                }
                _actions.Do(new RemoveCharacterCommand());
                
                break;
            case Key.Tab:
                Text.HandleTab();
                e.Handled = true;
                break;
            case Key.LeftCtrl:
                return;
            default:
                _actions.Do(new TypeCharacterCommand(e));
                break;
        }
        _fileService.SetFileDirty?.Invoke(true);
        Selection = null;
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
        if (_imported)
        {
            return;
        }
        Caret.Line = 0;
        
        var text = File.ReadAllText(file.LocalPath);
        text = text.Replace("\r", "");
        
        Text = new PieceTableTextAdapter(this, new PieceTableText(text));
        Extension = Path.GetExtension(file.LocalPath);
        
        Caret.Line = Text.LinesCount - 1;
        Caret.Column = Text.GetLineLength(Caret.Line);
        _imported = true;
    }
    public Actions GetActions()
    {
        return _actions;
    }

    public void UnsubscribeToFileImported()
    {
        _fileService.FileImported -= Import;
    }
}