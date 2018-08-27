using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;


namespace ViewRSOM
{
    public class ViewModelPatientsManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string argument)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(argument));
            }
        }
    
        private const string _patientFileNameExtension = ".ptm";

        private ObservableCollection<ViewModelPatient> _patients;
        private ViewModelPatient _selectedPatient;
        private ViewModelPatient _activePatient;
        private string _operator;
        private string _previousOperator;

        protected void AfterInitialize()
        {
            _patients = new ObservableCollection<ViewModelPatient>();
            _selectedPatient = null;
            _activePatient = null;
            _operator = Environment.UserName;
            _previousOperator = "";
 //           base.AfterInitialize();
            System.Windows.MessageBox.Show("Now dafterinitialize is called");
        }

        public void Initiate(string rootDirectory)
        {
            try
            {
                if (!Directory.Exists(rootDirectory))
                {
                    System.Windows.MessageBox.Show("Couldn't load patient list.", "Directory doesn't exist: " + rootDirectory);
                    return;
                }

                string[] patientsPaths = Directory.GetDirectories(rootDirectory);

                string patientMetaXMLPath;
                string patientStudyName;
                string studyNodePath;
                DataModelPatient patientDM;
                foreach (string path in patientsPaths)
                {
                    patientStudyName = Path.GetFileName(path);
                    patientMetaXMLPath = Path.Combine(path, patientStudyName + _patientFileNameExtension);

                    studyNodePath = Path.Combine(path, patientStudyName + studyParameters.StudyFileExtension);
                    if (!File.Exists(studyNodePath))
                        continue;

                    if (File.Exists(patientMetaXMLPath))
                    {
                        //patientDM = DataModelSerialization.LoadFromPath<DataModelPatient>(patientMetaXMLPath, Log);
                        //TODO Remove PatientSex.O enum
 //                       if (patientDM.Sex == PatientSex.O)
 //                       {
 //                           patientDM.Sex = PatientSex.M;
 ////                           DataModelSerialization.SaveToPath(patientDM, patientMetaXMLPath, Log);
 //                       }
 //                       patientDM.PatientFilePath = patientMetaXMLPath;
                       ViewModelPatient patientVM = new ViewModelPatient();
                        //patientVM.InitializePlugin(patientDM.FriendlyName, new DataModelCatalogItem(patientDM), this);
                        //patientVM.CommandLoad.Execute(null);
                        patientVM.CheckDuplicateId += patientVM_CheckDuplicateId;
                        _patients.Add(patientVM);
                    }
                    
                    //added for testing, will create a default patient file .ptm for every study found in RootDirectory
                    //else
                    //{
                    //    random rg = new random();
                    //    datamodelpatient mockpatientdm = new datamodelpatient()
                    //    {
                    //        birthdate = datetime.now,
                    //        comment = "automatically created patient meta file.",
                    //        creationtime = datetime.now,
                    //        firstname = "first " + patientstudyname,
                    //        lastname = "last " + patientstudyname,
                    //        friendlyname = "friendly " + patientstudyname,
                    //        sex = (patientsex)(rg.next(3)),
                    //        lastexam = datetime.minvalue,
                    //        id = patientstudyname,
                    //        patientfilepath = patientmetaxmlpath
                    //    };
                    //    datamodelserialization.savetopath(mockpatientdm, patientmetaxmlpath, log);
                    //    viewmodelpatient patientvm = new viewmodelpatient();
                    //    patientvm.initializeplugin(mockpatientdm.friendlyname, new datamodelcatalogitem(mockpatientdm), this);
                    //    patientvm.commandload.execute(null);
                    //    _patients.add(patientvm);
                    //}
                }
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(EnumLogType.Warning, DisplayName, "Exception initiating patients manager: " + ex.Message);
                System.Windows.MessageBox.Show("Exception initiating patients manager: " + ex.Message);
            }
        }

        public string Operator
        {
            get
            {
                return Environment.UserName; 
            }
            set
            {
                if (value != _operator)
                {
                    if (string.IsNullOrEmpty(_previousOperator))
                        _previousOperator = _operator;
                    _operator = value;
                    OnPropertyChanged("Operator");
                }
            }
        }

        public bool HasOperatorPendingChanges
        {
            get
            {
                if (!string.IsNullOrEmpty(_previousOperator))
                {
                    if (_previousOperator != _operator)
                        return true;
                }

                return false;
            }
        }

        private void saveOperatorPendingChanges()
        {
            _previousOperator = "";
 //           MSOTService.IMSOTHardware.IManualScan.NewScanScientistName = Operator;
        }

        private void discardOperatorPendingChanges()
        {
            _operator = _previousOperator;
            _previousOperator = "";
            OnPropertyChanged("Operator");
        }

        public bool VerifyCancelOperatorPendingChanges()
        {
            bool retValue = false;
            try
            {
                if (string.IsNullOrEmpty(Operator))
                {
                    if (!string.IsNullOrEmpty(_previousOperator))
                    {
                        discardOperatorPendingChanges();
                    }
                    else
                    {
                        _operator = Environment.UserName;
                        OnPropertyChanged("Operator");
                    }
                    return retValue;
                }

                //if (HasOperatorPendingChanges)
                //{
                //    bool? decision = MSOTService.VerifyUserAction("Save operator name?", "Save Changes", "Save", "Don't Save", true);
                //    if (decision == null)
                //        retValue = true;
                //    else if (decision == true)
                //        saveOperatorPendingChanges();
                //    else if (decision == false)
                //        discardOperatorPendingChanges();
                //}
                else
                    saveOperatorPendingChanges();

                return retValue;
            }
            catch (Exception ex)
            {
               // MSOTService.IMSOTLog.HandleError(DisplayName, "Exception checking out with cancel pending changes: " + ex.Message);
                System.Windows.MessageBox.Show("Exception checking out with cancel pending changes: " + ex.Message);
                return retValue;
            }
        }

        public ObservableCollection<ViewModelPatient> Patients
        {
            get
            {
                return _patients;
            }
        }

        public ViewModelPatient SelectedPatient
        {
            get
            {
                return _selectedPatient;
            }
            set
            {
                if (value != _selectedPatient)
                {
                    _selectedPatient = value;
                    OnPropertyChanged("SelectedPatient");
                }
            }
        }

        //DelegateCommand selectPatientCommand;
        //public ICommand SelectPatientCommand
        //{
        //    get
        //    {
        //        if (selectPatientCommand == null)
        //        {
        //            selectPatientCommand = new DelegateCommand(selectPatientEmpty, canSelectPatient);
        //        }
        //        return selectPatientCommand;
        //    }
        //}

        private bool canSelectPatient()
        {
            if (SelectedPatient == null)
                return false;
            else
            {
                if (SelectedPatient.IsNewPatient || !SelectedPatient.IsValid)// || ActivePatient == SelectedPatient)
                    return false;
                else
                    return true;
            }
        }

        public void SelectPatient()
        {
            try
            {
                if (ActivePatient == SelectedPatient)
                    return;

                ActivePatient = SelectedPatient;
                //bool success = MSOTService.IProjectManager.OpenPatientStudy(ActivePatient.PatientFilePath);
                //if (!success)
                //    ActivePatient = null;
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(DisplayName, "Exception opening selected patient: " + ex.Message);
                System.Windows.MessageBox.Show("Exception opening selected patient: " + ex.Message);
            }
        }

        private void selectPatientEmpty() { }

        public ViewModelPatient ActivePatient
        {
            get
            {
                return _activePatient;
            }
            set
            {
                if (value != _activePatient)
                {
                    _activePatient = value;
                    OnPropertyChanged("ActivePatient");
                }
            }
        }

        //DelegateCommand deletePatientCommand;
        //public ICommand DeletePatientCommand
        //{
        //    get
        //    {
        //        if (deletePatientCommand == null)
        //        {
        //            deletePatientCommand = new DelegateCommand(deleteSelectedPatientEmpty, canDeleteSelectedPatient);
        //        }
        //        return deletePatientCommand;
        //    }
        //}

        private bool canDeleteSelectedPatient()
        {
            if (SelectedPatient == null)
                return false;
            else
            {
                if (SelectedPatient.IsNewPatient)
                    return false;
                else if (!SelectedPatient.IsValid)
                    return false;
                else
                    return true;
            }
        }

        public void DeleteSelectedPatient()
        {
            try
            {
                //if (!MSOTService.VerifyUserAction("Are you sure you want to delete patient: '" + SelectedPatient.LastName + " " + SelectedPatient.FirstName + "'?", "Delete Patient", "Delete", "Cancel"))
                //    return;

                bool deleted = true;//MSOTService.IProjectManager.DeletePatientStudy(SelectedPatient.PatientFilePath);
                if (deleted)
                {
                    Patients.Remove(SelectedPatient);
                }
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(DisplayName, "Exception deleting selected patient: " + ex.Message);
                System.Windows.MessageBox.Show("Exception deleting selected patient: " + ex.Message);
            }
        }

        private void deleteSelectedPatientEmpty() { }

        //DelegateCommand newPatientCommand;
        //public ICommand NewPatientCommand
        //{
        //    get
        //    {
        //        if (newPatientCommand == null)
        //        {
        //            newPatientCommand = new DelegateCommand(newPatientEmpty, canNewPatient);
        //        }
        //        return newPatientCommand;
        //    }
        //}

        private bool canNewPatient()
        {
            if (SelectedPatient != null)
                if (SelectedPatient.IsNewPatient || !SelectedPatient.IsValid)
                    return false;
            return true;
        }

        public string CreateNewPatientStudy()
        {
            try
            {
                string FullPath;
                string newStudyName = "testStudy";
                string newStudyFullFolder = Path.Combine("D:\\", newStudyName);
                System.IO.Directory.CreateDirectory(newStudyFullFolder);
                //DataModelStudyNode newFile = new DataModelStudyNode();
                //newFile.Version = DataModelStudyNode.CurrentVersion;
                FullPath = Path.Combine(newStudyFullFolder, newStudyName + studyParameters.StudyFileExtension);
                //DataModelSerialization.SaveToPath(newFile, newFile.FullPath, Log);
                return FullPath;
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(DisplayName, "Exception creating new study: " + ex.Message);
                System.Windows.MessageBox.Show("Exception creating new study: " + ex.Message);
                return "";
            }
        }
        public void NewPatient()
        {
            try
            {
                string newPatientStudyFilePath;
                newPatientStudyFilePath = CreateNewPatientStudy();

                if (string.IsNullOrWhiteSpace(newPatientStudyFilePath))
                    return;

                string newPatientStudyName = Path.GetFileNameWithoutExtension(newPatientStudyFilePath);
                string studyPath = Path.GetDirectoryName(newPatientStudyFilePath);

                string patientFilePath = Path.Combine(studyPath, newPatientStudyName + _patientFileNameExtension);
                DataModelPatient patientDM = new DataModelPatient(patientFilePath);
                patientDM.Id = getNextAvailablePatientId();
                //DataModelSerialization.SaveToPath(patientDM, patientDM.PatientFilePath, Log);
                System.Windows.MessageBox.Show("Hier sollte gesaved werden");
                ViewModelPatient patientVM = new ViewModelPatient();
                //patientVM.InitializePlugin(patientDM.FriendlyName, new DataModelCatalogItem(patientDM), this);
                //patientVM.CommandLoad.Execute(null);
                patientVM.IsNewPatient = true;
                patientVM.DiscardNewPatient += patientVM_DiscardNewPatient;
                patientVM.CheckDuplicateId += patientVM_CheckDuplicateId;
                patientVM.NewPatientSaved += patientVM_NewPatientSaved;
                _patients.Insert(0, patientVM);
                SelectedPatient = _patients[0];
                SelectedPatient.RefreshIsDuplicateId();
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(DisplayName, "Exception creating new patient: " + ex.Message);
                System.Windows.MessageBox.Show("Exception creating new patient: " + ex.Message);
            }
        }

        private void newPatientEmpty() { }

        void patientVM_NewPatientSaved(object sender, EventArgs e)
        {
            try
            {
                ViewModelPatient patient = sender as ViewModelPatient;
                string message = "Patient " + patient.FullName + " created.";
                string caption = "Notice";

                DateTime existingPatientBirthDate;
                DateTime newPatientBirthDate;

                if (patient.BirthDate == null)
                    return;

                newPatientBirthDate = (DateTime)patient.BirthDate;

                foreach (ViewModelPatient existingPatient in _patients)
                {

                    if (existingPatient == patient)
                        continue;

                    if (existingPatient.BirthDate == null)
                        continue;

                    existingPatientBirthDate = (DateTime)existingPatient.BirthDate;

                    if (existingPatientBirthDate.Date == newPatientBirthDate.Date &&
                        existingPatient.FirstName == patient.FirstName &&
                        existingPatient.LastName == patient.LastName)
                    {
                        message += "\r\n\r\nPatient with same last name, first name and birth date already exists. Please modify patient information if necessary.";
                        caption = "Warning";
                        break;
                    }
                }

                //MSOTService.NotifyUserOnAction(message, caption, UserNotificationType.Warning, false);
                System.Windows.MessageBox.Show("Warning: " + message);
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(DisplayName, "Exception checking for duplicate patient entries: " + ex.Message);
                System.Windows.MessageBox.Show("Exception checking for duplicate patient entries: " + ex.Message);
            }
        }

        private string getNextAvailablePatientId()
        {
            try
            {
                //int i = 1;
                //while (patientIdExists(i.ToString("D5")))
                //{
                //    i++;
                //}
                //string newPatientId = i.ToString("D5");
                //return newPatientId;
                int maxTries = 99999;
                string retValue = "";
                bool found = false;
                for (int i = 0; i < maxTries; i++)
                {
                    retValue = "nächster bitte";//MSOTService.NextAutoIncrementedPatientId();
                    if (!patientIdExists(retValue))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    //MSOTService.NotifyUserOnAction("Could not get auto-generated patient ID.", "Warning", UserNotificationType.Warning, false);
                    System.Windows.MessageBox.Show("Could not get auto-generated patient ID.");
           
                    retValue = "";
                }

                return retValue;
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(DisplayName, "Exception generating next available patient ID: " + ex.Message);
                System.Windows.MessageBox.Show("Exception generating next available patient ID: " + ex.Message);
                return "";
            }
        }

        private bool patientIdExists(string id)
        {
            try
            {
                foreach (ViewModelPatient existingPatient in _patients)
                {
                    if (existingPatient.Id == id)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(DisplayName, "Exception checking if patient id exists: " + ex.Message);
                System.Windows.MessageBox.Show("Exception checking if patient id exists: " + ex.Message);
                
                return false;
            }
        }

        //DelegateCommand cancelPatientCommand;
        //public ICommand CancelPatientCommand
        //{
        //    get
        //    {
        //        if (cancelPatientCommand == null)
        //        {
        //            cancelPatientCommand = new DelegateCommand(cancelPatient, canCancelPatient);
        //        }
        //        return cancelPatientCommand;
        //    }
        //}

        private void cancelPatient() { }

        private bool canCancelPatient()
        {
            if (SelectedPatient != null)
            {
                if (Patients.Count == 1 && SelectedPatient.IsNewPatient)
                {
                    return false;
                }

                if (!SelectedPatient.IsValid)
                    return true;
            }

            if (ActivePatient == null)
                return false;

            return true;
        }

        void patientVM_DiscardNewPatient(object sender, EventArgs e)
        {
            try
            {
                ViewModelPatient patient = sender as ViewModelPatient;
                bool deleted = true;//MSOTService.IProjectManager.DeletePatientStudy(patient.PatientFilePath);
                if (deleted)
                {
                    Patients.Remove(patient);
                }
            }
            catch (Exception ex)
            {
                //MSOTService.IMSOTLog.HandleError(DisplayName, "Exception deleting patient: " + ex.Message);
                System.Windows.MessageBox.Show("Exception checking if patient id exists: " + ex.Message);
                
            }
        }

        bool patientVM_CheckDuplicateId(ViewModelPatient patient)
        {
            try
            {
                string id = patient.Id;
                foreach (ViewModelPatient existingPatient in _patients)
                {
                    if (existingPatient == patient)
                        continue;
                    if (existingPatient.Id == id)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
               // MSOTService.IMSOTLog.HandleError(DisplayName, "Exception checking patient id: " + ex.Message);
                System.Windows.MessageBox.Show("Exception checking  patient id : " + ex.Message);
                return false;
            }
        }

    }
}
