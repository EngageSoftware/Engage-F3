// <copyright file="ViewLinks.ascx.cs" company="Engage Software">
// Engage: F3
// Copyright (c) 2004-2010
// by Engage Software ( http://www.engagesoftware.com )
// </copyright>

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify and merge copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

namespace Engage.Dnn.F3
{
    using System;
    using System.Data;
    using System.Web;
    using DotNetNuke.Common;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using Publish;

    /// <summary>
    /// The ViewF3 class displays the content
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    public partial class ViewLinks : PortalModuleBase
    {
        private int _lowerTabId = -1;

        private int _upperTabId = -1;

        public static string ApplicationUrl
        {
            get
            {
                if (HttpContext.Current.Request.ApplicationPath == "/")
                {
                    return string.Empty;
                }

                return HttpContext.Current.Request.ApplicationPath;
            }
        }

        public int LowerTabId
        {
            get
            {
                object o = this.Settings["lowerTabId"];
                if (o == null || (!int.TryParse(o.ToString(), out this._lowerTabId) && o != string.Empty))
                {
                    this._lowerTabId = Convert.ToInt32(o);
                }

                return this._lowerTabId;
            }
        }

        public int UpperTabId
        {
            get
            {
                object o = this.Settings["upperTabId"];
                if (o == null || (!int.TryParse(o.ToString(), out this._upperTabId) && o != string.Empty))
                {
                    this._upperTabId = Convert.ToInt32(o);
                }

                return this._upperTabId;
            }
        }

        public string CleanupText(string text)
        {
            string returnVal = this.Server.HtmlEncode(text);
            if (returnVal.Length > 500)
            {
                return returnVal.Substring(0, 500);
            }

            return returnVal;
        }

        public string GetEditLink(int moduleId, int tabid)
        {
            return Globals.NavigateURL(tabid, "edit", "&mid=" + moduleId);
        }

        public string GetPublishLink(int itemId)
        {
            return ApplicationUrl + "/desktopmodules/engagepublish/itemlink.aspx?itemId=" + itemId;
        }

        protected void BindData(string searchString)
        {
            // bind the data
            if (this.UserInfo.IsSuperUser)
            {
                this.dgResults.DataSource = DataProvider.Instance().GetLinks(searchString, this.LowerTabId, this.UpperTabId);
                this.dgResults.DataBind();
            }
            else
            {
                this.dgResults.DataSource = DataProvider.Instance().GetLinks(searchString, this.PortalId, this.LowerTabId, this.UpperTabId);
                this.dgResults.DataBind();
            }

            this.pnlReplacement.Visible = true;
        }

        protected void BindPublishData(string searchString)
        {
            // bind the data
            this.dgPublishResults.DataSource = DataProvider.Instance().GetPublishLinks(searchString, this.PortalId);
            this.dgPublishResults.DataBind();
        }

        protected void btnEngagePublish_Click(object sender, EventArgs e)
        {
            this.BindPublishData(this.txtSearchString.Text.Trim());

            this.dgResults.Visible = false;
            this.dgPublishResults.Visible = true;
            this.pnlReplacement.Visible = true;
        }

        protected void btnReplace_Click(object sender, EventArgs e)
        {
            try
            {
                // loop through the Text/HTML modules and start updating fields
                string replacementString = this.txtReplacementText.Text.Trim();
                string searchString = this.txtSearchString.Text.Trim();
                if (replacementString != string.Empty && searchString != string.Empty)
                {
                    DataTable dt = this.UserInfo.IsSuperUser
                                           ? DataProvider.Instance().GetLinks(searchString, this.LowerTabId, this.UpperTabId)
                                           : DataProvider.Instance().GetLinks(searchString, this.PortalId, this.LowerTabId, this.UpperTabId);

                    foreach (DataRow dr in dt.Rows)
                    {
                        int itemId = Convert.ToInt32(dr["ItemId"]);
                        int stateId = Convert.ToInt32(dr["StateId"]);
                        bool isPublished = Convert.ToBoolean(dr["IsPublished"]);
                        string content = dr["Content"].ToString();
                        content = content.Replace(searchString, replacementString);

                        DataProvider.Instance().ReplaceTextHTML(itemId, content, stateId, isPublished, this.UserId);
                    }

                    string replacementResults = Localization.GetString("replacementResults", this.LocalResourceFile);
                    this.lblReplacementResults.Text = String.Format(replacementResults, searchString, replacementString, dt.Rows.Count);
                    this.lblReplacementResults.Visible = true;
                    this.dgResults.Visible = false;
                    this.dgPublishResults.Visible = false;
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
                // loop through the Text/HTML modules and start updating fields
                string replacementString = this.txtReplacementText.Text.Trim();
                string searchString = this.txtSearchString.Text.Trim();
                if (replacementString != string.Empty && searchString != string.Empty)
                {
                    DataTable dt = DataProvider.Instance().GetPublishLinks(searchString, this.PortalId);

                    foreach (DataRow dr in dt.Rows)
                    {
                        Article a = Article.GetArticle(Convert.ToInt32(dr["ItemId"]), this.PortalId, true, true);

                        string articleDescription = a.Description.Replace(searchString, replacementString);
                        string articleBody = a.ArticleText.Replace(searchString, replacementString);
                        a.ArticleText = articleBody;
                        a.Description = articleDescription;
                        a.Save(this.UserInfo.UserID);
                    }

                    string replacementResults = Localization.GetString("replacementResults", this.LocalResourceFile);
                    this.lblReplacementResults.Text = String.Format(replacementResults, searchString, replacementString, dt.Rows.Count);
                    this.lblReplacementResults.Visible = true;
                    this.dgResults.Visible = false;
                    this.dgPublishResults.Visible = false;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                // search for the String and setup the results
                this.BindData(this.txtSearchString.Text.Trim());
                this.txtReplacementText.Text = string.Empty;
                this.lblReplacementResults.Text = string.Empty;
                this.dgResults.Visible = true;
                this.dgPublishResults.Visible = false;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            this.Load += this.Page_Load;
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.btnEngagePublish.Visible = this.Settings.Contains("chkEnablePublish")
                                                && Convert.ToBoolean(this.Settings["chkEnablePublish"].ToString());
            }
            catch (Exception exc)
            {
                // Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}