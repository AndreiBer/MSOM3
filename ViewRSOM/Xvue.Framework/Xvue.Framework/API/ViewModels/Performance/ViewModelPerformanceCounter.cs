
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;


//using Xvue.MSOT.Framework.Services.Core;
namespace Xvue.Framework.API.ViewModels.Performance
{
	/// <summary>
    /// Offers a counter that can monitor time critical tasks with high precision timer. Also offers
	/// the additional functionality of keeping time interval records for each Start/Stop sequence.
	/// Additionally  the min, max and average value of the event history are available
	/// </summary>
	[Guid("2922FCA6-DDAD-4242-81C9-4841F664A7A6")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [BrowsableAttribute(true)]//,XmlIgnore]
    public class ViewModelPerformanceCounter : Xvue.Framework.API.ViewModels.Base.ViewModelBase
	{
        #region localvariables
        private string _deviceName;
        private bool _keepRecords = false;
        
        private string _functionName;
        private int _counterId;        
		private ViewModelPerformanceCounterRecord currentRecord;
		private long frequency;
        private Xvue.Framework.API.ViewModels.Base.SafeObservableCollection<ViewModelPerformanceCounterRecord> _counterRecords;
		private long _min;
		private long _max;
        private int _iterations;
		private double _average;
        private Stopwatch stopwatch;

		Decimal multiplier = new Decimal(1.0e6);
        #endregion

		/// <summary>The default constructor </summary>
        public ViewModelPerformanceCounter(System.Windows.Threading.Dispatcher uiDispatcher)
		{
            _counterRecords = new Xvue.Framework.API.ViewModels.Base.SafeObservableCollection<ViewModelPerformanceCounterRecord>(uiDispatcher);
            stopwatch = new Stopwatch();
            frequency = Stopwatch.Frequency;
            stopwatch.Reset();
            currentRecord = new ViewModelPerformanceCounterRecord();
			this.Init();

		}

		/// <summary>Clears all interval records, min, max and average values</summary> 
		public void Init()
		{
            using (Xvue.Framework.API.ViewModels.Base.TimedLock timedLock = _counterRecords.AcquireLock())
            {
                _counterRecords.Clear();    
            }			
            _min = long.MaxValue;
            _max = long.MinValue;
			_average = 0.0;
            _iterations = 0;
            currentRecord.CounterValue = 0;
            refreshValueProperties();
		}

		
		/// <summary>Creates a new interval record</summary>
		/// <param name="curFrameTime">Provide the frame time of the new interval record</param>
		public void Start(double curFrameTime)
		{
			this.currentRecord.FrameTime = curFrameTime;
            stopwatch.Reset();
            stopwatch.Start();
		}

		/// <summary>Terminates the current interval record</summary>		
		public void Stop()
		{
            long newInterval;
            stopwatch.Stop();            
            newInterval = stopwatch.ElapsedTicks;
            this.currentRecord.CounterValue = GetValueAs_msecs(newInterval);

			if( newInterval > _max ) _max = newInterval;
			if( newInterval < _min ) _min = newInterval;
            _average = (_average * (double)_iterations + (double)newInterval) / ((double)_iterations + 1);
            _iterations++;
            if (_keepRecords)
            {
                _counterRecords.AddThreadSafeItem(this.currentRecord);
                OnPropertyChanged("CounterRecords");
            }
            refreshValueProperties();
		}

		/// <summary>Returns the interval of the last record in usecs</summary>		
		/// <returns>The interval of the last record in msecs</returns>
		public double LastInterval
		{
            get
            {
                try
                {
                    if (_keepRecords)
                    {
                        double retValue = 0;
                        if (_counterRecords.Count > 0)
                            retValue = _counterRecords[_counterRecords.Count - 1].CounterValue;
                                //((double)(this._counterRecords[_counterRecords.Count - 1].CounterValue) * (double)multiplier) / (double)frequency;
                        return retValue;
                    }
                    else
                        return currentRecord.CounterValue;
                            //((((double)(this.currentRecord.CounterValue) * (double)multiplier) / (double)frequency));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message + "@PerformanceCounter");
                    return 0;
                }

            }
		}
		/// <summary>Returns the number of available interval records in the performance counter history</summary>		
		/// <returns>Available interval records</returns>
		public int Iterations
		{
            get
            {
                return _iterations;// CounterRecords.Count;
            }
		}
        ///// <summary>Returns the interval of the requested record in usecs</summary>		
        ///// <param name="Iteration">Provide the requested iteration number</param>
        ///// <returns>The interval of the requested record in usecs</returns>
        //public double GetInterval(int Iteration)
        //{
        //    if (( Iteration > CounterRecords.Count ) || ( Iteration == 0 )) return 0.0;

        //    counetrRecord theRecord = (counetrRecord)this.CounterRecords[Iteration-1];
        //    return ((((double)(theRecord.CounterValue)* (double) multiplier) / (double) frequency) );
        //}

        ///// <summary>Returns the interval of the requested record in usecs, and the associated time frame</summary>		
        ///// <param name="Iteration">Provide the requested iteration number</param>
        ///// <param name="curFrameTime">Get the timeframe for the requested record</param>
        ///// <returns>The interval of the requested record in usecs</returns>
        //public double GetInterval(int Iteration, out double curFrameTime)
        //{
        //    if (( Iteration > CounterRecords.Count ) || ( Iteration == 0 )) 
        //    {
        //        curFrameTime = 0;
        //        return 0.0;
        //    }

        //    counetrRecord theRecord = (counetrRecord)this.CounterRecords[Iteration-1];
        //    curFrameTime = theRecord.FrameTime;
        //    return ((((double)(theRecord.CounterValue)* (double) multiplier) / (double) frequency) );
        //}

		/// <summary>Returns the minimum recorded interval in the performance counter history</summary>		
		/// <returns>The minimum recorded interval in msecs</returns>
		public double Min
		{
            get
            {
                if (_iterations > 0)
                    return GetValueAs_msecs(_min);
                //((((double)(this._min) * (double)multiplier) / (double)frequency));
                else
                    return 0;
            }
		}

		/// <summary>Returns the maximum recorded interval in the performance counter history</summary>		
		/// <returns>Maximum recorded interval in msecs</returns>
		public double Max
		{
            get
            {
                if (_iterations > 0)
                    return GetValueAs_msecs(_max);
                //((((double)(this._max) * (double)multiplier) / (double)frequency));
                else
                    return 0;
            }
		}

		/// <summary>Returns the average recorded interval of the performance counter</summary>		
		/// <returns>The average recorded interval in msecs</returns>
		/// 
		public double Average
		{
            get
            {
                return GetValueAs_msecs(_average);
                    //(((this._average * (double)multiplier) / (double)frequency));
            }
		}

        public string DeviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; }
        }

        public string FunctionName
        {
            get { return _functionName; }
            set { _functionName = value; }
        }

        public int CounterId
        {
            get { return _counterId; }
            set { _counterId = value; }
        }

        public bool KeepRecords
        {
            get { return _keepRecords; }
            set
            { 
                _keepRecords = value;
                this.Init();
            }
        }

        public ObservableCollection<ViewModelPerformanceCounterRecord> CounterRecords
        {
            get { return _counterRecords.ObservableCollection; }
        }

        private double GetValueAs_msecs(long counter)
        {
           return (((double)(counter) * (double)multiplier) / (double)frequency/1000.0);
        }

        private double GetValueAs_msecs(double counter)
        {
            return (((counter) * (double)multiplier) / (double)frequency/1000.0);
        }

        
        private void refreshValueProperties()
        {
            //OnPropertyChanged("FunctionName");
            OnPropertyChanged("Min");
            OnPropertyChanged("Max");
            OnPropertyChanged("Average");
            OnPropertyChanged("Iterations");
            OnPropertyChanged("LastInterval");
        }

	}

    

    
}
