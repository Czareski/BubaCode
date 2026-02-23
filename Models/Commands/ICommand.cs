using System.Collections.Generic;
using System.Text;
using BubaCode.ViewModels;

namespace BubaCode.Models;

public interface ICommand
{
    public ActionResult Execute(CodeBoxViewModel sender);

    public void Undo(CodeBoxViewModel sender);

    public void Redo(CodeBoxViewModel sender);
}
public enum ActionResult
{
    AddToStack,
    DontAddToStack,
}