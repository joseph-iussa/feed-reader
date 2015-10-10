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
    public class Feed : IEditableObject
    {
        private Feed cache;

        public int ID { get; set; }
        public virtual ObservableCollection<FeedItem> FeedItems { get; private set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        public DateTime? LastUpdated { get; set; }

        public Feed()
        {
            FeedItems = new ObservableCollection<FeedItem>();
        }

        public void BeginEdit()
        {
            cache = new Feed
            {
                Title = this.Title,
                Url = this.Url,
                FeedItems = new ObservableCollection<FeedItem>(this.FeedItems)
            };
        }

        public void CancelEdit()
        {
            Title = cache.Title;
            Url = cache.Url;
            FeedItems = cache.FeedItems;
            cache = null;
        }

        public void EndEdit()
        {
            cache = null;
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class FeedItem
    {
        public int ID { get; set; }

        [Required]
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

        // Html properties for display.
        [Column(TypeName = "ntext")]
        [MaxLength]
        public string HtmlHeader { get; set; }

        [Column(TypeName = "ntext")]
        [MaxLength]
        public string HtmlMainContent { get; set; }

        public void PopulateHtmlFields()
        {
            HtmlHeader = $"<h1><a href=\"{Url}\">{Title}</a></h1>";
            HtmlMainContent = $"<div>{Summary}</div> <hr> <div>{Content}</div>";
        }
    }

    public class DB : DbContext
    {
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<FeedItem> FeedItems { get; set; }
    }
}