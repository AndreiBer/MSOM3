using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Xvue.Framework.API.Services.Log
{

    [Guid("6D569B1D-F9F4-4a15-A3FA-F3E0D966CADE")]
    public interface ICoreServiceLog 
    {
        void AddEventLog(int logType, string reason, string eventMessage);
    }
}
