/// Author:					
/// Created:				2004-09-12
/// Last Modified:			2018-03-28
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.UI.WebControls;
using log4net;
using mojoPortal.Business;
using mojoPortal.Business.WebHelpers;
using mojoPortal.Web.Framework;
using Resources;

namespace mojoPortal.Web.AdminUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RoleManagerPage : NonCmsBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RoleManagerPage));
        private static bool debugLog = log.IsDebugEnabled;

        protected string EditPropertiesImage = "~/Data/SiteImages/" + WebConfigSettings.EditPropertiesImage;
        protected string DeleteLinkImage = "~/Data/SiteImages/" + WebConfigSettings.DeleteLinkImage;
        //protected bool IsAdmin = false;
        

        protected void Page_Load(object sender, EventArgs e)
		{
			if (!Request.IsAuthenticated)
			{
				SiteUtils.RedirectToLoginPage(this);
				return;
			}
			if (!WebUser.IsAdminOrRoleAdmin)
            {
                SiteUtils.RedirectToAccessDeniedPage(this);
                return;
            }

            if (SiteUtils.IsFishyPost(this))
            {
                SiteUtils.RedirectToAccessDeniedPage(this);
                return;
            }

            //http://www.mojoportal.com/Forums/Thread.aspx?thread=5815&mid=34&pageid=5&ItemID=2&pagenumber=1#post24015
            //if ((WebConfigSettings.UseRelatedSiteMode) && (WebConfigSettings.RelatedSiteModeHideRoleManagerInChildSites))
            //{
            //    if (!siteSettings.IsServerAdminSite)
            //    {
            //        SiteUtils.RedirectToAccessDeniedPage(this);
            //        return;

            //    }
            //}

            LoadSettings();
            PopulateLabels();
            PopulateControls();

        }

        private void PopulateControls()
        {
            if (Page.IsPostBack) return;

            BindRoleList();

        }

        protected string FormatMemberLink(int memberCount)
        {
            return string.Format(CultureInfo.InvariantCulture,
                Resource.RolesMemberLinkFormat,
                memberCount);
        }

        private void BindRoleList()
        {
            Collection<Role> siteRoles = Role.GetBySite(siteSettings.SiteId);
            if (!WebUser.IsAdmin)
            {
                // must be only Role Admin
                // remove admins role and role admins role
                // from list. Role Admins can't edit or manage
                // membership in Admins role or Role Admins role
                foreach (Role r in siteRoles)
                {
                    if (r.Equals("Admins"))
                    {
                        siteRoles.Remove(r);
                        break;
                    }

                }

                foreach (Role r in siteRoles)
                {
                    if (r.Equals("Role Admins"))
                    {
                        siteRoles.Remove(r);
                        break;
                    }
                }

                if (!WebConfigSettings.AllowRoleAdminsToCreateContentManagers)
                {
                    foreach (Role r in siteRoles)
                    {
                        if (r.Equals("Content Administrators"))
                        {
                            siteRoles.Remove(r);
                            break;
                        }
                    }
                }


            }
            
            rolesList.DataSource = siteRoles;
            rolesList.DataBind();
            
            

        }

        protected void btnAddRole_Click(Object sender, EventArgs e)
        {
            if (this.txtNewRoleName.Text.Length > 0)
            {
                Role role = new Role();
                role.SiteId = siteSettings.SiteId;
                role.SiteGuid = siteSettings.SiteGuid;
                role.RoleName = txtNewRoleName.Text;
				role.DisplayName = txtNewDisplayName.Text;
                //role.EnforceRelatedSitesMode = WebConfigSettings.UseRelatedSiteMode;
                role.Save();
            }

            WebUtils.SetupRedirect(this, Request.RawUrl);
            return;
        }


        protected void RolesList_ItemCommand(object sender, DataListCommandEventArgs e)
        {
            if (debugLog) { log.Debug("fired event RolesList_ItemCommand"); }

            int roleID = (int)rolesList.DataKeys[e.Item.ItemIndex];
            Role role = new Role(roleID);

            switch (e.CommandName)
            {
                case "edit":
                    rolesList.EditItemIndex = e.Item.ItemIndex;
                    BindRoleList();
                    break;

                case "apply":
                    role.DisplayName = ((TextBox)e.Item.FindControl("displayName")).Text;
                    role.Save();
                    rolesList.EditItemIndex = -1;
                    BindRoleList();
                    break;

                case "delete":

                    if (role.CanBeDeleted(WebConfigSettings.RolesThatCannotBeDeleted.SplitOnChar(';')))
                    {
                        Role.DeleteRole(roleID);
                        rolesList.EditItemIndex = -1;
                    }
                    BindRoleList();
                    break;

                //case "members":
                //    roleName = ((TextBox)e.Item.FindControl("roleName")).Text;
                //    role.RoleName = roleName;
                //    role.Save();
                //    string redirectUrl 
                //        = SiteRoot + "/Admin/SecurityRoles.aspx?roleId=" 
                //        + roleID + "&rolename=" + roleName;

                //    WebUtils.SetupRedirect(this, redirectUrl);
                //    break;

                case "cancel":
                    WebUtils.SetupRedirect(this, Request.RawUrl);
                    break;
            }

        }


        void rolesList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            ImageButton btnDelete = e.Item.FindControl("btnDelete") as ImageButton;
            UIHelper.AddConfirmationDialog(btnDelete, Resource.RolesDeleteWarning);
        }


        private void PopulateLabels()
        {
            Title = SiteUtils.FormatPageTitle(siteSettings, Resource.AdminMenuRoleAdminLink);

            heading.Text = Resource.AdminMenuRoleAdminLink;

            lnkAdminMenu.Text = Resource.AdminMenuLink;
            lnkAdminMenu.ToolTip = Resource.AdminMenuLink;
            lnkAdminMenu.NavigateUrl = SiteRoot + "/Admin/AdminMenu.aspx";

            lnkRoleAdmin.Text = Resource.AdminMenuRoleAdminLink;
            lnkRoleAdmin.ToolTip = Resource.AdminMenuRoleAdminLink;
            lnkRoleAdmin.NavigateUrl = SiteRoot + "/Admin/RoleManager.aspx";
           
            btnAddRole.Text = Resource.RolesAddButton;
            btnAddRole.ToolTip = Resource.RolesAddButton;
            SiteUtils.SetButtonAccessKey(btnAddRole, AccessKeys.RolesAddButtonAccessKey);

			txtNewRoleName.Attributes.Add("placeholder", Resource.RoleSystemName);
			txtNewDisplayName.Attributes.Add("placeholder", Resource.RoleDisplayName);



		}

        private void LoadSettings()
        {
            //IsAdmin = WebUser.IsAdmin;

            AddClassToBody("administration");
            AddClassToBody("rolemanager");
            
        }


        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);
            this.btnAddRole.Click += new EventHandler(btnAddRole_Click);
            this.rolesList.ItemCommand += new DataListCommandEventHandler(RolesList_ItemCommand);
            this.rolesList.ItemDataBound += new DataListItemEventHandler(rolesList_ItemDataBound);

            SuppressMenuSelection();
            SuppressPageMenu();
            
        }

        #endregion
    }
}
