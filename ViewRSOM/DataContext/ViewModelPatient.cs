using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;

namespace ViewRSOM
{

    public delegate bool CheckDuplicateIdEventHandler(ViewModelPatient patient);

    public class ViewModelPatient : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string argument)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(argument));
            }
        }
        public ViewModelPatient()
        {
            DMMS = new DataModelPatient();
        }

        public event EventHandler DiscardNewPatient;
        void OnDiscardNewPatient()
        {
            DiscardNewPatient.Invoke(this,EventArgs.Empty);
        }

        public event CheckDuplicateIdEventHandler CheckDuplicateId;
        bool OnCheckDuplicateId(ViewModelPatient patient)
        {
            CheckDuplicateIdEventHandler handler = CheckDuplicateId;
            if (handler != null)
            {
                return handler(patient);
            }
            else
            {
                return false;
            }
        }


        public event EventHandler NewPatientSaved;
        void OnNewPatientSaved()
        {
            NewPatientSaved.Invoke(this, EventArgs.Empty);
        }

        DataModelPatient DMMS;
        DataModelPatient previousDMMS;
        private bool _isNewPatient;
        private bool _isDuplicateId;

        protected void Load()
        {
            
        }

        protected void Unload()
        {
            
        }

        protected void AfterInitialize()
        {
            
            _isNewPatient = false;
        }

       

        public string PatientFilePath { get { return DMMS.PatientFilePath; } }

        public string FriendlyName
        {
            get
            {
                return DMMS.FriendlyName;
            }
            set
            {
                if (value != DMMS.FriendlyName)
                {
                    copyToPreviousDMMS();
                    DMMS.FriendlyName = value;
                    OnPropertyChanged("FriendlyName");
                }
            }
        }

        public DateTime CreationTime
        {
            get
            {
                return DMMS.CreationTime;
            }
            set
            {
                if (value != DMMS.CreationTime)
                {
                    copyToPreviousDMMS();
                    DMMS.CreationTime = value;
                    OnPropertyChanged("CreationTime");
                }
            }
        }

        public string Comment
        {
            get
            {
                return DMMS.Comment;
            }
            set
            {
                if (value != DMMS.Comment)
                {
                    copyToPreviousDMMS();
                    DMMS.Comment = value;
                    //commentChanged();
                    OnPropertyChanged("Comment");
                }
            }
        }

        public string FirstName
        {
            get
            {
                return DMMS.FirstName;
            }
            set
            {
                if (value != DMMS.FirstName)
                {
                    copyToPreviousDMMS();
                    DMMS.FirstName = value;
                    OnPropertyChanged("FirstName");
                    OnPropertyChanged("FullName");
                    confirmPropertiesChanged();
                }
            }
        }

        public string LastName
        {
            get
            {
                return DMMS.LastName;
            }
            set
            {
                if (value != DMMS.LastName)
                {
                    copyToPreviousDMMS();
                    DMMS.LastName = value;
                    OnPropertyChanged("LastName");
                    OnPropertyChanged("FullName");
                    confirmPropertiesChanged();
                }
            }
        }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public string Id
        {
            get
            {
                return DMMS.Id;
            }
            set
            {
                if (value != DMMS.Id)
                {
                    copyToPreviousDMMS();
                    DMMS.Id = value;
                    OnPropertyChanged("Id");
                    RefreshIsDuplicateId();
                    confirmPropertiesChanged();
                }
            }
        }

        public void RefreshIsDuplicateId()
        {
            IsDuplicateId = false;
            IsDuplicateId = OnCheckDuplicateId(this);
        }

        public bool IsDuplicateId
        {
            get
            {
                return _isDuplicateId;
            }
            private set
            {
                if (value != _isDuplicateId)
                {
                    _isDuplicateId = value;
                    OnPropertyChanged("IsDuplicateId");
                }
            }
        }

        public DateTime? BirthDate
        {
            get
            {
                return DMMS.BirthDate;
            }
            set
            {
                if (value != null && value > DateTime.Now)
                {
                    OnPropertyChanged("BirthDate");
                    OnPropertyChanged("BirthDateAsString");
                    OnPropertyChanged("BirthDateAsStringInBrackets");
                }
                else if (value != DMMS.BirthDate)
                {
                    copyToPreviousDMMS();
                    DMMS.BirthDate = value;
                    OnPropertyChanged("BirthDate");
                    OnPropertyChanged("BirthDateAsString");
                    OnPropertyChanged("BirthDateAsStringInBrackets");
                    confirmPropertiesChanged();
                }
            }
        }

        public string BirthDateAsString
        {
            get
            {
                DateTime bd;
                if (BirthDate != null)
                {
                    bd = (DateTime)BirthDate;
                    return bd.ToString("d", System.Globalization.CultureInfo.CurrentCulture);
                }
                else
                {
                    return "";
                }
            }
        }

        public string BirthDateAsStringInBrackets
        {
            get
            {
                return "(" + BirthDateAsString + ")";
            }
        }

        public PatientSex Sex
        {
            get
            {
                return DMMS.Sex;
            }
            set
            {
                if (value != DMMS.Sex)
                {
                    copyToPreviousDMMS();
                    DMMS.Sex = value;
                    OnPropertyChanged("Sex");
                    OnPropertyChanged("SexFriendlyName");
                    confirmPropertiesChanged();
                }
            }
        }
        
        
        public string SexFriendlyName
        {
            get
            {
                return "What?";
            }
        }

        public DateTime LastExam
        {
            get
            {
                return DMMS.LastExam;
            }
            set
            {
                if (value != DMMS.LastExam)
                {
                    DMMS.LastExam = value;
                    saveChangesToPatientFilePath(DMMS);
                    OnPropertyChanged("LastExam");
                    OnPropertyChanged("LastExamAsString");
                }
            }
        }

        public string LastExamAsString
        {
            get
            {
                if (DMMS.LastExam == DateTime.MinValue)
                    return "-";
                else
                    return DMMS.LastExam.ToString("g", CultureInfo.CurrentCulture);
            }
        }

        
        
        public bool HasPendingChanges
        {
            get
            {
                try
                {
                    if (previousDMMS != null)
                    {
                        if (!previousDMMS.CompareAsStrings(DMMS))
                            return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    
                    previousDMMS = null;
                    return false;
                }
            }
        }

        public bool IsNewPatient
        {
            get
            {
                return _isNewPatient;
            }
            set
            {
                if (value != _isNewPatient)
                {
                    _isNewPatient = value;
                    OnPropertyChanged("IsNewPatient");
                }
            }
        }

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DMMS.FirstName))
                    return false;
                if (string.IsNullOrWhiteSpace(DMMS.LastName))
                    return false;
                if (string.IsNullOrWhiteSpace(DMMS.Id))
                    return false;
                if (IsDuplicateId)
                    return false;
                if (DMMS.BirthDate == null)
                    return false;
                if (DMMS.BirthDate > DateTime.Now)
                    return false;
                if (DMMS.Sex == PatientSex.N)
                    return false;
                return true;
            }
        }

        private void confirmPropertiesChanged()
        {
            OnPropertyChanged("IsValid");
            if (IsValid && IsNewPatient)
            {
                saveChanges();
                OnNewPatientSaved();
            }
            OnPropertyChanged("HasPendingChanges");
        }

        private void copyToPreviousDMMS()
        {
            try
            {
                if (previousDMMS == null) { }
                    //previousDMMS = DMMS.DeepCopy<DataModelPatient>();
                System.Windows.MessageBox.Show("Now data from previous patient should be copied");
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Warning, DisplayName, "Exception saving patient changes: " + ex.Message);
            }
        }

        private void saveChangesToPatientFilePath(DataModelPatient dmms)
        {
            try
            {
//!!!                //DataModelSerialization.SaveToPath(dmms, dmms.PatientFilePath, Log);
                System.Windows.MessageBox.Show("Now you should save the data");
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(Services.Log.EnumLogType.Warning, DisplayName, "Exception saving patient changes: " + ex.Message);
            }
        }

        private void saveChanges()
        {
            saveChangesToPatientFilePath(DMMS);
            previousDMMS = null;
            if (IsNewPatient)
            {
                IsNewPatient = false;
            }
        }

        private void discardChanges()
        {
            if (!IsNewPatient)
            {
                if (previousDMMS != null)
                {
//                    DMMS = previousDMMS.DeepCopy<DataModelPatient>();
                    previousDMMS = null;
                }
                RefreshIsDuplicateId();
                dataModelCompleteRefreshed();
                OnPropertyChanged("IsValid");
            }
            else
            {
                OnDiscardNewPatient();
            }
        }

        private void dataModelCompleteRefreshed()
        {
            OnPropertyChanged("FriendlyName");
            OnPropertyChanged("CreationTime");
            OnPropertyChanged("Comment");
            OnPropertyChanged("FirstName");
            OnPropertyChanged("LastName");
            OnPropertyChanged("Id");
            OnPropertyChanged("BirthDate");
            OnPropertyChanged("BirthDateAsString");
            OnPropertyChanged("BirthDateAsStringInBrackets");
            OnPropertyChanged("Sex");
            OnPropertyChanged("LastExam");
            OnPropertyChanged("LastExamAsString");
            OnPropertyChanged("PatientFilePath");
            OnPropertyChanged("FullName");
        }

        private void finishEdit()
        {
            previousDMMS = null;
        }

/*
        DelegateCommand discardAnyPendingChangesCommand;
        public ICommand DiscardAnyPendingChangesCommand
        {
            get
            {
                if (discardAnyPendingChangesCommand == null)
                {
                    discardAnyPendingChangesCommand = new DelegateCommand(discardChanges);
                }
                return discardAnyPendingChangesCommand;
            }
        }

        DelegateCommand verifyPendingChangesCommand;
        public ICommand VerifyPendingChangesCommand
        {
            get
            {
                if (verifyPendingChangesCommand == null)
                {
                    verifyPendingChangesCommand = new DelegateCommand(verifyPendingChanges, canVerifyPendingChanges);
                }
                return verifyPendingChangesCommand;
            }
        }

        private bool canVerifyPendingChanges()
        {
            if (IsValid)
                return true;
            else
                return false;
        }

        private void verifyPendingChanges()
        {
            try
            {
                if (HasPendingChanges)
                {
                    if (MSOTService.VerifyUserAction("Save pending patient data?", "Save Changes"))
                        saveChanges();
                    else
                        discardChanges();
                }
                else
                    finishEdit();
            }
            catch (Exception ex)
            {
                MSOTService.IMSOTLog.HandleError(DisplayName, "Exception checking out pending changes: " + ex.Message);
            }
        }

        public bool VerifyCancelPendingChanges()
        {
            bool retValue = false;
            try
            {
                if (HasPendingChanges)
                {
                    bool? decision = MSOTService.VerifyUserAction("Save pending patient data?", "Save Changes", "Save", "Don't Save", true);
                    if (decision == null)
                        retValue = true;
                    else if (decision == true)
                        saveChanges();
                    else if (decision == false)
                        discardChanges();
                }
                else
                    finishEdit();

                return retValue;
            }
            catch (Exception ex)
            {
                MSOTService.IMSOTLog.HandleError(DisplayName, "Exception checking out with cancel pending changes: " + ex.Message);
                return retValue;
            }
        }

        public void SaveChanges()
        {
            try
            {
                if (HasPendingChanges)
                {
                    saveChanges();
                }
            }
            catch (Exception ex)
            {
                MSOTService.IMSOTLog.HandleError(DisplayName, "Exception save changes: " + ex.Message);
            }
        }

        private bool _isEditEnabled = false;
        public bool IsEditEnabled
        {
            get
            {
                return _isEditEnabled;
            }
            set
            {
                if (value != _isEditEnabled)
                {
                    _isEditEnabled = value;
                    OnPropertyChanged("IsEditEnabled");
                }
            }
        }

        public void StartEdit()
        {
            IsEditEnabled = true;
        }

        DelegateCommand commandStartEdit;
        public ICommand CommandStartEdit
        {
            get
            {
                if (commandStartEdit == null)
                {
                    commandStartEdit = new DelegateCommand(StartEdit, canStartEdit);
                }
                return commandStartEdit;
            }
        }
        bool canStartEdit()
        {
            return !IsEditEnabled;
        }

        bool canEndEdit()
        {
            return IsEditEnabled;
        }

        public void DiscardPendingChanges()
        {
            discardChanges();
            IsEditEnabled = false;
        }

        public void SavePendingChanges()
        {
            saveChanges();
            IsEditEnabled = false;
        }

        DelegateCommand commandSavePendingChanges;
        public ICommand CommandSavePendingChanges
        {
            get
            {
                if (commandSavePendingChanges == null)
                {
                    commandSavePendingChanges = new DelegateCommand(SavePendingChanges, canEndEdit);
                }
                return commandSavePendingChanges;
            }
        }

        DelegateCommand commandDiscardPendingChanges;
        public ICommand CommandDiscardPendingChanges
        {
            get
            {
                if (commandDiscardPendingChanges == null)
                {
                    commandDiscardPendingChanges = new DelegateCommand(DiscardPendingChanges, canEndEdit);
                }
                return commandDiscardPendingChanges;
            }
        }

 */
    }
}
