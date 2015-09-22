using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader.Model
{
    public class Feed : IEditableObject, INotifyPropertyChanged
    {
        private string title;
        private string url;

        private Feed cache;

        public int ID { get; set; }
        public virtual ObservableCollection<FeedItem> FeedItems { get; private set; }

        [Required]
        [StringLength(100)]
        public string Title
        {
            get { return title; }
            set
            {
                if (value != title)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        [Required]
        [DataType(DataType.Url)]
        public string Url
        {
            get { return url; }
            set
            {
                if (value != url)
                {
                    url = value;
                    NotifyPropertyChanged("Url");
                }
            }
        }

        public DateTime? LastUpdated { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Feed()
        {
            FeedItems = new ObservableCollection<FeedItem>();
        }

        public void BeginEdit()
        {
            cache = new Feed { Title = this.Title, Url = this.Url };
        }

        public void CancelEdit()
        {
            Title = cache.Title;
            Url = cache.Url;
            cache = null;
            NotifyPropertyChanged("");
        }

        public void EndEdit()
        {
            cache = null;
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class FeedItem
    {
        public int ID { get; set; }
        public virtual Feed Feed { get; set; }

        public string Title { get; set; }
        public string Url { get; set; }

        [Column(TypeName = "ntext")]
        // SqlServerCe needs this to actually allow strings longer than 4000 chars into the db.
        [MaxLength]
        public string Summary { get; set; }

        [Column(TypeName = "ntext")]
        [MaxLength] // See above.
        public string Content { get; set; }

        public DateTime? PublishDate { get; set; }
        public bool IsRead { get; set; } = false;
    }

    public class DB : DbContext
    {
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<FeedItem> FeedItems { get; set; }
    }
}