using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UclaExt.Common.BaseClasses;

namespace Talent.Domain
{
    /// <summary>
    /// Represent an association between a Person and a Show, giving them credit for 
    /// their role in the Show.
    /// </summary>
    /// <remarks>
    /// Character is only really applicable for the Actor CreditType.
    /// </remarks>
    public class Credit : DomainBase
    {
        #region Fields

        private int _id;
        private int _showId;
        private int _personId;
        private int _creditTypeId;
        private string _character = String.Empty;

        #endregion

        #region Properties

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        public int ShowId
        {
            get { return _showId; }
            set
            {
                if (_showId == value) return;
                _showId = value;
                OnPropertyChanged();
                ValidateProperty(_showId);
            }
        }

        [Required]
        public int PersonId
        {
            get { return _personId; }
            set
            {
                if (_personId == value) return;
                _personId = value;
                OnPropertyChanged();
                ValidateProperty(_personId);
            }
        }

        [Required]
        public int CreditTypeId
        {
            get { return _creditTypeId; }
            set
            {
                if (_creditTypeId == value) return;
                _creditTypeId = value;
                OnPropertyChanged();
                ValidateProperty(_creditTypeId);
            }
        }

        public string Character
        {
            get { return _character; }
            set
            {
                if (_character == value) return;
                _character = value;
                OnPropertyChanged();
            }
        }
        
        #endregion
    }
}
