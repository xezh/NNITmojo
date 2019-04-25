﻿
// 
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)  
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.


using System.Web.UI;
using System.Web.UI.WebControls;
using mojoPortal.Web.UI;

namespace mojoPortal.Web.Editor
{
    public class AjaxEditorAdapter : IWebEditor
    {
        #region Constructors

        public AjaxEditorAdapter()
        {
            InitializeEditor();
        }

        #endregion

        #region Private Properties

        private AjaxEditor Editor = new AjaxEditor();
        private string scriptBaseUrl = string.Empty;
        private string siteRoot = string.Empty;
        private string skinName = string.Empty;
        private Unit editorWidth = Unit.Percentage(98);
        private Unit editorHeight = Unit.Pixel(350);
        private string editorCSSUrl = string.Empty;
        private Direction textDirection = Direction.LeftToRight;
        private ToolBar toolBar = ToolBar.AnonymousUser;
        private bool setFocusOnStart = false;
        private bool fullPageMode = false;
        private bool useFullyQualifiedUrlsForResources = false;

        #endregion

        #region Public Properties

        public string ControlID
        {
            get
            {
                return Editor.ID;
            }
            set
            {
                Editor.ID = value;
            }
        }

        public string ClientID
        {
            get
            {
                return Editor.ClientID;
            }

        }

        public string Text
        {
            get { return Editor.Content; }
            set { Editor.Content = value; }
        }

        public string ScriptBaseUrl
        {
            get
            {
                return scriptBaseUrl;
            }
            set
            {
                scriptBaseUrl = value;

            }
        }

        public string SiteRoot
        {
            get
            {
                return siteRoot;
            }
            set
            {
                siteRoot = value;


            }
        }

        public string SkinName
        {
            get { return skinName; }
            set { skinName = value; }
        }

        public string EditorCSSUrl
        {
            get
            {
                return editorCSSUrl;
            }
            set
            {
                editorCSSUrl = value;
               
            }
        }

        public Unit Width
        {
            get
            {
                return editorWidth;
            }
            set
            {
                editorWidth = value;
                Editor.Width = editorWidth;
            }
        }

        public Unit Height
        {
            get
            {
                return editorHeight;
            }
            set
            {
                editorHeight = value;
                Editor.Height = editorHeight;

            }
        }

        public Direction TextDirection
        {
            get
            {
                return textDirection;
            }
            set
            {
                textDirection = value;
                


            }
        }

        public ToolBar ToolBar
        {
            get
            {
                return toolBar;
            }
            set
            {
                toolBar = value;
                
            }
        }

        public bool SetFocusOnStart
        {
            get { return setFocusOnStart; }
            set
            {
                setFocusOnStart = value;
                //Editor.AutoFocus = setFocusOnStart;
            }
        }



        public bool FullPageMode
        {
            get { return fullPageMode; }
            set
            {
                fullPageMode = value;
                
            }
        }

        public bool UseFullyQualifiedUrlsForResources
        {
            get { return useFullyQualifiedUrlsForResources; }
            set
            {
                useFullyQualifiedUrlsForResources = value;

            }
        }

        #endregion

        #region Private Methods

        private void InitializeEditor()
        {
           

        }

        

        #endregion

        #region Public Methods

        public Control GetEditorControl()
        {
            InitializeEditor();

            return Editor;
        }



        #endregion


    }
}