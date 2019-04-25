﻿/// Author:					 and Walter Ferrari
/// Created:				2008-09-27
/// Last Modified:			2017-03-15
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.

using System;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Text;
using System.Threading;
using log4net;
using Argotic.Common;
using Argotic.Syndication;
using mojoPortal.Web.Framework;

namespace mojoPortal.Web.FeedUI
{
    public static class FeedCache
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FeedCache));

        private static bool debugLog = log.IsDebugEnabled;

        private static ReaderWriterLock cacheLock = new ReaderWriterLock();
        private const int cacheLockTimeoutInMilliseconds = 2000; //2 seconds

        
        private static string FormatFeedUrl(string feedUrl, string siteRoot, string secureSiteRoot)
        {
            Guid internalSecurtyByPassKey = WebConfigSettings.InternalFeedSecurityBypassKey;

            if (internalSecurtyByPassKey != Guid.Empty)
            {
                if((feedUrl.StartsWith(siteRoot))||(feedUrl.StartsWith(secureSiteRoot)))
                {
                    //if (feedUrl.EndsWith(".aspx"))
                    //{
                    //    return feedUrl + "?g=" + internalSecurtyByPassKey.ToString();
                    //}
                    //// the above does not add the required key to a mojoPortal blog feed with URL like 
                    //// https://mysite.com/Blog/RSS.aspx?p=200~180~-1
                    //else
                    //{
                    //    // assume we already have qrystr param
                    //    return feedUrl + "&g=" + internalSecurtyByPassKey.ToString();
                    //}

                    string qryAppendage = feedUrl.Contains("?") ? "&g=" : "?g=";

                    return feedUrl + qryAppendage + internalSecurtyByPassKey.ToString();
                }

            }

            return feedUrl;
        }

        private static DateTime EnsureDate(DateTime d)
        {
            if (d == null) { return DateTime.UtcNow; }
            if (d == DateTime.MinValue) { return DateTime.UtcNow; }
            if (d == DateTime.MaxValue) { return DateTime.UtcNow; }
            return d;
        }

        

        public static DataTable GetRssFeedEntries(
            int moduleId,
            Guid moduleGuid,
            int entryCacheTimeout,
            int maxDaysOld,
            int maxEntriesPerFeed,
            bool enableSelectivePublishing)
        {
            DateTime cutoffDate = DateTime.UtcNow.AddDays(-maxDaysOld);
            DateTime cacheExpiration = DateTime.UtcNow.AddMinutes(-entryCacheTimeout);
            DateTime lastCacheTime = mojoPortal.Business.RssFeed.GetLastCacheTime(moduleGuid);

            if (lastCacheTime > cacheExpiration)
            {
                // data "cached" in the db has not expired so just return it
                return mojoPortal.Business.RssFeed.GetEntries(moduleGuid);
            }

            
            try
            {
                if (FeedManagerConfiguration.UseReadWriteLockForCacheMenagement)
                {
                    cacheLock.AcquireWriterLock(cacheLockTimeoutInMilliseconds);
                }
                //if (debugLog) { log.Debug("got lock in GetRssFeeds"); }

                lastCacheTime = mojoPortal.Business.RssFeed.GetLastCacheTime(moduleGuid);

                if (lastCacheTime > cacheExpiration)
                {
                    // data "cached" in the db has not expired so just return it
                    return mojoPortal.Business.RssFeed.GetEntries(moduleGuid);
                }

                if (enableSelectivePublishing)
                {
                    mojoPortal.Business.RssFeed.DeleteExpiredEntriesByModule(moduleGuid, cutoffDate);
                    mojoPortal.Business.RssFeed.DeleteUnPublishedEntriesByModule(moduleGuid);
                }
                else
                {
                    mojoPortal.Business.RssFeed.DeleteEntriesByModule(moduleGuid);
                }

                DataTable dtFeeds = mojoPortal.Business.RssFeed.GetFeeds(moduleId);

                string siteRoot = SiteUtils.GetNavigationSiteRoot();
                string secureSiteRoot = SiteUtils.GetSecureNavigationSiteRoot();

                foreach (DataRow dr in dtFeeds.Rows)
                {
                    Guid feedGuid = new Guid(dr["ItemGuid"].ToString());
                    int feedId = Convert.ToInt32(dr["ItemID"]);
                    string feedUrl = dr["RssUrl"].ToString();
                    if (feedUrl.StartsWith("~/"))
                    {
                        feedUrl = WebUtils.ResolveServerUrl(feedUrl).Replace("https:", "http:");
                    }
                    bool publishByDefault = Convert.ToBoolean(dr["PublishByDefault"]);
                    int countOfPreservedEntries = Convert.ToInt32(dr["TotalEntries"]);

                    bool publish = true;
                    if (enableSelectivePublishing)
                    {
                        if (!publishByDefault)
                        {
                            publish = false;
                        }
                    }

                    int entriesAdded = countOfPreservedEntries;

                    try
                    {
                        GenericSyndicationFeed gsFeed = GenericSyndicationFeed.Create(new Uri(FormatFeedUrl(feedUrl, siteRoot, secureSiteRoot)));

                        #region RSSFeed_management
                        if (gsFeed.Format == SyndicationContentFormat.Rss)
                        {
                            Argotic.Syndication.RssFeed rssFeed = gsFeed.Resource as Argotic.Syndication.RssFeed;
                            if (rssFeed != null)
                            {

                                foreach (Argotic.Syndication.RssItem rssItem in rssFeed.Channel.Items)
                                {
                                    DateTime itemPubDate = EnsureDate(rssItem.PublicationDate);

                                    if ((itemPubDate >= cutoffDate) || (maxDaysOld == 0))
                                    {
                                        if ((entriesAdded < maxEntriesPerFeed) || (maxEntriesPerFeed == 0))
                                        {
                                            string entryBlob = rssItem.Title + rssItem.Link.ToString();
                                            int entryHash = GetEntryHash(entryBlob);
                                            string channelLink = string.Empty;
                                            if ((rssFeed.Channel != null)&&(rssFeed.Channel.Link != null))
                                            {
                                                channelLink = rssFeed.Channel.Link.ToString();
                                            }
                                            


                                            if (UpdateEntry(
                                                moduleGuid,
                                                itemPubDate,
                                                rssItem.Title,
                                                rssItem.Author,
                                                channelLink,
                                                rssItem.Description,
                                                rssItem.Link.ToString(),
                                                entryHash,
                                                feedGuid,
                                                feedId,
                                                publish) > 0)
                                            {
                                                entriesAdded += 1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region ATOMFeed_management
                        if (gsFeed.Format == SyndicationContentFormat.Atom)
                        {
                            Argotic.Syndication.AtomFeed atomFeed = gsFeed.Resource as Argotic.Syndication.AtomFeed;

                            foreach (AtomEntry atItem in atomFeed.Entries)
                            {
                                string entryLink = string.Empty;
                                StringBuilder entryAuthor = new StringBuilder();
                                string comma = string.Empty;
                                DateTime itemPubDate = EnsureDate(atItem.UpdatedOn);

                                if ((itemPubDate >= cutoffDate) || (maxDaysOld == 0))
                                {
                                    if ((entriesAdded < maxEntriesPerFeed) || (maxEntriesPerFeed == 0))
                                    {
                                        foreach (AtomPersonConstruct atPerson in atItem.Authors)
                                        {
                                            entryAuthor.Append(comma + atPerson.Name);
                                            comma = ",";
                                        }

                                        if (entryAuthor.Length == 0)
                                        {
                                            foreach (AtomPersonConstruct atPerson in atomFeed.Authors)
                                            {
                                                entryAuthor.Append(comma + atPerson.Name);
                                                comma = ",";
                                            }
                                        }

                                        foreach (AtomLink atLink in atItem.Links)
                                        {
                                            if (atLink.Relation == "alternate")
                                            {
                                                entryLink = atLink.Uri.ToString();
                                            }
                                        }

                                        if ((entryLink.Length == 0) && (atItem.Links.Count > 0))
                                        {
                                            entryLink = atItem.Links[0].Uri.ToString();

                                        }

                                        string content = string.Empty;
                                        if (atItem.Content == null)
                                        {
                                            if (atItem.Summary != null)
                                            {
                                                content = atItem.Summary.Content;
                                            }
                                        }
                                        else
                                        {
                                            content = atItem.Content.Content;
                                        }

                                        // commented out 2010-02-27 some feeds have only a title and link
                                        //if (content.Length == 0) { continue; }

                                        string entryBlob = atItem.Title.Content + entryLink;
                                        int entryHash = GetEntryHash(entryBlob);

                                        if (UpdateEntry(
                                            moduleGuid,
                                            itemPubDate,
                                            atItem.Title.Content,
                                            entryAuthor.ToString(),
                                            feedUrl,
                                            content,
                                            entryLink,
                                            entryHash,
                                            feedGuid,
                                            feedId,
                                            publish) > 0)
                                        {
                                            entriesAdded += 1;
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                    catch (WebException ex)
                    {
                        if (log.IsErrorEnabled)
                        {
                            string logMsg = String.Format("There was a problem trying to read the feed for url {0}.  Ignoring.", (string)dr["RssUrl"]);
                            log.Error(logMsg, ex);
                        }
                    }
                    catch (UriFormatException ex)
                    {
                        if (log.IsErrorEnabled)
                        {
                            string logMsg = String.Format("There was a problem trying to read the feed for url {0}.  Ignoring.", (string)dr["RssUrl"]);
                            log.Error(logMsg, ex);
                        }
                    }
                    catch (System.Net.Sockets.SocketException ex)
                    {
                        if (log.IsErrorEnabled)
                        {
                            string logMsg = String.Format("There was a problem trying to read the feed for url {0}.  Ignoring.", (string)dr["RssUrl"]);
                            log.Error(logMsg, ex);
                        }
                    }
                    catch (System.Xml.XmlException ex)
                    {
                        if (log.IsErrorEnabled)
                        {
                            string logMsg = String.Format("There was a problem trying to read the feed for url {0}.  Ignoring.", (string)dr["RssUrl"]);
                            log.Error(logMsg, ex);
                        }
                    }
                    catch (System.Security.SecurityException ex)
                    {
                        log.Error("Could not load feed due to security exception. Must be running in restricted trust level. Creating server side web requests is not allowed in current configuration.", ex);

                    }
                    catch (ArgumentNullException ex)
                    {
                        string logMsg = String.Format("There was a problem trying to read the feed for url {0}.  Ignoring.", (string)dr["RssUrl"]);
                        log.Error(logMsg, ex);
                    }
                    catch (System.Data.Common.DbException ex)
                    {
                        log.Error("Error updating feed database cache", ex);

                    }

                }
            }
            finally
            {
                if (FeedManagerConfiguration.UseReadWriteLockForCacheMenagement)
                {
                    try
                    {
                        cacheLock.ReleaseWriterLock();
                    }
                    catch (ApplicationException ex)
                    {
                        log.Error("swallowed error", ex);
                    }
                }
            }

            return mojoPortal.Business.RssFeed.GetEntries(moduleGuid);

        }

        public static void RefreshFeed(
            mojoPortal.Business.RssFeed feedInfo,
            int moduleId,
            Guid moduleGuid,
            int maxDaysOld,
            int maxEntriesPerFeed,
            bool enableSelectivePublishing)
        {
            if (feedInfo == null) { return; }

            try
            {
                if (FeedManagerConfiguration.UseReadWriteLockForCacheMenagement)
                {
                    cacheLock.AcquireWriterLock(cacheLockTimeoutInMilliseconds);
                }

                DateTime cutoffDate = DateTime.Now.AddDays(-maxDaysOld);

                mojoPortal.Business.RssFeed.DeleteExpiredEntriesByModule(moduleGuid, cutoffDate);
                mojoPortal.Business.RssFeed.DeleteUnPublishedEntriesByFeed(feedInfo.ItemId);

                int entriesAdded = 0;
                string siteRoot = SiteUtils.GetNavigationSiteRoot();
                string secureSiteRoot = SiteUtils.GetSecureNavigationSiteRoot();

                bool publish = true;
                if (enableSelectivePublishing)
                {
                    if (!feedInfo.PublishByDefault)
                    {
                        publish = false;
                    }
                }

                string feedUrl = feedInfo.RssUrl;
                if (feedUrl.StartsWith("~/"))
                {
                    feedUrl = WebUtils.ResolveServerUrl(feedUrl).Replace("https:", "http:");
                }

                try
                {
                    GenericSyndicationFeed gsFeed = GenericSyndicationFeed.Create(new Uri(FormatFeedUrl(feedUrl, siteRoot, secureSiteRoot)));

                    #region RSSFeed_management
                    if (gsFeed.Format == SyndicationContentFormat.Rss)
                    {
                        Argotic.Syndication.RssFeed rssFeed = gsFeed.Resource as Argotic.Syndication.RssFeed;
                        if (rssFeed != null)
                        {

                            foreach (Argotic.Syndication.RssItem rssItem in rssFeed.Channel.Items)
                            {
                                if ((rssItem.PublicationDate >= cutoffDate) || (maxDaysOld == 0))
                                {
                                    if ((entriesAdded < maxEntriesPerFeed) || (maxEntriesPerFeed == 0))
                                    {

                                        string entryBlob = rssItem.Title + rssItem.Link.ToString();
                                        int entryHash = GetEntryHash(entryBlob);

                                        if (UpdateEntry(
                                            moduleGuid,
                                            EnsureDate(rssItem.PublicationDate),
                                            rssItem.Title,
                                            rssItem.Author,
                                            feedInfo.RssUrl,
                                            rssItem.Description,
                                            rssItem.Link.ToString(),
                                            entryHash,
                                            feedInfo.ItemGuid,
                                            feedInfo.ItemId,
                                            publish) > 0)
                                        {
                                            entriesAdded += 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region ATOMFeed_management
                    if (gsFeed.Format == SyndicationContentFormat.Atom)
                    {
                        Argotic.Syndication.AtomFeed atomFeed = gsFeed.Resource as Argotic.Syndication.AtomFeed;

                        foreach (AtomEntry atItem in atomFeed.Entries)
                        {

                            if ((atItem.PublishedOn >= cutoffDate) || (maxDaysOld == 0))
                            {
                                if ((entriesAdded < maxEntriesPerFeed) || (maxEntriesPerFeed == 0))
                                {

                                    string entryLink = string.Empty;
                                    StringBuilder entryAuthor = new StringBuilder();
                                    string comma = string.Empty;

                                    foreach (AtomPersonConstruct atPerson in atItem.Authors)
                                    {
                                        entryAuthor.Append(comma + atPerson.Name);
                                        comma = ",";
                                    }

                                    if (entryAuthor.Length == 0)
                                    {
                                        foreach (AtomPersonConstruct atPerson in atomFeed.Authors)
                                        {
                                            entryAuthor.Append(comma + atPerson.Name);
                                            comma = ",";
                                        }

                                    }

                                    foreach (AtomLink atLink in atItem.Links)
                                    {
                                        if (atLink.Relation == "alternate")
                                        {
                                            entryLink = atLink.Uri.ToString();
                                        }
                                    }

                                    if ((entryLink.Length == 0) && (atItem.Links.Count > 0))
                                    {
                                        entryLink = atItem.Links[0].Uri.ToString();

                                    }

                                    string content = string.Empty;
                                    if (atItem.Content == null)
                                    {
                                        if (atItem.Summary != null)
                                        {
                                            content = atItem.Summary.Content;
                                        }
                                    }
                                    else
                                    {
                                        content = atItem.Content.Content;
                                    }

                                    if (content.Length == 0) { continue; }

                                    string entryBlob = atItem.Title.ToString() + entryLink;
                                    int entryHash = GetEntryHash(entryBlob);

                                    if (UpdateEntry(
                                        moduleGuid,
                                        EnsureDate(atItem.PublishedOn),
                                        atItem.Title.ToString(),
                                        entryAuthor.ToString(),
                                        feedInfo.RssUrl,
                                        content,
                                        entryLink,
                                        entryHash,
                                        feedInfo.ItemGuid,
                                        feedInfo.ItemId,
                                        publish) > 0)
                                    {
                                        entriesAdded += 1;
                                    }


                                }

                            }

                        }

                    }

                    #endregion
                }
                catch (WebException ex)
                {
                    if (log.IsErrorEnabled)
                    {
                        string logMsg = String.Format("There was a problem trying to read the feed for url {0}.  Ignoring.", feedInfo.RssUrl);
                        log.Error(logMsg, ex);
                    }
                }
                catch (UriFormatException ex)
                {
                    if (log.IsErrorEnabled)
                    {
                        string logMsg = String.Format("There was a problem trying to read the feed for url {0}.  Ignoring.", feedInfo.RssUrl);
                        log.Error(logMsg, ex);
                    }
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    if (log.IsErrorEnabled)
                    {
                        string logMsg = String.Format("There was a problem trying to read the feed for url {0}.  Ignoring.", feedInfo.RssUrl);
                        log.Error(logMsg, ex);
                    }
                }
                catch (System.Xml.XmlException ex)
                {
                    if (log.IsErrorEnabled)
                    {
                        string logMsg = String.Format("There was a problem trying to read the feed for url {0}.  Ignoring.", feedInfo.RssUrl);
                        log.Error(logMsg, ex);
                    }
                }
                catch (System.Security.SecurityException ex)
                {
                    log.Error("Could not load feed due to security exception. Must be running in restricted trust level. Creating server side web requests is not allowed in current configuration.", ex);

                }
                catch (System.Data.Common.DbException ex)
                {
                    log.Error("Error updating feed database cache", ex);

                }
            }
            finally
            {
                if (FeedManagerConfiguration.UseReadWriteLockForCacheMenagement)
                {
                    try
                    {
                        cacheLock.ReleaseWriterLock();
                    }
                    catch (ApplicationException ex)
                    {
                        log.Error("swallowed error", ex);
                    }
                }
            }

        }

       
        private static int GetEntryHash(string entryHash)
        {
            return entryHash.GetHashCode();
        }

        private static int UpdateEntry(
            Guid moduleGuid, 
            DateTime entryPublishedOn, 
            string entryTitle,              
            string entryAuthor, 
            string feedUrl, 
            string entryDescription,
            string entryLink, 
            int entryHash, 
            Guid itemGuid, 
            int itemId, 
            bool publish)
        {
            if (mojoPortal.Business.RssFeed.EntryExists(moduleGuid, entryHash))
            {
                bool bRes = mojoPortal.Business.RssFeed.UpdateEnry(
                     moduleGuid,
                     entryTitle,
                     entryAuthor,
                     feedUrl,
                     entryDescription,
                     entryLink,
                     entryHash,
                     DateTime.UtcNow);
                if (bRes)
                    return 1;
                else
                    return -1;
            }
            else
            {
                int res= -1;
                try
                {
                    res = mojoPortal.Business.RssFeed.CreateEntry(
                         Guid.NewGuid(),
                         moduleGuid,
                         itemGuid,
                         itemId,
                         entryPublishedOn,
                         entryTitle,
                         entryAuthor,
                         feedUrl,
                         entryDescription,
                         entryLink,
                         publish,
                         entryHash,
                         DateTime.UtcNow);
                }
                catch (DbException ex)
                {
                    log.Error(ex);
                    log.Info("last error produced by post " + entryLink + " contents: " + entryDescription);
                }

                return res;
            }


        }

    }


}
