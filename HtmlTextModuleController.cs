// <copyright file="HtmlTextModuleController.cs" company="Engage Software">
// Engage: F3
// Copyright (c) 2004-2010
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
    using System.Reflection;
    using DotNetNuke.Framework;

    /// <summary>
    /// Implements <see cref="IHtmlTextModuleController"/> by wrapping a <see cref="DotNetNuke.Modules.Html.HtmlTextController"/> instance created via reflection
    /// </summary>
    /// <remarks>Need to use reflection because type could be in <c>DotNetNuke.Modules.Html</c> assembly or in <c>DotNetNuke.Professional.HtmlPro</c> assembly.</remarks>
    public class HtmlTextModuleController : IHtmlTextModuleController
    {
        private static readonly Type HtmlTextControllerType = Reflection.CreateType("DotNetNuke.Modules.Html.HtmlTextController");
        private static readonly Type WorkflowStateControllerType = Reflection.CreateType("DotNetNuke.Modules.Html.WorkflowStateController");
        private readonly object htmlTextController;
        private readonly object workflowStateController;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTextModuleController"/> class.
        /// </summary>
        public HtmlTextModuleController()
        {
            this.htmlTextController = Reflection.CreateInstance(HtmlTextControllerType);
            this.workflowStateController = Reflection.CreateInstance(WorkflowStateControllerType);
        }

        public int GetWorkflowId(int moduleId, int portalId)
        {
            return (int)HtmlTextControllerType.InvokeMember("GetWorkflowID", BindingFlags.InvokeMethod, null, this.htmlTextController, new object[] { moduleId, portalId }, null);
        }

        public IHtmlTextInfo GetTopHtmlText(int moduleId, bool isPublished, int workflowId)
        {
            return new HtmlTextInfo(HtmlTextControllerType.InvokeMember("GetTopHtmlText", BindingFlags.InvokeMethod, null, this.htmlTextController, new object[] { moduleId, isPublished, workflowId }, null));
        }

        public IHtmlTextInfo CreateNewHtmlTextInfo()
        {
            return new HtmlTextInfo();
        }

        public void UpdateHtmlText(IHtmlTextInfo htmlTextInfo, int maximumVersionHistory)
        {
            Reflection.InvokeMethod(HtmlTextControllerType, "UpdateHtmlText", this.htmlTextController, new object[] { htmlTextInfo.HtmlTextInfoInstance, maximumVersionHistory });
        }

        public int GetFirstWorkflowStateId(int workflowId)
        {
            return (int)WorkflowStateControllerType.InvokeMember("GetFirstWorkflowStateID", BindingFlags.InvokeMethod, null, this.workflowStateController, new object[] { workflowId }, null);
        }

        public int GetMaximumVersionHistory(int portalId)
        {
            return (int)HtmlTextControllerType.InvokeMember("GetMaximumVersionHistory", BindingFlags.InvokeMethod, null, this.htmlTextController, new object[] { portalId }, null);
        }
    }
}