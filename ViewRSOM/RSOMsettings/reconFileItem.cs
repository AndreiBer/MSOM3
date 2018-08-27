namespace ViewRSOM
{
    public class reconFileItem
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public string folderPath { get; set; }
        public bool isChecked { get; set; }

        public reconFileItem(int _id, string _fileName, string _folderPath, bool _isChecked)
        {
            id = _id;
            fileName = _fileName;
            folderPath = _folderPath;
            isChecked = _isChecked;
        }
    }
}
