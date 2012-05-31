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



namespace Engage.Dnn.F3
{
    using System;
    using System.Data;
    using System.Web;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using Publish;


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
           Load += Page_Load;

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
		private void Page_Load(object sender, EventArgs e)
		{
			try
			{
                btnEngagePublish.Visible = Settings.Contains("chkEnablePublish") && Convert.ToBoolean(Settings["chkEnablePublish"].ToString());
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
                var collection = new DotNetNuke.Entities.Modules.Actions.ModuleActionCollection();
				//Actions.Add(GetNextActionID, Localization.GetString(Entities.Modules.Actions.ModuleActionType.AddContent, LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "", "", EditUrl(), false, Security.SecurityAccessLevel.Edit, true, false);
				return collection;
			}
		}

#endregion

        private int _lowerTabId = -1;
        public int LowerTabId
        {
            get
            {
                object o = Settings["lowerTabId"];
                if (o == null || !int.TryParse(o.ToString(), out _lowerTabId)&& o!=string.Empty)
                {
                    _lowerTabId = Convert.ToInt32(o);
                }
                return _lowerTabId;
            }
        }

        private int _upperTabId = -1;
        public int UpperTabId
        {
            get
            {
                object o = Settings["upperTabId"];
                if (o == null || !int.TryParse(o.ToString(), out _upperTabId) && o != string.Empty)
                {
                    _upperTabId = Convert.ToInt32(o);
                }
                return _upperTabId;
            }
        }

        
        public string CleanupText(string text)
        {
            string returnVal = Server.HtmlEncode(text);
            if (returnVal.Length > 500)
                return returnVal.Substring(0,500);
            return returnVal;
        }

        public string GetEditLink(int moduleId, int tabid)
        {
            return DotNetNuke.Common.Globals.NavigateURL(tabid, "edit", "&mid=" + moduleId);            
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
                return HttpContext.Current.Request.ApplicationPath;
            }
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {

                //search for the String and setup the results
                BindData(txtSearchString.Text.Trim());
                txtReplacementText.Text = "";
                lblReplacementResults.Text = "";
                dgResults.Visible = true;
                dgPublishResults.Visible = false;
                  
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }                
        }

        protected void btnEngagePublish_Click(object sender, EventArgs e)
        {
            BindPublishData(txtSearchString.Text.Trim());
            
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
                    DataTable dt = UserInfo.IsSuperUser ? DataProvider.Instance().GetLinks(searchString, LowerTabId, UpperTabId) : DataProvider.Instance().GetLinks(searchString, PortalId, LowerTabId, UpperTabId);

                    foreach (DataRow dr in dt.Rows)
                    {
                        int itemId = Convert.ToInt32(dr["ItemId"]);
                        int stateId = Convert.ToInt32(dr["StateId"]);
                        bool isPublished= Convert.ToBoolean(dr["IsPublished"]);
                        string content = dr["Content"].ToString();
                        content = content.Replace(searchString, replacementString);

                        DataProvider.Instance().ReplaceTextHTML(itemId, content, stateId, isPublished, UserId);
                    }
                    string replacementResults = Localization.GetString("replacementResults", LocalResourceFile);
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
                    DataTable dt = DataProvider.Instance().GetPublishLinks(searchString, PortalId);

                    foreach (DataRow dr in dt.Rows)
                    {
                        Article a = Article.GetArticle(Convert.ToInt32(dr["ItemId"]),PortalId, true, true);

                        string articleDescription = a.Description.Replace(searchString, replacementString);
                        string articleBody = a.ArticleText.Replace(searchString, replacementString);
                        a.ArticleText = articleBody;
                        a.Description = articleDescription;
                        a.Save(UserInfo.UserID);
                    }
                    string replacementResults = Localization.GetString("replacementResults", LocalResourceFile);
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
