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
    using System;
    using System.Data;
    using System.Data.SqlClient;
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

        private ProviderConfiguration providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataProvider"/> class.
        /// </summary>
        public SqlDataProvider()
        {
            // Read the configuration specific information for this provider
            var objProvider = (Provider)this.providerConfiguration.Providers[this.providerConfiguration.DefaultProvider];

            // Read the attributes for this provider

            // Get Connection string from web.config
            this.ConnectionString = Config.GetConnectionString();

            if (string.IsNullOrEmpty(this.ConnectionString))
            {
                // Use connection string specified in provider
                this.ConnectionString = objProvider.Attributes["connectionString"];
            }

            this.ProviderPath = objProvider.Attributes["providerPath"];

            this.ObjectQualifier = objProvider.Attributes["objectQualifier"];
            if (!string.IsNullOrEmpty(this.ObjectQualifier) && !this.ObjectQualifier.EndsWith("_", StringComparison.Ordinal))
            {
                this.ObjectQualifier += "_";
            }

            this.DatabaseOwner = objProvider.Attributes["databaseOwner"];
            if (!string.IsNullOrEmpty(this.DatabaseOwner) && !this.DatabaseOwner.EndsWith(".", StringComparison.Ordinal))
            {
                this.DatabaseOwner += ".";
            }
        }

        public string ConnectionString
        {
            get;
            private set;
        }

        public string DatabaseOwner
        {
            get;
            private set;
        }

        public string NamePrefix
        {
            get
            {
                return this.DatabaseOwner + this.ObjectQualifier + ModuleQualifier;
            }
        }

        public string ObjectQualifier
        {
            get;
            private set;
        }

        public string ProviderPath
        {
            get;
            private set;
        }

        public override DataTable GetLinks(string searchValue, int portalId, int lowerTab, int upperTab)
        {
            var sql = new StringBuilder(128);

            sql.Append("SELECT ht.ModuleId, ht.ItemId, tm.TabId, ht.Content, ht.StateID, ht.IsPublished, m.ModuleTitle, t.TabName ");
            sql.Append("FROM ");
            sql.Append(this.NamePrefix);
            sql.Append("htmltext ht ");
            sql.Append(" JOIN ");
            sql.Append(this.NamePrefix);
            sql.Append("tabmodules tm ON (tm.moduleid = ht.moduleid)");
            sql.Append(" JOIN ");
            sql.Append(this.NamePrefix);
            sql.Append("modules m ON (m.moduleid = tm.moduleid)");

            sql.Append(" JOIN ");
            sql.Append(this.NamePrefix);
            sql.Append("tabs t ON (t.tabid = tm.tabid)");

            sql.Append("WHERE ");
            sql.Append("ht.Content COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '%");
            sql.Append(searchValue);
            sql.Append("%' AND m.PortalId = @portalId");
            if (lowerTab > 0 && upperTab > lowerTab)
            {
                sql.Append(" AND t.tabid >= ");
                sql.Append(lowerTab);
                sql.Append(" AND t.tabid <= ");
                sql.Append(upperTab);
            }

            DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql.ToString(), new SqlParameter("@portalId", portalId));
            return ds.Tables[0];
        }

        public override DataTable GetLinks(string searchValue, int lowerTab, int upperTab)
        {
            var sql = new StringBuilder(128);

            sql.Append("SELECT ht.ModuleId, ht.ItemId, tm.TabId, ht.Content, ht.StateID, ht.IsPublished, m.ModuleTitle, t.TabName ");
            sql.Append("FROM ");
            sql.Append(this.NamePrefix);
            sql.Append("htmltext ht ");
            sql.Append(" JOIN ");
            sql.Append(this.NamePrefix);
            sql.Append("tabmodules tm ON (tm.moduleid = ht.moduleid)");
            sql.Append(" JOIN ");
            sql.Append(this.ObjectQualifier);
            sql.Append("modules m ON (m.moduleid = tm.moduleid)");
            sql.Append(" JOIN ");
            sql.Append(this.NamePrefix);
            sql.Append("tabs t ON (t.tabid = tm.tabid)");
            sql.Append("WHERE ");
            sql.Append("ht.Content COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '%");
            sql.Append(searchValue);
            sql.Append("%' ");
            if (lowerTab > 0 && upperTab > lowerTab)
            {
                sql.Append(" AND t.tabid >= ");
                sql.Append(lowerTab);
                sql.Append(" AND t.tabid <= ");
                sql.Append(upperTab);
            }

            DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql.ToString());
            return ds.Tables[0];
        }

        public override DataTable GetPublishLinks(string searchValue, int portalId)
        {
            var sql = new StringBuilder(128);

            sql.Append("SELECT va.name, va.ItemId, va.articletext, va.displaytabid, t.TabName ");
            sql.Append("FROM ");
            sql.Append(this.ObjectQualifier);
            sql.Append("publish_vwarticles va ");
            sql.Append(" JOIN ");
            sql.Append(this.ObjectQualifier);
            sql.Append("tabs t ON t.tabid = va.displaytabid ");

            sql.Append(" WHERE articletext COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '%");
            sql.Append(searchValue);
            sql.Append("%' AND va.IsCurrentVersion=1 AND va.PortalId = @portalId");

            DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql.ToString(), new SqlParameter("@portalId", portalId));
            return ds.Tables[0];
        }

        public override void ReplaceTextHtml(int itemId, string content, int stateId, bool isPublished, int userId)
        {
            SqlHelper.ExecuteNonQuery(this.ConnectionString, this.DatabaseOwner + this.ObjectQualifier + "UpdateHtmlText", itemId, content, stateId, isPublished, userId);
        }
    }
}