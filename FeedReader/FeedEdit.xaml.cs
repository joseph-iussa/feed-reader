using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.Xml.Linq;
using FeedReader.Model;

namespace FeedReader
{
    /// <summary>
    /// Interaction logic for FeedEdit.xaml
    /// </summary>
    public partial class FeedEdit : Window
    {
        private ListCollectionView feedView;
        private Feed feed;

        public FeedEdit()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
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
            // Validate Url.
            if (string.IsNullOrWhiteSpace(feed.Url)
                || !Uri.IsWellFormedUriString(feed.Url, UriKind.Absolute))
            {
                MessageBox.Show("Invalid Feed Url");
                return;
            }

            XElement feedXml = XElement.Load(feed.Url);
            feed.Title = feedXml.Element("channel").Element("title").Value;
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
        }
    }
}