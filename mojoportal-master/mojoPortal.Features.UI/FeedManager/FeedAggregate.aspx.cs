﻿/// Author:				
/// Created:			2005-04-02
/// Last Modified:		2010-10-18
/// 
/// Based on code sample by Joseph Hill
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Xml;
using mojoPortal.Business;
using mojoPortal.Business.WebHelpers;
using mojoPortal.Web.Framework;
using Argotic.Syndication;

namespace mojoPortal.Web.FeedUI
{
    public partial class FeedAggregatePage : System.Web.UI.Page
    {
        private int feedListCacheTimeout = 3660;
        private int entryCacheTimeout = 3620;
        private int maxDaysOld = 90;
        private int maxEntriesPerFeed = 90;
        protected bool EnableSelectivePublishing = false;
        private string cssBaseUrl = string.Empty;
        private int moduleId = -1;
        private PageSettings pageSettings = null;
        private Module module = null;
        private Hashtable moduleSettings = null;
        private Guid securityBypassGuid = Guid.Empty;
        private bool canView = false;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // nothing should post here
            if (Page.IsPostBack) return;

            LoadSettings();

            if (canView)
            {
                RenderRss();

            }
            else
            {
                RenderError("Invalid Request");
            }

        }

        private void RenderRss()
        {
            
            DataView dv = FeedCache.GetRssFeedEntries(
                module.ModuleId,
                module.ModuleGuid, 
                entryCacheTimeout,
                maxDaysOld, 
                maxEntriesPerFeed, 
                EnableSelectivePublishing).DefaultView;

            dv.Sort = "PubDate DESC";

            if (dv.Table.Rows.Count == 0) { return; }


            Argotic.Syndication.RssFeed feed = new Argotic.Syndication.RssFeed();

            RssChannel channel = new RssChannel();
            channel.Generator = "mojoPortal Feed Manager module";
            feed.Channel = channel;

            
            if (module != null)
            {
                channel.Title = module.ModuleTitle;
                channel.Description = module.ModuleTitle;
                //channel.LastBuildDate = channel.Items.LatestPubDate();
                try
                {
                    channel.Link = new System.Uri(WebUtils.ResolveServerUrl(SiteUtils.GetCurrentPageUrl()));
                }
                catch (UriFormatException) 
                {
                    channel.Link = new System.Uri(SiteUtils.GetNavigationSiteRoot());
                }

            }
            else
            {
                // this prevents an error: Can't close RssWriter without first writing a channel. 
                channel.Title = "Not Found";
                channel.Description = "Not Found";
                channel.LastBuildDate = DateTime.UtcNow;
                //channel.Link = new System.Uri(SiteUtils.GetCurrentPageUrl());

            }

            foreach (DataRowView row in dv)
            {
                bool confirmed = Convert.ToBoolean(row["Confirmed"]);
                if (!EnableSelectivePublishing)
                {
                    confirmed = true;
                }

                if (confirmed)
                {
                    RssItem item = new RssItem();


                    item.Title = row["Title"].ToString();
                    item.Description = row["Description"].ToString();
                    item.PublicationDate = Convert.ToDateTime(row["PubDate"]);
                    item.Link = new System.Uri(row["Link"].ToString());
                    Trace.Write(item.Link.ToString());
                    item.Author = row["Author"].ToString();

                    channel.AddItem(item);
                }
            }

            Response.Cache.SetExpires(DateTime.Now.AddMinutes(5));
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.ContentType = "application/xml";

            Encoding encoding = new UTF8Encoding();
            Response.ContentEncoding = encoding;

            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(Response.OutputStream, encoding))
            {
                xmlTextWriter.Formatting = Formatting.Indented;

                //////////////////
                // style for RSS Feed viewed in browsers
                if (ConfigurationManager.AppSettings["RSSCSS"] != null)
                {
                    string rssCss = ConfigurationManager.AppSettings["RSSCSS"].ToString();
                    xmlTextWriter.WriteWhitespace(" ");
                    xmlTextWriter.WriteRaw("<?xml-stylesheet type=\"text/css\" href=\"" + cssBaseUrl + rssCss + "\" ?>");

                }

                if (ConfigurationManager.AppSettings["RSSXsl"] != null)
                {
                    string rssXsl = ConfigurationManager.AppSettings["RSSXsl"].ToString();
                    xmlTextWriter.WriteWhitespace(" ");
                    xmlTextWriter.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"" + cssBaseUrl + rssXsl + "\" ?>");

                }
                ///////////////////////////


                feed.Save(xmlTextWriter);

            }



        }

        private void LoadSettings()
        {
            moduleId = WebUtils.ParseInt32FromQueryString("mid", -1);
            securityBypassGuid = WebUtils.ParseGuidFromQueryString("g", securityBypassGuid);
            pageSettings = CacheHelper.GetCurrentPage();
            module = GetModule();

            if ((moduleId == -1) || (module == null)) { return; }
            
            bool bypassPageSecurity = false;

            if ((securityBypassGuid != Guid.Empty) && (securityBypassGuid == WebConfigSettings.InternalFeedSecurityBypassKey))
            {
                bypassPageSecurity = true;
            }

            if (
                (bypassPageSecurity)
                || (WebUser.IsInRoles(pageSettings.AuthorizedRoles))
                || (WebUser.IsInRoles(module.ViewRoles))
                )
            {
                canView = true;
            }

            if (!canView) { return; }

            moduleSettings = ModuleSettings.GetModuleSettings(moduleId);

            feedListCacheTimeout = WebUtils.ParseInt32FromHashtable(
                moduleSettings, "RSSFeedFeedListCacheTimeoutSetting", 3660);

            entryCacheTimeout = WebUtils.ParseInt32FromHashtable(
                moduleSettings, "RSSFeedEntryCacheTimeoutSetting", 3620);

            maxDaysOld = WebUtils.ParseInt32FromHashtable(
                moduleSettings, "RSSFeedMaxDayCountSetting", 90);

            maxEntriesPerFeed = WebUtils.ParseInt32FromHashtable(
                moduleSettings, "RSSFeedMaxPostsPerFeedSetting", 90);

            EnableSelectivePublishing = WebUtils.ParseBoolFromHashtable(
                moduleSettings, "EnableSelectivePublishing", EnableSelectivePublishing);

            cssBaseUrl = WebUtils.GetSiteRoot();

        }


        private void RenderError(string message)
        {

            Response.Write(message);
            
        }

        private Module GetModule()
        {
            if (pageSettings != null)
            {
                foreach (Module module in pageSettings.Modules)
                {
                    if (module.ModuleId == moduleId)
                        return module;
                }
            }
            return null;
           
        }


        


        

    }
}
