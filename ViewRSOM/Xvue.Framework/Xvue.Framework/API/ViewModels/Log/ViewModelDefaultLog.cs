using System;
using System.Collections;
using System.Runtime.InteropServices;
//using System.Windows.Forms;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Xvue.Framework.API.ViewModels.Base;
using Xvue.Framework.API.Services.Log;
using Xvue.Framework.API.ViewModels.Log;
namespace Xvue.Framework.API.ViewModels.Log
{
	/// <summary>
	/// Summary description for xvueEventLog.
	/// </summary>
	/// 

    //class EventLogService:
    //{
    //    #region ICoreServiceLog Members

    //    public void  AddEventLog(EnumLogType xtype, string xobject, string xmessage)
    //    {
    //        plugin.AddEventLog(xtype, xobject, xmessage); 	        
    //    }

    //    public void  AddAVInfoLog(Xvue.Fx.API.Services.VideoAudio.IxvueAVFileInfo xtype, string xobject, string xmessage)
    //    {
    //        plugin.AddAVInfoLog(xtype, xobject, xmessage);
    //    }
        
    //    public long CaptureErrorCount
    //    {
    //        get { return plugin.CaptureErrorCount; }
    //        set { plugin.CaptureErrorCount = value; }
    //    }
    //    #endregion

    //    ViewModelLogBase plugin;
    //    DataModelLog dm;

    //    #region ICoreServiceLog Members

    //    public ViewModelLogBase ViewModel
    //    {
    //        set { plugin = value;
    //            //to provide fast access without box/unbox
    //        dm = plugin.DMConfiguration as DataModelLog;
    //        }
    //    }

    //    public EnumLogConfigurationTypes LogType
    //    {
    //        get { return dm.LogConfigurationType ; }
    //    }

    //    #endregion
    //}
    //public enum EnumLogType { Critical = 0, Warning = 1, Plugin = 2 }
	public class ViewModelDefaultLog:ViewModelLogBase
	{
        private Thread xvThreadLog = null;
        private bool xvKillThread = false;
        Collection<ViewModelLogItem> safelogarray = new Collection<ViewModelLogItem>();

        protected override bool Load()
        {
            _safeLogRows = new SafeObservableCollection<ViewModelLogItem>(System.Windows.Threading.Dispatcher.CurrentDispatcher);// = new ObservableList<DataModelLogItem>();
            //using (_safeLogRows.AcquireLock())
            //{
            //    ViewModelLogItem log1 = new ViewModelLogItem(EnumLogType.Info, "how", "when");
            //    _safeLogRows.Add(log1);
            //}
            OnPropertyChanged("LogUI");
            //using (_safeLogRows.AcquireLock())
            //{
            //    ViewModelLogItem log1 = new ViewModelLogItem(EnumLogType.Info, "how1", "when1");
            //    _safeLogRows.Add(log1);
            //}
            if (xvThreadLog == null)
            {
                xvThreadLog = new Thread(new ThreadStart(ThreadProc));
                //       xvThreadSequencer.Priority = ThreadPriority.Highest;
                xvThreadLog.Start();
            }

            return base.Load();
        }
        protected override bool Unload()
        {
            try
            {
                if (xvThreadLog != null)
                {
                    xvKillThread = true;
                    Thread.Sleep(200);
                    xvThreadLog.Abort();
                }
            }
            catch (Exception)
            {
            }
            return true;
        }
        protected override void AfterInitialize()
        {

            base.AfterInitialize();
        }
 

        public override void AddEventLog(int logType, string reason, string eventMessage)
        {
            ViewModelLogItem item = new ViewModelLogItem(logType, reason, eventMessage);
            lock (safelogarray)
                safelogarray.Add(item);
                        
        }

        void ThreadProc()
        {
            ArrayList threadLogArray = new ArrayList();
            while (!xvKillThread)
            {
                lock (safelogarray)
                {
                    threadLogArray.AddRange(safelogarray);
                    safelogarray.Clear();
                }

                try
                {
                    foreach (ViewModelLogItem pa in threadLogArray)
                    {
                        try
                        {
                            string sSource;
                            string sLog;
                            //string sEvent;

                            sSource = "iOrasis Xvue.Framework";
                            sLog = "Application";
                            //sEvent = "Core Event";

                            _safeLogRows.AddThreadSafeItem(pa);//.Add(lg);
                            if (!EventLog.SourceExists(sSource))
                                EventLog.CreateEventSource(sSource, sLog);
                            //EventLog.WriteEntry(sSource, sEvent);
                            //EventLog.WriteEntry(sSource, sEvent,
                            //    EventLogEntryType.Warning, 234);
                            //EventLog.WriteEntry(sSource, "Filename:" + pav.xFileName + "W:" + pav.xVideoWidth + "H:" + pav.xVideoHeight + "D:" + pav.xDuration,
                            //    EventLogEntryType.Warning, 234);
                            if (pa.LogType != 0)
                                EventLog.WriteEntry(sSource, pa.Message + ":" + pa.LogType.ToString() + " * " + pa.SenderDescription, EventLogEntryType.Information, 155);
                            else
                                EventLog.WriteEntry(sSource, pa.Message + ":" + pa.LogType.ToString() + " * " + pa.SenderDescription, EventLogEntryType.Warning, 153);
                            //}
                        }
                        catch (Exception)
                        {
                        }
                    }
                    threadLogArray.Clear();
                }
                catch (Exception)
                {
                }

                Thread.Sleep(50);
            }
        }
   
    }
}
