using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using FeedReader.Command;
using FeedReader.Model;
using FeedReader.Repository;

namespace FeedReader.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private DataRepository repo;

        private ObservableCollection<FeedViewModel> feeds;
        private ObservableCollection<FeedItemViewModel> feedItems;

        public MainWindowViewModel(DataRepository repo, ShowDialogDelegate showDialog = null)
            : base(ShowDialog: showDialog)
        {
            this.repo = repo.ThrowIfNull();

            feeds = new ObservableCollection<FeedViewModel>(
                repo.AllFeeds().Select(feed => new FeedViewModel(repo, feed)));
            feedItems = new ObservableCollection<FeedItemViewModel>(
                repo.AllFeedItems().Select(feedItem => new FeedItemViewModel(feedItem)));

            FeedsView = CollectionViewSource.GetDefaultView(feeds);
            FeedsView.CurrentChanged += HandleFeedsViewCurrentChanged;

            FeedItemsView = CollectionViewSource.GetDefaultView(feedItems);
            FeedItemsView.Filter = FeedItemsViewFilter;
            FeedItemsView.SortDescriptions.Add(new SortDescription("PublishDate",
                                                                   ListSortDirection.Descending));

            NewFeedCommand = new RelayCommand(param => NewFeed());
            ModifyFeedCommand = new RelayCommand(param => ModifyFeed());
            ClearFeedItemsFilterCommand = new RelayCommand(param => ClearFeedItemsFilter());

            repo.FeedAdded += HandleFeedAdded;
            repo.FeedModified += HandleFeedModified;
            repo.FeedDeleted += HandleFeedDeleted;
            repo.FeedItemsAdded += HandleFeedItemsAdded;

            FeedsView.MoveCurrentTo(null);
        }

        public ICollectionView FeedsView { get; private set; }
        public ICollectionView FeedItemsView { get; private set; }

        private void HandleFeedsViewCurrentChanged(object sender, EventArgs e)
        {
            FeedItemsView.Refresh();
        }

        private bool FeedItemsViewFilter(object obj)
        {
            if (obj == null) return false;

            FeedItemViewModel feedItem = (FeedItemViewModel)obj;
            FeedViewModel selectedFeed = (FeedViewModel)FeedsView.CurrentItem;
            return selectedFeed == null ? true : feedItem.Feed == selectedFeed;
        }

        public ICommand NewFeedCommand { get; private set; }
        public ICommand ModifyFeedCommand { get; private set; }
        public ICommand ClearFeedItemsFilterCommand { get; private set; }

        private void NewFeed()
        {
            bool? success = ShowDialog("New Feed", new FeedViewModel(repo, new Feed()));
        }

        private void ModifyFeed()
        {
            FeedViewModel feedBeingModified = ((FeedViewModel)FeedsView.CurrentItem);
            feedBeingModified.BeginEdit();
            bool? success = ShowDialog("Edit Feed", feedBeingModified);
            if (!success.GetValueOrDefault(false))
            {
                feedBeingModified.CancelEdit();
            }
        }

        private void ClearFeedItemsFilter()
        {
            FeedsView.MoveCurrentTo(null);
        }

        private void HandleFeedAdded(object sender, FeedAddedEventArgs e)
        {
            feeds.Add(new FeedViewModel(repo, e.AddedFeed));

            foreach (FeedItem feedItem in e.AddedFeed.FeedItems)
            {
                feedItems.Add(new FeedItemViewModel(feedItem));
            }
        }

        private void HandleFeedModified(object sender, FeedModifiedEventArgs e)
        {
            for (int i = 0; i < feeds.Count; i++)
            {
                if (feeds[i] == e.ModifiedFeed)
                {
                    feeds[i] = new FeedViewModel(repo, e.ModifiedFeed);
                    break;
                }
            }
        }

        private void HandleFeedDeleted(object sender, FeedDeletedEventArgs e)
        {
            FeedViewModel toDelete = feeds.Single(feedViewModel => feedViewModel == e.DeletedFeed);
            feeds.Remove(toDelete);

            // TODO: look into a better way to do this.
            for (int i = 0; i < feedItems.Count; i++)
            {
                FeedItemViewModel feedItemVM = feedItems[i];
                if (feedItemVM.Feed == e.DeletedFeed)
                {
                    feedItems[i] = null;
                }
            }

            FeedItemViewModel[] temp = feedItems.ToArray();
            feedItems.Clear();

            foreach (FeedItemViewModel feedItemVM in temp)
            {
                if (feedItemVM != null)
                {
                    feedItems.Add(feedItemVM);
                }
            }
        }

        private void HandleFeedItemsAdded(object sender, FeedItemsAddedEventArgs e)
        {
            foreach (FeedItem addedFeedItem in e.AddedFeedItems)
            {
                feedItems.Add(new FeedItemViewModel(addedFeedItem));
            }
        }
    }
}