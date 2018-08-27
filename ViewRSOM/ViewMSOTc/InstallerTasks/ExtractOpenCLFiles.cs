using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Security.Principal;

namespace ViewMSOTc.InstallerTasks
{
    [RunInstaller(true)]
    public partial class ExtractOpenCLFiles : System.Configuration.Install.Installer
    {
        public ExtractOpenCLFiles()
        {
            InitializeComponent();
        }

        string targetDir
        {
            get
            {
                string appSubfolderPath = this.Context.Parameters["assemblypath"];
                return System.IO.Path.GetDirectoryName(appSubfolderPath);
                //Environment.GetEnvironmentVariable("ProgramW6432") + appSubfolderPath;                
            }
        }


        public override void Install(System.Collections.IDictionary stateSaver)
        {
            try
            {
                base.Install(stateSaver);                

                if (IsUserAdministrator())
                {
                    ExtractFiles();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format("Error performing installer MCR task: {0}", ex.Message));
            }
        }

        public static bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
            }
            catch (Exception)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        void ExtractFiles()//(object sender, InstallEventArgs e)
        {
            try
            {
                System.IO.Directory.SetCurrentDirectory(targetDir);
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(System.IO.Path.Combine(targetDir, "itheractclexe.exe"), "-bb");
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(startInfo);
                while (!p.HasExited)
                {
                    System.Threading.Thread.Sleep(10);
                }
                if (p.ExitCode != 0)
                {
                    System.Windows.MessageBox.Show(string.Format("Error 0 installing CL files"));
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format("Error 1 installing CL files: {0}", ex.Message));
            }
        }
    }
}
