using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO.Ports;
using Xvue.Framework.API.DataModels.Base;
namespace Xvue.Framework.API.DataModels.Plugins.SerialDevices
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DataModelControllerSerialPort : DataModelPluginConfiguration  
    {
        string _portName = "";
        Parity _parity = Parity.None;
        int _baudRate = 9600;
        int _dataBits = 8;
        int _readBufferSize = 4096;
        int _writeBufferSize = 4096;
        int _readTimeout = -1;
        Handshake _handshake = Handshake.None;
        StopBits _stopBits = StopBits.One;
        int _receivedBytesThreshold = 1;
        bool _dtrEnable = false;
        bool _rtsEnable = false;
        bool _discardNull = false;
        string _newLine = "";
       
        //Encoding _encoding = Encoding.Default;

        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }
        public Parity Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        public int BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        public int DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

        public int ReadBufferSize
        {
            get { return _readBufferSize; }
            set { _readBufferSize = value; }
        }

        public int WriteBufferSize
        {
            get { return _writeBufferSize; }
            set { _writeBufferSize = value; }
        }

        public int ReadTimeout
        {
            get { return _readTimeout; }
            set { _readTimeout = value; }
        }

        public Handshake Handshake
        {
            get { return _handshake; }
            set { _handshake = value; }
        }

        public StopBits StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }

        public int ReceivedBytesThreshold
        {
            get { return _receivedBytesThreshold; }
            set { _receivedBytesThreshold = value; }
        }

        public bool DtrEnable
        {
            get { return _dtrEnable; }
            set { _dtrEnable = value; }
        }

        public bool RtsEnable
        {
            get { return _rtsEnable; }
            set { _rtsEnable = value; }
        }

        public bool DiscardNull
        {
            get { return _discardNull; }
            set { _discardNull = value; }
        }

        //public Encoding Encoding
        //{
        //    get { return _encoding; }
        //    set { _encoding = value; }
        //}

        public string NewLine
        {
            get { return _newLine; }
            set { _newLine = value; }
        }
    }
}
