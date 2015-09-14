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
        public DB db { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            db = new DB();
            Closing += MainWindow_Closing;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource feedListSource = ((CollectionViewSource)(FindResource("feedListSource")));
            db.Feeds.OrderBy(f => f.Title).Load();
            feedListSource.Source = db.Feeds.Local;
        }

        private void LaunchFeedEditWindow(object sender, RoutedEventArgs e)
        {
            FeedEdit feedEditWindow = new FeedEdit();
            feedEditWindow.Owner = this;
            feedEditWindow.DataContext = feedListView.SelectedItem;
            bool successful = (bool)feedEditWindow.ShowDialog();
            if (successful)
            {
                feedListView.Items.Refresh();
            }
        }

        private void MainWindow_Closing(object sender, EventArgs e)
        {
            db.Dispose();
        }
    }
}