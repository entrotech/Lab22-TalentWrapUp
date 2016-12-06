using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UclaExt.Common.BaseClasses;

namespace Talent.Domain
{
    /// <summary>
    /// Association between a Show and a Genre
    /// </summary>
    public class ShowGenre : DomainBase
    {
        #region Fields

        private int _id;
        private int _showId;
        private int _genreId;

        #endregion

        #region Properties

        public int Id
        {
            get { return _id; }
            set {_id = value; }
        }

        public int ShowId
        {
            get { return _showId; }
            set
            {
                if (_showId == value) return;
                _showId = value;
                OnPropertyChanged();
            }
        }

        public int GenreId
        {
            get { return _genreId; }
            set
            {
                if (_genreId == value) return;
                _genreId = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}