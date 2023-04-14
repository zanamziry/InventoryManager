using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using InventoryManager.Contracts.Services;

namespace InventoryManager.Services;

public class SystemService : ISystemService
{
    public void OpenInWebBrowser(string url)
    {
        // For more info see https://github.com/dotnet/corefx/issues/10361
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(psi);
    }
}
