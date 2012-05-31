// <copyright file="SqlDataProvider.cs" company="Engage Software">
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
    using System.Text;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Framework.Providers;
    using Microsoft.ApplicationBlocks.Data;

    /// <summary>
    /// SQL Server implementation of the abstract <see cref="DataProvider"/> class
    /// </summary>
    public class SqlDataProvider : DataProvider
    {
        private const string ModuleQualifier = "";

        private const string ProviderType = "data";

        private string _connectionString;

        private string _databaseOwner;

        private string _objectQualifier;

        private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);

        private string _providerPath;

        public SqlDataProvider()
        {
            // Read the configuration specific information for this provider
            var objProvider = (Provider)this._providerConfiguration.Providers[this._providerConfiguration.DefaultProvider];

            // Read the attributes for this provider

            // Get Connection string from web.config
            this._connectionString = Config.GetConnectionString();

            if (this._connectionString == string.Empty)
            {
                // Use connection string specified in provider
                this._connectionString = objProvider.Attributes["connectionString"];
            }

            this._providerPath = objProvider.Attributes["providerPath"];

            this._objectQualifier = objProvider.Attributes["objectQualifier"];
            if (this._objectQualifier != string.Empty & this._objectQualifier.EndsWith("_") == false)
            {
                this._objectQualifier += "_";
            }

            this._databaseOwner = objProvider.Attributes["databaseOwner"];
            if (this._databaseOwner != string.Empty & this._databaseOwner.EndsWith(".") == false)
            {
                this._databaseOwner += ".";
            }
        }

        public string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return this._databaseOwner;
            }
        }

        public string NamePrefix
        {
            get
            {
                return this._databaseOwner + this._objectQualifier + ModuleQualifier;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return this._objectQualifier;
            }
        }

        public string ProviderPath
        {
            get
            {
                return this._providerPath;
            }
        }

        public override DataTable GetLinks(string searchString, int portalId, int lowerTab, int upperTab)
        {
            var sql = new StringBuilder(128);

            // sql.Append("select ht.ModuleId, tm.TabId, ht.Content, m.ModuleTitle, t.TabName ");
            sql.Append("select ht.ModuleId, ht.ItemId, tm.TabId, ht.Content, ht.StateID, ht.IsPublished, m.ModuleTitle, t.TabName ");
            sql.Append("from ");
            sql.Append(this.NamePrefix);
            sql.Append("htmltext ht ");
            sql.Append(" join ");
            sql.Append(this.NamePrefix);
            sql.Append("tabmodules tm on (tm.moduleid = ht.moduleid)");
            sql.Append(" join ");
            sql.Append(this.NamePrefix);
            sql.Append("modules m on (m.moduleid = tm.moduleid)");

            sql.Append(" join ");
            sql.Append(this.NamePrefix);
            sql.Append("tabs t on (t.tabid = tm.tabid)");

            sql.Append("where ");
            sql.Append("ht.Content collate SQL_Latin1_General_CP1_CS_AS  like '%");
            sql.Append(searchString);
            sql.Append("%' and m.PortalId = ");
            sql.Append(portalId.ToString());
            if (lowerTab > 0 && upperTab > lowerTab)
            {
                sql.Append(" and t.tabid >= ");
                sql.Append(lowerTab);
                sql.Append(" and t.tabid <= ");
                sql.Append(upperTab);
            }

            DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql.ToString());
            return ds.Tables[0];
        }

        public override DataTable GetLinks(string searchString, int lowerTab, int upperTab)
        {
            var sql = new StringBuilder(128);

            sql.Append("select ht.ModuleId, ht.ItemId, tm.TabId, ht.Content, ht.StateID, ht.IsPublished, m.ModuleTitle, t.TabName ");
            sql.Append("from ");
            sql.Append(this.NamePrefix);
            sql.Append("htmltext ht ");
            sql.Append(" join ");
            sql.Append(this.NamePrefix);
            sql.Append("tabmodules tm on (tm.moduleid = ht.moduleid)");
            sql.Append(" join ");
            sql.Append(this.ObjectQualifier);
            sql.Append("modules m on (m.moduleid = tm.moduleid)");
            sql.Append(" join ");
            sql.Append(this.NamePrefix);
            sql.Append("tabs t on (t.tabid = tm.tabid)");
            sql.Append("where ");
            sql.Append("ht.Content collate SQL_Latin1_General_CP1_CS_AS like '%");
            sql.Append(searchString);
            sql.Append("%' ");
            if (lowerTab > 0 && upperTab > lowerTab)
            {
                sql.Append(" and t.tabid >= ");
                sql.Append(lowerTab);
                sql.Append(" and t.tabid <= ");
                sql.Append(upperTab);
            }

            DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql.ToString());
            return ds.Tables[0];
        }

        // public abstract DataTable GetPublishLinks(string searchString, int portalId);
        public override DataTable GetPublishLinks(string searchString, int portalId)
        {
            var sql = new StringBuilder(128);

            sql.Append("select va.name, va.ItemId, va.articletext, va.displaytabid, t.TabName ");
            sql.Append("from ");
            sql.Append(this.ObjectQualifier);
            sql.Append("publish_vwarticles va ");
            sql.Append(" join ");
            sql.Append(this.ObjectQualifier);
            sql.Append("tabs t on t.tabid = va.displaytabid ");

            sql.Append(" where articletext collate SQL_Latin1_General_CP1_CS_AS like '%");
            sql.Append(searchString);
            sql.Append("%' and va.IsCurrentVersion=1 and va.PortalId = ");
            sql.Append(portalId.ToString());

            DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql.ToString());
            return ds.Tables[0];
        }

        public override void ReplaceTextHTML(int itemId, string content, int stateId, bool isPublished, int userId)
        {
            SqlHelper.ExecuteNonQuery(
                    this.ConnectionString, this.DatabaseOwner + this.ObjectQualifier + "UpdateHtmlText", itemId, content, stateId, isPublished, userId);
        }
    }
}