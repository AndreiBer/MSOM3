using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Xml;
using System.Xml.Linq;




namespace ViewRSOM
{   


    public class SelectOverviewEventArgs : EventArgs
    {
        public bool Canceled { get; set; }
        
    }

    /// <summary>
    /// Interaction logic for ViewPatientsPopup.xaml
    /// </summary>
    public partial class ViewPatientsPopup : UserControl
    {
        public event EventHandler CancelPopupEvent;
        public event EventHandler SelectPopupEvent;
        private bool EditingNewPatient = false;
                
        List<DataModelPatient> patientList = new List<DataModelPatient>(); 
        public ViewPatientsPopup()
        {
            InitializeComponent();
            
            //TEST
            /*
            patientList.Add(new DataModelPatient { FirstName = "Sepp", LastName = "Hornauer", Id = "1", BirthDate = new DateTime(1980, 2, 3), Sex = PatientSex.M, Comment = "this is a test patient" });
            patientList.Add(new DataModelPatient { FirstName = "Hans", LastName = "Dopfer", Id = "2", BirthDate = new DateTime(1977, 1, 31), Sex = PatientSex.O,  });
            */
            //TEST
            InitiatePatientList();
            patientsListView.ItemsSource = patientList;
            operatorTextBox.Text = studyParameters.Operator;
            patientsListView.SelectedIndex = 0;

                                
        }

        public void InitiatePatientList()
        {
            patientList.Clear();
            try
            {
                if (!Directory.Exists(fileParameters.RSOMdataFolder))
                {
                    System.Windows.MessageBox.Show("Couldn't load patient list. Directory doesn't exist: " + fileParameters.RSOMdataFolder);
                    return;
                }
                // Get all directories in study folder
                string[] patientsPaths = Directory.GetDirectories(fileParameters.RSOMdataFolder);

                string patientMetaXMLPath;
                string patientStudyName;
                string studyNodePath;
                
                foreach (string path in patientsPaths)
                {
                    // Combine paths to get the patientMetaData-file
                    patientStudyName = System.IO.Path.GetFileName(path);
                    patientMetaXMLPath = System.IO.Path.Combine(path, patientStudyName + studyParameters.PatientFileNameExtension);

                    studyNodePath = System.IO.Path.Combine(path, patientStudyName + studyParameters.StudyFileExtension);
                    //if (!File.Exists(studyNodePath))
                    //    continue;

                    if (File.Exists(patientMetaXMLPath))
                    {
                        // Open Patient.Metadata-file
                        XmlDocument xml = new XmlDocument();
                        try
                        {
                            xml.Load(@patientMetaXMLPath);
                        }
                        catch
                        {
                            Console.Write("ERROR Loading Data from Study Folder '" + patientMetaXMLPath + "'.\n");
                            return;
                        }
                        try
                        {
                            string _firstName = xml.SelectSingleNode("DataModelPatient/FirstName").InnerText;
                            string _lastName = xml.SelectSingleNode("DataModelPatient/LastName").InnerText;
                            string _birthDateString = xml.SelectSingleNode("DataModelPatient/BirthDate").InnerText;
                            DateTime _birthDate = Convert.ToDateTime(_birthDateString);
                            string _id = xml.SelectSingleNode("DataModelPatient/Id").InnerText;
                            string _sexString = xml.SelectSingleNode("DataModelPatient/Sex").InnerText;
                            string _lastExamString = xml.SelectSingleNode("DataModelPatient/LastExam").InnerText;
                            DateTime _lastExam = DateTime.Now;
                            if(_lastExamString!="")
                                _lastExam=Convert.ToDateTime(_lastExamString);
                            string _comment = xml.SelectSingleNode("DataModelPatient/Comment").InnerText;
                            string _creationTimeString = xml.SelectSingleNode("DataModelPatient/CreationTime").InnerText;
                            //DateTime _creationTime = DateTime.ParseExact(_creationTimeString, "yyyy-MM-ddTHH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture)
                            DateTime _creationTime =Convert.ToDateTime(_creationTimeString);
                            string _friendlyName = xml.SelectSingleNode("DataModelPatient/FriendlyName").InnerText;
                            PatientSex _sex = PatientSex.N;
                            switch(_sexString)
                            {
                                case "F":
                                _sex=PatientSex.F;
                                    break;
                                case "M":
                                _sex=PatientSex.M;
                                    break;
                                case "N":
                                _sex=PatientSex.N;
                                    break;
                                case "O":
                                _sex=PatientSex.O;
                                    break;

                            }
                            //DateTime _birthDate = new DateTime(Convert.ToInt32(_birthDateString.Substring(0, 4)), Convert.ToInt32(_birthDateString.Substring(5, 2)), Convert.ToInt32(_birthDateString.Substring(8, 2)));
                            patientList.Add(new DataModelPatient { FirstName = _firstName, LastName = _lastName, Id = _id, BirthDate = _birthDate, Sex = _sex, Comment = _comment, FriendlyName=_friendlyName, LastExam=_lastExam,CreationTime=_creationTime, PatientFilePath= path});
                            
                            //check the patient ID , and save it if it is higher then the previous one. This will get you get the max current patient ID
                            int _actualLoadedID = Convert.ToInt32(_id);
                            if (_actualLoadedID > studyParameters.highestCurrentStudyId)
                                studyParameters.highestCurrentStudyId = _actualLoadedID;

                        }
                        catch 
                        {
                            Console.Write("ERROR reading Study Meta Data: '" + patientMetaXMLPath + "'.\n");
                            return;
                        }
                        
                        //patientDM = DataModelSerialization.LoadFromPath<DataModelPatient>(patientMetaXMLPath, Log);
                        //TODO Remove PatientSex.O enum
                        //                       if (patientDM.Sex == PatientSex.O)
                        //                       {
                        //                           patientDM.Sex = PatientSex.M;
                        ////                           DataModelSerialization.SaveToPath(patientDM, patientMetaXMLPath, Log);
                        //                       }
                        //                       patientDM.PatientFilePath = patientMetaXMLPath;
                        //ViewModelPatient patientVM = new ViewModelPatient();
                        //patientVM.InitializePlugin(patientDM.FriendlyName, new DataModelCatalogItem(patientDM), this);
                        //patientVM.CommandLoad.Execute(null);
                        //patientVM.CheckDuplicateId += patientVM_CheckDuplicateId;
                        //_patients.Add(patientVM);
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
        
        public void selectPatient(int index)
        {
            patientsListView.SelectedIndex = index;    
        }
       
        public event EventHandler<SelectOverviewEventArgs> SelectOverview;
        protected bool OnSelectOverview()
        {
            SelectOverviewEventArgs eventArgs = new SelectOverviewEventArgs();
            SelectOverview.Invoke(this, eventArgs);
            return !eventArgs.Canceled;
        }

        public void fillPatientDetails(DataModelPatient SelectedPatient)
        {
            try
            {
                patientFirstNameTextBox.Text = SelectedPatient.FirstName;
                patientLastNameTextBox.Text = SelectedPatient.LastName;
                patientIdTextBox.Text = SelectedPatient.Id.ToString();
                patientCommentTextBox.Text = SelectedPatient.Comment;
                patientBirthDate.SelectedDate = SelectedPatient.BirthDate;
            
                int patientSexIndex=0;
                switch (SelectedPatient.Sex)
                {
                    case PatientSex.F:
                        patientSexIndex=0;
                        break;
                    case PatientSex.M:
                        patientSexIndex=1;
                        break;
                    case PatientSex.O:
                        patientSexIndex=2;
                        break;
                    case PatientSex.N:
                        patientSexIndex=3;
                        break;
                }
                patientSexComboBox.SelectedIndex=patientSexIndex;
            }
            catch { }
                
        }

        public bool CloseControl
        {
            get { return (bool)GetValue(CloseControlProperty); }
            set { SetValue(CloseControlProperty, value); }
            
        }

        public static readonly DependencyProperty CloseControlProperty =
            DependencyProperty.Register(
            "CloseControl",
            typeof(bool),
            typeof(ViewPatientsPopup),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(closeControlChanged)));

        private static void closeControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                ViewPatientsPopup control = d as ViewPatientsPopup;
                

//                control.selectAndScrollToPatientListViewItem(ActivePatient);
            }
        }

        private void selectAndScrollToPatientListViewItem(ViewModelPatient patient)
        {
            if (patient != null)
            {
                patientsListView.SetCurrentValue(ListView.SelectedItemProperty, patient);
                patientsListView.ScrollIntoView(patient);
            }
        }

        private void hideModalControl()
        {
            this.SetCurrentValue(ViewPatientsPopup.CloseControlProperty, true);
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                DataModelPatient SelectedPatient = patientsListView.SelectedItem as DataModelPatient;

                string selectedPatientID = SelectedPatient.Id;
                studyParameters.ActualPatientFirstName = SelectedPatient.FirstName;
                studyParameters.ActualPatientLastName = SelectedPatient.LastName;
                studyParameters.ActualPatientId = SelectedPatient.Id;
                //myGlobalSettings.ActualPatientComment = patientList[patientIndex].Comment;
                studyParameters.ActualPatientBirthDate = (DateTime)SelectedPatient.BirthDate;

                fileParameters.studyFolder = SelectedPatient.PatientFilePath;

                // setup nested list of study dates, acq files, and recon files
                //loadStudyFileList(SelectedPatient.PatientFilePath);
            
                // forward cancel event to the main GUI
                if (this.SelectPopupEvent != null)
                    this.SelectPopupEvent(new object(), new EventArgs());
            }
            catch { }
              
        }


        //private void loadStudyFileList(string studyFolder)
        //{
            

        //}

        
        private void cancelPatientPopupButton_Click(object sender, RoutedEventArgs e)
        {
            // check
            if (!Directory.Exists(fileParameters.studyFolder))
            {
                if (patientList.Count == 0)
                {
                    fileParameters.studyFolder = null;
                    studyParameters.ActualPatientFirstName = "";
                    studyParameters.ActualPatientId = "0";
                    studyParameters.ActualPatientLastName = "";
                    studyParameters.ActualPatientBirthDate = DateTime.Now;
                    studyParameters.BirthDateAsString = "";
               
                }
                else
                {
                    if (fileParameters.studyFolder != null)
                    {
                        System.Windows.MessageBox.Show("Selected study does not exist any more. Please choose different study.");
                        return;
                    }
                }
            }

            //Reset all GUI Ellements to default
            patientsListView.IsEnabled = true;
            deletePatientButton.IsEnabled = true;
            clearFilterButton.IsEnabled = true;
            filterTextBox.IsEnabled = true;
            selectButton.Visibility = Visibility.Visible;
            saveNewPatientButton.Visibility = Visibility.Collapsed;

            // fill the patient details with last selected patient
            fillPatientDetails((patientsListView.SelectedItem as DataModelPatient));
            EditingNewPatient = false;

            // forward this event to the main GUI
            if (this.CancelPopupEvent != null)
                this.CancelPopupEvent(new object(), new EventArgs());


        }
        
        
        private void patientsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                fillPatientDetails((patientsListView.SelectedItem as DataModelPatient)); 
            }
            catch { }
           
        }
         

        private void patientsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selectButton_Click(sender, e);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && IsLoaded == true)
                defaultSort();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitiatePatientList();
        }

        private bool isPatientsListViewEmpty()
        {
            ViewModelPatientsManager model = this.DataContext as ViewModelPatientsManager;
            if (model == null)
                return true;
            else if (model.Patients.Count == 0)
                return true;
            return false;
            
        }

        private void createNewPatient()
        {
            if (newPatientButton.Command.CanExecute(null))
            {
                newPatientButton_Click(newPatientButton, new RoutedEventArgs());
            }
        }

        private void selectFirstPatient()
        {
            if (isPatientsListViewEmpty())
                createNewPatient();
            else if (patientsListView.HasItems)
                selectAndScrollToPatientListViewItem(patientsListView.Items[0] as ViewModelPatient);
        }

        private void selectNextPatient()
        {
            if (!patientsListView.HasItems)
            {
                if (newPatientButton.Command.CanExecute(null) && string.IsNullOrWhiteSpace(filterTextBox.Text))
                {
                    newPatientButton_Click(newPatientButton, new RoutedEventArgs());
                }
            }
            else
            {
                selectAndScrollToPatientListViewItem(patientsListView.Items[0] as ViewModelPatient);
            }
        }

        private void newPatientButton_Click(object sender, RoutedEventArgs e)
        {
            
            // focus on patient ID Box
            patientIdTextBox.Focus();
            // show next available ID in patientIDTextBox
            patientIdTextBox.Text = (studyParameters.highestCurrentStudyId+1).ToString("0000");
            // focus on patient ID Box
            patientIdTextBox.Focus();
            // Clear Filter
            clearFilterButton_Click(sender, e);
            //prepare GUI Elements
            patientsListView.IsEnabled = false;
            deletePatientButton.IsEnabled = false;
            clearFilterButton.IsEnabled = false;
            filterTextBox.IsEnabled = false;
            selectButton.Visibility = Visibility.Collapsed;
            saveNewPatientButton.Visibility = Visibility.Visible;
            // Clear Previous Values
            patientFirstNameTextBox.Clear();
            patientLastNameTextBox.Clear();
            patientCommentTextBox.Clear();
            patientSexComboBox.SelectedIndex = 3;
            patientBirthDate.SelectedDate=null;

            EditingNewPatient = true;

            
        }

        
        private void deletePatientButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure the message box to be displayed
            string messageBoxText = "Do you really want to delete the study?";
            string caption = "Delete study";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:

                    // delte study
                    string path = fileParameters.RSOMdataFolder + "\\study" + (patientsListView.SelectedItem as DataModelPatient).Id;
                    if (patientsListView.SelectedItem != null)
                    {
                        try
                        {
                            Directory.Delete(path, true);
                            patientList.Remove((patientsListView.SelectedItem as DataModelPatient));
                            patientsListView.SelectedIndex = patientList.Count() - 1;
                            patientsListView.Items.Refresh();
                            InitiatePatientList();
                        }
                        catch
                        {
                            InitiatePatientList();
                        }
                    }
                    break;

                case MessageBoxResult.No:

                    break;
            }
            
        }

        #region Sort and Filter
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private void gridViewColumnHeaderClickedHandler(object sender, MouseButtonEventArgs e)
        {
            try
            {
                

                GridViewColumnHeader headerClicked = sender as GridViewColumnHeader;
                ListSortDirection direction;
                if (headerClicked != null)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    if (_lastHeaderClicked != null)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    List<string> sortingParamNames = new List<string>();
                    sortingParamNames.Add(headerClicked.Tag.ToString());

                    if (headerClicked == lastNameGridViewColumnHeader)
                        sortingParamNames.Add(firstNameGridViewColumnHeader.Tag.ToString());
                    else if (headerClicked == firstNameGridViewColumnHeader)
                        sortingParamNames.Add(lastNameGridViewColumnHeader.Tag.ToString());

                    if (headerClicked != lastExamGridViewColumnHeader)
                        sortingParamNames.Add(lastExamGridViewColumnHeader.Tag.ToString());

                    clearRefreshPatientListViewSorting(sortingParamNames, direction);

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;

                    ViewModelPatient selected = patientsListView.SelectedItem as ViewModelPatient;
                    selectAndScrollToPatientListViewItem(selected);
                }
            }
            catch { }
        }

        private void clearRefreshPatientListViewSorting(List<string> sortingParamNames, ListSortDirection direction)
        {
            patientsListView.Items.SortDescriptions.Clear();
            SortDescription sd;
            for (int i = 0; i < sortingParamNames.Count; i++)
            {
                sd = new SortDescription(sortingParamNames[i], direction);
                patientsListView.Items.SortDescriptions.Add(sd);
            }
            patientsListView.Items.Refresh();
        }

        private void filterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(filterTextBox.Text))
            {
                patientsListView.Items.Filter = patientFilter;
            }
            else
            {
                patientsListView.Items.Filter = null;
            }
            //patientsListView.Items.Refresh();
            //selectFirstPatient();
        }

        private bool patientFilter(object item)
        {
            DataModelPatient patient = item as DataModelPatient;
            if (patient.Id.IndexOf(filterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (patient.LastName.IndexOf(filterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (patient.FirstName.IndexOf(filterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            return false;
        }

        private void clearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            //if (abortActionOnPendingSelectedPatientEdit())
            //    return;

            //if (abortActionOnPendingOperatorEdit())
            //    return;

            filterTextBox.SetCurrentValue(TextBox.TextProperty, "");
        }

        private void removeSort()
        {
            if (_lastHeaderClicked != null)
                _lastHeaderClicked.Column.HeaderTemplate = null;
            _lastHeaderClicked = null;
            patientsListView.Items.SortDescriptions.Clear();
            patientsListView.Items.Refresh();
        }

        private void defaultSort()
        {
            if (_lastHeaderClicked != null)
                _lastHeaderClicked.Column.HeaderTemplate = null;
            _lastHeaderClicked = lastNameGridViewColumnHeader;

            _lastDirection = ListSortDirection.Ascending;
            //_lastHeaderClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowUp"] as DataTemplate;

            List<string> sortingParamNames = new List<string>();
            sortingParamNames.Add(lastNameGridViewColumnHeader.Tag.ToString());
            sortingParamNames.Add(firstNameGridViewColumnHeader.Tag.ToString());
            sortingParamNames.Add(lastExamGridViewColumnHeader.Tag.ToString());

            clearRefreshPatientListViewSorting(sortingParamNames, _lastDirection);
        }
        #endregion Sort and Filter

        private void patientsListView_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            //e.Handled = true;
        }

        private bool abortActionOnPendingPatientEdit(ViewModelPatient patient)
        {
            bool retValue = false;

            if (patient != null)
            {
                
                    
                    {
                        FocusLastInputElement();
                    }
                
            }

            return true;
        }

        private bool abortActionOnPendingSelectedPatientEdit()
        {
            ViewModelPatient selectedPatient = patientsListView.SelectedItem as ViewModelPatient;
            bool retValue = abortActionOnPendingPatientEdit(selectedPatient);
            return retValue;
        }

        private bool abortActionOnPendingOperatorEdit()
        {
            ViewModelPatientsManager model = this.DataContext as ViewModelPatientsManager;

            bool retValue = false;

            if (model != null)
            {
                retValue = model.VerifyCancelOperatorPendingChanges();
                if (retValue)
                {
                    giveElementFocus(operatorTextBox);
                }
            }

            return retValue;
        }

        private void filterTextBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (abortActionOnPendingSelectedPatientEdit())
            {
                e.Handled = true;
            }

            if (abortActionOnPendingOperatorEdit())
            {
                e.Handled = true;
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (abortActionOnPendingSelectedPatientEdit())
            {
                e.Handled = true;
            }

            if (abortActionOnPendingOperatorEdit())
            {
                e.Handled = true;
            }
        }

        private void patientsListView_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (abortActionOnPendingSelectedPatientEdit())
            {
                e.Handled = true;
            }

            if (abortActionOnPendingOperatorEdit())
            {
                e.Handled = true;
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !patientInformation.IsKeyboardFocusWithin && !operatorTextBox.IsKeyboardFocusWithin && !filterTextBox.IsKeyboardFocusWithin)
            {
                if (selectButton.Command.CanExecute(null) && selectButton.IsEnabled)
                    selectButton_Click(sender, e);
            }
        }

        private void patientsListView_KeyDown(object sender, KeyEventArgs e)
        {

            //if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
            //    e.Handled = true;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                focusMainElement();
            }
        }

        private void focusMainElement()
        {
            FrameworkElement element = patientsListView;

            int index = patientsListView.SelectedIndex;
            if (index >= 0)
            {
                ListViewItem lvi = patientsListView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
                if (lvi != null)
                    element = lvi;
            }

            giveElementFocus(element);
        }

        private void giveElementFocus(FrameworkElement element)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                element.Focus();
                Keyboard.Focus(element);
            }));
        }

        private void patientInformation_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (abortActionOnPendingOperatorEdit())
            {
                e.Handled = true;
            }
        }

        private void operatorTextBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (abortActionOnPendingSelectedPatientEdit())
            {
                e.Handled = true;
            }
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            focusSelectTextBox(sender as TextBox);
        }

        private void focusSelectTextBox(TextBox textBoxElement)
        {
            if (textBoxElement == null)
                return;

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                textBoxElement.Focus();
                Keyboard.Focus(textBoxElement);
                textBoxElement.SelectAll();
            }));
        }

        public bool IsDuplicateId
        {
            get { return (bool)GetValue(IsDuplicateIdProperty); }
            set { SetValue(IsDuplicateIdProperty, (bool)value); }
        }
        public static readonly DependencyProperty IsDuplicateIdProperty =
            DependencyProperty.Register(
             "IsDuplicateId",
             typeof(bool),
             typeof(ViewPatientInformation),
             new FrameworkPropertyMetadata(
                 new PropertyChangedCallback(isDuplicateIdChanged)));

        private static void isDuplicateIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        FrameworkElement _lastFocusedFrameworkElement = null;
        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            _lastFocusedFrameworkElement = e.OriginalSource as FrameworkElement;
        }

        public void FocusLastInputElement()
        {
            if (_lastFocusedFrameworkElement != null)
            {
                cancelSafetyDialog(_lastFocusedFrameworkElement);
            }
        }

        private void cancelSafetyDialog(FrameworkElement element)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                element.Focus();
                Keyboard.Focus(element);
            }));
        }



        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (cb.IsDropDownOpen)
            {
                return;
            }
            else
            {
                if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Right || e.Key == Key.Left)
                {
                    cb.SetCurrentValue(ComboBox.IsDropDownOpenProperty, true);
                    e.Handled = true;
                }
            }
        }

       
        private void selectAllTextBox(TextBox textBoxElement)
        {
            if (textBoxElement == null)
                return;

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(delegate()
            {
                textBoxElement.SelectAll();
            }));
        }

        private void textBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                BindingExpression binding = BindingOperations.GetBindingExpression(sender as UIElement, TextBox.TextProperty);
                if (binding != null)
                    binding.UpdateSource();
            }
        }

        private void saveNewPatientButton_Click(object sender, RoutedEventArgs e)
        {
            //here comes the code for saving a new patient
            // first check inputs
            string _newPatientId = patientIdTextBox.Text;
            string _newPatientFirstName = patientFirstNameTextBox.Text;
            string _newPatientLastName = patientLastNameTextBox.Text;
            string _newPatientComment = patientCommentTextBox.Text;
            DateTime _newPatientBirthDate = new DateTime(1900, 1, 1);
            try
            {
                _newPatientBirthDate = (DateTime)patientBirthDate.SelectedDate;
            }
            catch { }                
            int _newPatientSexIndex = patientSexComboBox.SelectedIndex;
            PatientSex _newPatientSex = PatientSex.N;
            switch(_newPatientSexIndex)
            {
                case 0:_newPatientSex=PatientSex.F;
                break;
                case 1: _newPatientSex = PatientSex.M;
                break;
                case 2: _newPatientSex = PatientSex.N;
                break;
                case 3: _newPatientSex = PatientSex.O;
                break;
            }
            //patientSexComboBox.SelectedItem as PatientSex;
            DateTime _newPatientCreationTime = DateTime.Now;
            // create new study folder
            string path=fileParameters.RSOMdataFolder+"\\study"+_newPatientId;
            if (Directory.Exists(path))
            {
                Console.WriteLine("That study exists already.");
                System.Windows.MessageBox.Show("That study exists already. Please change ID");
                return;
            }
            else 
            {
                try
                {
                    Directory.CreateDirectory(@path);
                }
                catch
                {
                    System.Windows.MessageBox.Show("Not able to create new patient folder. Please check permissions");
                }
            }

            // Create patient Study nod-file and save data
            XDocument objXDoc = new XDocument(
                
                new XElement("DataModelPatient",
                    new XElement("FriendlyName", ""),
                    new XElement("CreationTime", _newPatientCreationTime.ToString("o")),
                    new XElement("Comment", _newPatientComment),
                    new XElement("FirstName", _newPatientFirstName),
                    new XElement("LastName", _newPatientLastName),
                    new XElement("Id", _newPatientId),
                    new XElement("BirthDate", _newPatientBirthDate.ToString("o")),
                    new XElement("Sex", _newPatientSex.ToString()),
                    new XElement("LastExam", "")
                    )
               );
        
            objXDoc.Declaration = new XDeclaration("1.0", "utf-8", "true");
            //
            string filename=path+"\\study"+_newPatientId+studyParameters.PatientFileNameExtension;
            objXDoc.Save(@filename);   

            // Add to patient List
            DataModelPatient _newCreatedPatient = (new DataModelPatient { FirstName = _newPatientFirstName, LastName = _newPatientLastName, Id = _newPatientId, BirthDate = _newPatientBirthDate, Sex = _newPatientSex, Comment = _newPatientComment, FriendlyName = "", CreationTime = _newPatientCreationTime, PatientFilePath = path });

            patientList.Add(_newCreatedPatient);
            studyParameters.highestCurrentStudyId = Convert.ToInt32(_newPatientId);
            //
            //ViewModelPatient selected = patientsListView.SelectedItem as ViewModelPatient;
            //selectAndScrollToPatientListViewItem(selected);    
            // fill the patient details with last selected patient
            //fillPatientDetails(_newCreatedPatient);
            patientsListView.SelectedItem = _newCreatedPatient;
            patientsListView.Items.Refresh();
            EditingNewPatient = false;

           
            // Change back GUI behaviour
            patientsListView.IsEnabled = true;
            deletePatientButton.IsEnabled = true;
            clearFilterButton.IsEnabled = true;
            filterTextBox.IsEnabled = true;
            selectButton.Visibility = Visibility.Visible;
            saveNewPatientButton.Visibility = Visibility.Collapsed;


        }

        private void CheckAndSaveChangesInPatientData()
        {
            // first get inputs
            string _newPatientId = patientIdTextBox.Text;
            string _newPatientFirstName = patientFirstNameTextBox.Text;
            string _newPatientLastName = patientLastNameTextBox.Text;
            string _newPatientComment = patientCommentTextBox.Text;
            DateTime _newPatientBirthDate = (DateTime)patientBirthDate.SelectedDate;
            int _newPatientSexIndex = patientSexComboBox.SelectedIndex;
            PatientSex _newPatientSex = PatientSex.N;
            switch (_newPatientSexIndex)
            {
                case 0: _newPatientSex = PatientSex.F;
                    break;
                case 1: _newPatientSex = PatientSex.M;
                    break;
                case 2: _newPatientSex = PatientSex.O;
                    break;
                case 3: _newPatientSex = PatientSex.N;
                    break;
            }
            // check conformity of inputs
            // checkinputs();

            // compare inputs with actual data
            if (_newPatientId != (patientsListView.SelectedItem as DataModelPatient).Id || _newPatientFirstName != (patientsListView.SelectedItem as DataModelPatient).FirstName || _newPatientLastName != (patientsListView.SelectedItem as DataModelPatient).LastName || _newPatientComment != (patientsListView.SelectedItem as DataModelPatient).Comment || _newPatientBirthDate != (patientsListView.SelectedItem as DataModelPatient).BirthDate || _newPatientSex != (patientsListView.SelectedItem as DataModelPatient).Sex)
            {
                string messageBoxText = "Save changes to study?";
                string caption = "Study data changed";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
                
                // Process message box results
                switch (result)
                {
                    case MessageBoxResult.Yes:
                       try
                       {
                            //save Changes to patientInfoFile
                            XDocument objXDoc = new XDocument(

                                new XElement("DataModelPatient",
                                    new XElement("FriendlyName", ""),
                                    new XElement("CreationTime", (patientsListView.SelectedItem as DataModelPatient).CreationTime.ToString("o")),
                                    new XElement("Comment", _newPatientComment),
                                    new XElement("FirstName", _newPatientFirstName),
                                    new XElement("LastName", _newPatientLastName),
                                    new XElement("Id", _newPatientId),
                                    new XElement("BirthDate", _newPatientBirthDate.ToString("o")),
                                    new XElement("Sex", _newPatientSex.ToString()),
                                    new XElement("LastExam", "")
                                    )
                               );

                            objXDoc.Declaration = new XDeclaration("1.0", "utf-8", "true");
                            //
                            string path = fileParameters.RSOMdataFolder + "\\study" + _newPatientId;
                            string filename = path + "\\study" + _newPatientId + studyParameters.PatientFileNameExtension;
                            objXDoc.Save(@filename); 
                        
                            //Refresh patient List
                            (patientsListView.SelectedItem as DataModelPatient).FirstName = _newPatientFirstName;
                            (patientsListView.SelectedItem as DataModelPatient).LastName = _newPatientLastName;
                            (patientsListView.SelectedItem as DataModelPatient).BirthDate = _newPatientBirthDate;
                            (patientsListView.SelectedItem as DataModelPatient).Sex = _newPatientSex;
                            (patientsListView.SelectedItem as DataModelPatient).Comment = _newPatientComment;
                    
                            //refresh View
                            int lastSelectedIndex = patientsListView.SelectedIndex;
                            patientsListView.SelectedIndex = lastSelectedIndex;
                            patientsListView.Items.Refresh();
                        }
                        catch { }


                        break;

                    case MessageBoxResult.No:

                        break;
                }
                                
            }
            
        }

        private void patientInformation_LostFocus(object sender, RoutedEventArgs e)
        {
            // is a new petient created?
            if(!EditingNewPatient)
            { 
                //is the focus still in a patientdata field                
                UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
                if (elementWithFocus.Uid != "patientSexComboBox" && elementWithFocus.Uid != "patientFirstNameTextBox" && elementWithFocus.Uid != "patientLastNameTextBox" && elementWithFocus.Uid != "patientBirthDate" && elementWithFocus.Uid != "patientCommentTextBox")
                {
                    try
                    {
                        CheckAndSaveChangesInPatientData();
                    }
                    catch
                    {
                        Console.Write("ERROR: Could not change patient information. Most likely there is no study available and cancel button was pressed.\n");
                    }
                }
            }
                
        }
        

       
        
        
    }
}
