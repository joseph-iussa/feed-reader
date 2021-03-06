﻿using System;
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
        // TODO: Fix feed uri validation.
        public static SyndicationFeed LoadFeedDataFromUrl(string url)
        {
            try
            {
                return SyndicationFeed.Load(XmlReader.Create(url));
            }
            catch (ArgumentNullException ex)
            {
                throw new FeedDataLoadException("Feed url is empty", ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new FeedDataLoadException("Could not access feed", ex);
            }
            catch (Exception ex) when (ex is UriFormatException || ex is ArgumentException)
            {
                throw new FeedDataLoadException("Feed url is malformed", ex);
            }
            catch (XmlException ex)
            {
                throw new FeedDataLoadException("Feed content is invalid", ex);
            }
        }

        /// <summary>
        /// If the (reference type) object this method acts on is null, throws an <see
        /// cref="System.ArgumentNullException"/>. Otherwise, returns the object unchanged.
        /// </summary>
        public static T ThrowIfNull<T>(this T t) where T : class
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            return t;
        }
    }
}