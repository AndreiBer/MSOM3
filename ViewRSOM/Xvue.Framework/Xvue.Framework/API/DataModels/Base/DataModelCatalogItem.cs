using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Xml;

namespace Xvue.Framework.API.DataModels.Base
{

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class DataModelCatalogItemBase
    { 
    
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class DataModelCatalogItem : DataModelCatalogItemBase//:DataModelPluginConfiguration  
    {
        #region localvariables
        ulong _iD = 0;
        ulong _addToTableID = 0;
        bool _registerID = false;
        string _selector = "";
        string xmlItemDataModel = "";
        string xmlMainType = "";
        string _serializationError = "";
        DataModelPluginConfiguration _itemDataModel;
        readonly Collection<DataModelCatalogItem> _items;
        #endregion localvariables

        public ulong ID
        {
            get { return _iD; }
            set { _iD = value; }
        }
        
        public ulong AddToTableID
        {
            get { return _addToTableID; }
            set { _addToTableID = value; }
        }

        public bool RegisterID
        {
            get { return _registerID; }
            set { _registerID = value; }
        }

        public Collection<DataModelCatalogItem> Items
        {
            get { return _items; }
        }

        public string Selector
        {
            get { return _selector; }
            set { _selector = value; }
        }

        public string XMLItemDataModel
        {
            get
            {
                return xmlItemDataModel;
            }
            set //set is called via a templated call - so there is a reference to this propety!
            {
                xmlItemDataModel = value;
            }
        }

        public string XmlMainType
        {
            get
            {
                return xmlMainType;
            }
            set
            {
                xmlMainType = value;
            }
        }

        public string SerializationError
        {
            get { return _serializationError; }
        }

        //this must be called before saving to disk
        private void MakeLatestXml()
        {
            if (_itemDataModel == null)
                return;

            StringWriter stringWriter = null;
            XmlWriter xmlWriter = null;
            string stageDesc = "(3)";
            try
            {
                xmlMainType = _itemDataModel.GetType().AssemblyQualifiedName;
                //xmlItemDataModel = "";
                _serializationError = "";
                stringWriter = new StringWriter();
                stageDesc = "(2)";
                xmlWriter = XmlWriter.Create(stringWriter);
                stageDesc = "(1)";
                XmlSerializer ser = new XmlSerializer(_itemDataModel.GetType());//, allsertypes);
                ser.Serialize(xmlWriter, _itemDataModel);                                
                _serializationError = stringWriter.ToString();                                
                xmlWriter.Flush();
                xmlItemDataModel = _serializationError;
                _serializationError = "XML OK";
            }
            catch (Exception ex)
            {
                _serializationError = stageDesc + ex.Message;
                if (ex.InnerException != null)
                    _serializationError += "  Inner: " + ex.InnerException.Message;
            }
            finally
            {
                if (xmlWriter != null)
                    xmlWriter.Close();
                else
                    stringWriter?.Close();
            }
        }
        //this must be called only when we know that the dll is loaded and available
        //so it must be a hierarchical step by step procedure on a need to base and not
        //after load
        public void CreateDataModelFromXml()
        {
            if (!String.IsNullOrEmpty(xmlItemDataModel) && !String.IsNullOrEmpty(xmlMainType))
            {
                //itemDataModel = null;
                StringReader stringWriter = null;
                XmlReader xmlWriter = null;
                try
                {
                    stringWriter = new StringReader(xmlItemDataModel);
                    xmlWriter = XmlReader.Create(stringWriter);
                    DataModelPluginConfiguration obj = null;
                    XmlSerializer ser = new XmlSerializer(Type.GetType(xmlMainType));//, allsertypes);
                    obj = ser.Deserialize(xmlWriter) as DataModelPluginConfiguration;
                    _serializationError = "XML Read OK";
                    _itemDataModel = obj;
                }
                catch (Exception ex)
                {
                    _serializationError = ex.Message;
                }
                finally
                {
                    if(xmlWriter != null)
                        xmlWriter.Close();
                    else
                        stringWriter?.Close();
                }
            }
        }

        public DataModelPluginConfiguration ItemDataModel
        {
            get { return _itemDataModel; }
        }
        public void ReloadDataConfiguration(DataModelPluginConfiguration newData)
        {
            _itemDataModel = newData;
//#if DEBUG
            //this is called here for debug
            MakeLatestXml();
//#endif
        }

        //here we create catalogItem with empty data model
        public DataModelCatalogItem()
        {
            _items = new Collection<DataModelCatalogItem>();
        }

        //here I create catalogItem from explicit data model
        public DataModelCatalogItem(DataModelPluginConfiguration pluginDataModel,string selector=""):this()
        {
            Selector = selector;
            _itemDataModel = pluginDataModel;
        }
    }

}
