using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using FeedReader.Model;

namespace FeedReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public DB Db { get; private set; }
        public ObservableCollection<Feed> FeedCollection { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Db = new DB();
            Db.Feeds.Load();

            FeedCollection = Db.Feeds.Local;

            var feedViewSource = (CollectionViewSource)(FindResource("FeedViewSource"));
            feedViewSource.Source = FeedCollection;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Db.Dispose();
        }
    }
}