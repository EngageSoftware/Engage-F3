// <copyright file="ViewLinks.ascx.cs" company="Engage Software">
// Engage: F3
// Copyright (c) 2004-2013
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
    using System.Globalization;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Host;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using Publish;

    /// <summary>
    /// Allows the user to search for content in either module and to replace values once found
    /// </summary>
    public partial class ViewLinks : PortalModuleBase
    {
        /// <summary>
        /// Backing field for <see cref="LowerTabId"/>
        /// </summary>
        private int? lowerTabId;

        /// <summary>
        /// Backing field for <see cref="UpperTabId"/>
        /// </summary>
        private int? upperTabId;

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

        /// <summary>
        /// Gets the lowest tab ID to search when searching Text/HTML modules.
        /// </summary>
        /// <value>The lowest tab ID for Text/HTML search.</value>
        public int? LowerTabId
        {
            get
            {
                if (!this.lowerTabId.HasValue)
                {
                    int tabId;
                    var lowerTabIdValue = (string)this.Settings["lowerTabId"];
                    if (int.TryParse(lowerTabIdValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out tabId))
                    {
                        this.lowerTabId = tabId;
                    }
                }

                return this.lowerTabId;
            }
        }

        /// <summary>
        /// Gets the highest tab ID to search when searching Text/HTML modules.
        /// </summary>
        /// <value>The highest tab ID for Text/HTML search.</value>
        public int? UpperTabId
        {
            get
            {
                int tabId;
                var upperTabIdValue = (string)this.Settings["upperTabId"];
                if (int.TryParse(upperTabIdValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out tabId))
                {
                    this.upperTabId = tabId;
                }

                return this.upperTabId;
            }
        }

        /// <summary>
        /// Gets the ID of the portal to search, or <c>null</c> to search all portals.
        /// </summary>
        /// <value>The portal ID or <c>null</c> for all portals.</value>
        public int? SearchPortalId
        {
            get
            {
                return this.AllPortalsCheckBox.Checked ? (int?)null : this.PortalId;
            }
        }

        protected static string CleanupText(string text, bool encodeText)
        {
            if (encodeText)
            {
                text = HttpUtility.HtmlEncode(text);
            }

            if (text.Length > 500)
            {
                return text.Substring(0, 500) + "&#8230;";
            }

            return text;
        }

        protected static string GetPublishViewLink(int itemId)
        {
            return ApplicationUrl + "/desktopmodules/engagepublish/itemlink.aspx?itemId=" + itemId;
        }

        protected static string GetTextHtmlModuleEditLink(int moduleId, int tabId)
        {
            return Globals.NavigateURL(tabId, "edit", "&mid=" + moduleId);
        }

        protected void BindPublishData(string searchValue)
        {
            this.PublishResultsGrid.DataSource = DataProvider.Instance.SearchPublishContent(searchValue, this.SearchPortalId);
            this.PublishResultsGrid.DataBind();

            // only show portal name column when we're searching multiple portals
            this.PublishResultsGrid.Columns[0].Visible = !this.SearchPortalId.HasValue;
        }

        protected void BindTextHtmlData(string searchValue)
        {
            this.ResultsGrid.DataSource = DataProvider.Instance.SearchTextHtmlContent(searchValue, this.SearchPortalId, this.LowerTabId, this.UpperTabId);
            this.ResultsGrid.DataBind();

            // only show portal name column when we're searching multiple portals
            this.ResultsGrid.Columns[0].Visible = !this.SearchPortalId.HasValue;
        }

        /// <summary>
        /// Raises the <see cref="Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.Load += this.Page_Load;
            this.SearchTextHtmlButton.Click += this.SearchTextHtmlButton_Click;
            this.SearchPublishButton.Click += this.SearchPublishButton_Click;
            this.ReplaceTextHtmlButton.Click += this.ReplaceTextHtmlButton_Click;
            this.ReplacePublishButton.Click += this.ReplacePublishButton_Click;
            base.OnInit(e);
        }

        /// <summary>
        /// Creates the new version of the Text/HTML content for the module with the given ID, with <paramref name="searchValue"/> replaced by <paramref name="replacementValue"/>.
        /// </summary>
        /// <param name="searchValue">The search value.</param>
        /// <param name="replacementValue">The replacement value.</param>
        /// <param name="moduleId">The module id.</param>
        /// <param name="tabId">The tab id.</param>
        /// <param name="portalId">The portal id.</param>
        /// <param name="content">The content.</param>
        private static void CreateNewTextHtmlVersion(string searchValue, string replacementValue, int moduleId, int tabId, int portalId, string content)
        {
            var htmlTextController = CreateHtmlTextController();
            var workflowId = htmlTextController.GetWorkflowId(moduleId, tabId, portalId);
            var htmlInfo = htmlTextController.GetTopHtmlText(moduleId, false, workflowId) ?? htmlTextController.CreateNewHtmlTextInfo();
            htmlInfo.ModuleId = moduleId;
            htmlInfo.Content = content.Replace(HttpUtility.HtmlEncode(searchValue), HttpUtility.HtmlEncode(replacementValue));
            htmlInfo.WorkflowId = workflowId;
            htmlInfo.StateId = htmlTextController.GetFirstWorkflowStateId(workflowId);

            // TODO: allow direct publish
            ////if (canDirectlyPublish)
            ////{
            ////    htmlInfo.StateID = workflowStateController.GetNextWorkflowStateID(workflowId, htmlInfo.StateID);
            ////}

            htmlTextController.UpdateHtmlText(htmlInfo, htmlTextController.GetMaximumVersionHistory(portalId));
        }

        /// <summary>
        /// Creates an instance of the HTML/Text module's controller.
        /// </summary>
        /// <returns>A <see cref="IHtmlTextModuleController"/> instance</returns>
        private static IHtmlTextModuleController CreateHtmlTextController()
        {
            return new HtmlTextModuleController();
        }

        /// <summary>
        /// Handles the <see cref="Control.Load"/> event of this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.AllPortalsCheckBox.Visible = this.UserInfo.IsSuperUser;
                this.SearchPublishButton.Visible = this.Settings.Contains("chkEnablePublish")
                                                   && bool.Parse(this.Settings["chkEnablePublish"].ToString());
            }
            catch (Exception exc)
            {
                // Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the <see cref="Button.Click"/> event of the <see cref="ReplacePublishButton"/> control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ReplacePublishButton_Click(object sender, EventArgs e)
        {
            try
            {
                var searchValue = this.SearchTextBox.Text.Trim();
                var replacementValue = this.ReplacementTextBox.Text;
                if (string.IsNullOrEmpty(searchValue))
                {
                    return;
                }

                DataTable searchResults = DataProvider.Instance.SearchPublishContent(searchValue, this.SearchPortalId);

                foreach (DataRow resultRow in searchResults.Rows)
                {
                    var article = Article.GetArticle((int)resultRow["ItemId"], this.PortalId, true, true, true);

                    article.ArticleText = article.ArticleText.Replace(searchValue, replacementValue);
                    article.Description = article.Description.Replace(searchValue, replacementValue);

                    article.Save(this.UserInfo.UserID);
                }

                this.ReplacementResultsLabel.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Localization.GetString("replacementResults", this.LocalResourceFile),
                    searchValue,
                    replacementValue,
                    searchResults.Rows.Count);
                this.ReplacementResultsLabel.Visible = true;
                this.ResultsGrid.Visible = false;
                this.PublishResultsGrid.Visible = false;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the <see cref="Button.Click"/> event of the <see cref="ReplaceTextHtmlButton"/> control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ReplaceTextHtmlButton_Click(object sender, EventArgs e)
        {
            // Get the initial caching level 
            var initialCachingLevel = (int)Host.PerformanceSetting;

            try
            {
                var searchValue = this.SearchTextBox.Text.Trim();
                var replacementValue = this.ReplacementTextBox.Text;
                if (string.IsNullOrEmpty(searchValue))
                {
                    return;
                }

                DataTable searchResults = DataProvider.Instance.SearchUniqueTextHtmlContent(searchValue, this.SearchPortalId, this.LowerTabId, this.UpperTabId);

                // Disable caching 
                new HostSettingsController().UpdateHostSetting("PerformanceSetting", ((int)Globals.PerformanceSettings.NoCaching).ToString(CultureInfo.InvariantCulture));
                DataCache.ClearHostCache(true);

                foreach (DataRow searchResultRow in searchResults.Rows)
                {
                    var moduleId = (int)searchResultRow["ModuleID"];
                    var tabId = (int)searchResultRow["TabID"];
                    var portalId = (int)searchResultRow["PortalID"];
                    var content = searchResultRow["Content"].ToString();

                    CreateNewTextHtmlVersion(searchValue, replacementValue, moduleId, tabId, portalId, content);
                }

                this.ReplacementResultsLabel.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Localization.GetString("TextHtmlReplacementResults.Format", this.LocalResourceFile),
                    searchValue,
                    replacementValue,
                    searchResults.Rows.Count);
                this.ReplacementResultsLabel.Visible = true;
                this.ResultsGrid.Visible = false;
                this.PublishResultsGrid.Visible = false;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            finally
            {
                // Enable caching with 
                // the initial caching level 
                new HostSettingsController().UpdateHostSetting("PerformanceSetting", initialCachingLevel.ToString(CultureInfo.InvariantCulture));                
            }
        }

        /// <summary>
        /// Handles the <see cref="Button.Click"/> event of the <see cref="SearchPublishButton"/> control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SearchPublishButton_Click(object sender, EventArgs e)
        {
            this.BindPublishData(this.SearchTextBox.Text.Trim());
            this.ShowResults(true);
        }

        /// <summary>
        /// Handles the <see cref="Button.Click"/> event of the <see cref="SearchTextHtmlButton"/> control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SearchTextHtmlButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.BindTextHtmlData(this.SearchTextBox.Text.Trim());
                this.ShowResults(false);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Shows the proper results grid and the <see cref="ReplacementPanel"/>, resetting its display and showing either the Text/HTML results and buttons.
        /// </summary>
        /// <param name="showPublish">if set to <c>true</c> shows the Engage: Publish search results and replace button, otherwise the Text/HTML results and button.</param>
        private void ShowResults(bool showPublish)
        {
            this.ReplacementPanel.Visible = true;
            this.ReplacementTextBox.Text = string.Empty;
            this.ReplacementResultsLabel.Text = string.Empty;

            this.ReplaceTextHtmlButton.Visible = !showPublish;
            this.ReplacePublishButton.Visible = showPublish;

            this.ResultsGrid.Visible = !showPublish;
            this.PublishResultsGrid.Visible = showPublish;
        }
    }
}