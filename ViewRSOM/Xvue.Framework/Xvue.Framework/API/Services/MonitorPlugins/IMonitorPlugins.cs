using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xvue.Framework.API.Services.MonitorPlugins
{
    public enum DeviceStatus { Initialising, Ready, Critical, Warning, Uninitialized  }
    public interface IMonitorPlugins
    {
        int RegisterDevice(string name);
        int UnregisterDevice(string name);
        bool Report(string name, DeviceStatus status);
        bool IsSystemReady { get; }
        bool IsSystemCritical { get; }
        bool IsSystemHealthy { get; }
        bool NotifyCriticalChangePopup { get; set; }
        event EventHandler<MonitorNotificationEventArgs> StatusCritical;
    }

    public class MonitorNotificationEventArgs : EventArgs
    {
        public MonitorNotificationEventArgs(string message)
        {
            this.Message = message;
        }

        public string Message;

    }

}
