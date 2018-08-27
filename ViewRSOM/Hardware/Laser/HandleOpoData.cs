using System;
using System.Threading;
using General.Tools.Filewriter;
using General.Tools.Communication;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Linq;
using System.Drawing;
using System.Windows;
//using EnergyCalibrationTool;
//using EnergyMeasurementTool;

namespace Laser.OpoData
{


    public class HandleOpoData
    {

        List<OPOData> myOpoData = new List<OPOData>();
        public List<OPOMapData> myOpoMapData = new List<OPOMapData>();
        //MainWindow mnwind = Application.Current.MainWindow as MainWindow;

        public int PockelsCellDelaysCount = 1;
        int last_position = 0;
        int actualShotCount = 0;
        int sweepCount = 0;
        int pulseDividerCount = 0;
        int lastsweepCount = 0;
        public HandleOpoData()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
            {
                GUI_Communicator.MyLaserStatusMessage += new MyLaserMessageHandler(handleLaserStatusMessage);
            }));
        }

        /*
        private void handleLaserStatusMessage_(string sender, string receiver, string message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            {
                
                if (receiver == "OpoDataHandler" && sender == "OPO")
                {
                    actualShotCount++;
                    string[] stringSeparators = new string[] { "," };
                    string[] values = message.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    int shotcounter = Convert.ToInt32(values[0]);
                    int position = Convert.ToInt32(values[1]);
                    double energy = Convert.ToDouble(values[2]);
                    DateTime timestamp = DateTime.Now;
                    myOpoData.Add(new OPOData() { Timestamp = timestamp, Shotcounter = shotcounter, Position = position, Energy = energy, Sweep=sweepCount, PulseDivider=pulseDividerCount });

                    if (position != last_position)// new wavelength
                    {
                        if(position!=0)// not for first shot
                        { 
                            GUI_Communicator.sendStatus("OPO_DATA_HANDLER","COHERENT","NEW_WL_INDEX="+position);
                            actualShotCount = 0;
                            //last_position++;
                        }
                        else  // if sweep starts from the beginning
                        {
                                sweepCount++;
                                GUI_Communicator.sendStatus("OPO_DATA_HANDLER", "COHERENT", "NEW_WL_INDEX=0");
                                GUI_Communicator.sendStatus("OPO_DATA_HANDLER", "COHERENT", "NEW_SWEEP=" + sweepCount);
                                                       
                        }
                    }
                    

                    last_position = position;

                        
                }
                
                //LogTextBox.Text += data.Timestamp + ": " + data.Energy + "\n";

            });

        }
        */
        private void handleLaserStatusMessage(string sender, string receiver, string message)
        {
           

                if (receiver == "OpoDataHandler" && sender == "OPO")
                {
                    DateTime stamp1 = DateTime.Now;
                    actualShotCount++;
                    string[] stringSeparators = new string[] { "," };
                    string[] values = message.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    int shotcounter = Convert.ToInt32(values[0]);
                    int position = Convert.ToInt32(values[1]);
                    double energy = Convert.ToDouble(values[2]);
                    DateTime timestamp = DateTime.Now;
                    myOpoData.Add(new OPOData() { Timestamp = timestamp, Shotcounter = shotcounter, Position = position, Energy = energy, Sweep = sweepCount, PulseDivider = pulseDividerCount });
                    DateTime stamp2 = DateTime.Now;
                    System.Diagnostics.Trace.WriteLine("handle OPO Data   : " + ((TimeSpan)(stamp2 - stamp1)).Milliseconds);
                    
                    
                        if (position != last_position)// new wavelength
                        {
                            if (position != 0)// not for first shot
                            {
                                GUI_Communicator.sendStatus("OPO_DATA_HANDLER", "COHERENT", "NEW_WL_INDEX=" + position+","+shotcounter);
                                actualShotCount = 0;
                                //last_position++;
                            }
                            else  // if sweep starts from the beginning position==0
                            {

                                if (PockelsCellDelaysCount==1)
                                {
                                    sweepCount++;
                                    GUI_Communicator.sendStatus("OPO_DATA_HANDLER", "COHERENT", "NEW_SWEEP=" + sweepCount);
                                }
                                else // PockelsCellDelaysCount>1
                                {
                                    if (sweepCount < PockelsCellDelaysCount-1)// in PockelsCellSchleife
                                    {
                                        sweepCount++;
                                        //GUI_Communicator.sendStatus("OPO_DATA_HANDLER", "COHERENT", "NEW_WL_INDEX=0");
                                        GUI_Communicator.sendStatus("OPO_DATA_HANDLER", "COHERENT", "NEW_SWEEP=" + sweepCount);
                                    }
                                    else if (sweepCount >= PockelsCellDelaysCount-1) //PockelsCellSchleife Durchlaufen => setzte neuen Pulse Divider
                                    {
                                        sweepCount = 0;
                                        pulseDividerCount++;
                                        //GUI_Communicator.sendStatus("OPO_DATA_HANDLER", "COHERENT", "NEW_WL_INDEX=0");
                                        //GUI_Communicator.sendStatus("OPO_DATA_HANDLER", "COHERENT", "NEW_SWEEP=0");
                                        GUI_Communicator.sendStatus("OPO_DATA_HANDLER", "COHERENT", "NEW_PULSE_DIVIDER=" + pulseDividerCount);
                                    }
                                }
                            }
                        }

                    
                    last_position = position;
                    

                }

                //LogTextBox.Text += data.Timestamp + ": " + data.Energy + "\n";

            

        }

        public List<OPOData> showRecordedData()
        {
            return myOpoData;
        }
        public List<OPOData> showRecordedData(int startindex, int endindex)
        {
            if (startindex < 0)
                startindex = 0;
            List<OPOData> croppedList=new List<OPOData>();
            for(int i=startindex;i<=endindex;i++)
                croppedList.Add(myOpoData[i]);
            return croppedList;
        }
        

        public void ClearData()
        {
            myOpoData.Clear();
        }  

        public void fillWithDefaultValues(int valuesCompleteCount)
        {
            for (int i = myOpoMapData.Count; i < valuesCompleteCount; i++)
                myOpoMapData.Add(new OPOMapData() { MeanEnergy = 0, Stdv = 0 });
        }

        public Canvas displayOpoEnergy(List<OPOMapData> mapData)
        {

            Canvas myCanvas = new Canvas();
            myCanvas.Height = 200;
            myCanvas.Width = 200;
            myCanvas.Margin = new System.Windows.Thickness(10, 10, 10, 10);
            // search FluenceMapData for min and Max value
            double MinEnergy = 0;
            double MaxEnergy = mapData.Max(map => map.MeanEnergy);
            if (MaxEnergy == 0 || double.IsNaN(MaxEnergy))
                MaxEnergy = 1;
            // Min(map => map.Energy);    

            /*
            // Add Rectangle Elements
            for (int i = 0; i < mapData.Count; i++)
            {
                Rectangle myRect = new System.Windows.Shapes.Rectangle();
                
                myRect.HorizontalAlignment = HorizontalAlignment.Left;
                myRect.VerticalAlignment = VerticalAlignment.Center;
                double height = 0;
                try
                {
                    height = mapData[i].MeanEnergy / MaxEnergy * 200;
                    myRect.ToolTip = "WL: " + mapData[i].Wavelength + "nm; E: " + Math.Round(mapData[i].MeanEnergy, 2).ToString("0.0") + "; StDv: " + Math.Round(mapData[i].Stdv, 2) + ";";
                }
                catch
                {
                    height = 2;
                    myRect.ToolTip = "WL: " + mapData[i].Wavelength + "nm; E: not defined, StDv: not defined";
                }
                
                if (height == 0 ||double.IsNaN(height))
                {
                    height = 2;
                    myRect.ToolTip = "WL: " + mapData[i].Wavelength + "nm; E: not defined, StDv: not defined";
                }
                    
                double width = 600 / mapData.Count;
                myRect.Height = height;
                myRect.Width = width;
                if(width>=3)
                    myRect.StrokeThickness = 1;
                else
                    myRect.StrokeThickness = 0;
                myRect.Stroke = System.Windows.Media.Brushes.Black;
                myRect.Fill = System.Windows.Media.Brushes.DeepSkyBlue;
                myRect.Margin = new System.Windows.Thickness((i * 600 / mapData.Count), 200-height, 0, 0);
                myCanvas.Children.Add(myRect);
               
                Line myLine = new Line();
                myLine.Stroke = System.Windows.Media.Brushes.OrangeRed;
                double X = (i * 600 / mapData.Count) + (600/mapData.Count /2);
                myLine.X1 = X;
                myLine.X2 = X;
                if( double.IsNaN(mapData[i].Stdv))
                {
                    myLine.Y1 = 1;
                    myLine.Y2 = 1;
                }
                else
                {
                    myLine.Y1 = 200 - height - mapData[i].Stdv / MaxEnergy * 200;
                    myLine.Y2 = 200 - height + mapData[i].Stdv / MaxEnergy * 200;
                }
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 1;
                myCanvas.Children.Add(myLine);
            }
             */ 
            return myCanvas;
        }

        public Canvas displayOpoEnergy(List<OPOMapData> mapData, int CanvasHeight, int CanvasWidth)
        {

            Canvas myCanvas = new Canvas();
            myCanvas.Height = CanvasHeight;
            myCanvas.Width = CanvasWidth;
            myCanvas.Margin = new System.Windows.Thickness(10, 10, 10, 10);
            // search FluenceMapData for min and Max value
            double MinEnergy = 0;
            double MaxEnergy = mapData.Max(map => map.MeanEnergy);
            if (MaxEnergy == 0 || double.IsNaN(MaxEnergy))
                MaxEnergy = 1;
            // Min(map => map.Energy);    

            /*
            // Add Rectangle Elements
            for (int i = 0; i < mapData.Count; i++)
            {
                Rectangle myRect = new System.Windows.Shapes.Rectangle();

                myRect.HorizontalAlignment = HorizontalAlignment.Left;
                myRect.VerticalAlignment = VerticalAlignment.Center;
                double height = 0;
                try
                {
                    height = mapData[i].MeanEnergy / MaxEnergy * CanvasHeight;
                    myRect.ToolTip = "WL: " + mapData[i].Wavelength + "nm; E: " + Math.Round(mapData[i].MeanEnergy, 2).ToString("0.0") + "; StDv: " + Math.Round(mapData[i].Stdv, 2) + ";";
                }
                catch
                {
                    height = 2;
                    myRect.ToolTip = "WL: " + mapData[i].Wavelength + "nm; E: not defined, StDv: not defined";
                }

                if (height == 0 || double.IsNaN(height))
                {
                    height = 2;
                    myRect.ToolTip = "WL: " + mapData[i].Wavelength + "nm; E: not defined, StDv: not defined";
                }

                double width = CanvasWidth / mapData.Count;
                myRect.Height = height;
                myRect.Width = width;
                if (width >= 3)
                    myRect.StrokeThickness = 1;
                else
                    myRect.StrokeThickness = 0;
                myRect.Stroke = System.Windows.Media.Brushes.Black;
                myRect.Fill = System.Windows.Media.Brushes.DeepSkyBlue;
                myRect.Margin = new System.Windows.Thickness((i * CanvasWidth / mapData.Count), CanvasHeight - height, 0, 0);
                myCanvas.Children.Add(myRect);

                Line myLine = new Line();
                myLine.Stroke = System.Windows.Media.Brushes.OrangeRed;
                double X = (i * CanvasWidth / mapData.Count) + (CanvasWidth / mapData.Count / 2);
                myLine.X1 = X;
                myLine.X2 = X;
                if (double.IsNaN(mapData[i].Stdv))
                {
                    myLine.Y1 = 1;
                    myLine.Y2 = 1;
                }
                else
                {
                    myLine.Y1 = CanvasHeight - height - mapData[i].Stdv / MaxEnergy * CanvasHeight;
                    myLine.Y2 = CanvasHeight - height + mapData[i].Stdv / MaxEnergy * CanvasHeight;
                }
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 1;
                myCanvas.Children.Add(myLine);
            }
             */ 
            return myCanvas;
        }

        public void processOpoData()
        {
            myOpoMapData.Add(new OPOMapData() { MeanEnergy = MeanValue(myOpoData), Stdv = Stdv(myOpoData) });
            myOpoData.Clear();
        }

        public void processOpoData(List<OPOData> myList)
        {
            myOpoMapData.Add(new OPOMapData() { MeanEnergy = MeanValue(myList), Stdv = Stdv(myList) });
        }

        public void processOpoData(List<OPOData> myList, double wavelength)
        {
            myOpoMapData.Add(new OPOMapData() { MeanEnergy = MeanValue(myList), Stdv = Stdv(myList), Wavelength=wavelength });
        }
        //new TO BE IMPLEMENTED
        public int processOpoDataControlled(List<OPOData> myList, int wavelength_index)
        {
            foreach (var opodata in myList)
            {
                if (opodata.Position != wavelength_index)
                { }
            }
            int numberOfWrongValues=0;
            return numberOfWrongValues;

        }

//end new
        public double MeanValue(List<OPOData> data)
        {
            double sum = 0;
            for (int i = 0; i < data.Count; i++)
            {
                sum += data[i].Energy;
            }
            return sum / data.Count;
        }

        public double Variance(List<OPOData> data)
        {
            double variance = 0;
            double mean = MeanValue(data);
            for (int i = 0; i < data.Count; i++)
            {
                variance += Math.Pow((data[i].Energy - mean), 2);
            }
            return variance / data.Count;
        }

        public double Stdv(List<OPOData> data)
        {
            return Math.Sqrt(Variance(data));
        }

     }

    public class OPOData
    {
        public DateTime Timestamp { get; set; }

        public int Shotcounter { get; set; }

        public int Position { get; set; }

        public double Energy { get; set; }

        public int Sweep { get; set; }

        public int PulseDivider { get; set; }

    }

    public class OPOMapData
    {

        public double MeanEnergy { get; set; }

        public double Stdv { get; set; }

        public double Wavelength { get; set; }

    }

   
}

/*
namespace EnergyCalibrationTool  // Placebo
{ }
namespace EnergyMeasurementTool // Placebo
{ }
*/