namespace ViewRSOM
{
    public class unmixFileItem
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public string folderPath { get; set; }
        public bool isChecked { get; set; }

        public unmixFileItem(int _id, string _fileName, string _folderPath, bool _isChecked)
        {
            id = _id;
            fileName = _fileName;
            folderPath = _folderPath;
            isChecked = _isChecked;
        }
    }
}
