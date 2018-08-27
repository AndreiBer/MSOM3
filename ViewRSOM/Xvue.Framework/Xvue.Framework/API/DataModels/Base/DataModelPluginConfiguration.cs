using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;
namespace Xvue.Framework.API.DataModels.Base
{

    public abstract class DataModelObject
    {

        private DataModelObject CloneDataModel()
        {
            DataModelObject newItem = (DataModelObject)this.MemberwiseClone();
            newItem.HandleReferenceTypesCloning(this);
            return newItem;
        }

        /// <summary>
        /// This needs to be overriden and implement reference types deep clone,
        /// when such types exist on the child.
        /// </summary>
        /// <param name="sourceDM">The object is cloning from.</param>
        protected virtual void HandleReferenceTypesCloning(DataModelObject sourceDM)
        {
            //Nothing particular in the base class, but maybe usefull for childs.
            //Not abstract so childs may not implement this if they don't need to.
        }

        /// <summary>
        /// Deep copy current object
        /// </summary>
        /// <returns>A copied object T : DataModelObject reference independent</returns>
        public T DeepCopy<T>() where T : DataModelObject
        {
            return DeepCopy(this as T);
        }

        /// <summary>
        /// Deep copy the given object
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A copied object T : DataModelObject reference independent</returns>
        public static T DeepCopy<T>(T obj) where T : DataModelObject
        {
            DataModelObject retValue = obj.CloneDataModel();
            return (T)retValue;
        }

        /// <summary>
        /// Compares as strings.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if equals, <c>false</c> otherwise.</returns>
        public bool CompareAsStrings(DataModelObject obj)
        {
            bool retValue = false;
            System.IO.StringWriter textWriter1 = null;
            System.IO.StringWriter textWriter2 = null;
            try
            {
                System.Xml.Serialization.XmlSerializer xmlSer = new System.Xml.Serialization.XmlSerializer(obj.GetType());

                textWriter1 = new System.IO.StringWriter();
                xmlSer.Serialize(textWriter1, this);
                string thisString = textWriter1.ToString();

                textWriter2 = new System.IO.StringWriter();
                xmlSer.Serialize(textWriter2, obj);
                string objString = textWriter2.ToString();

                if (thisString == objString)
                    retValue = true;
            }
            catch { }
            finally
            {
                if (textWriter1 != null)
                    textWriter1.Close();
                if (textWriter2 != null)
                    textWriter2.Close();
            }
            return retValue;
        }

    }

/// <summary>
/// This class is the base for all plugin configurations. Contains a collection of Items for 
/// configuring internals of the plugin if needed.
/// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DataModelPluginConfiguration : DataModelObject
    {
        public override string ToString()
        {
            return "Xvue Framework Default Plugin Configuration" ;
        }

    }

}
