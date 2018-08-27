using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xvue.Framework.API.Services.Performance;
using System.Collections.ObjectModel;
using Xvue.Framework.API.ViewModels.Base;


namespace Xvue.Framework.API.ViewModels.Performance
{
    public abstract class ViewModelPerformanceMonitorBase : ViewModelSerializedPlugin, IPerformanceMonitor
    {
        #region localvariables
        Xvue.Framework.API.ViewModels.Base.SafeObservableCollection<ViewModelPerformanceCounter> _counters;
        protected System.Windows.Threading.Dispatcher _UIDispatcher;
        #endregion


        protected ViewModelPerformanceMonitorBase()//Xvue.MSOT.Framework.Services.Core.IMSOTSystem system)
        {
            _UIDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            _counters = new Xvue.Framework.API.ViewModels.Base.SafeObservableCollection<ViewModelPerformanceCounter>(_UIDispatcher);
        }

        int getUniqueId()
        {
            Random a = new Random(DateTime.Now.Millisecond);
            int newId;
            bool done;
            lock (_counters)
            {
                do
                {
                    done = true;
                    newId = a.Next(int.MaxValue);
                    for (int i = 0; i < _counters.Count; i++)
                    {
                        if (_counters[i] != null)
                        {
                            if (newId == _counters[i].CounterId)//id exists!!!
                            {
                                done = false;
                                break;
                            }
                        }
                    }
                } while (!done);
            }
            return newId;
        }

        ViewModelPerformanceCounter getCounter(int counterId)
        {
            for (int i = 0; i < _counters.Count; i++)
            {
                if (_counters[i] != null)
                    if (counterId == _counters[i].CounterId)//id exists!!!
                        return _counters[i];
            }
            return null;
        }

        #region IPerformanceMonitor Members
        public int RegisterCounter(string deviceName, string functionName)
        {            
            ViewModelPerformanceCounter counter = new ViewModelPerformanceCounter(_UIDispatcher);
            counter.DeviceName = deviceName;
            counter.FunctionName = functionName;
            
            int counterId;
            //bool noVacancy = true;
            lock (_counters)
            {
                counterId = getUniqueId();
                counter.CounterId = counterId;
                _counters.Add(counter);
            }
            //OnPropertyChanged("Counters");
            return (counterId);            
        }

        public void UnregisterCounter(int counterId)
        {
            bool nullWarning = false;
            if (counterId < 0)
                return;
            lock (_counters)
            {
                ViewModelPerformanceCounter c = getCounter(counterId);
                if (c == null)
                    nullWarning = true;
                else
                {
                    c.Stop();
                    _counters.Remove(c);
                }                
                System.Diagnostics.Debug.Assert(!nullWarning);
            }
        }

        public void Init(int counterId)
        {
            if (counterId <= 0)
                return;
            lock (_counters)
            {
                ViewModelPerformanceCounter c = getCounter(counterId);
                c.Init();                
            }
        }

        public void Start(int counterId, double curFrameTime)
        {
            if (counterId <= 0)
                return;
            lock (_counters)
            {
                ViewModelPerformanceCounter c = getCounter(counterId);
                c.Start(curFrameTime);
            }
        }

        public void StopCounter(int counterId)
        {
            if (counterId <= 0)
                return;
            lock (_counters)
            {
                ViewModelPerformanceCounter c = getCounter(counterId);
                c.Stop();
            }
        }

        //public double GetInterval(int counterId, int Iteration)
        //{
        //    lock (_counters)
        //    {
        //        return _counters[counterId].GetInterval(Iteration);
        //    }
        //}

        //public double GetInterval(int counterId, int Iteration, out double curFrameTime)
        //{
        //    lock (_counters)
        //    {
        //        return _counters[counterId].GetInterval(Iteration,out curFrameTime);
        //    }
        //}

        #endregion

        public ObservableCollection<ViewModelPerformanceCounter> Counters
        {
            get
            {
                if (_counters == null)
                    return null;
                return _counters.ObservableCollection; 
            }
        }

        //protected override void AfterInitialize()
        //{
        //    base.AfterInitialize();
        //    MSOTService.IMonitor.Report(deviceId, Xvue.MSOT.Framework.Services.Monitor.DeviceStatus.Ready);
        //}
    }
}
