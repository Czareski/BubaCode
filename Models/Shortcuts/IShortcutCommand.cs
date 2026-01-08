using System.Collections.Generic;
using System.Text;
using BubaCode.ViewModels;

namespace BubaCode.Models;

public interface IShortcutCommand
{
    public void Execute(CodeBoxViewModel sender);
}