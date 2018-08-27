using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Xvue.Framework.API.Services.Log;
using Xvue.Framework.API.DataModels.Base;
namespace Xvue.Framework.API.Serialization
{
    [ComVisibleAttribute(false)]
    public static class DataModelSerialization
    {

        public static T Clone<T>(T realObject)
        {
            //lock (allsertypes)
            {
                using (Stream objectStream = new MemoryStream())
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(objectStream, realObject);
                    objectStream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(objectStream);
                }
            }
        }

        public static T LoadBinaryFromPath<T>(string path, ICoreServiceLog xvLog)
        {
            T xvobject = default(T);
            //lock (allsertypes)
            {
                FileStream myFileStream = null;
                try
                {
                    myFileStream = new FileStream(path, FileMode.Open);
                    try
                    {
                        BinaryFormatter bformatter = new BinaryFormatter();
                        // if (ser.CanDeserialize(read))
                        {

                            xvobject = (T)bformatter.Deserialize(myFileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (xvLog != null)
                            xvLog.AddEventLog(0, "Exception1 in Load Object:", ex.Message);
                    }
                    finally
                    {
                        if(myFileStream!=null)
                            myFileStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    if (xvLog != null)
                        xvLog.AddEventLog(0, "Exception2 in Load Object:", ex.Message);
                }
            }
            return xvobject;
        }


        public static bool SaveBinaryToPath<T>(T xvobject, string path, ICoreServiceLog xvLog)
        {
            bool returnval = true;
            try
            {
                Stream stream = null;
                try
                {
                    stream = File.Open(path, FileMode.Create);
                    BinaryFormatter bformatter = new BinaryFormatter();
                    //Console.WriteLine("Writing Employee Information");
                    bformatter.Serialize(stream, xvobject);
                }
                catch (Exception ex)
                {
                    returnval = false;
                    if (xvLog != null)
                        xvLog.AddEventLog(0, "Exception1 in Binary Save Object:", ex.Message);
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            catch (Exception ex)
            {
                returnval = false;
                if (xvLog != null)
                    xvLog.AddEventLog(0, "Exception2 in Binary Save Object:", ex.Message);
            }
            return returnval;
        }


        public static bool SaveToPath<T>(T xvobject, string path, ICoreServiceLog xvLog, XmlSerializerNamespaces namespaces)
        {
            return saveToPath(xvobject, path, null, xvLog, namespaces);
        }

        public static bool SaveToPath<T>(T xvobject, string path, ICoreServiceLog xvLog)
        {
            return saveToPath(xvobject, path, null, xvLog, null);
        }

        public static bool SaveToPath<T>(T xvobject, FileStream pathFileStream, bool freeFileStreamHandle, ICoreServiceLog xvLog)
        {
            bool returnValue = saveToPath(xvobject, null, pathFileStream, xvLog, null);
            if (freeFileStreamHandle)
            {
                pathFileStream.Dispose();
                pathFileStream = null;
            }
            return returnValue;
        }

        readonly static string[] SaveToPathStageDesc = { "Create path text writer stream", "Set path file stream length", "Serialize" };

        static bool saveToPath<T>(T xvobject, string path, FileStream pathFileStream, ICoreServiceLog xvLog, XmlSerializerNamespaces namespaces)
        {
            bool returnValue = true;
            //lock (allsertypes)
            {
                TextWriter textWriterStream = null;
                TextWriter tempTextWriterStream = null;
                int operationDescIndex = 0;
                try
                {
                    if (pathFileStream == null)
                    {
                        tempTextWriterStream = new StreamWriter(path);
                        textWriterStream = tempTextWriterStream;
                    }
                    else
                    {
                        operationDescIndex = 1;
                        pathFileStream.SetLength(0);
                        operationDescIndex = 0;
                        textWriterStream = new StreamWriter(pathFileStream);
                    }
                    //FileStream myFileStream = new FileStream(path, FileMode.Open);
                    operationDescIndex = 2;
                    XmlSerializer ser = new XmlSerializer(typeof(T));
                    //StreamReader read = new StreamReader(file.FileName);
                    // if (ser.CanDeserialize(read))
                    {
                        if(namespaces!=null)
                            ser.Serialize(textWriterStream, xvobject, namespaces);
                        else
                            ser.Serialize(textWriterStream, xvobject);
                        textWriterStream.Flush();
                    }
                    //                xvLastOpenedFilename = file.FileName;
                }
                catch (Exception ex)
                {
                    returnValue = false;
                    xvLog?.AddEventLog(0, $"Exception during '{SaveToPathStageDesc[operationDescIndex]}'  in {path}:", ex.Message);
                }
                finally
                {
                    tempTextWriterStream?.Close();
                }
            }
            return returnValue;
        }

        public static T LoadFromPath<T>(string path, ICoreServiceLog xvLog)
        {
            string errorMessage;
            FileStream pathFileStream;
            bool shareViolation;
            return loadFromPath<T>( path, out pathFileStream, false, out errorMessage, out shareViolation, xvLog);
        }

        public static T LoadFromPath<T>(string path, out FileStream pathFileStream, out string errorMessage, out bool shareViolation, ICoreServiceLog xvLog)
        {
            return loadFromPath<T>(path, out pathFileStream, true, out errorMessage, out shareViolation, xvLog);
        }


        static readonly string[] LoadFromPathStageDesc = { "Create path file stream", "Create path text reader stream", "Deserialize" };

        static T loadFromPath<T>(string path, out FileStream pathFileStream, bool createNewFileStream, out string errorMessage, out bool shareViolation, ICoreServiceLog xvLog)
        {
            errorMessage = "";
            shareViolation = false;
            T xvobject = default(T);
            //lock (allsertypes)
            {
                TextReader textReaderStream = null;
                TextReader tempTextReaderStream = null;
                int operationDescIndex = 0;
                try
                {
                    
                    if (createNewFileStream)
                    {
                        pathFileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                        operationDescIndex = 1;
                        textReaderStream = new StreamReader(pathFileStream);
                    }
                    else
                    {
                        pathFileStream = null;
                        operationDescIndex = 1;
                        tempTextReaderStream = new StreamReader(path);
                        textReaderStream = tempTextReaderStream;
                    }
                    //FileStream myFileStream = new FileStream(path, FileMode.Open);
                    operationDescIndex = 2;
                    XmlSerializer ser = new XmlSerializer(typeof(T));
                    //StreamReader read = new StreamReader(file.FileName);
                    // if (ser.CanDeserialize(read))
                    {
                        xvobject = (T)ser.Deserialize(textReaderStream);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType().IsSubclassOf(typeof(IOException)))
                    {
                        if(ex is DirectoryNotFoundException || ex is FileNotFoundException)
                            shareViolation = false;
                        else
                            shareViolation = true;
                    }

                    xvLog?.AddEventLog(0, $"Exception during '{LoadFromPathStageDesc[operationDescIndex]}' in {path}:", ex.Message);
                    errorMessage = ex.Message;
                    pathFileStream = null;
                }
                finally
                {
                    tempTextReaderStream?.Close();
                }
            }
            return xvobject;
        }

        public static FileStream CreateFileStream(string path, out string errorMessage, ICoreServiceLog xvLog)
        {
            try
            {
                errorMessage = "";
                return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
            catch (Exception ex)
            {
                if (xvLog != null)
                    xvLog.AddEventLog(0, "Exception in creating file stream handle:", ex.Message);
                errorMessage = ex.Message;
                return null;
            }
        }
    }
}
