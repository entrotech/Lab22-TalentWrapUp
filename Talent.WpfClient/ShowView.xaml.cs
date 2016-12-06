using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Talent.DataAccess.Ado;
using Talent.Domain;

namespace Talent.WpfClient
{
    /// <summary>
    /// Interaction logic for ShowView.xaml
    /// </summary>
    public partial class ShowView : UserControl
    {
        public ShowView()
        {
            InitializeComponent();
        }

        private void GenresEditButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ShowsViewModel;
            if (vm == null) return;
            var show = vm.SelectedItem as Show;
            var dlg = new GenrePicker();
            dlg.SelectedGenreIds = show.ShowGenres
                .Where(o => o.IsMarkedForDeletion == false)
                .Select(o => o.GenreId).ToList();
            if (dlg.ShowDialog() == true)
            {
                var hasChanges = false;
                var selectedGenreIds = dlg.SelectedGenreIds;
                foreach (var sgid in selectedGenreIds)
                {
                    var showGenre = show.ShowGenres.Where(o =>
                        o.GenreId == sgid).FirstOrDefault();
                    if (showGenre == null)
                    {
                        show.ShowGenres.Add(new ShowGenre { GenreId = sgid });
                        hasChanges = true;
                    }
                    else
                    {
                        showGenre.IsMarkedForDeletion = false;
                    }
                }
                var showGenresToDelete = show.ShowGenres.Where(o =>
                    !selectedGenreIds.Contains(o.GenreId));
                foreach (var sg in showGenresToDelete)
                {
                    sg.IsMarkedForDeletion = true;
                    hasChanges = true;
                }
                // Force an update to the GenresTextBox binding
                // This is necessary because the TextBox does not listen for 
                // INotifyCollectionChanged events.
                BindingOperations.GetBindingExpression(GenresTextBox, TextBox.TextProperty)
                    .UpdateTarget();
                if(hasChanges)
                {
                    ((DomainViewModel<Show>)DataContext).SelectedItem.IsDirty = true;
                }
            }
        }

        private void CreditsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ((DomainViewModel<Show>)DataContext).SelectedItem.IsDirty = true;
        }
    }
}
