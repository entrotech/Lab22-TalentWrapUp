using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Talent.DataAccess.Ado;
using Talent.Domain;
using UclaExt.Common.BaseClasses;
using UclaExt.Common.Interfaces;

namespace Talent.WpfClient
{
    public class DomainViewModel<T> : INotifyPropertyChanged where T : DomainBase, new()
    {
        protected IRepository<T> _repo;
        private ObservableCollection<T> _items;
        private T _selectedItem;

        public DomainViewModel(IRepository<T> repository)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            _repo = repository;

            SearchCommand = new RelayCommand(OnSearch, CanSearch);
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            SaveCommand = new RelayCommand(OnSave, CanSave);
            CancelCommand = new RelayCommand(OnCancel, CanCancel);
        }

        #region Properties

        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public ObservableCollection<T> Items
        {
            get
            {
                return _items;
            }
            protected set  // Can only set via private class methods
            {
                if (_items == value) return;
                _items = value;
                OnPropertyChanged();
                OnPropertyChanged("SelectedItem");
            }
        }

        public T SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem == value) return;
                if (_selectedItem != null)
                {
                    _selectedItem.PropertyChanged -= OnModelPropertyChanged;
                    _selectedItem.ErrorsChanged -= RaiseCanSaveChanged;
                }
                _selectedItem = value;
                OnPropertyChanged();
                RaiseAllCanExecuteChanged();
                if (_selectedItem != null)
                {
                    _selectedItem.PropertyChanged += OnModelPropertyChanged;
                    _selectedItem.ErrorsChanged += RaiseCanSaveChanged;
                }
            }
        }

        protected void RaiseCanSaveChanged(object sender, DataErrorsChangedEventArgs e)
        {
            RaiseAllCanExecuteChanged();
        }

        protected void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseAllCanExecuteChanged();
        }

        protected virtual void RaiseAllCanExecuteChanged()
        {
            SearchCommand.RaiseCanExecuteChanged();
            AddCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
        }

        #endregion // Properties

        #region Command Implementation

        private bool CanSearch()
        {
            return SelectedItem == null
                || !SelectedItem.GetHasChanges();
        }

        protected virtual void OnSearch()
        {
            Items = new ObservableCollection<T>(_repo.Fetch());
            RaiseAllCanExecuteChanged();
        }

        private bool CanAdd()
        {
            return Items != null
                && (SelectedItem == null
                || !SelectedItem.GetHasChanges());
        }

        public void OnAdd()
        {
            Items.Add(new T());
            SelectedItem = Items.LastOrDefault();
        }

        private bool CanDelete()
        {
            return SelectedItem != null;
        }

        private void OnDelete()
        {
            if (SelectedItem != null)
            {
                SelectedItem.IsMarkedForDeletion = true;
                try
                {
                    _repo.Persist(SelectedItem);
                    Items.Remove(SelectedItem);
                    SelectedItem = null;
                    RaiseAllCanExecuteChanged();
                }
                catch
                {
                    return;
                }
            }
        }

        private bool CanCancel()
        {
            return SelectedItem != null 
                && SelectedItem.GetHasChanges();
        }

        private void OnCancel()
        {
            string selectedItem = SelectedItem.ToString();
            OnSearch();
            SelectedItem = Items
                .Where(r => r.ToString() == selectedItem.ToString())
                .FirstOrDefault();
        }

        private bool CanSave()
        {
            return SelectedItem != null 
                && SelectedItem.GetHasChanges()
                && !SelectedItem.HasErrors;
        }

        protected virtual void OnSave()
        {
            try
            {
                SelectedItem = _repo.Persist(SelectedItem);
                RaiseAllCanExecuteChanged();
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Error saving changes: "
                    + ex.Message, ex);
            }
        }

        #endregion // Commands

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "None")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
