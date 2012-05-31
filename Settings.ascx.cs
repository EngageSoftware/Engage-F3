// <copyright file="Settings.ascx.cs" company="Engage Software">
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
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Services.Exceptions;

    /// <summary>
    /// The Settings class manages Module Settings
    /// </summary>
    public partial class Settings : ModuleSettingsBase
    {
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                if (this.Page.IsPostBack == false)
                {
                    if (this.Settings.Contains("chkEnablePublish"))
                    {
                        this.chkEnablePublish.Checked = Convert.ToBoolean(this.Settings["chkEnablePublish"].ToString());
                    }

                    if (this.Settings.Contains("lowerTabId"))
                    {
                        this.txtLowerTab.Text = this.Settings["lowerTabId"].ToString();
                    }

                    if (this.Settings.Contains("upperTabId"))
                    {
                        this.txtUpperTab.Text = this.Settings["upperTabId"].ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                // Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            try
            {
                var objModules = new ModuleController();

                objModules.UpdateTabModuleSetting(this.TabModuleId, "chkEnablePublish", this.chkEnablePublish.Checked.ToString());
                objModules.UpdateTabModuleSetting(this.TabModuleId, "lowerTabId", this.txtLowerTab.Text);
                objModules.UpdateTabModuleSetting(this.TabModuleId, "upperTabId", this.txtUpperTab.Text);
            }
            catch (Exception exc)
            {
                // Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}