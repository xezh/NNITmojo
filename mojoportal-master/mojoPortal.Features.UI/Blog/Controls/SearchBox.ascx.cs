﻿// Author:					    
// Created:				        2013-01-20
//	Last Modified:              2013-01-20
// 
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software. 

using System;
using mojoPortal.Web;
using System.Web.UI;
using mojoPortal.Web.Framework;
using mojoPortal.Business;
using Resources;

namespace mojoPortal.Web.BlogUI
{
    public partial class SearchBox : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((WebConfigSettings.DisableSearchFeatureFilters)||(WebConfigSettings.DisableSearchIndex))
            {
                this.Visible = false;
                return;
            }

            btnSearch.Text = ForumResources.Search;
            reqSearchText.ErrorMessage = ForumResources.SearchTermRequiredWarning;
        }

        void btnSearch_Click(object sender, EventArgs e)
        {
            string redirectUrl = Request.RawUrl;
            if (txtSearch.Text.Length > 0)
            {
                redirectUrl = SiteUtils.GetNavigationSiteRoot()
                    + "/SearchResults.aspx?f=" + Blog.FeatureGuid.ToString() + "&q=" + Server.UrlEncode(txtSearch.Text);
            }

            WebUtils.SetupRedirect(this, redirectUrl);
            
        }

        //SearchResults.aspx?q=foo&f=38aa5a84-9f5c-42eb-8f4c-105983d419fb
        //btnSearch

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Load += new EventHandler(Page_Load);
            btnSearch.Click += new EventHandler(btnSearch_Click);
        }

        
    }
}