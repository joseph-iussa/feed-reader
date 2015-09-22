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
        public ObservableCollection<FeedItem> FeedItemCollection { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Db = new DB();
            Db.Database.Log = (string s) => System.Diagnostics.Trace.TraceInformation(s);

            Db.Feeds.Include(feed => feed.FeedItems).Load();
            Db.FeedItems.Load();

            FeedCollection = Db.Feeds.Local;
            FeedItemCollection = Db.FeedItems.Local;

            var feedViewSource = (CollectionViewSource)(FindResource("FeedViewSource"));
            feedViewSource.Source = FeedCollection;

            var feedItemViewSource = (CollectionViewSource)(FindResource("FeedItemViewSource"));
            feedItemViewSource.Source = FeedItemCollection;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Db.Dispose();
        }
    }
}