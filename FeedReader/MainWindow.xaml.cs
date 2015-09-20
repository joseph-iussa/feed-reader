using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
using FeedReader.Model;

namespace FeedReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FeedEditDialog feedEditDialog;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            feedEditDialog = new FeedEditDialog();
            feedEditDialog.Owner = this;
        }

        private void LaunchFeedEditDialog(object sender, RoutedEventArgs e)
        {
            if (sender == newFeedBtn)
            {
                feedEditDialog.BeginAdd();
            }
            else if (sender == feedEditContextMenuItem || sender == feedListView)
            {
                feedEditDialog.BeginEdit();
            }

            feedEditDialog.ShowDialog();
        }
    }
}