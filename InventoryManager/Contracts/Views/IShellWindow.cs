using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;

namespace InventoryManager.Contracts.Views;

public interface IShellWindow
{
    Frame GetNavigationFrame();

    void ShowWindow();

    void CloseWindow();
    Task<MessageDialogResult> ShowMessage(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null);
    Task<ProgressDialogController> ShowProgress(string title, string message, bool isCancelable = false, MetroDialogSettings settings = null);
}
