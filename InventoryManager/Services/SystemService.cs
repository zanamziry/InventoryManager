using System.Diagnostics;

using InventoryManager.Contracts.Services;

namespace InventoryManager.Services;

public class SystemService : ISystemService
{
    public SystemService()
    {
    }

    readonly string cmdArgs = "./dxn_api/manage.py 127.0.0.1 80";
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
        _process = new Process();
        _process.StartInfo = new ProcessStartInfo(@"C:\Python27\python.exe", cmdArgs)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        _process.Start();
        
        string output = _process.StandardOutput.ReadToEnd();
        p.WaitForExit();
        Console.WriteLine(output);
        Console.ReadLine();
    }

    public void StopServer()
    {
        if (_process == null)
            return;
        _process.Kill();
    }
}
