using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class TextEditingCommand
{
    private CaretPosition? _caretBefore;
    private CaretPosition _caretAfter;
    
    protected void OnExecute(CodeBoxViewModel vm)
    {
        if (_caretBefore != null)
        {
            vm.Caret.SetPosition((CaretPosition)_caretBefore!);
        }
        else
        {
            _caretBefore = new CaretPosition(vm.Caret.Line, vm.Caret.Column);
        }
    }

    protected void OnUndo(CodeBoxViewModel vm)
    {
        vm.Caret.SetPosition(_caretAfter);
    }
    protected void MakeCaretAfterSnapshot(CodeBoxViewModel vm)
    {
        _caretAfter = new CaretPosition(vm.Caret.Line, vm.Caret.Column);
    }
}