using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Talent.DataAccess.Ado;
using Talent.Domain;
using UclaExt.Common.BaseClasses;

namespace Talent.WpfClient
{
    public class PeopleViewModel : DomainViewModel<Person>
    {
        private PersonAttachment _selectedPersonAttachment;

        public PeopleViewModel() : base( new PersonRepository())
        {
            ImportAttachmentFromDiskCommand = 
                new RelayCommand(OnImportFromDisk, () => SelectedItem != null);
            ImportAttachmentFromClipboardCommand = 
                new RelayCommand(OnImportFromClipboard, () => SelectedItem != null);
            SaveAttachmentToDiskCommand = 
                new RelayCommand(OnSaveAttachmentToDisk, 
                    () => SelectedPersonAttachment != null);
            OpenAttachmentCommand = 
                new RelayCommand(OnOpenAttachment, 
                    () => SelectedPersonAttachment != null);
            DeleteAttachmentCommand = 
                new RelayCommand(OnDeleteAttachment, 
                    () => SelectedPersonAttachment != null);

            PersonCriteria = new PersonCriteria();
        }

        protected override void OnSearch()
        {
            Items = new ObservableCollection<Person>(_repo.Fetch(PersonCriteria));
            RaiseAllCanExecuteChanged();
        }

        private void OnImportFromDisk()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                LoadAttachmentFromFile(dlg.FileName);
            }
        }

        private void OnImportFromClipboard()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] selectedFiles = data.GetData(DataFormats.FileDrop) as string[];
                var fileName = selectedFiles[0];
                LoadAttachmentFromFile(fileName);
            }
        }

        private void OnSaveAttachmentToDisk()
        {
            PersonAttachment att = SelectedPersonAttachment;
            if (att == null) return;

            SaveFileDialog dlg = new SaveFileDialog();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dlg.DefaultExt = att.FileExtension;
            dlg.FileName = System.IO.Path.Combine(path, att.FileName) 
                    + "." + att.FileExtension;
            if (dlg.ShowDialog() == true)
            {
                File.WriteAllBytes(dlg.FileName, att.FileBytes);
            }
        }

        private void OnOpenAttachment()
        {
            PersonAttachment att = SelectedPersonAttachment;
            if (att == null) return;
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var fullPath = System.IO.Path.Combine(path, att.FileName)
                    + "." + att.FileExtension;
                File.WriteAllBytes(fullPath, att.FileBytes);
                Process.Start(fullPath);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Error opening file",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Other exception type",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnDeleteAttachment()
        {
            if(SelectedPersonAttachment != null)
            {
                SelectedPersonAttachment.IsMarkedForDeletion = true;
                RaiseAllCanExecuteChanged();
            }
        }

        #region Properties

        public RelayCommand ImportAttachmentFromDiskCommand { get; private set; }
        public RelayCommand ImportAttachmentFromClipboardCommand { get; private set; }
        public RelayCommand SaveAttachmentToDiskCommand { get; private set; }
        public RelayCommand OpenAttachmentCommand { get; private set; }
        public RelayCommand DeleteAttachmentCommand { get; private set; }

        public PersonAttachment SelectedPersonAttachment
        {
            get
            {
                return _selectedPersonAttachment;
            }
            set
            {
                if (_selectedPersonAttachment == value) return;
                if (_selectedPersonAttachment != null)
                {
                    _selectedPersonAttachment.PropertyChanged -= OnModelPropertyChanged;
                    _selectedPersonAttachment.ErrorsChanged -= RaiseCanSaveChanged;
                }
                _selectedPersonAttachment = value;
                OnPropertyChanged();
                RaiseAllCanExecuteChanged();
                if (_selectedPersonAttachment != null)
                {
                    _selectedPersonAttachment.PropertyChanged += OnModelPropertyChanged;
                    _selectedPersonAttachment.ErrorsChanged += RaiseCanSaveChanged;
                }
            }
        }

        public PersonCriteria PersonCriteria { get; set; }

        protected override void RaiseAllCanExecuteChanged()
        {
            base.RaiseAllCanExecuteChanged();
            ImportAttachmentFromDiskCommand.RaiseCanExecuteChanged();
            ImportAttachmentFromClipboardCommand.RaiseCanExecuteChanged();
            SaveAttachmentToDiskCommand.RaiseCanExecuteChanged();
            OpenAttachmentCommand.RaiseCanExecuteChanged();
            DeleteAttachmentCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region Lookup Lists

        public ObservableCollection<HairColor> HairColors
        {
            get {
                return new ObservableCollection<HairColor>( 
                    LookupCache.HairColors);
            }
        }

        public ObservableCollection<EyeColor> EyeColors
        {
            get
            {
                return new ObservableCollection<EyeColor>(
                    LookupCache.EyeColors);
            }
        }

        public ObservableCollection<CreditType> CreditTypes
        {
            get
            {
                return new ObservableCollection<CreditType>(
                    LookupCache.CreditTypes);
            }
        }

        public ObservableCollection<Show> Shows
        {
            get
            {
                return new ObservableCollection<Show>(
                    LookupCache.Shows);
            }
        }

        #endregion

        #region Overrides

        protected override void OnSave()
        {
            base.OnSave();
            var deletedAttachments = SelectedItem
                .PersonAttachments
                .Where(a => a.IsMarkedForDeletion)
                .ToList();
            for(int i = deletedAttachments.Count-1; i >= 0; i--)
            {
                SelectedItem.PersonAttachments.Remove(
                    deletedAttachments[i]);
            }
        }

        #endregion

        #region  Methods

        public void LoadAttachmentFromFile(string fileName)
        {
            if(SelectedItem == null)
            {
                throw new ApplicationException("Must select a Person First");
            }
            var attach = new PersonAttachment
            {
                FileName = System.IO.Path.GetFileNameWithoutExtension(fileName),
                FileExtension = System.IO.Path.GetExtension(fileName).Replace(".", ""),
                FileBytes = File.ReadAllBytes(fileName)
            };
            SelectedItem.PersonAttachments.Add(attach);
            RaiseAllCanExecuteChanged();
        }

        #endregion

    }
}
