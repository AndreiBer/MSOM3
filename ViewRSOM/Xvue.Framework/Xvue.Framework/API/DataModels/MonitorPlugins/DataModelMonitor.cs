using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO.Ports;
using Xvue.Framework.API.DataModels.Base;


namespace Xvue.Framework.API.DataModels.MonitorPlugins
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DataModelMonitor : DataModelPluginConfiguration
    {
        public override string ToString()
        {
            return "Xvue Framework Monitor Configuration";
        }

    }
}
