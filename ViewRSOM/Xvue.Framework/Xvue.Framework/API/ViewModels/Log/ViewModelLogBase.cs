using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Xvue.Framework.API.Services.Log;
using Xvue.Framework.API.ViewModels.Base;
using Xvue.Framework.API.DataModels.Base;

namespace Xvue.Framework.API.ViewModels.Log
{
    /// <summary>
	/// Summary description for xvueEventLog.
	/// </summary>
	/// 
    /// <summary>
    /// Indicate the type of warning or error that is logged
    /// </summary>
    public class ViewModelLogItem : ViewModelBase
    {
        readonly int _logType;
        public int LogType
        {
            get { return _logType; }
        }
        readonly string _senderDescription;
        public string SenderDescription
        { get { return _senderDescription; } }
        readonly string _message;
        public string Message 
        { get { return _message; }}
        DateTime _timestamp;
        public DateTime Timestamp
        { 
            get { return _timestamp; }
            set { _timestamp = value; }
        }
        public string TimestampString
        {
            get { return _timestamp.ToShortDateString() + " " + _timestamp.ToLongTimeString(); }
        }

        public ViewModelLogItem(int type, string sender, string message)
        {
            _logType = type;
            _senderDescription = sender;
            _message = message;
            //_timestamp = DateTime.Now;
        }
        public ViewModelLogItem(int type, string sender, string message,DateTime time)
        {
            _logType = type;
            _senderDescription = sender;
            _message = message;
            _timestamp = time;
        }
    }

    [ComVisibleAttribute(false)]
    public abstract class ViewModelLogBase : ViewModelSerializedPlugin//MVVM<DataModelLog>
        , ICoreServiceLog        
	{
        //DataModelLogBase _dMConfiguration;

        //public DataModelLogBase DMConfiguration
        //{
        //    get { return _dMConfiguration; }
        //}

        //protected override void SetSpeedDataModel(DataModelPluginConfiguration data)
        //{
        //    _dMConfiguration = data as DataModelLogBase;
        //    base.SetSpeedDataModel(data);
        //}

        //I need I think both mechanisms (begin invoke and my background thread)
        //to make sure that the add log will not delay
        protected SafeObservableCollection<ViewModelLogItem> _safeLogRows;//= new ObservableList<DataModelLogItem>(System.Windows.Threading.Dispatcher.CurrentDispatcher);// = new ObservableList<DataModelLogItem>();
        //private ObservableCollection<string> _l = new ObservableCollection<string>();
        public ObservableCollection<ViewModelLogItem> LogUI
        {
            get {
                if (_safeLogRows == null)
                    return null;
                return _safeLogRows.ObservableCollection; 
            }
        }

        public abstract void AddEventLog(int logType, string reason, string eventMessage);
    }
}
