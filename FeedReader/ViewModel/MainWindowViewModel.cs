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
        private FeedViewModel selectedFeed;
        private FeedItemViewModel selectedFeedItem;

        public MainWindowViewModel(DataRepository repo, ShowDialogDelegate showDialog = null)
            : base(ShowDialog: showDialog)
        {
            this.repo = repo.ThrowIfNull();

            Feeds = new ObservableCollection<FeedViewModel>(
                repo.AllFeeds().Select(feed => new FeedViewModel(repo, feed)));
            FeedItems = new ObservableCollection<FeedItemViewModel>(
                repo.AllFeedItems().Select(feedItem => new FeedItemViewModel(feedItem)));

            FeedItemsView = CollectionViewSource.GetDefaultView(FeedItems);
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
        }

        public ObservableCollection<FeedViewModel> Feeds { get; private set; }
        public ObservableCollection<FeedItemViewModel> FeedItems { get; private set; }

        public ICollectionView FeedItemsView { get; private set; }

        private bool FeedItemsViewFilter(object obj)
        {
            FeedItemViewModel feedItemVM = (FeedItemViewModel)obj;
            return SelectedFeed == null ? true : feedItemVM.Feed == SelectedFeed;
        }

        // TODO: Remove these selected item props maybe?
        public FeedViewModel SelectedFeed
        {
            get { return selectedFeed; }
            set
            {
                selectedFeed = value;
                NotifyPropertyChanged(nameof(SelectedFeed));
                FeedItemsView.Refresh();
            }
        }

        public FeedItemViewModel SelectedFeedItem
        {
            get { return selectedFeedItem; }
            set
            {
                selectedFeedItem = value;
                NotifyPropertyChanged(nameof(SelectedFeedItem));
            }
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
            SelectedFeed.BeginEdit();
            bool? success = ShowDialog("Edit Feed", SelectedFeed);
            if (!success.GetValueOrDefault(false))
            {
                SelectedFeed.CancelEdit();
            }
        }

        private void ClearFeedItemsFilter()
        {
            SelectedFeed = null;
        }

        // TODO: look into using defered refresh here and below.
        private void HandleFeedAdded(object sender, FeedAddedEventArgs e)
        {
            Feeds.Add(new FeedViewModel(repo, e.AddedFeed));

            foreach (FeedItem feedItem in e.AddedFeed.FeedItems)
            {
                FeedItems.Add(new FeedItemViewModel(feedItem));
            }
        }

        private void HandleFeedModified(object sender, FeedModifiedEventArgs e)
        {
            for (int i = 0; i < Feeds.Count; i++)
            {
                if (Feeds[i] == e.ModifiedFeed)
                {
                    Feeds[i] = new FeedViewModel(repo, e.ModifiedFeed);
                    break;
                }
            }
        }

        private void HandleFeedDeleted(object sender, FeedDeletedEventArgs e)
        {
            FeedViewModel toDelete = Feeds.Single(feedViewModel => feedViewModel == e.DeletedFeed);
            Feeds.Remove(toDelete);

            // TODO: look into a better way to do this.
            for (int i = 0; i < FeedItems.Count; i++)
            {
                FeedItemViewModel feedItemVM = FeedItems[i];
                if (feedItemVM.Feed == e.DeletedFeed)
                {
                    FeedItems[i] = null;
                }
            }

            FeedItemViewModel[] temp = FeedItems.ToArray();
            FeedItems.Clear();

            foreach (FeedItemViewModel feedItemVM in temp)
            {
                if (feedItemVM != null)
                {
                    FeedItems.Add(feedItemVM);
                }
            }
        }

        private void HandleFeedItemsAdded(object sender, FeedItemsAddedEventArgs e)
        {
            foreach (FeedItem addedFeedItem in e.AddedFeedItems)
            {
                FeedItems.Add(new FeedItemViewModel(addedFeedItem));
            }
        }
    }
}