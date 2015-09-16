using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader.Model
{
    public class Feed
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Url)]
        public string Url { get; set; }
    }

    public class DB : DbContext
    {
        public DbSet<Feed> Feeds { get; set; }
    }
}