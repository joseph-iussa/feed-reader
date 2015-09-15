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
using System.Windows.Shapes;
using FeedReader.Model;

namespace FeedReader
{
    /// <summary>
    /// Interaction logic for FeedEdit.xaml
    /// </summary>
    public partial class FeedEdit : Window
    {
        public FeedEdit()
        {
            InitializeComponent();
        }

        private void saveFeed_Click(object sender, RoutedEventArgs e)
        {
            var db = ((MainWindow)Owner).db;
            Feed feed = (Feed)DataContext;

            // New Feed, not yet tracked.
            bool feedIsNew = db.Entry(feed).State == EntityState.Detached;
            if (feedIsNew)
            {
                db.Feeds.Add(feed);
            }

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

                if (feedIsNew)
                {
                    db.Feeds.Remove(feed);
                }

                MessageBox.Show(sb.ToString());
            }

            if (successful)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}