using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ViewRSOM.Framework.API.DataModels.Base
{
/// <summary>
/// This class is the base for all plugin configurations. Contains a collection of Items for 
/// configuring internals of the plugin if needed.
/// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DataModelPluginConfiguration
    {
        public override string ToString()
        {
            return "Xvue Framework Default Plugin Configuration" ;
        }

        /// <summary>
        /// Compares as strings.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if equals, <c>false</c> otherwise.</returns>
        public bool CompareAsStrings(DataModelPluginConfiguration obj)
        {
            bool retValue = false;
            try
            {
                System.Xml.Serialization.XmlSerializer xmlSer = new System.Xml.Serialization.XmlSerializer(obj.GetType());

                System.IO.StringWriter textWri = new System.IO.StringWriter();
                xmlSer.Serialize(textWri, this);
                string thisString = textWri.ToString();

                textWri = new System.IO.StringWriter();
                xmlSer.Serialize(textWri, obj);
                string objString = textWri.ToString();

                if (thisString == objString)
                    retValue = true;
            }
            catch { }
            return retValue;
        }

        //Collection<Object> _items = new Collection<Object>();

        //public Collection<Object> Items
        //{
        //    get { return _items; }
        //    set { _items = value; }
        //}

        //DataModelPluginConfigurationDynamicBase _dynamicDataModel;

        //public DataModelPluginConfigurationDynamicBase DynamicDataModel
        //{
        //    get { return _dynamicDataModel; }
        //    set { _dynamicDataModel = value; }
        //}
    }
    //[Serializable]
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    //public abstract class DataModelPluginConfigurationDynamicBase
    //{

    //}
}
