using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
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
using System.Xml;
using FeedReader.Model;
using static FeedReader.Utils;

namespace FeedReader
{
    /// <summary>
    /// Interaction logic for FeedEdit.xaml
    /// </summary>
    public partial class FeedEditDialog : Window
    {
        private ListCollectionView feedView;
        private Feed feed;

        public FeedEditDialog()
        {
            InitializeComponent();
            feedView = (ListCollectionView)(((CollectionViewSource)(FindResource("FeedViewSource"))).View);
        }

        internal void BeginAdd()
        {
            feed = (Feed)feedView.AddNew();
        }

        internal void BeginEdit()
        {
            feedView.EditItem(feedView.CurrentItem);
            feed = (Feed)feedView.CurrentEditItem;
        }

        private void ProcessNewFeed(object sender, RoutedEventArgs e)
        {
            SyndicationFeed feedData;

            try
            {
                feedData = LoadFeedDataFromUrl(feed.Url);
            }
            catch (FeedDataLoadException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            feed.Title = string.IsNullOrEmpty(feedData.Title?.Text) ? feed.Url : feedData.Title.Text;
            DateTime feedLastUpdated = feedData.LastUpdatedTime.UtcDateTime;
            // SqlServerCe can't handle DateTime.MinValue, which is value given if LastUpdatedTime
            // is absent from feed.
            if (feedLastUpdated > DateTime.MinValue)
            {
                feed.LastUpdated = feedLastUpdated;
            }

            foreach (var feedDataItem in feedData.Items)
            {
                FeedItem newItem = new FeedItem();
                newItem.Title = feedDataItem.Title?.Text;
                newItem.Url = feedDataItem.Links?.First()?.GetAbsoluteUri().ToString();
                newItem.Summary = feedDataItem.Summary?.Text;
                newItem.Content = feedDataItem.Content?.ToString();
                DateTime itemPublishDate = feedDataItem.PublishDate.UtcDateTime;
                // SqlServerCe can't handle DateTime.MinValue, see above.
                if (itemPublishDate > DateTime.MinValue)
                {
                    newItem.PublishDate = itemPublishDate;
                }
                feed.FeedItems.Add(newItem);
            }
        }

        private void SaveFeed(object sender, RoutedEventArgs e)
        {
            DB db = ((App)Application.Current).Db;

            bool successful = false;
            try
            {
                db.SaveChanges();
                successful = true;
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var entityError in ex.EntityValidationErrors)
                {
                    foreach (var error in entityError.ValidationErrors)
                    {
                        sb.AppendLine($"{error.PropertyName} -- {error.ErrorMessage}");
                    }
                    sb.AppendLine();
                }

                MessageBox.Show(sb.ToString());
            }

            if (successful)
            {
                if (feedView.IsAddingNew)
                {
                    feedView.CommitNew();
                }
                else if (feedView.IsEditingItem)
                {
                    feedView.CommitEdit();
                }

                DialogResult = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (feedView.IsAddingNew)
            {
                feedView.CancelNew();
            }
            else if (feedView.IsEditingItem)
            {
                feedView.CancelEdit();
            }

            // If cancelling dialog then discard new yet unsaved FeedItems added to feed.
            if (!(bool)DialogResult)
            {
                feed.FeedItems.Clear();
            }
        }
    }
}