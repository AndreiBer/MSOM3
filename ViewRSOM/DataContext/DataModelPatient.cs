using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace ViewRSOM
{
    
    
    
    public enum PatientSex : int
    {
        [DisplayString("Female")]
        F = 0,
        [DisplayString("Male")]
        M,
        [DisplayString("Other")]
        O,
        [DisplayString("")]
        N
    }

    [Serializable]
    public class DataModelPatient : INotifyPropertyChanged
    {

        public bool CompareAsStrings(DataModelPatient obj)
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

        public event PropertyChangedEventHandler PropertyChanged;
       
        private void Notify(string argument)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(argument));
            }
        }
        
        public DataModelPatient()
        {
            CreationTime = DateTime.Now;
            BirthDate = null;
            Comment = "";
            FirstName = "";
            LastName = "";
            FriendlyName = "";
            Sex = PatientSex.N;
            LastExam = DateTime.MinValue;
            Id = "";
        }

        public DataModelPatient(string patientFilePath)
            : this()
        {
            PatientFilePath = patientFilePath;
        }

        public string FriendlyName { get; set; }

        public DateTime CreationTime { get; set; }

        public string Comment { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Id { get; set; }

        public DateTime? BirthDate { get; set; }

        public PatientSex Sex { get; set; }

        public DateTime LastExam { get; set; }

        [XmlIgnore]
        public string PatientFilePath { get; set; }

    }
    
}

