// <copyright file="DataProvider.cs" company="Engage Software">
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
    using System.Data;
    using DotNetNuke.Framework;

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
        // singleton reference to the instantiated object 
        private static DataProvider _objProvider;

        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider

        // return the provider
        public static new DataProvider Instance()
        {
            return _objProvider;
        }

        public abstract DataTable GetLinks(string searchString, int portalId, int lowerTab, int upperTab);

        public abstract DataTable GetLinks(string searchString, int lowerTab, int upperTab);

        public abstract DataTable GetPublishLinks(string searchString, int portalId);

        // public abstract void ReplaceTextHTML(int moduleId, string desktopHtml, string desktopSummary, int userId);
        public abstract void ReplaceTextHTML(int itemId, string content, int stateId, bool isPublished, int userId);

        private static void CreateProvider()
        {
            _objProvider = (DataProvider)Reflection.CreateObject("data", "Engage.Dnn.F3", string.Empty);
        }
    }
}