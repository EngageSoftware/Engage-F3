// <copyright file="HtmlTextInfo.cs" company="Engage Software">
// Engage: F3
// Copyright (c) 2004-2013
// by Engage Software ( http://www.engagesoftware.com )
// </copyright>
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

namespace Engage.Dnn.F3
{
    using System;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Framework;

    /// <summary>
    /// Implements <see cref="IHtmlTextInfo"/> by wrapping a <see cref="DotNetNuke.Modules.Html.HtmlTextInfo"/> instance created via reflection
    /// </summary>
    /// <remarks>Need to use reflection because type could be in <c>DotNetNuke.Modules.Html</c> assembly or in <c>DotNetNuke.Professional.HtmlPro</c> assembly.</remarks>
    public class HtmlTextInfo : IHtmlTextInfo
    {
        /// <summary>
        /// The type of the <c>HtmlTextInfo</c> that the module is using (CE or PE)
        /// </summary>
        private static readonly Type HtmlTextInfoType = 
            Reflection.CreateType("DotNetNuke.Modules.HtmlPro.HtmlTextInfo")
            ?? Reflection.CreateType("DotNetNuke.Modules.Html.HtmlTextInfo");

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTextInfo"/> class.
        /// </summary>
        public HtmlTextInfo()
        {
            this.HtmlTextInfoInstance = Reflection.CreateInstance(HtmlTextInfoType);
            Reflection.SetProperty(HtmlTextInfoType, "ItemID", this.HtmlTextInfoInstance, new object[] { Null.NullInteger });
        }

        public HtmlTextInfo(object htmlTextInfoInstance)
        {
            this.HtmlTextInfoInstance = htmlTextInfoInstance;
        }

        public int ModuleId
        {
            get
            {
                return (int)Reflection.GetProperty(HtmlTextInfoType, "ModuleID", this.HtmlTextInfoInstance);
            }

            set
            {
                Reflection.SetProperty(HtmlTextInfoType, "ModuleID", this.HtmlTextInfoInstance, new object[] { value });
            }
        }

        public string Content
        {
            get
            {
                return (string)Reflection.GetProperty(HtmlTextInfoType, "Content", this.HtmlTextInfoInstance);
            }

            set
            {
                Reflection.SetProperty(HtmlTextInfoType, "Content", this.HtmlTextInfoInstance, new object[] { value });
            }
        }

        public int WorkflowId
        {
            get
            {
                return (int)Reflection.GetProperty(HtmlTextInfoType, "WorkflowID", this.HtmlTextInfoInstance);
            }

            set
            {
                Reflection.SetProperty(HtmlTextInfoType, "WorkflowID", this.HtmlTextInfoInstance, new object[] { value });
            }
        }

        public int StateId
        {
            get
            {
                return (int)Reflection.GetProperty(HtmlTextInfoType, "StateID", this.HtmlTextInfoInstance);
            }

            set
            {
                Reflection.SetProperty(HtmlTextInfoType, "StateID", this.HtmlTextInfoInstance, new object[] { value });
            }
        }

        /// <summary>
        /// Gets the actual <see cref="DotNetNuke.Modules.Html.HtmlTextInfo"/> instance.
        /// </summary>
        /// <value>An <see cref="DotNetNuke.Modules.Html.HtmlTextInfo"/> instance.</value>
        public object HtmlTextInfoInstance
        {
            get;
            private set;
        }
    }
}