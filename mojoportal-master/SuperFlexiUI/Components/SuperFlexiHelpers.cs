﻿// Author:					i7MEDIA (joe davis)
// Created:				    2015-03-31
// Last Modified:			2017-10-16
//
// You must not remove this notice, or any other, from this software.
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;
using log4net;
using mojoPortal.Business;
using mojoPortal.Business.WebHelpers;
using mojoPortal.FileSystem;
using mojoPortal.Web;
using mojoPortal.Web.Framework;
using mojoPortal.Web.UI;
using Resources;
using SuperFlexiBusiness;

namespace SuperFlexiUI
{
	public class SuperFlexiHelpers
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SuperFlexiHelpers));

        public static string GetModuleLinks(ModuleConfiguration config, SuperFlexiDisplaySettings displaySettings, int moduleId, int pageId)
        {
            StringBuilder litExtraMarkup = new StringBuilder();
            string settings = string.Empty;
            string add = string.Empty;
            string header = string.Empty;
            string footer = string.Empty;
            string import = string.Empty;
            string export = string.Empty;
            try
            {
                settings = String.Format(
                    displaySettings.ModuleSettingsLinkFormat,
                    SiteUtils.GetNavigationSiteRoot() + "/Admin/ModuleSettings.aspx?pageid=" + pageId.ToString() + "&amp;mid=" + moduleId.ToString(),
                    SuperFlexiResources.SettingsLinkLabel);

                if (!String.IsNullOrWhiteSpace(config.MarkupDefinitionName) && config.MarkupDefinitionName != "0")
                {
					if (!config.IsGlobalView && (config.MaxItems == -1 || Item.GetCountForModule(moduleId) < config.MaxItems))
					{
						add = String.Format(
                      displaySettings.AddItemLinkFormat,
                      SiteUtils.GetNavigationSiteRoot() + "/SuperFlexi/Edit.aspx?pageid=" + pageId.ToString() + "&amp;mid=" + moduleId.ToString(),
                      SuperFlexiResources.AddItem);
                    }

                    if (config.UseHeader)
                    {
                        header = String.Format(
                            displaySettings.EditHeaderLinkFormat,
                            SiteUtils.GetNavigationSiteRoot() + "/SuperFlexi/EditHeader.aspx?pageid=" + pageId.ToString() + "&amp;mid=" + moduleId.ToString(),
                            SuperFlexiResources.EditHeader);
                    }

                    if (config.UseFooter)
                    {
                        footer = String.Format(
                            displaySettings.EditFooterLinkFormat,
                            SiteUtils.GetNavigationSiteRoot() + "/SuperFlexi/EditHeader.aspx?f=true&pageid=" + pageId.ToString() + "&amp;mid=" + moduleId.ToString(),
                            SuperFlexiResources.EditFooter);
                    }

                    if (config.AllowImport)
                    {
                        import = String.Format(
                            displaySettings.ImportLinkFormat,
                            SiteUtils.GetNavigationSiteRoot() + "/SuperFlexi/Import.aspx?pageid=" + pageId.ToString() + "&amp;mid=" + moduleId.ToString(),
                            SuperFlexiResources.ImportTitle);
                    }

                    if (config.AllowExport)
                    {
                        export = String.Format(
                            displaySettings.ExportLinkFormat,
                            SiteUtils.GetNavigationSiteRoot() + "/SuperFlexi/Export.aspx?pageid=" + pageId.ToString() + "&amp;mid=" + moduleId.ToString(),
                            SuperFlexiResources.ExportTitle);
                    }
                }

                litExtraMarkup.AppendFormat(displaySettings.ModuleLinksFormat, settings, add, header, footer, import, export);
            }
            catch (System.FormatException ex)
            {
                Module module = new Module(moduleId);
                string moduleTitle = "unknown";
                if (module != null) moduleTitle = module.ModuleTitle;
                log.ErrorFormat("Error rendering \"{0}\", with moduleID={1}, pageid={2}. Error was:\r\n{3}", moduleTitle, moduleId.ToString(), pageId.ToString(), ex);
            }
            return litExtraMarkup.ToString();
        }

        public static string GetHelpText(string helpKey, ModuleConfiguration config)
        {

			FileSystemProvider p = FileSystemManager.Providers[WebConfigSettings.FileSystemProvider];
			if (p == null)
			{
				log.Error("File System Provider Could Not Be Loaded.");
				return string.Empty;
			}
			IFileSystem fileSystem = p.GetFileSystem();
			if (fileSystem == null)
			{
				log.Error("File System Could Not Be Loaded.");
				return string.Empty;
			}

			string helpText = string.Empty;
			WebFile helpFile = new WebFile();
			if (helpKey.ToLower().EndsWith(".sfhelp") ||
				helpKey.ToLower().EndsWith(".config") ||
				helpKey.ToLower().EndsWith(".html"))
			{
				if (helpKey.IndexOf("$_FlexiHelp_$") >= 0)
				{
					string path = fileSystem.CombinePath("~/Data/SuperFlexi/Help/", helpKey.Replace("$_FlexiHelp_$", string.Empty));
					helpFile = fileSystem.RetrieveFile(path);

				}
				else if (helpKey.IndexOf("$_SitePath_$") >= 0)
				{
					string path = helpKey.Replace("$_SitePath_$", "~/Data/Sites/" + CacheHelper.GetCurrentSiteSettings().SiteId.ToInvariantString());
					helpFile = fileSystem.RetrieveFile(path);
				}
				else if (helpKey.IndexOf("$_Data_$") >= 0)
				{
					string path = helpKey.Replace("$_Data_$", "~/Data");
					helpFile = fileSystem.RetrieveFile(path);
				}
				else if (helpKey.IndexOf("~/") >= 0)
				{
					helpFile = fileSystem.RetrieveFile(helpKey);
				}
				else
				{
					string path = fileSystem.CombinePath(config.RelativeSolutionLocation, helpKey);
					helpFile = fileSystem.RetrieveFile(path);
				}
			}
			else
			{
				helpText = ResourceHelper.GetHelpFileText(helpKey);
				helpFile = null;
			}

            if (helpFile != null && fileSystem.FileExists(helpFile.VirtualPath))
            {
				//FileInfo file = new FileInfo(helpFilePath);
				//fileSystem.GetAsStream(helpFile.VirtualPath);
				//StreamReader sr = file.OpenText();
				
				StreamReader sr = new StreamReader(fileSystem.GetAsStream(helpFile.VirtualPath));
				helpText = sr.ReadToEnd();
				sr.Close();
            }

            return helpText;
        }

        public static void ReplaceStaticTokens(
            StringBuilder stringBuilder,
            ModuleConfiguration config,
            bool isEditable,
            SuperFlexiDisplaySettings displaySettings,
            int moduleId,
            int pageId,
            out StringBuilder sb)
        {
            SiteSettings siteSettings = CacheHelper.GetCurrentSiteSettings();

            if (siteSettings == null)
            {
                siteSettings = new SiteSettings(SiteSettings.GetRootSiteGuid());
            }

            PageSettings pageSettings = new PageSettings(siteSettings.SiteId, pageId);

            ReplaceStaticTokens(stringBuilder, config, isEditable, displaySettings, moduleId, pageSettings, siteSettings, out sb);
        }

        public static void ReplaceStaticTokens(
            StringBuilder stringBuilder,
            ModuleConfiguration config,
            bool isEditable,
            SuperFlexiDisplaySettings displaySettings,
            int moduleId,
            PageSettings pageSettings,
            SiteSettings siteSettings,
            out StringBuilder sb)
        {
            sb = stringBuilder;
            string featuredImageUrl = String.IsNullOrWhiteSpace(config.InstanceFeaturedImage) ? string.Empty : WebUtils.GetApplicationRoot() + config.InstanceFeaturedImage;
            string jsonObjName = "sflexi" + moduleId.ToString() + (config.IsGlobalView ? "Modules" : "Items");
            string currentSkin = string.Empty;
            string siteRoot = SiteUtils.GetNavigationSiteRoot();
			bool publishedOnCurrentPage = true;

			if (HttpContext.Current != null && HttpContext.Current.Request.Params.Get("skin") != null)
            {
                currentSkin = SiteUtils.SanitizeSkinParam(HttpContext.Current.Request.Params.Get("skin")) + "/";
            }

			if (isEditable)
			{
				var pageModules = PageModule.GetPageModulesByModule(moduleId);
				if (pageModules.Where(pm => pm.PageId == pageSettings.PageId).ToList().Count() == 0)
				{
					publishedOnCurrentPage = false;
				}
			}

			Module module = new Module(moduleId);

			if (module != null)
            {
                sb.Replace("$_ModuleTitle_$", module.ShowTitle ? String.Format(displaySettings.ModuleTitleFormat, module.ModuleTitle) : string.Empty);
                sb.Replace("$_RawModuleTitle_$", module.ModuleTitle);
                sb.Replace("$_ModuleGuid_$", module.ModuleGuid.ToString());
                if (String.IsNullOrWhiteSpace(config.ModuleFriendlyName))
                {
                    sb.Replace("$_FriendlyName_$", module.ModuleTitle);
                }

                siteSettings = new SiteSettings(module.SiteGuid);
            }
            if (!String.IsNullOrWhiteSpace(config.ModuleFriendlyName))
            {
                sb.Replace("$_FriendlyName_$", config.ModuleFriendlyName);
            }
            sb.Replace("$_FeaturedImageUrl_$", featuredImageUrl);
            sb.Replace("$_ModuleID_$", moduleId.ToString());
            sb.Replace("$_PageID_$", pageSettings.PageId.ToString());
            sb.Replace("$_PageUrl_$", siteRoot + pageSettings.Url.Replace("~/", ""));
            sb.Replace("$_PageName_$", siteRoot + pageSettings.PageName);
			//sb.Replace("$_ModuleLinks_$", isEditable ? SuperFlexiHelpers.GetModuleLinks(config, displaySettings, moduleId, pageSettings.PageId) : string.Empty);
			sb.Replace("$_ModuleLinks_$", isEditable ? SuperFlexiHelpers.GetModuleLinks(config, displaySettings, moduleId, publishedOnCurrentPage ? pageSettings.PageId : -1) : string.Empty);
			sb.Replace("$_JSONNAME_$", jsonObjName);
            sb.Replace("$_ModuleClass_$", SiteUtils.IsMobileDevice() && !String.IsNullOrWhiteSpace(config.MobileInstanceCssClass) ? config.MobileInstanceCssClass : config.InstanceCssClass);
            sb.Replace("$_ModuleTitleElement_$", module.HeadElement);
            sb.Replace("$_SiteID_$", siteSettings.SiteId.ToString());
            sb.Replace("$_SiteRoot_$", String.IsNullOrWhiteSpace(siteRoot) ? "/" : siteRoot);
            sb.Replace("$_SitePath_$", String.IsNullOrWhiteSpace(siteRoot) ? "/" : WebUtils.GetApplicationRoot() + "/Data/Sites/" + CacheHelper.GetCurrentSiteSettings().SiteId.ToInvariantString());
            sb.Replace("$_SkinPath_$", SiteUtils.DetermineSkinBaseUrl(currentSkin));
            sb.Replace("$_CustomSettings_$", config.CustomizableSettings); //this needs to be enhanced, a lot, right now we just dump the 'settings' where ever this token exists.
            sb.Replace("$_EditorType_$", siteSettings.EditorProviderName);
            sb.Replace("$_EditorSkin_$", siteSettings.EditorSkin.ToString());
            sb.Replace("$_EditorBasePath_$", WebUtils.ResolveUrl(ConfigurationManager.AppSettings["CKEditor:BasePath"]));
            sb.Replace("$_EditorConfigPath_$", WebUtils.ResolveUrl(ConfigurationManager.AppSettings["CKEditor:ConfigPath"]));
            sb.Replace("$_EditorToolbarSet_$", mojoPortal.Web.Editor.ToolBar.FullWithTemplates.ToString());
            sb.Replace("$_EditorTemplatesUrl_$", siteRoot + "/Services/CKeditorTemplates.ashx?cb=" + Guid.NewGuid().ToString());
            sb.Replace("$_EditorStylesUrl_$", siteRoot + "/Services/CKeditorStyles.ashx?cb=" + Guid.NewGuid().ToString().Replace("-", string.Empty));
            sb.Replace("$_DropFileUploadUrl_$", siteRoot + "/Services/FileService.ashx?cmd=uploadfromeditor&rz=true&ko=" + WebConfigSettings.KeepFullSizeImagesDroppedInEditor.ToString().ToLower()
                    + "&t=" + Global.FileSystemToken.ToString());
            sb.Replace("$_FileBrowserUrl_$", siteRoot + WebConfigSettings.FileDialogRelativeUrl);
            sb.Replace("$_HeaderContent_$", config.HeaderContent);
            sb.Replace("$_FooterContent_$", config.FooterContent);
			sb.Replace("$_SkinVersionGuid_$", siteSettings.SkinVersion.ToString());
        }

		[Obsolete("Use mojoPortal.Web.Framework.UIHelper.GetDictionaryFromString")]
        public static IDictionary<string, string> GetDictionaryFromString(string str)
        {
			//List<string> keyValuePairs = str.SplitOnChar(';');
			//Dictionary<string, string> dictionary = new Dictionary<string, string>();
			//foreach (string kvp in keyValuePairs)
			//{
			//    List<string> kv = kvp.SplitOnCharAndTrim('|');
			//    dictionary.Add(kv[0], kv[1]);
			//}
			//return dictionary;
			return UIHelper.GetDictionaryFromString(str);
        }

        public static Module GetSuperFlexiModule(int moduleId)
        {
            mojoBasePage bp = new mojoBasePage();
            Module m = bp.GetModule(moduleId);
            if (m != null) { return m; }

            bool isSiteEditor = SiteUtils.UserIsSiteEditor();

            // these extra checks allow for editing an instance from modulewrapper
            m = new Module(moduleId);
            if ((m.SiteId != CacheHelper.GetCurrentSiteSettings().SiteId)
                || (m.ModuleId == -1)
                || ((!WebUser.IsInRoles(m.AuthorizedEditRoles)) && (!WebUser.IsAdminOrContentAdmin) && (!isSiteEditor))
                )
            { m = null; }

            return m;
        }

        public static List<MarkupScript> ParseScriptsFromXmlNode(XmlNode childNode)
        {
            // Script Positions:
            // inHead
            // inBody (register script) (default)
            // aboveMarkupDefinition
            // belowMarkupDefinition
            // bottomStartup(register startup script)

            List<MarkupScript> workingMarkupScripts = new List<MarkupScript>();
            if (childNode.Name != "Scripts") return workingMarkupScripts;
            foreach (XmlNode child in childNode)
            {
                if (child.Name == "Script")
                {
                    XmlAttributeCollection childAttrs = child.Attributes;
                    string position = "inBody";
                    string scriptName = string.Empty;

                    if (childAttrs["position"] != null) { position = childAttrs["position"].Value; }
                    if (childAttrs["name"] != null) { scriptName = childAttrs["name"].Value; }

                    if (childAttrs["src"] != null)
                    {
                        MarkupScript script = new MarkupScript();
                        script.Url = childAttrs["src"].Value;
                        script.Position = position;
                        if (!String.IsNullOrWhiteSpace(scriptName)) { script.ScriptName = scriptName; }
                        workingMarkupScripts.Add(script);
                        continue;
                    }

                    if (!String.IsNullOrWhiteSpace(child.InnerText))
                    {
                        MarkupScript raw = new MarkupScript();
                        raw.RawScript = child.InnerText.Trim();
                        raw.Position = position;
                        if (!String.IsNullOrWhiteSpace(scriptName)) { raw.ScriptName = scriptName; }
                        workingMarkupScripts.Add(raw);
                    }
                }
            }

            return workingMarkupScripts;
        }

		public static string GetPathToFile(ModuleConfiguration config, string path, bool forceHttps = false)
		{
			string goodPath = string.Empty;
			if (path.StartsWith("/") ||
				path.StartsWith("http://") ||
				path.StartsWith("https://"))
			{
				return path;
			}
			else if (path.StartsWith("~/"))
			{
				return WebUtils.ResolveServerUrl(path, forceHttps);
			}
			else if (path.StartsWith("$_SitePath_$"))
			{
				return path.Replace("$_SitePath_$", "/Data/Sites/" + CacheHelper.GetCurrentSiteSettings().SiteId.ToString() + "/");
			}
			else
			{
				return new Uri(config.SolutionLocationUrl, path).ToString();
			}
		}

        public static void SetupScripts(
            List<MarkupScript> markupScripts,
            ModuleConfiguration config,
            SuperFlexiDisplaySettings displaySettings,
			bool forceHttps,
            bool isEditable,
            bool isPostBack,
            string clientID,
            int moduleID,
            int pageID,
            Page page,
            Control control)
        {
            string scriptRefFormat = "\n<script type=\"text/javascript\" src=\"{0}\" data-name=\"{1}\"></script>";
            string rawScriptFormat = "\n<script type=\"text/javascript\" data-name=\"{1}\">\n{0}\n</script>";

			foreach (MarkupScript script in markupScripts)
            {
                StringBuilder sbScriptText = new StringBuilder();
                StringBuilder sbScriptName = new StringBuilder();

                sbScriptName.Append(String.IsNullOrWhiteSpace(script.ScriptName) ? clientID + "flexiScript_" + markupScripts.IndexOf(script) : "flexiScript_" + script.ScriptName);
                SuperFlexiHelpers.ReplaceStaticTokens(sbScriptName, config, isEditable, displaySettings, moduleID, pageID, out sbScriptName);
                string scriptName = sbScriptName.ToString();
                if (!String.IsNullOrWhiteSpace(script.Url))
                {
					string scriptUrl = GetPathToFile(config, script.Url, forceHttps);
					sbScriptText.Append(string.Format(scriptRefFormat, scriptUrl, scriptName));
				}
                else if (!String.IsNullOrWhiteSpace(script.RawScript))
                {
                    sbScriptText.Append(string.Format(rawScriptFormat, script.RawScript, scriptName));
                }

                SuperFlexiHelpers.ReplaceStaticTokens(sbScriptText, config, isEditable, displaySettings, moduleID, pageID, out sbScriptText);

                // script position options
                // inHead
                // inBody (register script) (default)
                // aboveMarkupDefinition
                // belowMarkupDefinition
                // bottomStartup (register startup script)
                switch (script.Position)
                {
                    case "inHead":
                        if (!isPostBack && !page.IsCallback)
                        {
                            if (page.Header.FindControl(scriptName) == null)
                            {
                                LiteralControl headLit = new LiteralControl();
                                headLit.ID = scriptName;
                                headLit.Text = sbScriptText.ToString();
                                headLit.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                                headLit.EnableViewState = false;
                                page.Header.Controls.Add(headLit);
                            }
                        }
                        break;

                    case "aboveMarkupDefinition":
                        if (control == null) goto case "bottomStartup";
                        if (control.FindControlRecursive(scriptName) == null)
                        {
                            Control aboveMarkupDefinitionScripts = control.FindControlRecursive("aboveMarkupDefinitionScripts");
                            if (aboveMarkupDefinitionScripts != null)
                            {
                                LiteralControl aboveLit = new LiteralControl();
                                aboveLit.ID = scriptName;
                                aboveLit.Text = sbScriptText.ToString();
                                aboveMarkupDefinitionScripts.Controls.Add(aboveLit);
                            }
                            else
                            {
                                goto case "bottomStartup";
                            }
                        }
                        break;

                    case "belowMarkupDefinition":
                        if (control == null) goto case "bottomStartup";
                        if (control.FindControlRecursive(scriptName) == null)
                        {
                            Control belowMarkupDefinitionScripts = control.FindControlRecursive("belowMarkupDefinitionScripts");
                            if (belowMarkupDefinitionScripts != null)
                            {
                                LiteralControl belowLit = new LiteralControl();
                                belowLit.ID = scriptName;
                                belowLit.Text = sbScriptText.ToString();
                                belowMarkupDefinitionScripts.Controls.Add(belowLit);
                            }
                            else
                            {
                                goto case "bottomStartup";
                            }
                        }
                        //strBelowMarkupScripts.AppendLine(scriptText);
                        break;

                    case "bottomStartup":
                        if (!page.ClientScript.IsStartupScriptRegistered(scriptName))
                        {
                            ScriptManager.RegisterStartupScript(
                                page,
                                typeof(Page),
                                scriptName,
                                sbScriptText.ToString(),
                                false);
                        }
                        break;

                    case "inBody":
                    default:
                        if (!page.ClientScript.IsClientScriptBlockRegistered(scriptName))
                        {
                            ScriptManager.RegisterClientScriptBlock(
                                page,
                                typeof(Page),
                                scriptName,
                                sbScriptText.ToString(),
                                false);
                        }
                        break;
                }
            }
        }

        public static void SetupStyle(
            List<MarkupCss> markupCss,
            ModuleConfiguration config,
            SuperFlexiDisplaySettings displaySettings,
			bool forceHttps,
            string clientID,
            int moduleID,
            int pageID,
            Page page,
            Control control)
        {
            string styleLinkFormat = "\n<link rel=\"stylesheet\" href=\"{0}\" media=\"{2}\" data-name=\"{1}\">";
            string rawCSSFormat = "\n<style type=\"text/css\" data-name=\"{1}\" media=\"{2}\">\n{0}\n</style>";

            foreach (MarkupCss style in markupCss)
            {
                StringBuilder sbStyleText = new StringBuilder();
                StringBuilder sbStyleName = new StringBuilder();

                sbStyleName.Append(String.IsNullOrWhiteSpace(style.Name) ? clientID + "flexiStyle_" + markupCss.IndexOf(style) : "flexiStyle_" + style.Name);
                SuperFlexiHelpers.ReplaceStaticTokens(sbStyleName, config, false, displaySettings, moduleID, pageID, out sbStyleName);
                string styleName = sbStyleName.ToString();
                if (!String.IsNullOrWhiteSpace(style.Url))
                {
					string styleUrl = string.Empty;

					if (style.Url.StartsWith("/") ||
						style.Url.StartsWith("http://") ||
						style.Url.StartsWith("https://"))
					{
						styleUrl = style.Url;
					}
					else if (style.Url.StartsWith("~/"))
					{
						styleUrl = WebUtils.ResolveServerUrl(style.Url, forceHttps);
					}
					else if (style.Url.StartsWith("$_SitePath_$"))
					{
						styleUrl = style.Url.Replace("$_SitePath_$", "/Data/Sites/" + CacheHelper.GetCurrentSiteSettings().SiteId.ToString() + "/");
					}
					else
					{
						styleUrl = new Uri(config.SolutionLocationUrl, style.Url).ToString();
					}
					//else if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(config.MarkupDefinitionFile)))
					//{
					//	FileInfo fileInfo = new FileInfo(System.Web.Hosting.HostingEnvironment.MapPath(config.MarkupDefinitionFile));
					//	styleUrl = WebUtils.ResolveServerUrl(Path.Combine(fileInfo.DirectoryName.Replace(System.Web.Hosting.HostingEnvironment.MapPath("~"), "~/"), style.Url));
					//}

					sbStyleText.Append(string.Format(styleLinkFormat, styleUrl, styleName, style.Media));
				}
                else if (!String.IsNullOrWhiteSpace(style.CSS))
                {
                    sbStyleText.Append(string.Format(rawCSSFormat, style.CSS, styleName, style.Media));
                }

                SuperFlexiHelpers.ReplaceStaticTokens(sbStyleText, config, false, displaySettings, moduleID, pageID, out sbStyleText);

                LiteralControl theLiteral = new LiteralControl();
                theLiteral.Text = sbStyleText.ToString();

                StyleSheetCombiner ssc = (StyleSheetCombiner)page.Header.FindControl("StyleSheetCombiner");

                if (ssc != null)
                {
                    int sscIndex = page.Header.Controls.IndexOf(ssc);
                    if (style.RenderAboveSSC)
                    {
                        page.Header.Controls.AddAt(sscIndex, theLiteral);
                    }
                    else
                    {
                        page.Header.Controls.AddAt(sscIndex +1, theLiteral);
                    }
                }
                else
                {
                    page.Header.Controls.AddAt(0, theLiteral);
                }
            }
        }

        internal static List<MarkupCss> ParseCssFromXmlNode(XmlNode childNode)
        {
            List<MarkupCss> markupCss = new List<MarkupCss>();
            if (childNode.Name != "Styles") return markupCss;
            foreach (XmlNode child in childNode)
            {
                if (child.Name == "Style")
                {
                    XmlAttributeCollection childAttrs = child.Attributes;
                    string name = string.Empty;
                    string media = "all";
                    if (childAttrs["name"] != null) { name = childAttrs["name"].Value; }
                    if (childAttrs["media"] != null) { media = childAttrs["media"].Value; }
                    if (childAttrs["href"] != null)
                    {
                        MarkupCss style = new MarkupCss();
                        style.Url = childAttrs["href"].Value;
                        style.Media = media;
                        if (childAttrs["renderAboveSSC"] != null) { style.RenderAboveSSC = Convert.ToBoolean(childAttrs["renderAboveSSC"].Value); }
                        if (!String.IsNullOrWhiteSpace(name)) { style.Name = name; }
                        markupCss.Add(style);
                        continue;
                    }
                    if (!String.IsNullOrWhiteSpace(child.InnerText))
                    {
                        MarkupCss raw = new MarkupCss();
                        raw.CSS = child.InnerText.Trim();
                        raw.Media = media;
                        if (!String.IsNullOrWhiteSpace(name)) { raw.Name = name; }
                        markupCss.Add(raw);
                    }
                }
            }

            return markupCss;
        }

        public static void ParseSearchDefinition(XmlNode searchNode, Guid fieldDefinitionGuid, Guid siteGuid)
        {
            ModuleConfiguration config = new ModuleConfiguration();
            if (searchNode != null)
            {
                //XmlAttributeCollection attrCollection = node.Attributes;
                //if (attrCollection["fieldDefinitionGuid"] != null) fieldDefinitionGuid = Guid.Parse(attrCollection["fieldDefinitionGuid"].Value);
                //if (fieldDefinitionGuid == Guid.Empty) return;

                bool emptySearchDef = false;
                bool searchDefExists = true;
                SearchDef searchDef = SearchDef.GetByFieldDefinition(fieldDefinitionGuid);
                if (searchDef == null)
                {
                    searchDefExists = false;
                    emptySearchDef = true;

                    searchDef = new SearchDef();
                    searchDef.FieldDefinitionGuid = fieldDefinitionGuid;
                    searchDef.SiteGuid = siteGuid;
                    searchDef.FeatureGuid = config.FeatureGuid;
                }

                foreach (XmlNode childNode in searchNode)
                {
                    //need to find a way to clear out the searchdef if needed
                    switch (childNode.Name)
                    {
                        case "Title":
                            searchDef.Title = childNode.InnerText.Trim();
                            emptySearchDef = false;
                            break;

                        case "Keywords":
                            searchDef.Keywords = childNode.InnerText.Trim();
                            emptySearchDef = false;
                            break;

                        case "Description":
                            searchDef.Description = childNode.InnerText.Trim();
                            emptySearchDef = false;
                            break;

                        case "Link":
                            searchDef.Link = childNode.InnerText.Trim();
                            emptySearchDef = false;
                            break;

                        case "LinkQueryAddendum":
                            searchDef.LinkQueryAddendum = childNode.InnerText.Trim();
                            emptySearchDef = false;
                            break;
                    }

                    //}
                }
                if (searchDefExists && emptySearchDef)
                {
                    SearchDef.DeleteByFieldDefinition(fieldDefinitionGuid);
                }
                else if (!emptySearchDef)
                {
                    searchDef.Save();
                }
                //if (!emptySearchDef) searchDef.Save();
            }
        }

        //public static ExpandoObject GetExpandoForItem(Item item, ModuleConfiguration config)
        //{

        //}
        public static ExpandoObject GetExpandoForItem(Item item)
        {
            var fields = Field.GetAllForDefinition(item.DefinitionGuid);

            if (fields == null || item == null)
            {
                return null;
            }

            dynamic itemExpando = new ExpandoObject();
            itemExpando.Guid = item.ItemGuid;
            itemExpando.SortOrder = item.SortOrder;

            List<ItemFieldValue> fieldValues = ItemFieldValue.GetItemValues(item.ItemGuid);

            foreach (Field field in fields)
            {
                foreach (ItemFieldValue fieldValue in fieldValues)
                {
                    if (field.FieldGuid == fieldValue.FieldGuid)
                    {
                        ((IDictionary<String, Object>)itemExpando)[field.Name] = fieldValue.FieldValue;
                    }
                }
            }

            return itemExpando;
        }
        public static ExpandoObject GetExpandoForModuleItems(Module module, ModuleConfiguration config, bool allForDefinition = false)
        {
            var fields = Field.GetAllForDefinition(config.FieldDefinitionGuid);
            var items = new List<Item>();

            if (allForDefinition)
            {
                items = Item.GetAllForDefinition(config.FieldDefinitionGuid, module.SiteGuid, config.DescendingSort);
            }
            else
            {
                items = Item.GetModuleItems(module.ModuleId, config.DescendingSort);
            }

            if (fields == null || items == null)
            {
                return null;
            }

            dynamic expando = new ExpandoObject();

            expando.Definition = config.MarkupDefinitionName;
            expando.ModuleName = module.ModuleTitle;
            expando.Items = new List<dynamic>();

            foreach (Item item in items)
            {
                expando.Items.Add(GetExpandoForItem(item));
            }

            return expando;
        }
    }
}