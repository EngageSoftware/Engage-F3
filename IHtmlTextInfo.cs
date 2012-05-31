// <copyright file="IHtmlTextInfo.cs" company="Engage Software">
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
    /// <summary>
    /// The properties required to create a new version of an <see cref="DotNetNuke.Modules.Html.HtmlTextInfo"/> instance
    /// </summary>
    public interface IHtmlTextInfo
    {
        int ModuleId
        {
            get;
            set;
        }

        string Content
        {
            get;
            set;
        }

        int WorkflowId
        {
            get;
            set;
        }

        int StateId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the actual <see cref="DotNetNuke.Modules.Html.HtmlTextInfo"/> instance.
        /// </summary>
        /// <value>An <see cref="DotNetNuke.Modules.Html.HtmlTextInfo"/> instance.</value>
        object HtmlTextInfoInstance
        {
            get;
        }
    }
}