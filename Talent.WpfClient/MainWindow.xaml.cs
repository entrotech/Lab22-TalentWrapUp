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

namespace Talent.WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PeopleViewButton_Click(object sender, RoutedEventArgs e)
        {
            this.MainContentArea.Child = new PeopleView();
        }

        private void ShowViewButton_Click(object sender, RoutedEventArgs e)
        {
            this.MainContentArea.Child = new ShowsView();
        }

        private void GenresViewButton_Click(object sender, RoutedEventArgs e)
        {
            this.MainContentArea.Child = new GenresView();
        }

        private void CreditTypesViewButton_Click(object sender, RoutedEventArgs e)
        {
            this.MainContentArea.Child = new CreditTypesView();
        }

        private void RatingsViewButton_Click(object sender, RoutedEventArgs e)
        {
            this.MainContentArea.Child = new MpaaRatingsView();
        }
  
    }
}
