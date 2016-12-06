using UclaExt.Common.BaseClasses;

namespace Talent.Domain
{
    /// <summary>
    /// A file that can be attached to a Person.
    /// </summary>
    public class PersonAttachment : DomainBase
    {
        #region Fields

        private int _id;
        private int _personId;
        private string _caption;
        private string _fileName;
        private string _fileExtension;
        private byte[] _fileBytes;

        #endregion

        #region Properties

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int PersonId
        {
            get { return _personId; }
            set
            {
                if (_personId == value) return;
                _personId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Optional Description of File
        /// </summary>
        public string Caption
        {
            get { return _caption; }
            set
            {
                if (_caption == value) return;
                _caption = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Default root file name for downloading purposes
        /// (without path or file extension)
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName == value) return;
                _fileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Standard Windows file extension that determines the file
        /// format (e.g., jpg, png, docx, xlsx)
        /// </summary>
        public string FileExtension
        {
            get { return _fileExtension; }
            set
            {
                if (_fileExtension == value) return;
                _fileExtension = value;
                OnPropertyChanged();
            }
        }

        public byte[] FileBytes
        {
            get { return _fileBytes; }
            set
            {
                if(_fileBytes == value) return;
                _fileBytes = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return (FileName ?? "") + "." + (FileExtension ?? "");
        }

        #endregion
    }
}
