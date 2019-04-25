﻿//	Created:			    2010-01-04
//	Last Modified:		    2010-08-28
// 
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.

using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mojoPortal.Business.WebHelpers;
using mojoPortal.Web.Framework;
using Resources;

namespace mojoPortal.Web.UI
{
	/// <summary>
	/// a convenience link that pnly renders if the user can edit the current cms page
	/// </summary>
	public class PageEditFeaturesLink : HyperLink
	{
		private CmsPage basePage = null;

		private bool renderAsListItem = false;
		public bool RenderAsListItem
		{
			get { return renderAsListItem; }
			set { renderAsListItem = value; }
		}

		private string listItemCSS = string.Empty;
		public string ListItemCss
		{
			get { return listItemCSS; }
			set { listItemCSS = value; }
		}

		private string literalExtraTopContent = string.Empty;
		public string LiteralExtraTopContent
		{
			get { return literalExtraTopContent; }
			set { literalExtraTopContent = value; }
		}

		private string literalExtraBottomContent = string.Empty;
		public string LiteralExtraBottomContent
		{
			get { return literalExtraBottomContent; }
			set { literalExtraBottomContent = value; }
		}

		private string linkImageUrl = string.Empty;
		public string LinkImageUrl
		{
			get { return linkImageUrl; }
			set { linkImageUrl = value; }
		}

		private bool ShouldRender()
		{
			if (basePage == null)
			{
				return false;
			}

			if (!Page.Request.IsAuthenticated)
			{
				return false;
			}

			if (basePage.CurrentPage == null)
			{
				return false;
			}

			if (basePage.CurrentPage.PageId == -1)
			{
				return false;
			}

			if (basePage.CurrentPage.EditRoles == "Admins;")
			{
				if (!WebUser.IsAdmin)
				{
					return false;
				}
			}

			if (WebUser.IsAdminOrContentAdmin)
			{
				return true;
			}

			if (WebUser.IsInRoles(basePage.CurrentPage.EditRoles) || (basePage.CurrentPage.IsPending && WebUser.IsInRoles(basePage.CurrentPage.DraftEditOnlyRoles)))
			{
				return true;
			}

			if (SiteUtils.UserIsSiteEditor())
			{
				return true;
			}

			//if (!WebConfigSettings.UseRelatedSiteMode) { return false; }

			//if (basePage.SiteInfo == null) { return false; }
			//// in related sites mode usersin site eidotrs role can use admin menu
			//if (WebUser.IsInRoles(basePage.SiteInfo.SiteRootEditRoles)) { return true; }

			return false;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (HttpContext.Current == null)
			{
				return;
			}

			EnableViewState = false;
			basePage = Page as CmsPage;

			Visible = ShouldRender();

			if (!Visible)
			{
				return;
			}

			if (basePage == null)
			{
				return;
			}

			if (CssClass.Length > 0)
			{
				CssClass = "adminlink pageeditlink " + CssClass;
			}
			else
			{
				CssClass = "adminlink pageeditlink";
			}

			ToolTip = Resource.PageLayoutTooltip;

			if (SiteUtils.SslIsAvailable())
			{
				if (basePage.CurrentPage != null)
				{
					NavigateUrl = basePage.SiteRoot + "/Admin/PageLayout.aspx?pageid=" + basePage.CurrentPage.PageId.ToInvariantString();
				}
			}
			else
			{
				if (basePage.CurrentPage != null)
				{
					NavigateUrl = basePage.RelativeSiteRoot + "/Admin/PageLayout.aspx?pageid=" + basePage.CurrentPage.PageId.ToInvariantString();
				}
			}

			Literal literalTop = new Literal();
			literalTop.Text = literalExtraTopContent;
			Controls.Add(literalTop);

			Literal literalText = new Literal();
			literalText.Text = Resource.PageEditLink;
			Controls.Add(literalText);

			Literal literalBottom = new Literal();
			literalBottom.Text = literalExtraBottomContent;
			Controls.Add(literalBottom);

			if (!string.IsNullOrWhiteSpace(linkImageUrl))
			{
				if (linkImageUrl.StartsWith("~/"))
				{
					ImageUrl = Page.ResolveUrl(linkImageUrl);
				}
				else
				{
					string skinPath = SiteUtils.GetSkinBaseUrl(Page);

					ImageUrl = Page.ResolveUrl(skinPath + linkImageUrl.TrimStart('/'));
				}
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (HttpContext.Current == null)
			{
				writer.Write("[" + ID + "]");
				return;
			}

			if (renderAsListItem)
			{
				if (listItemCSS.Length > 0)
				{
					writer.Write("<li class='" + listItemCSS + "'>");
				}
				else
				{
					writer.Write("<li>");
				}
			}

			base.Render(writer);

			if (renderAsListItem)
			{
				writer.Write("</li>");
			}
		}
	}
}