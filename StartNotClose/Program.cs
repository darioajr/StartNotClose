using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StartNotClose
{
    static class Program
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        static extern bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        const uint SC_CLOSE = 0xF060;
        const uint SC_MINIMIZE = 0xF020;
        const uint MF_BYCOMMAND = 0x00000000;

        [STAThread]
        static void Main(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                MessageBox.Show("Informe os parametros para o programa StartNotClose.exe\n\nEx: StartNotClose.exe c:\\...javaws.exe c:\\...arquivo.jnlp");
                return;
            }

            var javaws = args[0];
            var jnlp = args[1];
            String command = @" -silent -wait " + jnlp;
            ProcessStartInfo cmdsi = new ProcessStartInfo(javaws);
            cmdsi.Arguments = command;
            var parentProc = Process.Start(cmdsi);

            int timeout = 60;

            while (timeout > 0)
            {
                timeout--;

                //wait process
                Process[] processes = Process.GetProcessesByName("jp2launcher");
                if (processes != null)
                {
                    foreach (Process p in processes)
                    {
                        IntPtr pFoundWindow = p.MainWindowHandle;
                        IntPtr nSysMenu = GetSystemMenu(pFoundWindow, false);
                        if (nSysMenu != IntPtr.Zero)
                        {
                            //remove minimize and close button
                            if (DeleteMenu(nSysMenu, SC_MINIMIZE, MF_BYCOMMAND))
                                if (DeleteMenu(nSysMenu, SC_CLOSE, MF_BYCOMMAND))
                                    return;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }

            //execute shutdown when process don´t exists
            Process.Start("shutdown.exe /l");
            return;
        }
    }
}
