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
        /// <summary>
        /// Initializes static members of the <see cref="DataProvider"/> class.
        /// </summary>
        static DataProvider()
        {
            CreateProvider();
        }

        /// <summary>
        /// Gets the <see cref="DataProvider"/> instance.
        /// </summary>
        /// <value>The instance of the <see cref="DataProvider"/> class.</value>
        public static new DataProvider Instance
        {
            get;
            private set;
        }

        public abstract DataTable SearchTextHtmlContent(string searchValue, int? portalId, int? lowerTab, int? upperTab);

        public abstract DataTable SearchPublishContent(string searchValue, int portalId);

        public abstract void ReplaceTextHtml(int itemId, string content, int stateId, bool isPublished, int userId);

        /// <summary>
        /// Creates the provider.
        /// </summary>
        private static void CreateProvider()
        {
            Instance = (DataProvider)Reflection.CreateObject("data", "Engage.Dnn.F3", string.Empty);
        }
    }
}