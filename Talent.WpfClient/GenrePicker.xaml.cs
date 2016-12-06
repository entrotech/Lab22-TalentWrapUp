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
using System.Windows.Shapes;
using Talent.Domain;

namespace Talent.WpfClient
{
    /// <summary>
    /// Interaction logic for GenrePicker.xaml
    /// </summary>
    public partial class GenrePicker : Window
    {
        public GenrePicker()
        {
            InitializeComponent();
            GenresListBox.ItemsSource = LookupCache.Genres;
        }

        public List<int> SelectedGenreIds
        {
            get
            {
                return GenresListBox.SelectedItems
                .Cast<Genre>().Select(o => o.Id)
                .ToList();
            }
            set
            {
                GenresListBox.SelectedItems.Clear();
                foreach (Genre g in LookupCache.Genres)
                {
                    if (value.Contains(g.Id))
                        GenresListBox.SelectedItems.Add(g);
                }
            }
        }


        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
