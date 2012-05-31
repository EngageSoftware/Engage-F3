//Engage: F3 - http://www.engagemodules.com
//Copyright (c) 2004-2007
//by Engage Software ( http://www.engagesoftware.net )

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify and merge copies of the Software, and to permit persons to whom the Software 
//is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions 
//of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
//DEALINGS IN THE SOFTWARE.

using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.Modules;
using DotNetNuke.Security;
using DotNetNuke.Services;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Reflection;
using DotNetNuke.Entities.Modules;
using Engage.Dnn.Publish;


namespace Engage.Dnn.F3
{

	/// -----------------------------------------------------------------------------
	/// <summary>
	/// The ViewF3 class displays the content
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>
	/// </history>
	/// -----------------------------------------------------------------------------
	public partial class ViewLinks : PortalModuleBase, IActionable
	{

#region Event Handlers


        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }


        private void InitializeComponent()
        {
           this.Load += new System.EventHandler(this.Page_Load);

        }


		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Page_Load runs when the control is loaded
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// </history>
		/// -----------------------------------------------------------------------------
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
                if (Settings.Contains("chkEnablePublish"))
                {
                    if (Convert.ToBoolean(Settings["chkEnablePublish"].ToString()))
                    {
                        btnEngagePublish.Visible = true;
                    }
                    else
                    {
                        btnEngagePublish.Visible = false;
                    }
                }
                else
                {
                    btnEngagePublish.Visible = false;
                }
			}
			catch (Exception exc) //Module failed to load
			{
                Exceptions.ProcessModuleLoadException(this, exc);
			}
		}


#endregion

#region Optional Interfaces

		/// -----------------------------------------------------------------------------
		/// <summary>
		/// Registers the module actions required for interfacing with the portal framework
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		/// <history>
		/// </history>
		/// -----------------------------------------------------------------------------
		public DotNetNuke.Entities.Modules.Actions.ModuleActionCollection ModuleActions
		{
			get
			{
                DotNetNuke.Entities.Modules.Actions.ModuleActionCollection Actions = new DotNetNuke.Entities.Modules.Actions.ModuleActionCollection();
				//Actions.Add(GetNextActionID, Localization.GetString(Entities.Modules.Actions.ModuleActionType.AddContent, LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "", "", EditUrl(), false, Security.SecurityAccessLevel.Edit, true, false);
				return Actions;
			}
		}

#endregion

        private int lowerTabId = -1;
        public int LowerTabId
        {
            get
            {
                object o = this.Settings["lowerTabId"];
                if (o == null || !int.TryParse(o.ToString(), out this.lowerTabId)&& o!=string.Empty)
                {
                    this.lowerTabId = Convert.ToInt32(o);
                }
                return this.lowerTabId;
            }
        }

        private int upperTabId = -1;
        public int UpperTabId
        {
            get
            {
                object o = this.Settings["upperTabId"];
                if (o == null || !int.TryParse(o.ToString(), out this.upperTabId) && o != string.Empty)
                {
                    this.upperTabId = Convert.ToInt32(o);
                }
                return this.upperTabId;
            }
        }

        
        public string CleanupText(string text)
        {
            string returnVal = Server.HtmlEncode(text).ToString();
            if (returnVal.Length > 500)
                return returnVal.Substring(0,500);
            else
                return returnVal;
        }

        public string GetEditLink(int moduleId, int tabid)
        {
            return DotNetNuke.Common.Globals.NavigateURL(tabid, "edit", "&mid=" + moduleId.ToString());            
        }

        public string GetPublishLink(int itemId)
        {
            return ApplicationUrl + "/desktopmodules/engagepublish/itemlink.aspx?itemId=" + itemId;
        }

        public static string ApplicationUrl
        {
            get
            {
                if (HttpContext.Current.Request.ApplicationPath == "/")
                    return "";
                else
                    return HttpContext.Current.Request.ApplicationPath;
            }
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
                //search for the String and setup the results
                BindData(txtSearchString.Text.Trim());
                txtReplacementText.Text = "";
                lblReplacementResults.Text = "";
                dgResults.Visible = true;
                dgPublishResults.Visible = false;
        }

        protected void btnEngagePublish_Click(object sender, EventArgs e)
        {
            BindPublishData(txtSearchString.Text.Trim().ToString());
            
            dgResults.Visible = false;
            dgPublishResults.Visible = true;
            pnlReplacement.Visible = true;
        }


        protected void BindData(string searchString)
        {
            //bind the data

            if (UserInfo.IsSuperUser)
            {
                dgResults.DataSource = DataProvider.Instance().GetLinks(searchString, LowerTabId, UpperTabId);
                dgResults.DataBind();
            }
            else
            {
                dgResults.DataSource = DataProvider.Instance().GetLinks(searchString, PortalId, LowerTabId, UpperTabId);
                dgResults.DataBind();
            }

            pnlReplacement.Visible = true;
        }

        protected void BindPublishData(string searchString)
        {
            //bind the data
            dgPublishResults.DataSource = DataProvider.Instance().GetPublishLinks(searchString, PortalId);
            dgPublishResults.DataBind();
            

        }

        protected void btnReplace_Click(object sender, EventArgs e)
        {
            try
            {
                //loop through the Text/HTML modules and start updating fields
                string replacementString = txtReplacementText.Text.Trim();
                string searchString = txtSearchString.Text.Trim();
                if (replacementString != string.Empty && searchString != string.Empty)
                {

                    DataTable dt;
                    if (UserInfo.IsSuperUser)
                    {
                        dt = DataProvider.Instance().GetLinks(searchString, LowerTabId, UpperTabId);
                        
                    }
                    else
                    {
                        dt = DataProvider.Instance().GetLinks(searchString, PortalId, LowerTabId, UpperTabId);
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        int tModuleId = Convert.ToInt32(dr["ModuleId"]);
                        string desktopHtml = dr["DesktopHtml"].ToString();
                        string desktopSummary = dr["DesktopSummary"].ToString();
                        desktopHtml = desktopHtml.Replace(searchString, replacementString);
                        desktopSummary = desktopSummary.Replace(searchString, replacementString);
                        DataProvider.Instance().ReplaceTextHTML(tModuleId, desktopHtml, desktopSummary, UserId);
                    }
                    string replacementResults = Localization.GetString("replacementResults", LocalResourceFile).ToString();
                    lblReplacementResults.Text = String.Format(replacementResults, searchString, replacementString, dt.Rows.Count);
                    lblReplacementResults.Visible = true;
                    dgResults.Visible = false;
                    dgPublishResults.Visible = false;
                }  
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void btnReplaceEngagePublish_Click(object sender, EventArgs e)
        {
            try
            {
                //loop through the Text/HTML modules and start updating fields
                string replacementString = txtReplacementText.Text.Trim();
                string searchString = txtSearchString.Text.Trim();
                if (replacementString != string.Empty && searchString != string.Empty)
                {

                    DataTable dt;
                    dt = DataProvider.Instance().GetPublishLinks(searchString, PortalId);

                    foreach (DataRow dr in dt.Rows)
                    {
                        Article a = Article.GetArticle(Convert.ToInt32(dr["ItemId"]),PortalId, true, true);

                        string articleDescription = a.Description.Replace(searchString, replacementString);
                        string articleBody = a.ArticleText.Replace(searchString, replacementString);
                        a.ArticleText = articleBody;
                        a.Description = articleDescription;
                        a.Save(UserInfo.UserID);
                    }
                    string replacementResults = Localization.GetString("replacementResults", LocalResourceFile).ToString();
                    lblReplacementResults.Text = String.Format(replacementResults, searchString, replacementString, dt.Rows.Count);
                    lblReplacementResults.Visible = true;
                    dgResults.Visible = false;
                    dgPublishResults.Visible = false;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }
	}
}
