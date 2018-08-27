
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xvue.Framework.API.ViewModels.Performance
{
    public class ViewModelPerformanceCounterRecord:Xvue.Framework.API.ViewModels.Base.ViewModelBase
    {

        /// <summary>
        /// returns value as msecs
        /// </summary>
        /// <returns></returns>
        public double FrameTime { get; set; }
        public double CounterValue{get; set; }



    }
}
