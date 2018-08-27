using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Laser.Parameter
{
    public static class LaserParameter
    {

        public static string laserSerial = "not defined";
        public static string proxyIP = "127.0.0.1";
        //public static int proxyPort = 3023;
        public static int proxyPort = 1025;
        public static int pulsesPerWl = 10;

        public static int WarmUpInSecs = 2;
        public static string PowerControlMethod = "None";  //"GlobalAttenuator" or "SweepTableAttenuator" or "PockelsCellAttenuator" or "AmplifierAttenuator" or "None",
            
        public static int AmplifierMin = 42;
        public static int AmplifierMax = 300;
        public static int Amplifier = AmplifierMin;
        public static double Attenuator = 0.0;            
        public static int PockelscellDelayMin = 170;
        public static int PockelscellDelayMax=350;
        public static int PockelscellDelay = PockelscellDelayMin;
        public static int ActualPulseDivider = 1;
        public static double GlobalAttenuator = 0;


        public static double LaserMinAllowedWavelength = 420;
        public static double LaserMaxAllowedWavelength = 680;
        public static double LaserDefaultWavelength = 532;
        public static double SystemMinAllowedWavelength = 420;
        public static double SystemMaxAllowedWavelength = 680;
        public static string CurrentState = "";
        public static double minWaveLength = 420;
        public static double maxWaveLength = 680;
        public static double LaserTriggerRate = 0;
        

        public static string Controller_SW_Version = "not defined";
        public static string FPGA_SW_Version = "not defined";
        public static string HW_Version = "not defined";
        public static string GUI_Version = "not defined";
        public static string Proxy_Version = "not defined";
        public static string Opo_Version = "not defined";
        public static string Energy_Monitor_Version = "not defined";
        public static List<AttenuationValues> myAttenuationValues = new List<AttenuationValues>();  

        public static double GetAttenuation(double wavelength, int pulseDivider)
        {
            try
            {
                return myAttenuationValues.Find(data => (data.wavelength == wavelength && data.pulseDivider==pulseDivider)).attenuation;                    
            }
            catch
            {
                return 0;
            }
            
        }

        public static double SetAttenuation(double wavelength, int pulseDivider, double newValue)
        {
            try
            {
                myAttenuationValues.Find(data => (data.wavelength == wavelength && data.pulseDivider == pulseDivider)).attenuation=newValue;
                return myAttenuationValues.Find(data => (data.wavelength == wavelength && data.pulseDivider == pulseDivider)).attenuation;
            }
            catch
            {
                return 0;
            }
        }
    }

    public class AttenuationValues
    {
        public int pulseDivider;
        public double wavelength;
        public double attenuation;  // in the range from 0.000 to 1.000 - (*)
    }

}
/* (*) The attenuation value is abstract value between 1.0 and 100.0,
 * as the Laser Communication (Rayfos Code) multiplies the value times 100.0 before sending the command to the Laser, 
 * the values habve to b indexer the range 0.000 - 1.000
*/