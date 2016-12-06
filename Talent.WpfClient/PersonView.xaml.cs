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
using Talent.Domain;

namespace Talent.WpfClient
{
    public partial class PersonView : UserControl
    {
        public PersonView()
        {
            InitializeComponent();
        }

        private void CreditsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ((DomainViewModel<Person>)DataContext).SelectedItem.IsDirty = true;
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            var strs = (string[])e.Data.GetData(DataFormats.FileDrop);
            string fileName = strs[0];

            ((PeopleViewModel)DataContext).LoadAttachmentFromFile(fileName);

            e.Handled = true;
        }

        private void File_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void File_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

    }
}
