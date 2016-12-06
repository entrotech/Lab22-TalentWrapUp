using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UclaExt.Common.BaseClasses;
using UclaExt.Common.Interfaces;

namespace Talent.Domain
{
    public class Show : DomainBase, IDisplayable
    {
        #region Constructor

        #endregion

        #region Fields

        private int _id;
        private string _title;
        private int? _lengthInMinutes;
        private DateTime? _releaseDate;
        private int? _mpaaRatingId;
        private ObservableCollection<ShowGenre> _showGenres = new ObservableCollection<ShowGenre>();
        private ObservableCollection<Credit> _credits = new ObservableCollection<Credit>();

        #endregion

        #region Properties

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Title must be between 2 and 100 characters.")]
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged();
                ValidateProperty(_title);
            }
        }

        [Range(1.0, 300.0)]
        public int? LengthInMinutes
        {
            get { return _lengthInMinutes; }
            set
            {
                if (_lengthInMinutes == value) return;
                _lengthInMinutes = value;
                OnPropertyChanged();
                ValidateProperty(_lengthInMinutes);
            }
        }

        [Range(typeof(DateTime), "1/1/1920", "12/31/2035" )]
        public DateTime? ReleaseDate
        {
            get { return _releaseDate; }
            set
            {
                if (_releaseDate == value) return;
                _releaseDate = value;
                OnPropertyChanged();
                ValidateProperty(_releaseDate);
            }
        }

        public int? MpaaRatingId
        {
            get { return _mpaaRatingId; }
            set
            {
                if (_mpaaRatingId == value) return;
                _mpaaRatingId = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ShowGenre> ShowGenres
        {
            get { return _showGenres; }
        }

        public ObservableCollection<Credit> Credits
        {
            get { return _credits; }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return Title;
        }

        public override bool GetHasChanges()
        {
            return base.GetHasChanges()
                || Credits.Any(o => o.IsDirty
                    || (o.Id == 0 && !o.IsMarkedForDeletion)
                    || (o.Id > 0 && o.IsMarkedForDeletion))
                || ShowGenres.Any(o => o.IsDirty
                    || (o.Id == 0 && !o.IsMarkedForDeletion)
                    || (o.Id > 0 && o.IsMarkedForDeletion));
        }


        #endregion

        #region IDisplayable

        public string Display()
        {
            string msg = Title ?? "";
            if(ReleaseDate.HasValue)
            {
                msg += "(" + ReleaseDate.Value.Year + ")";
            }
            return msg;
        }

        #endregion
    }

}
