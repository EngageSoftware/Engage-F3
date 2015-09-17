// <copyright file="HtmlTextModuleController.cs" company="Engage Software">
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
    using System.Collections.Generic;
    using System.Reflection;

    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Framework;

    /// <summary>
    /// Implements <see cref="IHtmlTextModuleController"/> by wrapping a <see cref="DotNetNuke.Modules.Html.HtmlTextController"/> instance created via reflection
    /// </summary>
    /// <remarks>Need to use reflection because type could be in <c>DotNetNuke.Modules.Html</c> assembly or in <c>DotNetNuke.Professional.HtmlPro</c> assembly.</remarks>
    public class HtmlTextModuleController : IHtmlTextModuleController
    {
        /// <summary>
        /// The type of the <c>HtmlTextController</c> that the module is using (CE or PE)
        /// </summary>
        private static readonly Type HtmlTextControllerType = 
            Reflection.CreateType("DotNetNuke.Professional.HtmlPro.HtmlTextController")  // DNN 7.0 Professional
            ?? Reflection.CreateType("DotNetNuke.Modules.HtmlPro.HtmlTextController")  // DNN 5.6 Professional
            ?? Reflection.CreateType("DotNetNuke.Modules.Html.HtmlTextController");

        /// <summary>
        /// The type of the <c>WorkflowStateController</c> that the module is using (CE or PE)
        /// </summary>
        private static readonly Type WorkflowStateControllerType =
            Reflection.CreateType("DotNetNuke.Professional.HtmlPro.Components.WorkflowStateController") // DNN 5.6 Professional
            ?? Reflection.CreateType("DotNetNuke.Modules.HtmlPro.WorkflowStateController") // DNN 5.6 Professional
            ?? Reflection.CreateType("DotNetNuke.Modules.Html.WorkflowStateController");

        /// <summary>
        /// An <c>HtmlTextController</c> instance, of the <see cref="HtmlTextControllerType"/>
        /// </summary>
        private readonly object htmlTextController;

        /// <summary>
        /// A <c>WorkflowStateController</c> instance, of the <see cref="WorkflowStateControllerType"/>
        /// </summary>
        private readonly object workflowStateController;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTextModuleController"/> class.
        /// </summary>
        public HtmlTextModuleController()
        {
            this.htmlTextController = Reflection.CreateInstance(HtmlTextControllerType);
            this.workflowStateController = Reflection.CreateInstance(WorkflowStateControllerType);
        }

        private int? GetWorkflowId(int moduleId, int tabId, int portalId)
        {
            var getWorkflowId = HtmlTextControllerType.GetMethod("GetWorkflowID");
            if (getWorkflowId != null)
            {
                // DNN 5.2
                return (int)getWorkflowId.Invoke(
                    this.htmlTextController,
                    BindingFlags.InvokeMethod,
                    null,
                    new object[] { moduleId, portalId },
                    null);
            }

            // DNN 5.4
            var getWorkflow = HtmlTextControllerType.GetMethod("GetWorkflow");
            if (getWorkflow != null)
            {
                var workflowPair = (KeyValuePair<string, int>)HtmlTextControllerType.InvokeMember(
                    "GetWorkflow",
                    BindingFlags.InvokeMethod,
                    null,
                    this.htmlTextController,
                    new object[] { moduleId, tabId, portalId },
                    null);

                return workflowPair.Value;
            }

            return null;
        }

        public IHtmlTextInfo GetTopHtmlText(int moduleId, int tabId, int portalId)
        {
            var workflowId = this.GetWorkflowId(moduleId, tabId, portalId);
            if (workflowId != null)
            {
                return
                    new HtmlTextInfo(
                        HtmlTextControllerType.InvokeMember(
                            "GetTopHtmlText",
                            BindingFlags.InvokeMethod,
                            null,
                            this.htmlTextController,
                            new object[] { moduleId, false, workflowId.Value },
                            null));
            }

            var serviceLocatorType = Reflection.CreateType("DotNetNuke.Framework.ServiceLocator`2");
            var versionControllerInterface = Reflection.CreateType("DotNetNuke.Professional.HtmlPro.Components.IVersionController");
            var versionControllerClass = Reflection.CreateType("DotNetNuke.Professional.HtmlPro.Components.VersionController");
            var versionControllerLocatorType = serviceLocatorType.MakeGenericType(versionControllerInterface, versionControllerClass);
            var versionControllerInstance = versionControllerLocatorType.InvokeMember(
                "Instance",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty,
                null,
                null,
                new object[0],
                null);
            
            var htmlInfo = versionControllerInterface.InvokeMember(
                "GetLatestVersion",
                BindingFlags.InvokeMethod,
                null,
                versionControllerInstance,
                new object[] { moduleId },
                null);

            return htmlInfo == null ? null : new HtmlTextInfo(htmlInfo);
        }

        public IHtmlTextInfo CreateNewHtmlTextInfo()
        {
            return new HtmlTextInfo();
        }

        public void SaveHtmlContent(IHtmlTextInfo htmlTextInfo, int moduleId, int tabId, int portalId)
        {
            if (htmlTextInfo == null)
            {
                throw new ArgumentNullException("htmlTextInfo");
            }

            var workflowId = this.GetWorkflowId(moduleId, tabId, portalId);
            if (workflowId != null)
            {
                htmlTextInfo.WorkflowId = workflowId.Value;
                htmlTextInfo.StateId = this.GetFirstWorkflowStateId(workflowId.Value);
                this.UpdateHtmlText(htmlTextInfo, this.GetMaximumVersionHistory(portalId));
                return;
            }

            var module = new ModuleController().GetModule(moduleId, tabId, true);
            Reflection.InvokeMethod(HtmlTextControllerType, "SaveHtmlContent", this.htmlTextController, new object[] { htmlTextInfo.HtmlTextInfoInstance, module, });
        }

        public void UpdateHtmlText(IHtmlTextInfo htmlTextInfo, int maximumVersionHistory)
        {
            if (htmlTextInfo == null)
            {
                throw new ArgumentNullException("htmlTextInfo");
            }

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