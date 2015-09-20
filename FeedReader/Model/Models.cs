using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        public event PropertyChangedEventHandler PropertyChanged;

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
    }

    public class DB : DbContext
    {
        public DbSet<Feed> Feeds { get; set; }
    }
}