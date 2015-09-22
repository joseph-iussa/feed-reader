using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FeedReader
{
    public static class Utils
    {
        public static SyndicationFeed LoadFeedDataFromUrl(string url)
        {
            try
            {
                return SyndicationFeed.Load(XmlReader.Create(url));
            }
            catch (ArgumentNullException)
            {
                throw new FeedDataLoadException("Feed url is empty");
            }
            catch (FileNotFoundException)
            {
                throw new FeedDataLoadException("Could not access feed");
            }
            catch (Exception ex) when (ex is UriFormatException || ex is ArgumentException)
            {
                throw new FeedDataLoadException("Feed url is malformed");
            }
            catch (XmlException)
            {
                throw new FeedDataLoadException("Feed content is invalid");
            }
        }
    }
}