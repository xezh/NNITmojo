﻿// Author:					
// Created:				    2011-03-23
// Last Modified:			2011-03-23
// 
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)  
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.

using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Hosting;
using mojoPortal.Business;
using mojoPortal.Web;
using mojoPortal.Web.Framework;

namespace mojoPortal.Features.UI
{
    public class ImageGalleryContentInstaller : IContentInstaller
    {
        public void InstallContent(Module module, string configInfo)
        {
            if (string.IsNullOrEmpty(configInfo)) { return; }

            string basePath = "~/Data/Sites/" + module.SiteId.ToInvariantString() + "/media/";

            if (!Directory.Exists(HostingEnvironment.MapPath(basePath)))
            {
                Directory.CreateDirectory(HostingEnvironment.MapPath(basePath));
            }

            basePath = "~/Data/Sites/" + module.SiteId.ToInvariantString() + "/media/GalleryImages/";

            if (!Directory.Exists(HostingEnvironment.MapPath(basePath)))
            {
                Directory.CreateDirectory(HostingEnvironment.MapPath(basePath));
            }

            basePath = "~/Data/Sites/" + module.SiteId.ToInvariantString() + "/media/GalleryImages/" + module.ModuleId.ToInvariantString() + "/";

            if (!Directory.Exists(HostingEnvironment.MapPath(basePath)))
            {
                Directory.CreateDirectory(HostingEnvironment.MapPath(basePath));
            }

            IOHelper.CopyFolderContents(HostingEnvironment.MapPath(configInfo), HostingEnvironment.MapPath(basePath));

            basePath = "~/Data/Sites/" + module.SiteId.ToInvariantString() + "/media/GalleryImages/" + module.ModuleId.ToInvariantString() + "/FullSizeImages/";

            string[] files = Directory.GetFiles(HostingEnvironment.MapPath(basePath));
            foreach (string file in files)
            {
                GalleryImage img = new GalleryImage();
                img.ModuleGuid = module.ModuleGuid;
                img.ModuleId = module.ModuleId;
                img.ImageFile = Path.GetFileName(file);
                img.ThumbnailFile = img.ImageFile;
                img.WebImageFile = img.ImageFile;
                img.Save();
            }

        }
    }
}