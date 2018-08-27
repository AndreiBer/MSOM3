using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
//using Xvue.MSOT.DataModels.Plugins.HardwareSystem;
using General.Tools.Communication;
using System.Threading;

namespace ViewRSOM.MSOT.Hardware.ViewModels.Laser.Innolas
{
 
    internal enum StandardCommandType
    {
        StartupLaser,
        GetGuiVersion,
        GetControllerVersion,
        GetFPGAVersion,
        GetHWVersion,
        GetEMONVersion,
        GetOPOVersion,
        GetMaxWavelength,
        GetMinWavelength,
        GetChannelSwitch,
        SetChannelSwitch,
        SetUserModeAdmin,
        StartCharger,
        StopCharger,
        IsChecksumValid,
        FlashLampOn,
        FlashLampOff,
        SimmerOn,
        SimmerOff,
        GetSimmertState,
        CheckPowerCorrectionFactorStatus,
        GetPockelscellTriggerState,
        SetPockelscellTriggerState,        
        GetPulseDivider,
        SetPulseDivider,
        
        ShutterState,
        GetShutterState,
        SetShutterState,
        GetPockelScellDelay1,
        SetPockelScellDelay1,
        GetAmplifierDelay,
        SetAmplifierDelay,
        SetAttenuation,
        ShutdownLaser,
        ShutdownWorkAround,
        ResetSweepTable,
        SetNumberOfPulsesPerWavelength,
        SetNumberOfSweeps,
        SetNumberOfSweepTableLines,
        AddSweepTableLine,
        GetSweepState,
        StartSweep,
        StopSweep,
        GetWarningList,
        GetErrorList,
        GetLaserState,
        SetWavelength
    }

    internal enum SimplifiedCommandType
    {
        GetGuiVersion,
        GetControllerVersion,
        GetFPGAVersion,
        GetHWVersion,
        GetEMONVersion,
        GetOPOVersion,
        GetMaxWavelength,
        GetMinWavelength,
        GetLaserState,
        GetChannelSwitch,
        SetChannelSwitch,
        SetUserModeAdmin,
        IsChecksumValid,
        PumpSourceOn,
        PumpSourceOff,
        CheckPowerCorrectionFactorStatus,
        GetPulseDivider,
        SetPulseDivider,
        StartCharger,
        StopCharger,
        GetBeamBrakerState,
        GetPockelScellDelay1,
        SetPockelScellDelay1,
        SetAttenuation,
        ResetSweepTable,
        SetNumberOfPulsesPerWavelength,
        SetNumberOfSweeps,
        SetNumberOfSweepTableLines,
        AddSweepTableLine,
        GetSweepState,
        StartSweep,
        StopSweep,
        GetWarningList,
        GetErrorList,
        SetWavelength
    }

    internal enum IncomingMessageType
    {
        Normal = 0,
        OpoData,
        FlashLampShotCounter,
        PockelScellCounter,
        WaterFlow,
        WaterTemperature,
        Warning,
        InterlockOpen,
        InterlockClose,
        Error
    }

    internal struct ProtocolCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolCommand"/> struct.
        /// </summary>
        /// <param name="timeoutInMilliseconds">The timeout in milliseconds.</param>
        /// <param name="command">The command.</param>
        /// <param name="expectedAnswers">The expected answers.</param>
        internal ProtocolCommand(int timeoutInMilliseconds, string command, string[] expectedAnswers)
        {
            TimeoutInMilliseconds = timeoutInMilliseconds;
            Command = command;
            ExpectedAnswers = expectedAnswers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolCommand"/> struct.
        /// </summary>
        /// <param name="command">The command.</param>

        internal ProtocolCommand(string command) :
            this(0, command, new string[0]) { }

        /// <summary>
        /// The timeout in milliseconds
        /// </summary>
        internal readonly int TimeoutInMilliseconds;

        /// <summary>
        /// The command
        /// </summary>
        internal readonly string Command;

        /// <summary>
        /// The expected answers
        /// </summary>
        internal readonly string[] ExpectedAnswers;
    }

    internal class ProtocolWrapper<T> : IDisposable
    {
        System.Net.Sockets.TcpClient _innolasProxy;
        NetworkStream _innolasStream;
        IPAddress _proxyIp;        
        Queue<string> _incomingMessages;
        Dictionary<T,ProtocolCommand> _registeredCommands;
        Dictionary<string,IncomingMessageType> _errorTable;                
        long _msgCounter;
        bool _exchangeCommandActive;
        readonly int _maxCommandLength;
        /// <summary>
        /// Struct InnolasCommand. Stores the parameters used in communication between the host and the Laser
        /// </summary>

        public ProtocolWrapper(Dictionary<T, ProtocolCommand> commandDictionary, Dictionary<string, IncomingMessageType> errorTable)
        {
            _maxCommandLength = 500;
            _incomingMessages = new Queue<string>();
            _errorTable = new Dictionary<string, IncomingMessageType>(errorTable);
            _registeredCommands = new Dictionary<T, ProtocolCommand>(commandDictionary);
        }


        public async void Connect_(IPAddress proxyIP, int port)
        {
            int errorCode = 0;
            _msgCounter = 0;
           // message = "";
            _exchangeCommandActive = false;
            GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Step 1");
            try
            {
                _proxyIp = proxyIP;
                errorCode = 0;
                _innolasProxy = new System.Net.Sockets.TcpClient();
                await _innolasProxy.ConnectAsync(_proxyIp, Convert.ToInt32(port));
                if (_innolasProxy.Connected)
                {
                    _innolasStream = _innolasProxy.GetStream();
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receiveThread));
                }
                else
                {
                    errorCode = -1;
           //         message = "Unable to connect";
                }
            }
            catch 
            {
                errorCode = -2;
           //     message = "Exception : " + ex.Message;
            }
          //  return errorCode;
        }


        public int Connect(IPAddress proxyIP, int port)
        {
            int errorCode = 0;
            _msgCounter = 0;
            //message = "";
            _exchangeCommandActive = false;
            GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Connecting - Step 1");
            try
            {
                
                
                //IPAddress _proxyIP = IPAddress.Parse(proxyIP);
                GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Connecting - Step 2");
                errorCode = 0;
                _innolasProxy = new System.Net.Sockets.TcpClient();
                GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Connecting - Step 3");
                _innolasProxy.Connect(proxyIP, Convert.ToInt32(port));
                GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Connecting - Step 4");
                if (_innolasProxy.Connected)
                {
                    _innolasStream = _innolasProxy.GetStream();
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receiveThread));
                    GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Could Connect");
                }
                else
                {
                    errorCode = -1;
                    GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Unable to Connect");
                    //message = "Unable to connect";
                }
            }
            catch (Exception ex)
            {
                errorCode = -2;
                GUI_Communicator.sendStatus("Laser", "HardwareMonitor", "Connecting Error:"+ex.Message);
                //message = "Exception : " + ;
            }
            return errorCode;
        }

        public int Close(out string message)
        {
            int errorCode = 0;
            message = "";
            try
            {
                if (_innolasProxy.Connected)
                {
                    _innolasStream.Close();
                    _innolasProxy.Close();
                }
                else
                {
                    message = "Proxy not connected";
                    errorCode = -1;
                }
            }
            catch (Exception ex)
            {
                message = "Exception: " + ex.Message;
                errorCode = -2;
            }
            return errorCode;
        }

        void receiveThread(object input)
        {
            byte[] inputArray = new byte[50];                        
            List<String> newMessages;
            IncomingMessageType errorLevel;
            StringBuilder myCompleteMessage = new StringBuilder(_maxCommandLength, _maxCommandLength);
            while (_innolasProxy.Connected)
            {
                try
                {
                    int readCount = -1;
                    Array.Clear(inputArray, 0, inputArray.Length);
                    if (!_innolasStream.CanRead)
                        break;
                    if ((readCount = _innolasStream.Read(inputArray, 0, inputArray.Length)) > 0)
                    {
                        
                        newMessages = parseSubMessage(inputArray, readCount,ref myCompleteMessage);
                        foreach (string msgItem in newMessages)//we have some complete commands                        
                        {
                            //if (msgItem.Contains("\r") || msgItem.Contains("\n"))                            
                            //    System.Diagnostics.Trace.WriteLine("Error deb1: " + msgItem);
                            
                            errorLevel = IncomingMessageType.Normal;
                            _msgCounter++;
                            string dbgMsg = "[" + _msgCounter.ToString() + "] " + msgItem;
                            //System.Diagnostics.Debug.WriteLine("@:" + DateTime.Now.ToLongTimeString() + " Parsed command: " + dbgMsg);
                            //-------------------------------------------
                            // check here for any general response actions
                            //------------------------------------------
                            lock (_incomingMessages)
                            {
                                if (_exchangeCommandActive)
                                    _incomingMessages.Enqueue(msgItem);
                            }
                            //trygetvalue returns 0 if not found which means no error...
                            if (!_errorTable.TryGetValue(msgItem, out errorLevel))
                            {
                                
                                //no errors, check if LaserState
                                string LaserStateReady = "LASER_STATE=READY";
                                if (msgItem.StartsWith(LaserStateReady))
                                {
                                    GUI_Communicator.sendStatus("LASER_STATE", "GUI", "LaserReady");
                                }
                                
                                string LaserStateOff = "LASER_STATE=OFF";
                                if (msgItem.StartsWith(LaserStateOff))
                                {
                                    GUI_Communicator.sendStatus("LASER_STATE", "GUI", "LaserOff");
                                }

                                string  LaserCharged= "LASER_STATE=CBANK.Wait";
                                if (msgItem.StartsWith(LaserCharged))
                                {
                                    GUI_Communicator.sendStatus("LASER_STATE", "GUI", "LaserCharged");
                                }

                                string LaserNotCharged = "LASER_STATE=CBANK.Off";
                                if (msgItem.StartsWith(LaserNotCharged))
                                {
                                    GUI_Communicator.sendStatus("LASER_STATE", "GUI", "LaserConnectedNotCharged");
                                }

                                string LaserStateWarmUp = "LASER_STATE=WARMUP";
                                if (msgItem.StartsWith(LaserStateWarmUp))
                                {
                                    GUI_Communicator.sendStatus("LASER_STATE", "GUI", "LaserWarmingUp");
                                }

                                string LaserStateSwitchOff = "LASER_STATE=SWITCH_OFF";
                                if (msgItem.StartsWith(LaserStateSwitchOff))
                                {
                                    GUI_Communicator.sendStatus("LASER_STATE", "GUI", "LaserSwitchingOff");
                                }

                                //no errors,check if it is A LaserParameter
                                string FPGA = "FPGA_SW_VERSION=";
                                if (msgItem.StartsWith(FPGA))
                                {
                                    string msgBody = msgItem.Replace(FPGA, "");
                                    errorLevel = IncomingMessageType.OpoData;
                                    dbgMsg = msgBody;
                                    GUI_Communicator.sendStatus("FPGA_SW_VERSION", "VERSION_NUMBERS", dbgMsg);
                                }

                                string Controller = "CONTROLLER_SW_VERSION=";
                                if (msgItem.StartsWith(Controller))
                                {
                                    string msgBody = msgItem.Replace(Controller, "");
                                    errorLevel = IncomingMessageType.OpoData;
                                    dbgMsg = msgBody;
                                    GUI_Communicator.sendStatus("CONTROLLER_SW_VERSION", "VERSION_NUMBERS", dbgMsg);
                                }
                                
                                string HW = "HW_VERSION=";
                                if (msgItem.StartsWith(HW))
                                {
                                    string msgBody = msgItem.Replace(HW, "");
                                    errorLevel = IncomingMessageType.OpoData;
                                    dbgMsg = msgBody;
                                    GUI_Communicator.sendStatus("HW_VERSION", "VERSION_NUMBERS", dbgMsg);
                                }

                                string GUI = "GUI_VERSION=";
                                if (msgItem.StartsWith(HW))
                                {
                                    string msgBody = msgItem.Replace(GUI, "");
                                    errorLevel = IncomingMessageType.OpoData;
                                    dbgMsg = msgBody;
                                    GUI_Communicator.sendStatus("GUI", "VERSION_NUMBERS", dbgMsg);
                                }

                                string Proxy = "PROXY_VERSION=";
                                if (msgItem.StartsWith(HW))
                                {
                                    string msgBody = msgItem.Replace(Proxy, "");
                                    errorLevel = IncomingMessageType.OpoData;
                                    dbgMsg = msgBody;
                                    GUI_Communicator.sendStatus("PROXY_VERSION", "VERSION_NUMBERS", dbgMsg);
                                }

                                string Opo = "OPO_VERSION=";
                                if (msgItem.StartsWith(HW))
                                {
                                    string msgBody = msgItem.Replace(Opo, "");
                                    errorLevel = IncomingMessageType.OpoData;
                                    dbgMsg = msgBody;
                                    GUI_Communicator.sendStatus("OPO_VERSION", "VERSION_NUMBERS", dbgMsg);
                                }

                                string EnergyMon = "ENERGY_MONITOR_VERSION=";
                                if (msgItem.StartsWith(EnergyMon))
                                {
                                    string msgBody = msgItem.Replace(EnergyMon, "");
                                    errorLevel = IncomingMessageType.OpoData;
                                    dbgMsg = msgBody;
                                    GUI_Communicator.sendStatus("ENERGY_MONITOR_VERSION", "VERSION_NUMBERS", dbgMsg);
                                }
                                
                                //no errors, check if it is SweepState
                                string sweepState = "SWEEP_STATE=";
                                if (msgItem.StartsWith(sweepState))
                                {
                                    string opoBody = msgItem.Replace(sweepState, "");
                                    errorLevel = IncomingMessageType.OpoData;
                                    dbgMsg = opoBody;
                                    GUI_Communicator.sendStatus("SWEEP_STATE", "GUI", dbgMsg);
                                }
                                
                                //no errors, check if it is OPO3
                                string opoCommand= "OPO3_MEASURE=";
                                if (msgItem.StartsWith(opoCommand))
                                {
                                    string opoBody = msgItem.Replace(opoCommand, "");
                                    errorLevel = IncomingMessageType.OpoData;
                                    dbgMsg = opoBody;
                                    GUI_Communicator.sendStatus("OPO", "OpoDataHandler", dbgMsg);
                                }
                                else
                                {
                                    opoCommand= "SHOT_COUNTER=";
                                    if (msgItem.StartsWith(opoCommand))
                                    {
                                        string opoBody = msgItem.Replace(opoCommand, "");
                                        errorLevel = IncomingMessageType.FlashLampShotCounter;
                                        dbgMsg = opoBody;
                                    }
                                    else
                                    {
                                        opoCommand = "SHOT_COUNTER_2=";
                                        if (msgItem.StartsWith(opoCommand))
                                        {
                                            string opoBody = msgItem.Replace(opoCommand, "");
                                            errorLevel = IncomingMessageType.PockelScellCounter;
                                            dbgMsg = opoBody;
                                        }
                                        else
                                        {                                            
                                            opoCommand = "WATER_FLOW=";
                                            if (msgItem.StartsWith(opoCommand))
                                            {
                                                string opoBody = msgItem.Replace(opoCommand, "");
                                                errorLevel = IncomingMessageType.WaterFlow;
                                                dbgMsg = opoBody;
                                            }
                                            else
                                            {
                                                opoCommand = "TEMPERATURE=";
                                                if (msgItem.StartsWith(opoCommand))
                                                {
                                                    string opoBody = msgItem.Replace(opoCommand, "");
                                                    errorLevel = IncomingMessageType.WaterTemperature;
                                                    dbgMsg = opoBody;
                                                }
                                                else
                                                {
                                                    opoCommand = "WARNING=";
                                                    if (msgItem.StartsWith(opoCommand))
                                                    {
                                                        string opoBody = msgItem.Replace(opoCommand, "");
                                                        errorLevel = IncomingMessageType.Warning;
                                                        dbgMsg = "Unknown warning reported by Innolas:" + opoBody;
                                                    }
                                                    else
                                                    {
                                                        opoCommand = "ERROR=";
                                                        if (msgItem.StartsWith(opoCommand))
                                                        {
                                                            string opoBody = msgItem.Replace(opoCommand, "");
                                                            errorLevel = IncomingMessageType.Error;
                                                            dbgMsg = "Unknown error reported by Innolas:" + opoBody;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            //if (dbgMsg.Contains("\r") || dbgMsg.Contains("\n"))
                            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("o") + "-Received: "+ msgItem);
                            GUI_Communicator.sendStatus("Laser", "StatusDisplay", "EMPFANGE: "+dbgMsg+ " ("+errorLevel+")");
                            OnNewInnolasMessageEvent(new InnolasMessageEventArgs(dbgMsg,errorLevel));                            
                        }
                    }
                }
                catch 
                {
 /*
                    if (ViewModelLaserBase.laserTrace.TraceWarning)
                        Xvue.MSOT.ViewModels.Log.ViewModelLog.MsotTrace("Innolas receive thread: " + ex.Message, ViewModelLaserBase.laserTrace);
   */
                }
            }
        }

        public List<String> parseSubMessage(byte[] inputArray, int readCount,ref StringBuilder myCompleteMessage)
        {
            List<String> newMessages = new List<string>();
            char[] newCommand = new char[_maxCommandLength];
            char[] incomingChars = ASCIIEncoding.ASCII.GetChars(inputArray);
            myCompleteMessage.Append(incomingChars, 0, readCount);
            bool found;
            int offset=0;
            while (offset < myCompleteMessage.Length)
            {
                switch (myCompleteMessage[offset])
                {
                    case '\r':
                    case '\n':
                        found = true;
                        myCompleteMessage.Remove(offset,1);//remove the control character
                        break;                    
                    default:
                        found = false;
                        break;
                }
                if (found)
                {
                    if (offset > 0)//check if we have some content
                    {
                        myCompleteMessage.CopyTo(0, newCommand, 0, offset);
                        myCompleteMessage.Remove(0, offset);
                        String logItem = new String(newCommand, 0, offset);
                        newMessages.Add(logItem);
                    }
                    offset = 0; //restart offset count since we just removed the last command                    
                    found = false;
                }
                else offset++;
                
            }
            return newMessages;
        }

        /// <summary>
        /// Exchanges the command without waiting for any answers
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="payLoad">The pay load.</param>
        /// <param name="message">The message.</param>
        /// <returns>0 if no errors, or less than 0 if errors occur</returns>
        /// <remarks></remarks>
        internal int ExchangeCommand(T commandType, string payLoad, out string message)
        {
            List<string> receivedCommands;
            return ExchangeCommand(commandType, payLoad, out receivedCommands, out message);
        }

        /// <summary>
        /// Exchanges the command and returns the received answers
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="payLoad">The pay load.</param>
        /// <param name="receivedCommands">The received commands.</param>
        /// <param name="message">The message.</param>
        /// <returns>0 if no errors, or less than 0 if errors occur</returns>
        /// <remarks></remarks>
        internal int ExchangeCommand(T commandType, string payLoad, out List<string> receivedCommands, out string message)
        {
            receivedCommands = new List<string>();
            message = "";
            string commandText = "";
            int errorCode = 0;
            try
            {
                int busyTimeout = 0;
                if (_exchangeCommandActive)
                {
                }
                while (_exchangeCommandActive)
                {
                    Thread.Sleep(100);
                    if (busyTimeout > (10 * 5)) //*100 = 5 secs grace
                    {
                        System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("o") + "-Exchange command busy-timeout");
                        break;
                    }
                    busyTimeout++;
                }
                if (busyTimeout > 0)
                    System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("o") + " -Exchange command was busy for: " + busyTimeout * 100 + "ms");

                List<string> expectedCommands = new List<string>();
                int timeoutInMilliseconds;
                if (_registeredCommands.ContainsKey(commandType))
                {
                    ProtocolCommand commandToExecute = _registeredCommands[commandType];
                    commandText = commandToExecute.Command;
                    if (commandToExecute.ExpectedAnswers != null)
                        expectedCommands = new List<string>(commandToExecute.ExpectedAnswers);
                    timeoutInMilliseconds = commandToExecute.TimeoutInMilliseconds;
                }
                else
                {
                    errorCode = -4;
                    message = "Unrecognized command: [" + commandType + "]";
                    return errorCode;
                }
                lock (_incomingMessages)//notify we are expecting messages
                {
                    _exchangeCommandActive = true;
                }
                errorCode = sendCommand(commandText, payLoad, out message);
                if (errorCode == 0)
                {
                    if (expectedCommands.Count == 0)
                        return errorCode;
                    else
                    {
                        int tOutCounter = 0;
                        int answerCount = 0;
                        do
                        {
                            int currentQueueSize = 0;
                            string newMessage = "";
                            lock (_incomingMessages)
                            {
                                currentQueueSize = _incomingMessages.Count;
                                if (currentQueueSize > 0)
                                {
                                    newMessage = _incomingMessages.Dequeue();
                                }
                            }
                            if (newMessage.Length > 0)
                            {
                                if (newMessage.StartsWith(expectedCommands[answerCount], StringComparison.InvariantCultureIgnoreCase))
                                {
                                    int headerLength = expectedCommands[answerCount].Length;
                                    string answer = newMessage.Substring(headerLength);
                                    answerCount++;
                                    receivedCommands.Add(answer);
                                }
                            }
                            if (currentQueueSize < 2)
                            {
                                System.Threading.Thread.Sleep(10);
                                if (tOutCounter++ > timeoutInMilliseconds / 10)
                                {
                                    errorCode = -3; // timeout
                                    message = " Timeout expecting answer to command: " + commandType;
                                    break;
                                }
                            }
                        } while (answerCount < expectedCommands.Count);
                    }
                    return errorCode;
                }
                else
                    return errorCode;
                //this.NewIncomingMessageCompleted += new EventHandler<InolasIncomingMessageEventArgs>(processIncomingMessage);            
            }
            catch (Exception ex)
            {
                message = "Exception exchanging command: " + ex.Message;
                errorCode = -2;
            }
            finally
            {
                lock (_incomingMessages)
                {
                    _exchangeCommandActive = false;
                    _incomingMessages.Clear();
                }
            }
            return errorCode;
        }

        int sendCommand(string commandText, string payload, out string message)
        {
            int errorCode = 0;
            message = "";
            try
            {
                if (_innolasProxy.Connected)
                {                   
                    string commandBody = commandText + payload;
                    byte[] commandBytes = ASCIIEncoding.ASCII.GetBytes(commandBody + "\r\n");
                    OnNewInnolasMessageEvent(new InnolasMessageEventArgs("=>New command: " + commandBody,0));
                    _innolasStream.Write(commandBytes, 0, commandBytes.Length);
                    System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("o") + "-Sent: " + commandBody);
                }
                else
                {
                    message = "Proxy not connected";
                    errorCode = -1;
                }
            }
            catch (Exception ex)
            {
                message = "Exception sending command: " + ex.Message;
                errorCode = -2;
            }
            return errorCode;
        }

        public event EventHandler<InnolasMessageEventArgs> NewInnolasMessageReceived;
        void OnNewInnolasMessageEvent(InnolasMessageEventArgs e)
        {
            EventHandler<InnolasMessageEventArgs> handler = this.NewInnolasMessageReceived;
            if (handler != null)
                handler(this, e);
        }

        public void Dispose()
        {
            if( _innolasProxy != null)
            {                
                _innolasProxy.Close();
            }
        }
    }

    internal class InnolasMessageEventArgs : EventArgs
    {
        internal InnolasMessageEventArgs(string _data, IncomingMessageType _errorLevel)
        {
            this.data = _data;
            this.errorLevel = _errorLevel;
        }
        internal string data;
        internal IncomingMessageType errorLevel;
    }

}
