using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UclaExt.Common.BaseClasses;
using UclaExt.Common.Interfaces;

namespace Talent.Domain
{
    /// <summary>
    /// A Person object representing an Actor, Director, etc.
    /// </summary>
    public class Person : DomainBase, IDisplayable, IComparable<Person>
    {

        #region Fields

        private int _id;
        private string _salutation;
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private string _suffix;
        private DateTime? _dateOfBirth;
        private double? _height;
        private int _hairColorId;
        private int _eyeColorId;
        private ObservableCollection<Credit> _credits = 
            new ObservableCollection<Credit>();
        private ObservableCollection<PersonAttachment> _personAttachments = 
            new ObservableCollection<PersonAttachment>();

        #endregion

        #region Properties

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [StringLength(50,
            ErrorMessage = "Salutation must be no more than 50 characters.")]
        public string Salutation
        {
            get { return _salutation; }
            set
            {
                if (_salutation == value) return;
                _salutation = value;
                OnPropertyChanged();
                OnPropertyChanged("FullName");
                ValidateProperty(_salutation);
            }
        }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50,
            ErrorMessage = "First name must be no more than 50 characters.")]
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName == value) return;
                _firstName = value;
                OnPropertyChanged();
                OnPropertyChanged("FullName");
                OnPropertyChanged("FirstLastName");
                OnPropertyChanged("LastFirstName");
                ValidateProperty(_firstName);
            }
        }

        [StringLength(50,
            ErrorMessage = "Middle name must be no more than 50 characters.")]

        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                if (_middleName == value) return;
                _middleName = value;
                OnPropertyChanged();
                OnPropertyChanged("FullName");
                ValidateProperty(_middleName);
            }
        }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50,
            ErrorMessage = "Last name must be no more than 50 characters.")]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName == value) return;
                _lastName = value;
                OnPropertyChanged();
                OnPropertyChanged("FullName");
                OnPropertyChanged("FirstLastName");
                OnPropertyChanged("LastFirstName");
                ValidateProperty(_lastName);
            }
        }

        [StringLength(50,
            ErrorMessage = "Name suffix must be no more than 50 characters.")]
        public string Suffix
        {
            get { return _suffix; }
            set
            {
                if (_suffix == value) return;
                _suffix = value;
                OnPropertyChanged();
                OnPropertyChanged("FullName");
                ValidateProperty(_suffix);
            }
        }

        [Range(typeof(DateTime), "1/1/1900", "12/31/2050")]
        public DateTime? DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                if (_dateOfBirth == value) return;
                _dateOfBirth = value;
                OnPropertyChanged();
                OnPropertyChanged("Age");
                ValidateProperty(_dateOfBirth);
            }
        }

        [Range(22, 100)]
        public double? Height
        {
            get { return _height; }
            set
            {
                if (_height == value) return;
                _height = value;
                OnPropertyChanged();
                ValidateProperty(_height);
            }
        }

        public int HairColorId
        {
            get { return _hairColorId; }
            set
            {
                if (_hairColorId == value) return;
                _hairColorId = value;
                OnPropertyChanged();
            }
        }

        public int EyeColorId
        {
            get { return _eyeColorId; }
            set
            {
                if (_eyeColorId == value) return;
                _eyeColorId = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Credit> Credits
        {
            get { return _credits; }
        }

        public ObservableCollection<PersonAttachment> PersonAttachments
        {
            get { return _personAttachments; }
        }

        #endregion

        #region Computed Properties

        public string FirstLastName
        {
            get
            {
                return (
                        (FirstName ?? "")
                        + " " +
                        (LastName ?? "")
                    ).Trim();
            }
        }

        public string LastFirstName
        {
            get
            {
                var sb = new StringBuilder();
                var joinCharacter = "";

                if (!String.IsNullOrWhiteSpace(LastName))
                {
                    sb.Append(joinCharacter + LastName);
                    joinCharacter = ", ";
                }
                if (!String.IsNullOrWhiteSpace(FirstName))
                {
                    sb.Append(joinCharacter + FirstName);
                }
                return sb.ToString();
            }
        }

        public string FullName
        {
            get
            {
                var sb = new StringBuilder();
                var joinCharacter = "";

                if (!String.IsNullOrWhiteSpace(Salutation))
                {
                    sb.Append(joinCharacter + Salutation);
                    joinCharacter = " ";
                }
                if (!String.IsNullOrWhiteSpace(FirstName))
                {
                    sb.Append(joinCharacter + FirstName);
                    joinCharacter = " ";
                }
                if (!String.IsNullOrWhiteSpace(MiddleName))
                {
                    sb.Append(joinCharacter + MiddleName);
                    joinCharacter = " ";
                }
                if (!String.IsNullOrWhiteSpace(LastName))
                {
                    sb.Append(joinCharacter + LastName);
                    joinCharacter = " ";
                }
                if (!String.IsNullOrWhiteSpace(Suffix))
                {
                    if (sb.Length > 0) joinCharacter = ", ";
                    sb.Append(joinCharacter + Suffix);
                    joinCharacter = " ";
                }
                return sb.ToString();
            }
        }

        public int? Age
        {
            get
            {
                if (DateOfBirth.HasValue == false) return null;
                var today = DateTime.Today;
                int years = today.Year - DateOfBirth.Value.Year;
                if (
                    (DateOfBirth.Value.Date.Month > today.Month)
                    || (DateOfBirth.Value.Date.Month == today.Month
                        && DateOfBirth.Value.Date.Day > today.Day))
                {
                    years--;
                }
                return years >= 0 ? years : 0;
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return FirstLastName;
        }

        public override bool GetHasChanges()
        {
            return base.GetHasChanges() ||
                this.Credits.Any(o => o.IsDirty
                    || (o.Id > 0 && o.IsMarkedForDeletion)
                    || (o.Id == 0 && !o.IsMarkedForDeletion))
                ||
                this.PersonAttachments.Any(o => o.IsDirty
                    || (o.Id > 0 && o.IsMarkedForDeletion)
                    || (o.Id == 0 && !o.IsMarkedForDeletion));
        }


        #endregion

        #region  IComparable<Person>

        public int CompareTo(Person other)
        {
            int nameCompare = String.Compare(this.LastFirstName,
                other.LastFirstName, true);
            if (nameCompare != 0)
            {
                return nameCompare;
            }
            int suffixCompare = String.Compare(this.Suffix, other.Suffix, true);
            if (suffixCompare != 0)
            {
                return suffixCompare;
            }
            if (this.DateOfBirth.HasValue && other.DateOfBirth.HasValue)
            {
                return this.DateOfBirth.Value < other.DateOfBirth.Value ? -1 : 1;
            }
            return 0;
        }

        #endregion

        #region IDisplayable<Person>

        public string Display()
        {
            return FullName
                + (Age == null ? "" : "(" + Age + ")");
        }

        #endregion
    }
}
