// <copyright file="IHtmlTextModuleController.cs" company="Engage Software">
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
    /// <summary>
    /// Represents the behaviors required to create a new text/html version
    /// </summary>
    public interface IHtmlTextModuleController
    {
        IHtmlTextInfo GetTopHtmlText(int moduleId, int tabId, int portalId);

        IHtmlTextInfo CreateNewHtmlTextInfo();

        void SaveHtmlContent(IHtmlTextInfo htmlTextInfo, int moduleId, int tabId, int portalId);
    }
}