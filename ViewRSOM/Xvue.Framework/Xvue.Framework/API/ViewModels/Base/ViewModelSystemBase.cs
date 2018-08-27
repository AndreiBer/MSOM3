using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Xvue.Framework.API.Services.Log;
using Xvue.Framework.API.DataModels.Base;
using Xvue.Framework.API.ViewModels.Log;

namespace Xvue.Framework.API.ViewModels.Base
{
	public abstract class ViewModelSystemBase : ViewModelSerializedPlugin
    {
        #region localvariables
		readonly System.Windows.Threading.Dispatcher _UIDispatcher;        
        #endregion

        protected ViewModelSystemBase()
		{
            _UIDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
		}
            
        protected override bool Load()
        {
            xvLog = CreateNewLog();
            xvLog.InitializePlugin("SystemLog", LogCatalogItem, null);
            xvLog.CommandLoad.Execute(null);
#if DEBUG
            xvLog.AddEventLog(2, "Framework", "Debug");
#endif
            return base.Load();
        }

        ViewModelLogBase xvLog;
        public ViewModelLogBase Logger
        {
            get { return xvLog; }
        }
        //must do that here in order to prevent circular calls from internals
        public sealed override ICoreServiceLog Log
        {
            get { return xvLog; }
        }

        public sealed override ViewModelSystemBase Service
        {
            get
            {
                return this;
            }
        }

        public System.Windows.Threading.Dispatcher UIDispatcher { get { return _UIDispatcher;} }

        public abstract string SystemDescription{get;}
        public abstract string SystemName { get; }
        public abstract ViewModelLogBase CreateNewLog();
        public abstract string PluginsDirectoryName{get;}
        public abstract DataModelCatalogItem LogCatalogItem { get; }
        public static string SystemPath { 
            get 
            {
                FileInfo file = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                return file.DirectoryName+@"\";
            }
		}
 
        public static bool IsDirectoryWritable(string directoryPath)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(directoryPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose)) { }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
