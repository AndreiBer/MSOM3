using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.InteropServices;
using Xvue.Framework.API.DataModels.Base;
using Xvue.Framework.API.ViewModels.Base;
using Xvue.Framework.API.Services.Log;
using Xvue.Framework.API.Commands;

namespace Xvue.Framework.API.ViewModels.Base
{


    [ComVisibleAttribute(false)]
    public class ViewModelPluginInstances : ViewModelBase
    {
        readonly ObservableCollection<ViewModelPlugin> _collection = new ObservableCollection<ViewModelPlugin>();

        public ObservableCollection<ViewModelPlugin> Collection
        {
            get { return _collection; }
        }
    }
    [ComVisibleAttribute(false)]
    public abstract class ViewModelSerializedPlugin : ViewModelPlugin
    {
        public void InitializePlugin(string description, DataModelCatalogItem dm, ViewModelPlugin parent)
        {
            if (dm.ItemDataModel == null)
                dm.CreateDataModelFromXml();
            DMCatalogItem = dm;
            base.InitializePlugin(description, dm.ItemDataModel, parent);
        }
        public void PrepareForSave()
        {
            DMCatalogItem.ReloadDataConfiguration(DataModel);
        }
        public void ReloadCatalogDataModel(DataModelCatalogItem dm)
        {
            dm.CreateDataModelFromXml();
            DMCatalogItem = dm;
            ReloadDataModel(dm.ItemDataModel);
        }
        DataModelCatalogItem _dMCatalogItem;

        public DataModelCatalogItem DMCatalogItem
        {
            get { return _dMCatalogItem; }
            private set
            {
                _dMCatalogItem = value;
                OnPropertyChanged("DMCatalogItem");
            }
        }
    }

    [ComVisibleAttribute(false)]
    public abstract class ViewModelPlugin : ViewModelBase
    {
        ViewModelPlugin _parent;
 
        ViewModelSystemBase _service;
//        ICoreServiceBootStrapper _bootstrapper;
        ICoreServiceLog _log;
        readonly ViewModelPluginInstances _plugins = new ViewModelPluginInstances();
        DataModelPluginConfiguration _dataModel;
        public void InitializePlugin(string description, ViewModelPlugin parent)
        {
            InitializePlugin(description, new DataModelPluginConfiguration(), parent);
        }

        public void InitializePlugin(string description, DataModelPluginConfiguration dm, ViewModelPlugin parent)
        {
            if (!IsInitialized)
            {
                _parent = parent;
                //for speed
                if (parent != null)
                {
                    if (parent.Service != null)
                    {
                        _log = parent.Service.Log;
                        _service = parent.Service;
                    }
                }
                DisplayName = description;
                ReloadDataModel(dm);
                IsInitialized = true;
                //Apply catalogitem.items to load plugins
                AfterInitialize();
                //if (AutoLoad)
                //    CommandLoad.Execute(null);
            }
        }
        protected void ReloadDataModel(DataModelPluginConfiguration dm)
        {
            _dataModel = dm;
            SetSpeedDataModel(dm);
            OnPropertyChanged("DataModel");

        }
        /// <summary>
        /// Beware must be changed in future version to remove unwanted access to the hierarchy of plugins
        /// </summary>
        public ViewModelPlugin Parent
        {
            get { return _parent; }
        }

        public virtual ViewModelSystemBase Service
        {
            get { return _service; }
            //protected set { _service = value; }
        }

        public virtual ICoreServiceLog Log
        {
            get { return _log; }
        }

        public DataModelPluginConfiguration DataModel
        {
            get { return _dataModel; }
        }

        public ViewModelPluginInstances Plugins
        {
            get { return _plugins; }
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(DisplayName))
                return DisplayName;
            return base.ToString();
        }

        /// <summary>
        /// Sets the viewmodel's data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <remarks></remarks>
        protected virtual void SetSpeedDataModel(DataModelPluginConfiguration data){}
        /// <summary>
        /// After the initialization of the view model.
        /// </summary>
        /// <remarks></remarks>
        protected virtual void AfterInitialize(){}
        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual bool Load()
        {
            return true;
        }

        protected virtual bool Unload()
        {
            if (Service != null)
            {
                if (!this.Equals(Service))
                {
                    foreach (ViewModelPlugin plug in Plugins.Collection)
                    {
                        Service.Plugins.Collection.Remove(plug);
                        //plug.Dispose();
                        //if (!this.Equals(Service))
                        //    Service.Plugins.Remove(plug);
                    }
                }
            }
            //foreach (ViewModelPlugin plug in Plugins.Collection )
            //{
            //    plug.CommandUnload.Execute(null);
            //}            
            if (Service.UIDispatcher.CheckAccess())
                Plugins.Collection.Clear();
            else //we are not the UI thread so use Invoke
            {
                Service.UIDispatcher.Invoke(new Action(() =>
                {
                    Plugins.Collection.Clear();
                }));
            }            
            //here I must not clear info plugins
            //I must refill catalog
            return true;
        }

        protected virtual bool CanLoad()
        {
            return true;
        }
        bool _isLoaded;
        private bool _isInitialized = false;

        public bool IsInitialized
        {
            get { return _isInitialized; }
            protected set
            {
                _isInitialized = value;
                OnPropertyChanged("IsInitialized");
            }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            private set
            {
                if (_isLoaded != value)
                {
                    if (value)
                    {
                        if (_canEnable())
                        {
                            try
                            {
                                if (Load())
                                    _isLoaded = value;
                            }
                            catch (Exception ex)
                            {
                                Log.AddEventLog(0, "Trying to load:" + this.ToString(), ex.Message);
                            }
                            //else error
                        }
                    }
                    else
                    {
                        if (Unload())
                            _isLoaded = value;
                        //else error

                    }
                    OnPropertyChanged("IsLoaded");
                    OnPropertyChanged("IsUnloaded");
                    OnPropertyChanged("AllowLoadUnload");
                }
            }
        }

        public bool IsUnloaded
        {
            get { return !IsLoaded; }
        }
        DelegateCommand commandUnload;
        public ICommand CommandUnload
        {
            get
            {
                if (commandUnload == null)
                {
                    commandUnload = new DelegateCommand(_disable, _canDisable);
                }
                return commandUnload;
            }
        }

        DelegateCommand commandLoad;
        public ICommand CommandLoad
        {
            get
            {
                if (commandLoad == null)
                {
                    commandLoad = new DelegateCommand(_enable, _canEnable);
                }
                return commandLoad;
            }
        }

        void _disable()
        {
            IsLoaded = false;
        }

        bool _canDisable()
        {
            return IsLoaded;
        }

        void _enable()
        {
            IsLoaded = true;
        }

        public bool AllowLoadUnload
        {
            get
            {
                if (IsLoaded)
                    return _canDisable();
                else
                    return _canEnable();
            }
        }

        bool _canEnable()
        {
            if (!_isInitialized)
                return false;
            if (IsLoaded)
                return false;
            return CanLoad();
        }
    }
}
