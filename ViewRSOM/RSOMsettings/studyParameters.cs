using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ViewRSOM
{
    public static class studyParameters
    {
        public static event PropertyChangedEventHandler PropertyChanged;

        private static void Notify(string argument)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(null, new PropertyChangedEventArgs(argument));
            }
        }

        // public file extensions
        public static string StudyFileExtension { get { return ".nod"; } }
        public static string PatientFileNameExtension { get { return ".ptm"; } }

        // private 
        private static string _actualPatientId = "0";
        private static string _actualPatientFirstName = "";
        private static string _actualPatientLastName = "";
        private static DateTime _actualPatientBirthDate;
        private static string _birthDateAsString = "";
        private static int _actualPatientListIndex=0;

        public static List<studyDateItem> myStudyDates_list = new List<studyDateItem>();
        private static int _myStudyDates_listIndex;

        public static int myStudyDates_listIndex
        {
            get { return _myStudyDates_listIndex; }
            set
            {
                _myStudyDates_listIndex = value;
                Notify("myStudyDates_listIndex");
            }
        }

        public static int highestCurrentStudyId = 0;

        public static string ActualPatientId
        {
            get { return _actualPatientId; }
            set
            {
                _actualPatientId = value;
                Notify("ActualPatientId");
            }
        }

        public static string ActualPatientFirstName
        {
            get { return _actualPatientFirstName; }
            set
            {
                _actualPatientFirstName = value;
                Notify("ActualPatientFirstName");
            }
        }

        public static string ActualPatientLastName
        {
            get { return _actualPatientLastName; }
            set
            {
                _actualPatientLastName = value;
                Notify("ActualPatientLastName");
            }
        }

        public static DateTime ActualPatientBirthDate
        {
            get { return _actualPatientBirthDate; }
            set
            {
                _actualPatientBirthDate = value;
                _birthDateAsString = value.ToString("");
                Notify("ActualPatientBirthDate");
            }
        }

        public static string BirthDateAsString
        {
            get { return _birthDateAsString; }
            set
            {
                _actualPatientLastName = value;
                Notify("BirthDateAsString");
            }
        }

        public static int ActualPatientListIndex        {
            get { return _actualPatientListIndex; }
            set
            {
                _actualPatientListIndex = value;
                Notify("ActualPatientListIndex");
            }
        }

        public static string Operator
        {
            get{return Environment.UserName; }
            set{}
        }
    }
}
