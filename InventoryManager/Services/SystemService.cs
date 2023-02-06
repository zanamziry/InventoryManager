using System.Diagnostics;

using InventoryManager.Contracts.Services;

namespace InventoryManager.Services;

public class SystemService : ISystemService
{
    public SystemService()
    {
    }

    readonly string migrateCMD = "./dxn_api/manage.py migrate";
    readonly string makemigrationsCMD = "./dxn_api/manage.py makemigrations";
    readonly string runserverCMD = "./dxn_api/manage.py runserver 127.0.0.1:80";
    Process _process;
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

    public void StartServer()
    {
        if (_process != null)
            return;
        makemigrations();
        migrate();
        _process = new Process();
        _process.StartInfo = new ProcessStartInfo(@"python.exe", runserverCMD)
        {
            RedirectStandardOutput = false,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        _process.Start();
    }

    public void migrate()
    {
        using (Process p = new Process())
        {

            p.StartInfo = new ProcessStartInfo(@"python.exe", migrateCMD)
            {
                RedirectStandardOutput = false,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
        p.Start();
        p.WaitForExit();
        }
    }
    public void makemigrations()
    {
        Process p = new Process();
        p.StartInfo = new ProcessStartInfo(@"python.exe", makemigrationsCMD)
        {
            RedirectStandardOutput = false,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        p.Start();
        p.WaitForExit();
        p.Kill();
    }
    public void StopServer()
    {
        if (_process == null)
            return;
        _process.Kill();
    }
}
