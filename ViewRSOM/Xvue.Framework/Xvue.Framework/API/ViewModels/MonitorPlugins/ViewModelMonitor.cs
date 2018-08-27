using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xvue.Framework.API.Services.MonitorPlugins;
using Xvue.Framework.API.ViewModels.Base;

namespace Xvue.Framework.API.ViewModels.MonitorPlugins
{
    public class ViewModelMonitor:ViewModelSerializedPlugin,IMonitorPlugins
    {
        #region localvariables
        
        ObservableCollection<ViewModelMonitoredDevice> _devices = new ObservableCollection<ViewModelMonitoredDevice>();
        public ObservableCollection<ViewModelMonitoredDevice> Devices
        {
            get { return _devices; }            
        }
        bool _notifyCriticalChangePopUp=false;
        #endregion

        #region IMonitor Members
        public int RegisterDevice(string name)
        {
            int deviceId=-1;
            lock (_devices)
            {
                bool found = false;
                foreach (ViewModelMonitoredDevice dev in _devices)
                {
                    if (dev.DeviceName == name)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    ViewModelMonitoredDevice device = new ViewModelMonitoredDevice();
                    device.DeviceName = name;
                    device.DeviceDescription = "";
                    _devices.Add(device);
                    deviceId = _devices.Count - 1;
                }
            }
            OnPropertyChanged("Devices");
            return (deviceId);
        }

        public int UnregisterDevice(string name)
        {
            ViewModelMonitoredDevice found = null;
            lock (_devices)
            {
                foreach (ViewModelMonitoredDevice dev in _devices)
                {
                    if (dev.DeviceName == name)
                    {
                        found = dev;
                        break;
                    }
                }
                if (found != null)
                {
                    _devices.Remove(found);
                }
            }
            OnPropertyChanged("Devices");
            if (found!=null) return 1;
            else return -1;
        }

        public bool Report(string name, DeviceStatus status)
        {
            bool previousState  = IsSystemCritical;
            DeviceStatus reportingDeviceStatus = DeviceStatus.Uninitialized;
            bool found = false;
            lock (_devices)
            {
                foreach (ViewModelMonitoredDevice device in _devices)
                {
                    if (device.DeviceName == name)
                    {
                        device.Status = status;
                        reportingDeviceStatus = device.Status;
                        found = true;
                        break;
                    }
                }
            }
            if ( !previousState && (reportingDeviceStatus == DeviceStatus.Critical) && NotifyCriticalChangePopup)
            {
                //system has changed state from non critical to critical and we have been asked to notify once
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(notify), name);
            }
            OnPropertyChanged("IsSystemReady");
            OnPropertyChanged("IsSystemCritical");
            OnPropertyChanged("IsSystemHealthy");
            return found;
        }

        public bool IsSystemReady
        {

            get 
            {
                bool _isReady = true;
                lock (_devices)
                {
                    foreach (ViewModelMonitoredDevice device in _devices)
                    {
                        if ((device.Status == DeviceStatus.Critical) || (device.Status == DeviceStatus.Initialising) || (device.Status == DeviceStatus.Uninitialized ))
                        {
                            _isReady = false;
                            break;
                        }
                    }
                }
                return _isReady;
            }
        }

        public bool IsSystemCritical
        {
            get
            {
                bool _isCritical = false;
                lock (_devices)
                {
                    foreach (ViewModelMonitoredDevice device in _devices)
                    {
                        if (device.Status == DeviceStatus.Critical)
                        {
                            _isCritical = true;
                            break;
                        }
                    }
                }
                return _isCritical;
            }
        }

        public bool IsSystemHealthy
        {
            get
            {
                bool _isHealthy = true;
                lock (_devices)
                {
                    foreach (ViewModelMonitoredDevice device in _devices)
                    {
                        if ((device.Status == DeviceStatus.Critical) || (device.Status == DeviceStatus.Warning))
                        {
                            _isHealthy = false;
                            break;
                        }
                    }
                }
                return _isHealthy;
            }
        }

        public bool NotifyCriticalChangePopup
        {
            get { return _notifyCriticalChangePopUp; }
            set
            {
                if (value != _notifyCriticalChangePopUp)
                {
                    _notifyCriticalChangePopUp = value;
                    OnPropertyChanged("NotifyCriticalChangePopup");
                }
            }
        }


        #endregion

        void notify(object notification)
        {
            OnStatusCritical(new MonitorNotificationEventArgs((string)notification));                
        }

        public event EventHandler<MonitorNotificationEventArgs> StatusCritical;
        void OnStatusCritical(MonitorNotificationEventArgs e)
        {
            StatusCritical?.Invoke(this, e);            
        }

        //protected override bool Load()
        //{
        //    if (base.Load())
        //    {
        //        Report(deviceId, DeviceStatus.Ready);
        //        return true;
        //    }
        //    return false;
        //}
    }
}
