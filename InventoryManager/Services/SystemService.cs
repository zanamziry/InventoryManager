using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using InventoryManager.Contracts.Services;

namespace InventoryManager.Services;

public class SystemService : ISystemService
{
    public void OpenInWebBrowser(string url)
    {
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(psi);
    }
}
