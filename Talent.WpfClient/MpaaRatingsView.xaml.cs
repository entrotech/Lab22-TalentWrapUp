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
    /// Interaction logic for MpaaRatingsView.xaml
    /// </summary>
    public partial class MpaaRatingsView : UserControl
    {
        public MpaaRatingsView()
        {
            InitializeComponent();
            DataContext = new MpaaRatingsViewModel();
        }
    }
}
