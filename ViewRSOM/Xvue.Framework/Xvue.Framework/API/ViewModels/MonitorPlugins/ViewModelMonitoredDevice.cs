using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xvue.Framework.API.Services.MonitorPlugins;
using Xvue.Framework.API.ViewModels.Base;
namespace Xvue.Framework.API.ViewModels.MonitorPlugins
{
    public class ViewModelMonitoredDevice:ViewModelBase
    {
        #region localvariables
        
        DeviceStatus _status;
        //int _deviceId;
        string _deviceName;        
        string _deviceDescription;        
        #endregion

        public ViewModelMonitoredDevice()
        {
            _status = DeviceStatus.Uninitialized;
        }

        public DeviceStatus Status
        {
            get { return _status; }
            set
            { 
                _status = value;
                OnPropertyChanged("Status");
            }
        }
        
        
        public string DeviceName
        {
            get { return _deviceName; }
            set
            {
                _deviceName = value;
                OnPropertyChanged("DeviceName");
            }
        }

        public string DeviceDescription
        {
            get { return _deviceDescription; }
            set 
            {
                _deviceDescription = value;
                OnPropertyChanged("DeviceDescription");
            }
        }
    }
}
