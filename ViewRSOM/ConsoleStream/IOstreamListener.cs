using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ViewRSOM.ConsoleStream
{
    public class IOstreamListener : TextWriter
    {

        public override void Write(string value)
        {
            IOEventHandler.triggerIOEvent("Console", "EventHandler", value);
        }

        public override void WriteLine(string value)
        {
            IOEventHandler.triggerIOEvent("Console", "EventHandler", value);
        }
        
        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

    }
}
