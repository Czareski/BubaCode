using BubaCode.ViewModels;

namespace BubaCode.Models.Commands;

public interface ITypeCommand : ICommand
{
    public string GetTypedValue();
    public bool CanBeConcated();
    public void Concat(CodeBoxViewModel vm, ITypeCommand other);
}