using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;

public class PrivilegeBlockerService : ServiceBase
{
    [DllImport("advapi32.dll", SetLastError = true)]
    static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetCurrentProcess();

    const uint TOKEN_QUERY = 0x0008;

    protected override void OnStart(string[] args)
    {
        Thread monitorThread = new Thread(MonitorProcesses);
        monitorThread.Start();
    }

    protected override void OnStop()
    {
        EventLog.WriteEntry("PrivilegeBlockerService stopped.");
    }

    private void MonitorProcesses()
    {
        while (true)
        {
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    IntPtr tokenHandle;
                    if (OpenProcessToken(process.Handle, TOKEN_QUERY, out tokenHandle))
                    {
                        WindowsIdentity identity = new WindowsIdentity(tokenHandle);
                        if (identity.IsSystem)
                        {
                            EventLog.WriteEntry(string.Format("Privilege Escalation Blocked: {0}", process.ProcessName), EventLogEntryType.Warning);
                            process.Kill(); // Immediately terminate SYSTEM privilege process
                        }
                    }
                }
                catch { }
            }
            Thread.Sleep(3000);
        }
    }

    static void Main()
    {
        ServiceBase.Run(new PrivilegeBlockerService());
    }
}
