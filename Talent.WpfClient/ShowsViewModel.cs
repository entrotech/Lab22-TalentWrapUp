using System.Collections.Generic;
using System.Collections.ObjectModel;
using Talent.DataAccess.Ado;
using Talent.Domain;

namespace Talent.WpfClient
{
    public class ShowsViewModel : DomainViewModel<Show>
    {

        public ShowsViewModel() : base(new ShowRepository())
        {
            ShowCriteria = new ShowCriteria { MpaaRatingId = 0 };
            var list = new List<MpaaRating>(LookupCache.MpaaRatings);
            list.Insert(0, new MpaaRating { Id = 0, Code = "(Any)", Name = "(Any" });
            MpaaRatingsCriterion = new ObservableCollection<MpaaRating>(list);
        }

        public ShowCriteria ShowCriteria { get; private set;}

        protected override void OnSearch()
        {
            Items = new ObservableCollection<Show>(_repo.Fetch(ShowCriteria));
            RaiseAllCanExecuteChanged();
        }

        public ObservableCollection<MpaaRating> MpaaRatingsCriterion
        {
            get; private set;
        }

        public ObservableCollection<MpaaRating> MpaaRatings
        {
            get
            {
                return new ObservableCollection<MpaaRating>(
                    LookupCache.MpaaRatings);
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

        public ObservableCollection<Person> People
        {
            get
            {
                return new ObservableCollection<Person>(
                    LookupCache.People);
            }
        }

        public ObservableCollection<Genre> Genres
        {
            get
            {
                return new ObservableCollection<Genre>(
                    LookupCache.Genres);
            }
        }

    }
}
