using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader
{
    public class FeedDataLoadException : Exception
    {
        public FeedDataLoadException(string msg, Exception ex) : base(msg, ex)
        {
        }
    }
}