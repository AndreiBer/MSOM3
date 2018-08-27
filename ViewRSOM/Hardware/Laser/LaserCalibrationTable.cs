using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ViewRSOM.MSOT.Hardware.ViewModels.Laser
{
    /// <summary>        
    /// Binary File Structure
    /// [Header]
    /// File Version: UInt16
    /// Descriptive Header text Count byte count: UInt16
    /// Descriptive Header text : byte, byte, ... byte (Descriptive Header text Count) - UTF-8 encoding
    /// Safety number 0xAA55 (UInt16)
    /// Laser Undivided Trigger Rate(Hz): UInt16
    /// Number of Pulse Divider Values  : UInt16
    /// Number of Coefficients : UInt16
    /// Pulse Divider Values : UInt16, UInt16, ..., UInt16 (PulseDividerValues times)
    /// 
    /// [Table]
    /// Row count : UInt16
    /// Each row starts with a wavelength number : UInt16 (nm)
    /// 
    /// Then each row has as many as repetitions count says of the following repeatable [structs]:
    /// E                   : Double
    /// A                   : Double
    /// Coefficients Values : Double, Double, ... Double (Coefficients Count times)
    /// </summary>
    public class LaserCalibrationTable
    {
        struct LaserEnergyCorrectionStruct
        {
            internal LaserEnergyCorrectionStruct( double averagePower, double attenuation, double[] polynomialCoefficients)
            {
                AveragePower = averagePower;
                Attenuation = attenuation;
                PolynomialCoefficients = polynomialCoefficients;
            }
            internal readonly double AveragePower;
            internal readonly double Attenuation;
            internal readonly double[] PolynomialCoefficients;
        }        

        #region localvariables
        bool _noData;

        LaserEnergyCorrectionStruct[,] _laserWavelengthVsRepetitions;
        UInt16 _baseWavelength;
        UInt16 _basePulseDivider;

        List<int> _wavelengthsLookup;
        List<int> _pulseDividerLookup;
        List<UInt16> _wavelengthValues;
        List<UInt16> _pulseDividerValues;

        const UInt16 FileVersion = 0x0000;
        const UInt16 SafetyNumber = 0xAA55;
        #endregion localvariables

        public LaserCalibrationTable()
        {
            init();
        }

        void init()
        {
            NoOfPulseDividerValues = 0;
            NoOfCoefficients = 0;
            Description = "";
            LaserTrigerRateMin = 0; 
            LaserTrigerRateMax = 0;
            LaserUndividedTriggerRate = 0;
            NoOfWavelengths = 0;
            _laserWavelengthVsRepetitions = new LaserEnergyCorrectionStruct[0, 0];
            _wavelengthsLookup = new List<int>();
            _pulseDividerLookup = new List<int>();
            _wavelengthValues = new List<UInt16>();
            _pulseDividerValues = new List<UInt16>();
            _noData = true;
        }

        public bool LoadFile(string filePath, out string errorMessage)
        {       
            errorMessage="";
            try
            {                
                if (File.Exists(filePath))
                {
                    _noData = !readDouble2DArrayWithHeader(filePath, out errorMessage);
                }
                else
                {
                    _noData = true;
                    errorMessage = "File: " + filePath + " not found";
                }
            
            }
            catch (Exception ex)
            {
                errorMessage = "Exception loading file "+filePath+":" + ex.Message;
                _noData = true;
            }
            return !_noData;
        }

        public IEnumerable<double> GetTriggerRates(out string laserRatesSummary)
        {
            List<double> triggerRates = new List<double>();
            laserRatesSummary = "";
            foreach (UInt16 divider in PulseDividerValues)
            {
                double triggerRate = Math.Round((double)LaserUndividedTriggerRate / divider, 2);
                triggerRates.Add(triggerRate);
                laserRatesSummary += triggerRate.ToString("F1") + "Hz ";
            }
            return triggerRates;
        }

        public string Description  { get; private set; }

        public UInt16 NoOfCoefficients { get; private set; }

        public UInt16 NoOfWavelengths { get; private set; }        

        public UInt16 NoOfPulseDividerValues{ get; private set; }        

        public UInt16 LaserTrigerRateMin { get; private set; }

        public UInt16 LaserTrigerRateMax { get; private set; }

        public UInt16 LaserUndividedTriggerRate { get; private set; }

        public List<UInt16> PulseDividerValues  {  get { return _pulseDividerValues; } }

        bool readDouble2DArrayWithHeader(
            string binaryFilePath,            
            out string errorMessage)            
        {
            errorMessage = "";
            bool continueF = true;
            _laserWavelengthVsRepetitions = new LaserEnergyCorrectionStruct[0, 0];
            NoOfPulseDividerValues = 0;
            NoOfCoefficients = 0;
            _wavelengthsLookup = new List<int>();
            _pulseDividerLookup = new List<int>();
            //int gap;
            try
            {
                using (FileStream fs = new System.IO.FileStream(binaryFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    using (System.IO.BinaryReader r = new System.IO.BinaryReader(fs))
                    {
                        UInt16 version = r.ReadUInt16();
                        if (version != FileVersion)
                        {
                            errorMessage = "Unknown file version: " + version;
                            continueF = false;
                        }
                        else
                        {
                            //Read Header
                            UInt16 headerTxtCount = r.ReadUInt16();
                            UTF8Encoding utf8 = new UTF8Encoding();
                            byte[] t = r.ReadBytes(headerTxtCount);
                            Description = utf8.GetString(t);
                            UInt16 safetyNumber = r.ReadUInt16();
                            if (SafetyNumber != safetyNumber)
                            {
                                errorMessage = "Header description is corrupt";
                                continueF = false;
                            }
                            else
                            {
                                LaserUndividedTriggerRate = r.ReadUInt16();
                                NoOfPulseDividerValues = r.ReadUInt16();
                                NoOfCoefficients = r.ReadUInt16();

                                if (NoOfPulseDividerValues < 1)
                                {
                                    continueF = false;
                                    errorMessage = "Pulse divider number is 0";
                                }
                                else
                                {
                                    //read base
                                    _basePulseDivider = r.ReadUInt16();
                                    UInt16 lastPulseDividerValue = _basePulseDivider;
                                    addLookupElement(ref _pulseDividerLookup, ref _pulseDividerValues, _basePulseDivider, ref lastPulseDividerValue, 0);
                                    for (int i = 1; i < NoOfPulseDividerValues; i++)
                                    {
                                        if (!addLookupElement(ref _pulseDividerLookup, ref _pulseDividerValues, r.ReadUInt16(), ref lastPulseDividerValue, i))
                                        {
                                            errorMessage = "Pulse divider values are not sorted";
                                            continueF = false;
                                            break;
                                        }
                                    }
                                    LaserTrigerRateMin = _pulseDividerValues[0];
                                    LaserTrigerRateMax = _pulseDividerValues[_pulseDividerValues.Count - 1];
                                }
                                if (continueF)
                                {
                                    //Prepare & Read wavelengthsLookup and data Table
                                    NoOfWavelengths = r.ReadUInt16();
                                    if (NoOfWavelengths < 1)
                                    {
                                        continueF = false;
                                        errorMessage = "Wavelength number is 0";
                                    }
                                    else
                                    {
                                        //_wavelengthsLookup = new int[noOfRows];
                                        _laserWavelengthVsRepetitions = new LaserEnergyCorrectionStruct[NoOfWavelengths, NoOfPulseDividerValues];
                                        _baseWavelength = r.ReadUInt16();
                                        UInt16 lastWavelengthValue = _baseWavelength;
                                        addLookupElement(ref _wavelengthsLookup, ref _wavelengthValues, _baseWavelength, ref lastWavelengthValue, 0);
                                        readLaserEnergyCorrectionStruct(r, 0);
                                        for (UInt16 rowIndex = 1; rowIndex < NoOfWavelengths; rowIndex++)
                                        {
                                            if (!addLookupElement(ref _wavelengthsLookup, ref _wavelengthValues, r.ReadUInt16(), ref lastWavelengthValue, rowIndex))
                                            {
                                                errorMessage = "Wavelength values are not sorted";
                                                break;
                                            }
                                            readLaserEnergyCorrectionStruct(r, rowIndex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Exception reading double 2D array with header: " + ex.Message;
                continueF = false;
            }
            if(!continueF)
            {
                init();
            }
            return continueF;
        }
        void readLaserEnergyCorrectionStruct(System.IO.BinaryReader r, UInt16 rowIndex)
        {            
            for (int repetitionIndex = 0; repetitionIndex < NoOfPulseDividerValues; repetitionIndex++)
            {
                double[] polynomialCoefficients = new double[NoOfCoefficients];                
                double averagePower = r.ReadDouble();
                double attenuation = r.ReadDouble();
                for (int coefficientIndex = 0; coefficientIndex < NoOfCoefficients; coefficientIndex++)
                    polynomialCoefficients[coefficientIndex] = r.ReadDouble();
                _laserWavelengthVsRepetitions[rowIndex, repetitionIndex] = new LaserEnergyCorrectionStruct(averagePower, attenuation, polynomialCoefficients);
            }
        }
        bool addLookupElement(ref List<int> lookup, ref List<UInt16> values, UInt16 newElement, ref UInt16 lastElement,int index)
        {
            int gap = newElement - lastElement;
            if (gap >= 0)
            {
                for (int k = 1; k < gap; k++)
                    lookup.Add(-1);
                lookup.Add(index);
                values.Add(newElement);
                lastElement = newElement;
                return true;
            }
            else return false;
        }

        public double GetAveragePower(int wavelength,int pulseDivider)
        {
            if (_noData)
                return 0;
            else
            {
                LaserEnergyCorrectionStruct obj = getElement(wavelength, pulseDivider);

                return obj.AveragePower;
            }
        }

        public double GetPowerEstimation(int wavelength, int pulseDivider, double x)
        {
            if (_noData || NoOfCoefficients<1 )
                return 0;
            else
            {
                LaserEnergyCorrectionStruct obj = getElement(wavelength, pulseDivider);
                int nCoefficients = NoOfCoefficients;
                double[] xN = new double[nCoefficients];
                xN[0] = 1;
                for( int i = 1; i< nCoefficients; i++)
                {
                    xN[i] = xN[i - 1] * x;
                }
                double mJ = 0;
                for (int i = 0; i < nCoefficients; i++)
                    mJ += (obj.PolynomialCoefficients[i] * xN[i]);                                               
                //double mJ = obj.c[0] + obj.c[1] * x + obj.c[2] * x2 + obj.c[3] * x3 + obj.c[4] * x4 + obj.c[5] * x5;
                return mJ;
            }
        }

        //This call returns the Attenuation value        
        public double GetAttenuationValue(double wavelength, int pulseDivider)
        {
            LaserEnergyCorrectionStruct obj = getElement(wavelength,pulseDivider);
            return obj.Attenuation;
        }

        LaserEnergyCorrectionStruct getElement(double wavelength,int pulseDivider)
        {
            //no validation check for performance reasons, let errors cause exceptions at this level
            int offsetW = (int)(Math.Round(wavelength) - _baseWavelength);
            int offsetP = (int)(pulseDivider - _basePulseDivider);
            int row = _wavelengthsLookup[offsetW];
            int column = _pulseDividerLookup[offsetP];
            return _laserWavelengthVsRepetitions[row, column];
        }

        //public static void CreateTestFile(string testFileName)
        //{
        //    var randomGenerator = new Random();

        //    //Helper Text File
        //    string testTextFileName = testFileName + ".txt";

        //    UInt16 coeffCount = (UInt16)randomGenerator.Next(3, 9);
        //    UInt16 repetitionsCount = 5;//(UInt16)randomGenerator.Next(5, 10);
        //    UInt16 wavelengthCount = 50;// (UInt16)randomGenerator.Next(10, 100);
        //    string fileDescriptor = "Test calibration file generated by:" + Environment.MachineName + "\\" + Environment.UserName + " on " + DateTime.Now.ToLongTimeString();
        //    UTF8Encoding utf8 = new UTF8Encoding();
        //    byte[] bFileDescriptor = utf8.GetBytes(fileDescriptor);
        //    //Write file
        //    System.IO.FileStream fs = new System.IO.FileStream(testFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

        //    using (BinaryWriter r = new System.IO.BinaryWriter(fs))
        //    using (TextWriter rText = File.CreateText(testTextFileName))
        //    {
        //        //header
        //        r.Write((UInt16)0);
        //        r.Write((UInt16)bFileDescriptor.Length);
        //        r.Write(bFileDescriptor);
        //        r.Write(LaserEnergyCorrectionHeaderStruct.SafetyNumber);
        //        rText.Write((UInt16)bFileDescriptor.Length);
        //        for (int i = 0; i < bFileDescriptor.Length; i++)
        //            rText.Write(Convert.ToChar(bFileDescriptor[i]));
        //        rText.Write("0x" + LaserEnergyCorrectionHeaderStruct.SafetyNumber.ToString("X4"));
        //        rText.WriteLine();
        //        r.Write((UInt16)100);// laser undivided trigger rate
        //        r.Write(repetitionsCount);
        //        r.Write(coeffCount);
        //        rText.WriteLine(repetitionsCount + " " + coeffCount);

        //        //repetition table
        //        UInt16 rep = 10;
        //        for (int i = 0; i < repetitionsCount; i++)
        //        {
        //            r.Write(rep);
        //            rText.Write(rep + " ");
        //            rep += 3;
        //        }
        //        rText.WriteLine();

        //        //attenuation table
        //        UInt16 wl = 700;
        //        r.Write(wavelengthCount);
        //        rText.WriteLine(wavelengthCount);
        //        for (int i = 0; i < wavelengthCount; i++)
        //        {
        //            //new table row
        //            //wavelength                    
        //            r.Write(wl);
        //            rText.Write("" + wl + "         ");
        //            wl += 10;
        //            //rest structures
        //            for (int k = 0; k < repetitionsCount; k++)
        //            {
        //                double randomNumber;

        //                //E
        //                randomNumber = randomGenerator.NextDouble() * 150;
        //                r.Write(randomNumber);
        //                rText.Write("" + randomNumber.ToString("F2") + "  ");

        //                //A
        //                randomNumber = randomGenerator.NextDouble() * 100;
        //                r.Write(randomNumber);
        //                rText.Write("" + randomNumber.ToString("F1") + "  ");

        //                //Coefficients
        //                for (int l = 0; l < coeffCount; l++)
        //                {
        //                    randomNumber = randomGenerator.NextDouble();
        //                    r.Write(randomNumber);
        //                    rText.Write(randomNumber.ToString("F3") + " ");
        //                }
        //                rText.Write("   ");
        //            }
        //            rText.WriteLine();
        //        }
        //    }

        //    string errorM;
        //    LaserCalibrationTable reader = new LaserCalibrationTable();
            
        //    reader.LoadFile(testFileName,out errorM);
        //    reader.GetAveragePower(730, 19);
        //}
    }
}
