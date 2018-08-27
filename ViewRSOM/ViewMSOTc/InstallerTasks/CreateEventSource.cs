using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;

namespace ViewRSOM.InstallerTasks
{
    [RunInstaller(true)]
    public partial class CreateEventSource : System.Configuration.Install.Installer
    {
        string sSource = "RSOM";
        string sLog = "Application";
        public CreateEventSource()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            try
            {
                base.Install(stateSaver);
                if (!EventLog.SourceExists(sSource))
                {
                    EventLog.CreateEventSource(sSource, sLog);
                    EventLog.WriteEntry(sSource, "Event source created");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(String.Format("Error performing installer tasks: {0}", ex.Message));
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            int uninstallStep = 0;
            try
            {
                base.Uninstall(savedState);
                uninstallStep++;
                if (EventLog.SourceExists(sSource))
                {
                    uninstallStep++;
                    EventLog.DeleteEventSource(sSource);
                }
                uninstallStep++;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error performing uninstaller task: " + ex.Message + ", at step " + uninstallStep);
            }
        }  
    }
}
