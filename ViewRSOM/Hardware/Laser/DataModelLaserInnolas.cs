using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ViewRSOM.MSOT.DataModels.Plugins.Laser
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DataModelLaserInnolas : DataModelLaserBase    
    {
        public enum InnolasPowerControlMethod
        {
            None,
            Amplifier,
            PockelscellDelay,
            Attenuator,
            SweepTableAttenuator
        }

        public enum InnolasChannelSwitch:int
        {
            None =0,
            Channel1 = 1,
            Channel2 = 2 
        }

        public override string ToString()
        {
            return "Innolas Laser";
        }
        public DataModelLaserInnolas():base()
        {
            WarmUpInSecs = 2;
            // ProxyIp = 0; leave as 0 so as to gradually remove from settings
            ProxyIpAsString = "127.0.0.1";
            Port = 8025;
            PowerControlMethod = InnolasPowerControlMethod.None;
            AmplifierMin = 42;
            AmplifierMax = 300;
            Amplifier = AmplifierMin;
            Attenuator = 50.0;            
            PockelscellDelayMin = 170;
            PockelscellDelayMax=350;
            PockelscellDelay = PockelscellDelayMin;
        }

        [Description("IP port to connect"), Category("Proxy")]
        public int Port { get; set; }
        /// <summary>
        /// Gets or sets the proxy ip as long.
        /// </summary>
        /// <value>The proxy ip as long.</value>
        [Obsolete("Use ProxyIpAsString instead")]
        [Description("IP address of Proxy. Obsolete: use ProxyIpAsString instead"), Category("Proxy")]
        public long ProxyIp { get; set; }

        /// <summary>
        /// Gets or sets the proxy ip as string in form XX.XX.XX.XX.
        /// </summary>
        /// <value>The proxy ip as string.</value>
        [Description("IP address of Proxy in form XX.XX.XX.XX"), Category("Proxy")]
        public string ProxyIpAsString { get; set; }

        [Description("Power Control Method"), Category("Power")]        
        public InnolasPowerControlMethod PowerControlMethod { get; set; }

        [Description("Amplifier default setting"), Category("Power")]
        public Int16 Amplifier { get; set; }

        [Description("Amplifier minimum setting"), Category("Power")]
        public Int16 AmplifierMin { get; set; }

        [Description("Amplifier maximum setting"), Category("Power")]
        public Int16 AmplifierMax { get; set; }

        [Description("Pockelscell delay default setting"), Category("Power")]
        public Int16 PockelscellDelay { get; set; }

        [Description("Pockelscell delay minimum setting"), Category("Power")]
        public Int16 PockelscellDelayMin { get; set; }

        [Description("Pockelscell delay maximum setting"), Category("Power")]
        public Int16 PockelscellDelayMax { get; set; }

        [Description("Attenuator default setting. Valid range is 0-100"), Category("Power")]
        public double Attenuator { get; set; }
    }

    [Serializable]
    public class DataModelLaserVersionInfo
    {
        public string GUIVersion { get; set; }
        public string ControllerFWVersion { get; set; }
        public string FPGAVersion { get; set; }
        public string OPOVersion { get; set; }
        public string EMONversion { get; set; }

        public override string ToString()
        {
            return
            "GUI Version:" + GUIVersion + ";" +
            "Controller Version:" + ControllerFWVersion + ";" +
            "FPGA Version:" + FPGAVersion + ";" +
            "OPO Version:" + OPOVersion + ";" +
            "Energy Monitor Version:" + EMONversion;            
        }
    }
}
