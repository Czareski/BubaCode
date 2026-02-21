using System.Threading.Tasks;

namespace BubaCode.ViewModels;

public class DialogService
{
    public static DialogService? Instance;
    public DialogService()
    {
        if (Instance != null) return;
        Instance = this;
    }
    public async Task<string?> ShowInputDialogAsync(string title, string prompt, string? defaultText = null)
    {
        var window = Avalonia.Application.Current?.ApplicationLifetime is 
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (window == null)
            return null;

        var vm = new InputDialogViewModel
        {
            Title = title,
            Prompt = prompt,
            InputText = defaultText
        };

        var dialog = new Views.InputDialog
        {
            DataContext = vm
        };

        vm.CloseRequested += () => dialog.Close();

        await dialog.ShowDialog(window);

        return vm.DialogResult ? vm.InputText : null;
    }
    public async Task<bool> ShowAreYouSureDialogAsync(string title, string prompt)
    {
        var window = Avalonia.Application.Current?.ApplicationLifetime is 
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (window == null)
            return false;

        var vm = new AreYouSureDialogViewModel
        {
            Title = title,
            Prompt = prompt
        };

        var dialog = new Views.AreYouSureDialog()
        {
            DataContext = vm
        };

        vm.CloseRequested += () => dialog.Close();

        await dialog.ShowDialog(window);

        return vm.DialogResult;
    }
}