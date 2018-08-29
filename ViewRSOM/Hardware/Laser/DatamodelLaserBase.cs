using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewRSOM.Framework.API.DataModels.Base;
using System.ComponentModel;

namespace ViewRSOM.MSOT.DataModels.Plugins.Laser
{
    public abstract class DataModelLaserBase : DataModelPluginConfiguration
    {
        protected DataModelLaserBase()
        {
            LowPower = 50;
            Power = 100;
            Wavelength = 532.0;
            WarmUpInSecs = 5;
        }

        /// <summary>
        /// Gets or sets the Laser SN.
        /// </summary>
        /// <value>The Laser SN.</value>
        /// <remarks></remarks>
        [Description("Laser Serial Number")]
        public String LaserSN { get; set; }

        /// <summary>
        /// The minimum warm up time for the Laser Lamp
        /// </summary>
        [Description("The minimum warm up time for the Laser Lamp")]
        public int WarmUpInSecs { get; set; }

        /// <summary>
        /// The starting Wavelength in nm
        /// </summary>
        [Description("The starting Wavelength in nm")]
        public double Wavelength { get; set; }

        /// <summary>
        /// Laser Nominal power as Percentage. Value range 0..100
        /// </summary>
        [Description("Laser Nominal power as Percentage. Value range 0..100"), Category("Power")]
        public Int16 Power { get; set; }

        /// <summary>
        /// Laser Low power mode setting as Percentage. Value range 0..100
        /// </summary>
        [Description("Laser Low power mode setting as Percentage. Value range 0..100"), Category("Power")]
        public Int16 LowPower { get; set; }
    }
}
