using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViewRSOM.Hardware.LaserSW
{
    public class ViewModelBrightSolutions : ViewModelLaserBase
    {
        #region localvariables
        private SerialPort mySerialPort;
        private string header, p1, p2, p3, checkSum;
        private string sendCommand, expectedAnswer, answer;
        private bool waitingForAnswer;
        private Stopwatch stopwatch = new Stopwatch();
        int timeOut = 1000, waitingTime = 0; // timeout in milliseconds
        #endregion localvariables

        public ViewModelBrightSolutions()
        {

        }

        #region publicvariables

        // define laser EventHandler
        public RoutedEventHandler laserHandle;

        #endregion publicvariables

        #region publicMethods

        // setup: initialization and switch on laser
        public void setup()
        {
            // init
            initialize(acquisitionParameters.laserPort);

            // set PRR, power, and trigger
            Power = acquisitionParameters.laserPower;
            PulseRepetitionRate = acquisitionParameters.PRR;
            if (acquisitionParameters.triggerMode == 0)
                ExternalTrigger = false;
            else
                ExternalTrigger = true;
            setPower();
            setPRR();
            setTrigger();

            // switch on
            EmissionON();
            EmissionON();
        }

        // initialization
        public override void initialize(string laserPort)
        {
            
            // set baud rate to 9600
            mySerialPort = new SerialPort(laserPort);
            mySerialPort.BaudRate = 9600;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;
            mySerialPort.RtsEnable = true;

            // Set the read/WriteLine timeouts
            mySerialPort.ReadTimeout = 500;
            mySerialPort.WriteTimeout = 500;

            // open port
            try
            {
                mySerialPort.Open();
            }
            catch
            {
                Console.WriteLine("ERROR: No connection on port " + laserPort + ".\n");
                throw new Exception("ERROR: No connection on port " + laserPort + ".");
            }

            // setup read port
            mySerialPort.DataReceived += serialPort_DataReceived;

            // check link
            checkLink();
        }

        // close connection
        public override bool closeConnection()
        {
            mySerialPort.Close();
            return true;
        }


        // link check
        public bool checkLink()
        {
            // set power
            try
            {
                // define header, parameters p1-p3, and checkSum
                header = "80";
                p1 = System.DateTime.Now.Hour.ToString("00");
                p2 = System.DateTime.Now.Minute.ToString("00");
                p3 = System.DateTime.Now.Second.ToString("00");
                checkSum = calc_checkSum(header, p1, p2, p3);

                // check correctness
                if (p1.Length != 2 || p2.Length != 2 || p3.Length != 2 || checkSum.Length != 2)
                {
                    Console.WriteLine("ERROR: laser error - link check failed. p1-3 or checkSum length is not 2.\n");
                    throw new Exception("ERROR: laser error - link check failed. p1-3 or checkSum length is not 2.");
                }

                // define command to be send and received
                sendCommand = header + p1 + p2 + p3 + checkSum + "\r";
                expectedAnswer = "81" + p1 + p2 + p3;

                // send command to COM port
                sendCommandToPort(sendCommand, expectedAnswer);
                return true;
            }
            catch
            {
                Console.WriteLine("ERROR: laser error - link check failed.\n");
                throw new Exception("ERROR: laser error - link check failed.");
            }
        }


        // trigger
        public bool setTrigger()
        {
            // set rigger to internal or external
            try
            {
                // define header, parameters p1-p3, and checkSum
                header = "82";
                if (_externalTrigger)
                {
                    p1 = "01";
                }
                else
                {
                    p1 = "00";
                }
                p2 = "00";
                p3 = "00";
                checkSum = calc_checkSum(header, p1, p2, p3);

                // check correctness
                if (checkSum.Length != 2)
                {
                    Console.WriteLine("ERROR: laser error - setTrigger failed. checkSum length is not 2.\n");
                    throw new Exception("ERROR: laser error - setTrigger failed. checkSum length is not 2.");
                }

                // define command to be send and received
                sendCommand = header + p1 + p2 + p3 + checkSum + "\r";
                expectedAnswer = "83" + p1 + p2 + p3;

                // send command to COM port
                sendCommandToPort(sendCommand, expectedAnswer);
                return true;
            }
            catch
            {
                Console.WriteLine("ERROR: laser error - setTrigger failed.\n");
                throw new Exception("ERROR: laser error - setTrigger failed.");
            }
        }

        public override bool EmissionON()
        {
            // set rigger to internal or external
            try
            {
                // define header, parameters p1-p3, and checkSum
                header = "84";
                p1 = "01";
                p2 = "00";
                p3 = "00";
                checkSum = calc_checkSum(header, p1, p2, p3);

                // check correctness
                if (checkSum.Length != 2)
                {
                    Console.WriteLine("ERROR: laser error - EmissionON failed. checkSum length is not 2.\n");
                    throw new Exception("ERROR: laser error - EmissionON failed. checkSum length is not 2.");
                }

                // define command to be send and received
                sendCommand = header + p1 + p2 + p3 + checkSum + "\r";
                expectedAnswer = "85" + p1 + p2 + p3;

                // send command to COM port
                sendCommandToPort(sendCommand, expectedAnswer);
                return true;
            }
            catch
            {
                Console.WriteLine("ERROR: laser error - EmissionON failed.\n");
                throw new Exception("ERROR: laser error - EmissionON failed.");
            }
        }

        public override bool EmissionOFF()
        {
            // set rigger to internal or external
            try
            {
                // define header, parameters p1-p3, and checkSum
                header = "84";
                p1 = "00";
                p2 = "00";
                p3 = "00";
                checkSum = calc_checkSum(header, p1, p2, p3);

                // check correctness
                if (checkSum.Length != 2)
                {
                    Console.WriteLine("ERROR: laser error - EmissionOFF failed. checkSum length is not 2.\n");
                    throw new Exception("ERROR: laser error - EmissionOFF failed. checkSum length is not 2.");
                }

                // define command to be send and received
                sendCommand = header + p1 + p2 + p3 + checkSum + "\r";
                expectedAnswer = "85" + p1 + p2 + p3;

                // send command to COM port
                sendCommandToPort(sendCommand, expectedAnswer);
                return true;
            }
            catch
            {
                Console.WriteLine("ERROR: laser error - EmissionOFF failed.\n");
                throw new Exception("ERROR: laser error - EmissionOFF failed.");
            }
        }


        // set laser power
        public override bool setPower()
        {
            // set power
            try
            {
                // define header, parameters p1-p3, and checkSum
                header = "88";
                p1 = "04";
                p2 = "00";
                p3 = string.Format("{0:X2}", _power);
                checkSum = calc_checkSum(header, p1, p2, p3);

                // check correctness
                if (p3.Length!=2 || checkSum.Length!=2)
                {
                    Console.WriteLine("ERROR: laser error - setPower failed. p3 or checkSum length is not 2.\n");
                    throw new Exception("ERROR: laser error - setPower failed. p3 or checkSum length is not 2.");
                }

                // define command to be send and received
                sendCommand = header + p1 + p2 + p3 + checkSum + "\r";
                expectedAnswer = "89" + p1 + p2 + p3;

                // send command to COM port
                sendCommandToPort(sendCommand, expectedAnswer);
                return true;
            }
            catch
            {
                Console.WriteLine("ERROR: laser error - setPower failed.\n");
                throw new Exception("ERROR: laser error - setPower failed.");
            }
        }


        // set PRR
        public override bool setPRR()
        {
            // set PRR
            try
            {
                // define header, parameters p1-p3, and checkSum
                header = "A6";
                string p13 = string.Format("{0:X6}", _pulseRepetitionRate);
                p1 = p13.Substring(0, 2);
                p2 = p13.Substring(2, 2);
                p3 = p13.Substring(4, 2);
                checkSum = calc_checkSum(header, p1, p2, p3);

                // check correctness
                if (p1.Length != 2 || p2.Length != 2 || p3.Length != 2 || checkSum.Length != 2)
                {
                    Console.WriteLine("ERROR: laser error - setPRR failed. p1-3 or checkSum length is not 2.\n");
                    throw new Exception("ERROR: laser error - setPRR failed. p1-3 or checkSum length is not 2.");
                }

                // define command to be send and received
                sendCommand = header + p1 + p2 + p3 + checkSum + "\r";
                expectedAnswer = "A7" + p1 + p2 + p3;

                // send command to COM port
                sendCommandToPort(sendCommand, expectedAnswer);

                // read pulse train freq
                sendCommand = "9A0000009A" + "\r";
                expectedAnswer = "9B" + p1 + p2 + p3;

                // send command to COM port
                sendCommandToPort(sendCommand, expectedAnswer);

                return true;
            }
            catch
            {
                Console.WriteLine("ERROR: setPRR failed.\n");
                throw new Exception("ERROR: setPRR failed.");
            }
        }


        // reset laser
        public bool resetLaser()
        {
            // set PRR
            try
            {
                // define header, parameters p1-p3, and checkSum
                header = "A0";
                p1 = "01";
                p2 = "00";
                p3 = "00";
                checkSum = calc_checkSum(header, p1, p2, p3);

                // check correctness
                if (checkSum.Length != 2)
                {
                    Console.WriteLine("ERROR: laser error - resetLaser failed. checkSum length is not 2.\n");
                    throw new Exception("ERROR: laser error - resetLaser failed. checkSum length is not 2.");
                }

                // define command to be send and received
                sendCommand = header + p1 + p2 + p3 + checkSum + "\r";
                expectedAnswer = "A1" + p1 + p2 + p3;

                // send command to COM port
                sendCommandToPort(sendCommand, expectedAnswer);

                return true;
            }
            catch
            {
                Console.WriteLine("ERROR: setPRR failed.\n");
                throw new Exception("ERROR: setPRR failed.");
            }
        }


        #endregion publicMethods

        #region privateMethods

        // calculate checkSum
        string calc_checkSum(string header, string p1, string p2, string p3)
        {
            Byte _CheckSumByte = 0x00;
            _CheckSumByte ^= Convert.ToByte(header, 16);
            _CheckSumByte ^= Convert.ToByte(p1, 16);
            _CheckSumByte ^= Convert.ToByte(p2, 16);
            _CheckSumByte ^= Convert.ToByte(p3, 16);

            string checkSum = string.Format("{0:X2}", _CheckSumByte);

            return checkSum;
        }

        // send command to port
        private void sendCommandToPort(string sendCommand, string answer)
        {
            // start time counter
            stopwatch.Start();
            waitingForAnswer = true;

            // sned message to port and wait for answer during timOut
            mySerialPort.Write(sendCommand);
            while (stopwatch.ElapsedMilliseconds<timeOut && waitingForAnswer)
            {
                // wait
            }

            // check answer
            if (waitingForAnswer == false)
            {
                if (answer.StartsWith(expectedAnswer) == false)
                {
                    Console.WriteLine("WARNING: received answer " + answer + "deviates from expected answer" + expectedAnswer + ".");
                }
                if (answer.StartsWith("E1"))
                {
                    check_ErrorMessage(answer);
                }
            }
            else
            {
                Console.WriteLine("ERROR: timeout occured while sending " + sendCommand + "to the BrightSolutions laser.\n");
                throw new Exception("ERROR: timeout occured while sending " + sendCommand + "to the BrightSolutions laser.");
            }

        }

        // answers received from COM-port
        void serialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
        {
            // measure time delay between command send and answer received
            stopwatch.Stop();
            waitingForAnswer = false;
            Console.WriteLine("Answer received after " + stopwatch.ElapsedMilliseconds + "milliseconds.");

            // Store received answer
            try
            {
                answer = mySerialPort.ReadExisting();
            }
            catch (Exception ex)
            {
                 Console.WriteLine("WARNING: Cannot read answer from laser port - " + ex.Message );
            }
            //System.Windows.Application.Current.Dispatcher.Invoke(
            //    System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            //    {
            //        answer = mySerialPort.ReadExisting();
            //    });

        }

        void check_ErrorMessage(string error)
        {
            Console.WriteLine("ERROR: BrightSolutions error - " + error + ".\n");
            throw new Exception("ERROR: BrightSolutions error - " + error);
        }

        #endregion privateMethods


    }
}
