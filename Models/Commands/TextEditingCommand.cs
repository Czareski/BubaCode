using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public class TextEditingCommand
{
    private CaretPosition? _caretBefore;
    private CaretPosition _caretAfter;
    private TextSnapshot? _textSnapshotBefore;
    private TextSnapshot? _textSnapshotAfter;

    protected void OnExecute(CodeBoxViewModel vm)
    {
        if (_caretBefore == null)
        {
            _caretBefore = new CaretPosition(vm.Caret.Line, vm.Caret.Column);
            _textSnapshotBefore = vm.Text.TakeSnapshot();
        }
    }

    protected void OnExecuteForce(CodeBoxViewModel vm)
    {
        _caretBefore = new CaretPosition(vm.Caret.Line, vm.Caret.Column);
        _textSnapshotBefore = vm.Text.TakeSnapshot();
    }

    protected void OnUndo(CodeBoxViewModel vm)
    {
        if (_textSnapshotAfter == null)
        {
            _textSnapshotAfter = vm.Text.TakeSnapshot();
        }
        vm.Text.RestoreSnapshot(_textSnapshotBefore!);
        vm.Caret.SetPosition(_caretBefore!.Value);
    }

    protected void OnRedo(CodeBoxViewModel vm)
    {
        vm.Text.RestoreSnapshot(_textSnapshotAfter!);
        vm.Caret.SetPosition(_caretAfter);
    }

    protected void MakeCaretAfterSnapshot(CodeBoxViewModel vm)
    {
        _caretAfter = new CaretPosition(vm.Caret.Line, vm.Caret.Column);
    }
}