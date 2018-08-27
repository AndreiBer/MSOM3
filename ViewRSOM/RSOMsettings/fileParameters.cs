namespace ViewRSOM
{

    public delegate void FileParameterChangedEventHandler(string argument);

    public static class fileParameters
    {

        public static event FileParameterChangedEventHandler FileParameterChanged;

        private static void Notify(string argument)
        {
            if (FileParameterChanged != null) FileParameterChanged(argument);
            // FileParameterChanged?.Invoke(argument);
        }

        // private fileParameters

        // general RSOM folders
        private static string _RSOMdataFolder;
        private static string _SFFile;
        private static string _SFFileLF;
        private static string _SFFileHF;
        private static string _ImageJ;

        // study and dataName folder
        private static string _studyFolder;
        private static string _dataName;

        // acquisition folders
        private static string _acqFolder;
        private static string _acqQuickScanFolder;
        private static string _acqLogFolder;
        private static string _acqThumbnailFolder;

        // Folders located in special folders
        private static string _acqPreviewFolder;
        private static string _SFFolder;
        private static string _cfgFolder;
        private static string _errorLogFolder;

        // reconstruction folders
        private static string _reconFolder;
        private static string _reconLogFolder;
        private static string _reconImageFolder;
        private static string _reconThumbnailFolder;
        private static string _reconExportFolder;


        // public versions (for DataBinding) - fileParameters

        // general RSOM folders
        public static string RSOMdataFolder
        {
            // get { return _RSOMdataFolder == null ? System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\RSOMdata\\" : _RSOMdataFolder; }
            get { return _RSOMdataFolder; }
            set
            {
                if (value.Contains("\\"))
                {
                    _RSOMdataFolder = value + "\\";
                }
                else
                {
                    _RSOMdataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\" + value + "\\";
                }
                Notify("RSOMdataFolder");
            }
        }

        public static string SFFile
        {
            get { return _SFFile; }
            set
            {
                _SFFile = value;
                Notify("SFFile");
            }
        }

        public static string SFFileLF
        {
            get { return _SFFileLF; }
            set
            {
                _SFFileLF = value;
                Notify("SFFileLF");
            }
        }

        public static string SFFileHF
        {
            get { return _SFFileHF; }
            set
            {
                _SFFileHF = value;
                Notify("SFFileHF");
            }
        }

        public static string ImageJ
        {
            get { return _ImageJ; }
            set
            {
                _ImageJ = value;
                Notify("ImageJ");
            }
        }


        // study and dataName folder
        public static string studyFolder
        {
            get { return _studyFolder; }
            set
            {
                _studyFolder = value;
                Notify("studyFolder");
            }
        }

        public static string dataName
        {
            get { return _dataName; }
            set
            {
                _dataName = value;
                Notify("dataName");
            }
        }

        
        // acquisition folders
        public static string acqFolder
        {
            get { return _acqFolder; }
            set
            {
                _acqFolder = value;
                Notify("acqFolder");
            }
        }

        public static string acqQuickScanFolder
        {
            get { return _acqQuickScanFolder; }
            set
            {
                _acqQuickScanFolder = value;
                Notify("acqQuickScanFolder");
            }
        }

        public static string acqLogFolder
        {
            get { return _acqLogFolder; }
            set
            {
                _acqLogFolder = value;
                Notify("acqLogFolder");
            }
        }

        public static string acqThumbnailFolder
        {
            get { return _acqThumbnailFolder; }
            set
            {
                _acqThumbnailFolder = value;
                Notify("acqThumbnailFolder");
            }
        }


        // special folders
        public static string acqPreviewFolder
        {
            get { return _acqPreviewFolder == null ? System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) + "\\iThera\\ViewRSOM\\Preview\\" : _acqPreviewFolder; }
            set
            {
                _acqPreviewFolder = value;
                Notify("acqPreviewFolder");
            }
        }

        public static string SFFolder
        {
            get { return _SFFolder == null ? System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) + "\\iThera\\ViewRSOM\\sf\\" : _SFFolder; }
            set
            {
                _SFFolder = value;
                Notify("SFFolder");
            }
        }
        
        public static string cfgFolder
        {
            get { return _cfgFolder == null ? System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData) + "\\iThera\\ViewRSOM\\cfg\\" : _cfgFolder; }
            set
            {
                _cfgFolder = value;
                Notify("cfgFolder");
            }
        }

        public static string errorLogFolder
        {
            get { return _errorLogFolder; }
            set
            {
                _errorLogFolder = value + "\\ErrorLog\\";
                Notify("errorLogFolder");
            }
        }

        
        // reconstruction folders
        public static string reconFolder
        {
            get { return _reconFolder; }
            set
            {
                _reconFolder = value;
                Notify("reconFolder");
            }
        }

        public static string reconLogFolder
        {
            get { return _reconLogFolder; }
            set
            {
                _reconLogFolder = value;
                Notify("reconLogFolder");
            }
        }

        public static string reconImageFolder
        {
            get { return _reconImageFolder; }
            set
            {
                _reconImageFolder = value;
                Notify("reconImageFolder");
            }
        }

        public static string reconThumbnailFolder
        {
            get { return _reconThumbnailFolder; }
            set
            {
                _reconThumbnailFolder = value;
                Notify("reconThumbnailFolder");
            }
        }

        public static string reconExportFolder
        {
            get { return _reconExportFolder; }
            set
            {
                _reconExportFolder = value;
                Notify("reconExportFolder");
            }
        }
                

    }
}
