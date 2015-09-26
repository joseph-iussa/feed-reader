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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LaunchFeedEditDialog(object sender, RoutedEventArgs e)
        {
            FeedEditDialog feedEditDialog = new FeedEditDialog();
            feedEditDialog.Owner = this;

            if (sender == newFeedBtn)
            {
                feedEditDialog.BeginAdd();
            }
            else if (sender == feedEditContextMenuItem || sender == feedList)
            {
                feedEditDialog.BeginEdit();
            }

            bool success = (bool)feedEditDialog.ShowDialog();

            if (success)
            {
                feedList.Items.Refresh();
                feedItemList.Items.Refresh();
            }
        }

        private void LaunchFeedDeleteDialog(object sender, RoutedEventArgs e)
        {
            Feed feed = (Feed)feedList.SelectedItem;

            MessageBoxResult result = MessageBox.Show(
                $"Really delete feed {feed.Title}?", "Delete Feed",
                MessageBoxButton.YesNo, MessageBoxImage.Warning,
                MessageBoxResult.No
            );

            if (result == MessageBoxResult.Yes)
            {
                DB db = ((App)Application.Current).Db;
                db.Feeds.Remove(feed);
                db.SaveChanges();
            }
        }
    }
}