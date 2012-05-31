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

namespace Engage.Dnn.F3
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// An abstract class for the data access layer
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public abstract class DataProvider
    {

        #region Shared/Static Methods

        // singleton reference to the instantiated object 
        private static DataProvider objProvider = null;

        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (DataProvider)(DotNetNuke.Framework.Reflection.CreateObject("data", "Engage.Dnn.F3", ""));
        }

        // return the provider
        public static new DataProvider Instance()
        {
            return objProvider;
        }

        #endregion

        #region Abstract methods

        public abstract DataTable GetLinks(string searchString, int portalId, int lowerTab, int upperTab);
        public abstract DataTable GetLinks(string searchString, int lowerTab, int upperTab);

        public abstract DataTable GetPublishLinks(string searchString, int portalId);

        public abstract void ReplaceTextHTML(int moduleId, string desktopHtml, string desktopSummary, int userId);

        #endregion

    }

}