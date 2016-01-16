using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FeedReader.Command;
using FeedReader.Model;
using FeedReader.Repository;
using static FeedReader.Utils;

namespace FeedReader.ViewModel
{
    class FeedViewModel : StaticFeedViewModel, IEditableObject
    {
        private DataRepository repo;

        public FeedViewModel(DataRepository repo, Feed feed) : base(feed)
        {
            this.repo = repo.ThrowIfNull();
            ProcessFeedCommand = new RelayCommand(param => ProcessFeed(), param => true);
            SaveFeedCommand = new RelayCommand(param => SaveFeed(), param => true);
            DeleteFeedCommand = new RelayCommand(param => DeleteFeed());
        }

        public new string Title
        {
            get { return feed.Title; }
            set
            {
                if (value != feed.Title)
                {
                    feed.Title = value;
                    NotifyPropertyChanged(nameof(Title));
                }
            }
        }

        public new string Url
        {
            get { return feed.Url; }
            set
            {
                if (value != feed.Url)
                {
                    feed.Url = value;
                    NotifyPropertyChanged(nameof(Url));
                }
            }
        }

        public ICommand ProcessFeedCommand { get; private set; }
        public ICommand SaveFeedCommand { get; private set; }
        public ICommand DeleteFeedCommand { get; private set; }

        private void SaveFeed()
        {
            if (repo.FeedExists(feed))
            {
                repo.ModifyFeed(feed);
            }
            else
            {
                repo.AddFeed(feed);
            }

            // Assuming successful.
            CloseDialog(new DialogClosingEventArgs(true));
        }

        private void ProcessFeed()
        {
            SyndicationFeed feedData;

            try
            {
                feedData = LoadFeedDataFromUrl(feed.Url);
            }
            catch (FeedDataLoadException ex)
            {
                InteractionService.ShowMessage(ex.Message);
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
                newItem.PopulateHtmlFields();
                feed.FeedItems.Add(newItem);
            }

            NotifyPropertyChanged("");
        }

        private void DeleteFeed()
        {
            if (InteractionService.RequestConfirmation($"Really Delete feed {feed.Title}?", "Delete Feed"))
            {
                repo.DeleteFeed(feed);
            }
        }

        public void BeginEdit()
        {
            ((IEditableObject)feed).BeginEdit();
        }

        public void EndEdit()
        {
            ((IEditableObject)feed).EndEdit();
        }

        public void CancelEdit()
        {
            ((IEditableObject)feed).CancelEdit();
            NotifyPropertyChanged("");
        }
    }
}