using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Gml.Launcher.Core.Services;

public class ProcessUtil
{
    public event EventHandler<string>? OutputReceived;

    public event EventHandler? Exited;

    public Process Process { get; private set; }

    public ProcessUtil(Process process) => this.Process = process;

    public void StartWithEvents()
    {
        this.Process.StartInfo.CreateNoWindow = true;
        this.Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        this.Process.StartInfo.UseShellExecute = false;
        this.Process.StartInfo.RedirectStandardError = true;
        this.Process.StartInfo.RedirectStandardOutput = true;
        this.Process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
        this.Process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
        this.Process.EnableRaisingEvents = true;
        this.Process.ErrorDataReceived += (DataReceivedEventHandler) ((s, e) =>
        {
            EventHandler<string> outputReceived = this.OutputReceived;
            if (outputReceived == null)
                return;
            outputReceived((object) this, e.Data ?? "");
        });
        this.Process.OutputDataReceived += (DataReceivedEventHandler) ((s, e) =>
        {
            EventHandler<string> outputReceived = this.OutputReceived;
            if (outputReceived == null)
                return;
            outputReceived((object) this, e.Data ?? "");
        });
        this.Process.Exited += (EventHandler) ((s, e) =>
        {
            EventHandler exited = this.Exited;
            if (exited == null)
                return;
            exited((object) this, new EventArgs());
        });
        this.Process.Start();
        this.Process.BeginErrorReadLine();
        this.Process.BeginOutputReadLine();
    }

    public Task WaitForExitTaskAsync() => Task.Run((Action) (() => this.Process.WaitForExit()));
}
