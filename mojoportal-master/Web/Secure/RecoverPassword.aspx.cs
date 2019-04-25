/// Author:					    
/// Created:				    2006-04-15
///	Last Modified:              2018-03-28
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.

using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Net;
using System.Net.Mail;
using log4net;
using mojoPortal.Web.Framework;
using mojoPortal.Web.Controls;
using mojoPortal.Net;
using Resources;

namespace mojoPortal.Web.UI.Pages
{

    public partial class RecoverPassword : NonCmsBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RecoverPassword));


        protected Label EnterUserNameLabel;


        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);
            this.Error += new EventHandler(RecoverPassword_Error);
            base.EnsureChildControls();
            this.PasswordRecovery1.SendingMail += new MailMessageEventHandler(PasswordRecovery1_SendingMail);
            this.PasswordRecovery1.SendMailError += new SendMailErrorEventHandler(PasswordRecovery1_SendMailError);
            //if (!siteSettings.AllowPasswordRetrieval)
            //{
            //    SiteUtils.RedirectToAccessDeniedPage();
            //    return;
            //}

            if (WebConfigSettings.HideMenusOnPasswordRecoveryPage) { SuppressAllMenus(); }

        }



        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SiteUtils.SslIsAvailable()) SiteUtils.ForceSsl();
            

            bool allow = ((siteSettings.AllowPasswordRetrieval || siteSettings.AllowPasswordReset) && (!siteSettings.UseLdapAuth ||
                                                                           (siteSettings.UseLdapAuth && siteSettings.AllowDbFallbackWithLdap)));

            if ((!allow))
            {
                SiteUtils.RedirectToAccessDeniedPage(this);
                return;
            }

            PopulateLabels();
        }

        private void PopulateLabels()
        {
            Title = SiteUtils.FormatPageTitle(siteSettings, Resource.PasswordRecoveryTitle);

            MetaDescription = Server.HtmlEncode(string.Format(CultureInfo.InvariantCulture,
                Resource.RecoverPasswordMetaDescriptionFormat,
                siteSettings.SiteName));
			if (Request.IsAuthenticated)
			{
				pnlRecoverPassword.Visible = false;

				AlertPanel alertPanel = new AlertPanel
				{
					SkinID = "Error"
				};
				Literal litMessage = new Literal();
				litMessage.Text = Resource.PasswordRecoveryNotAllowedWhenAuthenticated;
				alertPanel.Controls.Add(litMessage);
				phMessage.Controls.Add(alertPanel);
			}

            EnterUserNameLabel = (Label)this.PasswordRecovery1.UserNameTemplateContainer.FindControl("lblEnterUserName");
            if (EnterUserNameLabel != null)
            {
                if ((siteSettings != null) && (siteSettings.UseEmailForLogin))
                {
                    EnterUserNameLabel.Text = Resource.EnterEmailLabel;
                }
                else
                {
                    EnterUserNameLabel.Text = Resource.EnterUserNameLabel;
                }

            }

            // UserName template
            Button userNameNextButton = (Button)PasswordRecovery1.UserNameTemplateContainer.FindControl("SubmitButton");
            if (userNameNextButton != null)
            {
                userNameNextButton.Text = Resource.PasswordRecoveryNextButton;
                SiteUtils.SetButtonAccessKey(userNameNextButton, AccessKeys.PasswordRecoveryAccessKey);
            }

            RequiredFieldValidator reqUserName
                = (RequiredFieldValidator)PasswordRecovery1.UserNameTemplateContainer.FindControl("UserNameRequired");

            if (reqUserName != null)
            {
                reqUserName.ErrorMessage = Resource.PasswordRecoveryUserNameRequiredWarning;
            }

            // Question Template
            Button questionNextButton = (Button)PasswordRecovery1.QuestionTemplateContainer.FindControl("SubmitButton");
            if (questionNextButton != null)
            {
                questionNextButton.Text = Resource.PasswordRecoveryNextButton;
                SiteUtils.SetButtonAccessKey(questionNextButton, AccessKeys.PasswordRecoveryAccessKey);
            }

            RequiredFieldValidator reqAnswer
                = (RequiredFieldValidator)PasswordRecovery1.QuestionTemplateContainer.FindControl("AnswerRequired");

            if (reqAnswer != null)
            {
                reqAnswer.ErrorMessage = Resource.PasswordRecoveryAnswerRequired;
            }

            this.PasswordRecovery1.GeneralFailureText = Resource.PasswordRecoveryGeneralFailureText;
            this.PasswordRecovery1.QuestionFailureText = Resource.PasswordRecoveryQuestionFailureText;
            this.PasswordRecovery1.UserNameFailureText = Resource.PasswordRecoveryUserNameFailureText;

            this.PasswordRecovery1.MailDefinition.From = siteSettings.DefaultEmailFromAddress;
            this.PasswordRecovery1.MailDefinition.Subject
                = string.Format(Resource.PasswordRecoveryEmailSubjectFormatString,
                siteSettings.SiteName);

            string emailFilename;

            if (Membership.Provider.PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                emailFilename =
                    ResourceHelper.GetFullResourceFilePath(
                        System.Globalization.CultureInfo.CurrentUICulture,
                        this.GetEmailTemplatesFolder(),
                        WebConfigSettings.HashedPasswordRecoveryEmailTemplateFileNamePattern);

                this.PasswordRecovery1.MailDefinition.BodyFileName =
                    string.IsNullOrEmpty(emailFilename) ?
                    this.PasswordRecovery1.MailDefinition.BodyFileName
                        = Server.MapPath("~/Data/MessageTemplates/"
                        + SiteUtils.GetDefaultUICulture()
                        + "-" + WebConfigSettings.HashedPasswordRecoveryEmailTemplateFileNamePattern) :
                    emailFilename;

            }
            else
            {
                emailFilename =
                    ResourceHelper.GetFullResourceFilePath(
                        System.Globalization.CultureInfo.CurrentUICulture,
                        this.GetEmailTemplatesFolder(),
                        WebConfigSettings.PasswordRecoveryEmailTemplateFileNamePattern);

                this.PasswordRecovery1.MailDefinition.BodyFileName =
                    string.IsNullOrEmpty(emailFilename) ?
                    this.PasswordRecovery1.MailDefinition.BodyFileName
                        = Server.MapPath("~/Data/MessageTemplates/"
                        + SiteUtils.GetDefaultUICulture()
                        + "-" + WebConfigSettings.PasswordRecoveryEmailTemplateFileNamePattern) :
                    emailFilename;
            }

            this.PasswordRecovery1.MailDefinition.IsBodyHtml = ConfigHelper.GetBoolProperty("UseHtmlBodyInPasswordRecoveryEmail", false);

#if !NET35
            PasswordRecovery1.RenderOuterTable = false;
#endif

            AddClassToBody("passwordrecovery");
        }

        void PasswordRecovery1_SendingMail(object sender, MailMessageEventArgs e)
        {
            e.Message.Body = e.Message.Body.Replace("{SiteName}", siteSettings.SiteName);
            e.Message.Body = e.Message.Body.Replace("{SiteLink}", SiteUtils.GetNavigationSiteRoot());
            
            if (WebConfigSettings.DisableDotNetOpenMail)
            {
                SmtpSettings smtpSettings = SiteUtils.GetSmtpSettings();

                Email.SetMessageEncoding(smtpSettings, e.Message);

                if (siteSettings.DefaultFromEmailAlias.Length > 0)
                {
                    e.Message.From = new System.Net.Mail.MailAddress(siteSettings.DefaultEmailFromAddress, siteSettings.DefaultFromEmailAlias);
                }
                else
                {
                    e.Message.From = new System.Net.Mail.MailAddress(siteSettings.DefaultEmailFromAddress);
                }
                Email.Send(smtpSettings, e.Message);
                e.Cancel = true; //stop here so the "built-in" mail routine doesn't run
            }

        }

        
        

        void PasswordRecovery1_SendMailError(object sender, SendMailErrorEventArgs e)
        {
            String logMessage = "unable to send password recovery email. Please check the system.net MailSettings section in web.config";

            log.Error(logMessage, e.Exception);
            lblMailError.Text = Resource.PasswordRecoveryMailFailureMessage;
            e.Handled = true;
            SiteLabel successLabel
                = (SiteLabel)PasswordRecovery1.SuccessTemplateContainer.FindControl("successLabel");
            if (successLabel != null) successLabel.Visible = false;

        }

        protected void RecoverPassword_Error(object sender, EventArgs e)
        {
            //try
            //{
            Exception rawException = Server.GetLastError();
            if (rawException != null)
            {
                if (rawException is MojoMembershipException)
                {
                    Server.ClearError();
                    WebUtils.SetupRedirect(this, SiteUtils.GetNavigationSiteRoot());
                    return;
                }
            }
            //}
            //catch { }
        }

        private string GetEmailTemplatesFolder()
        {
            if (HttpContext.Current == null) return String.Empty;
            return HttpContext.Current.Server.MapPath(WebUtils.GetApplicationRoot()
                    + "/Data/MessageTemplates") + System.IO.Path.DirectorySeparatorChar;
        }
    }
}
