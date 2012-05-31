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


namespace Engage.Dnn.F3
{

    using DotNetNuke.Common.Utilities;
    using System.Data;
    using System.Text;
    using Microsoft.ApplicationBlocks.Data;
    using DotNetNuke.Framework.Providers;

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// SQL Server implementation of the abstract DataProvider class
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public class SqlDataProvider : DataProvider
    {


        #region Private Members

        private const string ProviderType = "data";
        private const string ModuleQualifier = "";

        private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private string _connectionString;
        private string _providerPath;
        private string _objectQualifier;
        private string _databaseOwner;
        
        #endregion

        #region Constructors

        public SqlDataProvider()
        {

            // Read the configuration specific information for this provider
            var objProvider = (Provider)(_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]);

            // Read the attributes for this provider

            //Get Connection string from web.config
            _connectionString = Config.GetConnectionString();

            if (_connectionString == "")
            {
                // Use connection string specified in provider
                _connectionString = objProvider.Attributes["connectionString"];
            }

            _providerPath = objProvider.Attributes["providerPath"];

            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if (_objectQualifier != "" & _objectQualifier.EndsWith("_") == false)
            {
                _objectQualifier += "_";
            }

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if (_databaseOwner != "" & _databaseOwner.EndsWith(".") == false)
            {
                _databaseOwner += ".";
            }

        }

        #endregion

        #region Properties

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        public string ProviderPath
        {
            get
            {
                return _providerPath;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return _objectQualifier;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return _databaseOwner;
            }
        }

        public string NamePrefix
        {
            get { return _databaseOwner + _objectQualifier + ModuleQualifier; }
        }


        #endregion


        #region Public Methods

        public override DataTable GetLinks(string searchString, int portalId, int lowerTab, int upperTab)
        {
            var sql = new StringBuilder(128);

            //sql.Append("select ht.ModuleId, tm.TabId, ht.Content, m.ModuleTitle, t.TabName ");
            sql.Append("select ht.ModuleId, ht.ItemId, tm.TabId, ht.Content, ht.StateID, ht.IsPublished, m.ModuleTitle, t.TabName ");
            sql.Append("from ");
            sql.Append(NamePrefix);
            sql.Append("htmltext ht ");
            sql.Append(" join ");
            sql.Append(NamePrefix);
            sql.Append("tabmodules tm on (tm.moduleid = ht.moduleid)");
            sql.Append(" join ");
            sql.Append(NamePrefix);
            sql.Append("modules m on (m.moduleid = tm.moduleid)");

            sql.Append(" join ");
            sql.Append(NamePrefix);
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

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql.ToString());
            return ds.Tables[0];
        }

        public override DataTable GetLinks(string searchString, int lowerTab, int upperTab)
        {
            var sql = new StringBuilder(128);

            sql.Append("select ht.ModuleId, ht.ItemId, tm.TabId, ht.Content, ht.StateID, ht.IsPublished, m.ModuleTitle, t.TabName ");
            sql.Append("from ");
            sql.Append(NamePrefix);
            sql.Append("htmltext ht ");
            sql.Append(" join ");
            sql.Append(NamePrefix);
            sql.Append("tabmodules tm on (tm.moduleid = ht.moduleid)");
            sql.Append(" join ");
            sql.Append(ObjectQualifier);
            sql.Append("modules m on (m.moduleid = tm.moduleid)");
            sql.Append(" join ");
            sql.Append(NamePrefix);
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

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql.ToString());
            return ds.Tables[0];
        }

        public override void ReplaceTextHTML(int itemId, string content, int stateId, bool isPublished, int userId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateHtmlText", itemId, content, stateId, isPublished, userId);

        }


        //public abstract DataTable GetPublishLinks(string searchString, int portalId);
        public override DataTable GetPublishLinks(string searchString, int portalId)
        {
            var sql = new StringBuilder(128);

            sql.Append("select va.name, va.ItemId, va.articletext, va.displaytabid, t.TabName ");
            sql.Append("from ");
            sql.Append(ObjectQualifier);
            sql.Append("publish_vwarticles va ");
            sql.Append(" join ");
            sql.Append(ObjectQualifier);
            sql.Append("tabs t on t.tabid = va.displaytabid ");

            sql.Append(" where articletext collate SQL_Latin1_General_CP1_CS_AS like '%");
            sql.Append(searchString);
            sql.Append("%' and va.IsCurrentVersion=1 and va.PortalId = ");
            sql.Append(portalId.ToString());

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql.ToString());
            return ds.Tables[0];
        }
        #endregion

    }

}