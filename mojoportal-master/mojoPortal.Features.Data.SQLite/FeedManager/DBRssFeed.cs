﻿/// Author:					
/// Created:				2007-11-03
/// Last Modified:			2013-08-23
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)  
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.
/// 

using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web;
using Mono.Data.Sqlite;

namespace mojoPortal.Data
{
    public static class DBRssFeed
    {
       
        
        private static string GetConnectionString()
        {
            string connectionString = ConfigurationManager.AppSettings["SqliteConnectionString"];
            if (connectionString == "defaultdblocation")
            {
                connectionString = "version=3,URI=file:"
                    + System.Web.Hosting.HostingEnvironment.MapPath("~/Data/sqlitedb/mojo.db.config");

            }
            return connectionString;
        }



        public static int AddRssFeed(
            Guid itemGuid,
            Guid moduleGuid,
            Guid userGuid,
            int moduleId,
            int userId,
            string author,
            string url,
            string rssUrl,
            DateTime createdUtc,
            string imageUrl,
            string feedType,
            bool publishByDefault,
            int sortRank)
        {
            #region Bit Conversion

            int intPublishByDefault;
            if (publishByDefault)
            {
                intPublishByDefault = 1;
            }
            else
            {
                intPublishByDefault = 0;
            }


            #endregion

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("INSERT INTO mp_RssFeeds (");
            sqlCommand.Append("ModuleID, ");
            sqlCommand.Append("CreatedDate, ");
            sqlCommand.Append("UserID, ");
            sqlCommand.Append("Author, ");
            sqlCommand.Append("Url, ");
            sqlCommand.Append("RssUrl, ");
            sqlCommand.Append("ItemGuid, ");
            sqlCommand.Append("ModuleGuid, ");
            sqlCommand.Append("UserGuid, ");
            sqlCommand.Append("LastModUserGuid, ");
            sqlCommand.Append("LastModUtc, ");
            sqlCommand.Append("ImageUrl, ");
            sqlCommand.Append("FeedType, ");
            sqlCommand.Append("SortRank, ");
            sqlCommand.Append("PublishByDefault )"); 

            sqlCommand.Append(" VALUES (");
            sqlCommand.Append(":ModuleID, ");
            sqlCommand.Append(":CreatedDate, ");
            sqlCommand.Append(":UserID, ");
            sqlCommand.Append(":Author, ");
            sqlCommand.Append(":Url, ");
            sqlCommand.Append(":RssUrl, ");
            sqlCommand.Append(":ItemGuid, ");
            sqlCommand.Append(":ModuleGuid, ");
            sqlCommand.Append(":UserGuid, ");
            sqlCommand.Append(":UserGuid, ");
            sqlCommand.Append(":CreatedDate, ");
            sqlCommand.Append(":ImageUrl, ");
            sqlCommand.Append(":FeedType, ");
            sqlCommand.Append(":SortRank, ");
            sqlCommand.Append(":PublishByDefault )"); 
            sqlCommand.Append(";");

            sqlCommand.Append("SELECT LAST_INSERT_ROWID();");

            SqliteParameter[] arParams = new SqliteParameter[13];

            arParams[0] = new SqliteParameter(":ModuleID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleId;

            arParams[1] = new SqliteParameter(":UserID", DbType.Int32);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = userId;

            arParams[2] = new SqliteParameter(":Author", DbType.String, 100);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = author;

            arParams[3] = new SqliteParameter(":Url", DbType.String, 255);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = url;

            arParams[4] = new SqliteParameter(":RssUrl", DbType.String, 255);
            arParams[4].Direction = ParameterDirection.Input;
            arParams[4].Value = rssUrl;

            arParams[5] = new SqliteParameter(":CreatedDate", DbType.DateTime);
            arParams[5].Direction = ParameterDirection.Input;
            arParams[5].Value = createdUtc;

            arParams[6] = new SqliteParameter(":ItemGuid", DbType.String, 36);
            arParams[6].Direction = ParameterDirection.Input;
            arParams[6].Value = itemGuid.ToString();

            arParams[7] = new SqliteParameter(":ModuleGuid", DbType.String, 36);
            arParams[7].Direction = ParameterDirection.Input;
            arParams[7].Value = moduleGuid.ToString();

            arParams[8] = new SqliteParameter(":UserGuid", DbType.String, 36);
            arParams[8].Direction = ParameterDirection.Input;
            arParams[8].Value = userGuid.ToString();

            arParams[9] = new SqliteParameter(":ImageUrl", DbType.String, 255);
            arParams[9].Direction = ParameterDirection.Input;
            arParams[9].Value = imageUrl;

            arParams[10] = new SqliteParameter(":FeedType", DbType.String, 20);
            arParams[10].Direction = ParameterDirection.Input;
            arParams[10].Value = feedType;

            arParams[11] = new SqliteParameter(":PublishByDefault", DbType.Int32);
            arParams[11].Direction = ParameterDirection.Input;
            arParams[11].Value = intPublishByDefault;

            arParams[12] = new SqliteParameter(":SortRank", DbType.Int32);
            arParams[12].Direction = ParameterDirection.Input;
            arParams[12].Value = sortRank;

            int newID = 0;

            newID = Convert.ToInt32(SqliteHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams).ToString());

            return newID;

        }


        public static bool UpdateRssFeed(
            int itemId,
            int moduleId,
            string author,
            string url,
            string rssUrl,
            Guid lastModUserGuid,
            DateTime lastModUtc,
            string imageUrl,
            string feedType,
            bool publishByDefault,
            int sortRank)
        {
            #region Bit Conversion

            int intPublishByDefault;
            if (publishByDefault)
            {
                intPublishByDefault = 1;
            }
            else
            {
                intPublishByDefault = 0;
            }


            #endregion

            StringBuilder sqlCommand = new StringBuilder();

            sqlCommand.Append("UPDATE mp_RssFeeds ");
            sqlCommand.Append("SET  ");
            sqlCommand.Append("ModuleID = :ModuleID, ");
            sqlCommand.Append("Author = :Author, ");
            sqlCommand.Append("Url = :Url, ");
            sqlCommand.Append("RssUrl = :RssUrl,");
            sqlCommand.Append("LastModUserGuid = :LastModUserGuid, ");
            sqlCommand.Append("LastModUtc = :LastModUtc, ");
            sqlCommand.Append("ImageUrl = :ImageUrl, ");
            sqlCommand.Append("FeedType = :FeedType, ");
            sqlCommand.Append("SortRank = :SortRank, ");
            sqlCommand.Append("PublishByDefault = :PublishByDefault "); 

            sqlCommand.Append("WHERE  ");
            sqlCommand.Append("ItemID = :ItemID ;");

            SqliteParameter[] arParams = new SqliteParameter[11];

            arParams[0] = new SqliteParameter(":ItemID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = itemId;

            arParams[1] = new SqliteParameter(":ModuleID", DbType.Int32);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = moduleId;

            arParams[2] = new SqliteParameter(":Author", DbType.String, 100);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = author;

            arParams[3] = new SqliteParameter(":Url", DbType.String, 255);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = url;

            arParams[4] = new SqliteParameter(":RssUrl", DbType.String, 255);
            arParams[4].Direction = ParameterDirection.Input;
            arParams[4].Value = rssUrl;

            arParams[5] = new SqliteParameter(":LastModUserGuid", DbType.String, 36);
            arParams[5].Direction = ParameterDirection.Input;
            arParams[5].Value = lastModUserGuid.ToString();

            arParams[6] = new SqliteParameter(":LastModUtc", DbType.DateTime);
            arParams[6].Direction = ParameterDirection.Input;
            arParams[6].Value = lastModUtc;

            arParams[7] = new SqliteParameter(":ImageUrl", DbType.String, 255);
            arParams[7].Direction = ParameterDirection.Input;
            arParams[7].Value = imageUrl;

            arParams[8] = new SqliteParameter(":FeedType", DbType.String, 20);
            arParams[8].Direction = ParameterDirection.Input;
            arParams[8].Value = feedType;

            arParams[9] = new SqliteParameter(":PublishByDefault", DbType.Int32);
            arParams[9].Direction = ParameterDirection.Input;
            arParams[9].Value = intPublishByDefault;

            arParams[10] = new SqliteParameter(":SortRank", DbType.Int32);
            arParams[10].Direction = ParameterDirection.Input;
            arParams[10].Value = sortRank;

            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

            return (rowsAffected > -1);

        }


        public static bool DeleteRssFeed(int itemId)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_RssFeeds ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ItemID = :ItemID ;");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":ItemID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = itemId;

            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

            return (rowsAffected > 0);

        }

        public static bool DeleteByModule(int moduleId)
        {
            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":ModuleID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleId;

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_RssFeedEntries ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("FeedID IN (SELECT ItemID FROM mp_RssFeeds WHERE ModuleID  = :ModuleID);");

            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

            sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_RssFeeds ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ModuleID  = :ModuleID ;");

            rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

            return (rowsAffected > 0);
        }

        public static bool DeleteBySite(int siteId)
        {
            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":SiteID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = siteId;

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_RssFeedEntries ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("FeedID IN (SELECT ItemID FROM mp_RssFeeds WHERE ModuleID IN (SELECT ModuleID FROM mp_Modules WHERE SiteID = :SiteID));");

            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

            sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_RssFeeds ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ModuleID IN (SELECT ModuleID FROM mp_Modules WHERE SiteID = :SiteID) ;");

            rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

            return (rowsAffected > 0);
        }


        public static IDataReader GetRssFeed(int itemId)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  * ");
            sqlCommand.Append("FROM	mp_RssFeeds ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ItemID = :ItemID ;");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":ItemID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = itemId;

            return SqliteHelper.ExecuteReader(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

        }


        public static IDataReader GetFeeds(int moduleId)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  f.*, ");
            sqlCommand.Append(" (SELECT COUNT(*) FROM mp_RssFeedEntries e WHERE e.FeedId = f.ItemID) AS TotalEntries ");
            sqlCommand.Append("FROM	mp_RssFeeds f ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("f.ModuleID = :ModuleID ");
            sqlCommand.Append("ORDER BY f.SortRank, f.Author ;");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":ModuleID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleId;

            return SqliteHelper.ExecuteReader(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

        }

        /// <summary>
        /// Gets an IDataReader with one row from the mp_RssFeedEntries table.
        /// </summary>
        /// <param name="rowGuid"> rowGuid </param>
        public static DataTable GetEntries(Guid moduleGuid)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT ");
            sqlCommand.Append("f.Author As FeedName, ");
            sqlCommand.Append("e.* ");

            sqlCommand.Append("FROM	mp_RssFeedEntries e ");
            sqlCommand.Append("JOIN ");
            sqlCommand.Append("mp_RssFeeds f ");
            sqlCommand.Append("ON ");
            sqlCommand.Append("e.FeedID = f.ItemID ");

            sqlCommand.Append("WHERE ");
            sqlCommand.Append("e.ModuleGuid = :ModuleGuid ");
            sqlCommand.Append("ORDER BY e.PubDate DESC ");
            sqlCommand.Append(";");
          

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":ModuleGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleGuid.ToString();

            IDataReader reader = SqliteHelper.ExecuteReader(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("FeedId", typeof(int));
            dataTable.Columns.Add("FeedName", typeof(string));
            dataTable.Columns.Add("PubDate", typeof(DateTime));
            dataTable.Columns.Add("Author", typeof(string));
            dataTable.Columns.Add("Title", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("BlogUrl", typeof(string));
            dataTable.Columns.Add("Link", typeof(string));
            dataTable.Columns.Add("Confirmed", typeof(bool));
            dataTable.Columns.Add("EntryHash", typeof(int));

            using (reader)
            {
                while (reader.Read())
                {
                    DataRow row = dataTable.NewRow();
                    row["FeedId"] = reader["FeedId"];
                    row["FeedName"] = reader["FeedName"];
                    row["PubDate"] = Convert.ToDateTime(reader["PubDate"]);
                    row["Author"] = reader["Author"];
                    row["Title"] = reader["Title"];
                    row["Description"] = reader["Description"];
                    row["BlogUrl"] = reader["BlogUrl"];
                    row["Link"] = reader["Link"];
                    row["Confirmed"] = Convert.ToBoolean(reader["Confirmed"]);
                    row["EntryHash"] = reader["EntryHash"];

                    dataTable.Rows.Add(row);

                }
            }

            return dataTable;

        }

        /// <summary>
        /// Deletes rows from the mp_RssFeedEntries table. Returns true if row deleted.
        /// </summary>
        /// <param name="moduleGuid"> moduleGuid </param>
        /// <returns>bool</returns>
        public static bool DeleteExpiredEntriesByModule(Guid moduleGuid, DateTime expiredDate)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_RssFeedEntries ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ModuleGuid = :ModuleGuid ");
            sqlCommand.Append(" AND PubDate < :ExpiredDate ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[2];

            arParams[0] = new SqliteParameter(":ModuleGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleGuid.ToString();

            arParams[1] = new SqliteParameter(":ExpiredDate", DbType.DateTime);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = expiredDate;


            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(), 
                sqlCommand.ToString(), 
                arParams);

            return (rowsAffected > 0);

        }

        /// <summary>
        /// Deletes rows from the mp_RssFeedEntries table. Returns true if row deleted.
        /// </summary>
        /// <param name="moduleGuid"> moduleGuid </param>
        /// <returns>bool</returns>
        public static bool DeleteEntriesByModule(Guid moduleGuid)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_RssFeedEntries ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ModuleGuid = :ModuleGuid ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":ModuleGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleGuid.ToString();

            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

            return (rowsAffected > 0);

        }



        /// <summary>
        /// Deletes rows from the mp_RssFeedEntries table. Returns true if row deleted.
        /// </summary>
        /// <param name="moduleGuid"> moduleGuid </param>
        /// <returns>bool</returns>
        public static bool DeleteUnPublishedEntriesByModule(Guid moduleGuid)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_RssFeedEntries ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ModuleGuid = :ModuleGuid ");
            sqlCommand.Append(" AND Confirmed = 0 ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":ModuleGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleGuid.ToString();

            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

            return (rowsAffected > 0);

        }

        /// <summary>
        /// Deletes rows from the mp_RssFeedEntries table. Returns true if row deleted.
        /// </summary>
        /// <param name="moduleGuid"> moduleGuid </param>
        /// <returns>bool</returns>
        public static bool DeleteUnPublishedEntriesByFeed(int feedId)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_RssFeedEntries ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("FeedId = :FeedId ");
            sqlCommand.Append(" AND Confirmed = 0 ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":FeedId", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = feedId;

            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

            return (rowsAffected > 0);

        }

        /// <summary>
        /// Deletes rows from the mp_RssFeedEntries table. Returns true if row deleted.
        /// </summary>
        /// <param name="feedId"> feedId </param>
        /// <returns>bool</returns>
        public static bool DeleteEntriesByFeed(int feedId)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_RssFeedEntries ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("FeedId = :FeedId ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":FeedId", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = feedId;

            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

            return (rowsAffected > 0);
        }

        /// <summary>
        /// Gets a count of rows in the mp_RssFeedEntries table.
        /// </summary>
        public static bool EntryExists(Guid moduleGuid, int entryHash)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  Count(*) ");
            sqlCommand.Append("FROM	mp_RssFeedEntries ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ModuleGuid = :ModuleGuid ");
            sqlCommand.Append(" AND EntryHash = :EntryHash ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[2];

            arParams[0] = new SqliteParameter(":ModuleGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleGuid.ToString();

            arParams[1] = new SqliteParameter(":EntryHash", DbType.Int32);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = entryHash;

            int count = Convert.ToInt32(SqliteHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            return (count > 0);
        }

        /// <summary>
        /// Gets the most recent cache time for the module
        /// </summary>
        public static DateTime GetLastCacheTime(Guid moduleGuid)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  CachedTimeUtc ");
            sqlCommand.Append("FROM	mp_RssFeedEntries ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ModuleGuid = :ModuleGuid ");
            sqlCommand.Append("ORDER BY CachedTimeUtc DESC ");
            sqlCommand.Append("LIMIT 1 ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":ModuleGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleGuid.ToString();

            DateTime result = DateTime.UtcNow.AddDays(-1);

            using (IDataReader reader = SqliteHelper.ExecuteReader(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams))
            {
                if (reader.Read())
                {
                    result = Convert.ToDateTime(reader["CachedTimeUtc"]);
                }
            }

            return result;

        }

        /// <summary>
        /// Inserts a row in the mp_RssFeedEntries table. Returns rows affected count.
        /// </summary>
        /// <param name="rowGuid"> rowGuid </param>
        /// <param name="moduleGuid"> moduleGuid </param>
        /// <param name="feedGuid"> feedGuid </param>
        /// <param name="pubDate"> pubDate </param>
        /// <param name="title"> title </param>
        /// <param name="author"> author </param>
        /// <param name="blogUrl"> blogUrl </param>
        /// <param name="description"> description </param>
        /// <param name="link"> link </param>
        /// <param name="confirmed"> confirmed </param>
        /// <param name="entryHash"> entryHash </param>
        /// <param name="cachedTimeUtc"> cachedTimeUtc </param>
        /// <returns>int</returns>
        public static int CreateEntry(
            Guid rowGuid,
            Guid moduleGuid,
            Guid feedGuid,
            int feedId,
            DateTime pubDate,
            string title,
            string author,
            string blogUrl,
            string description,
            string link,
            bool confirmed,
            int entryHash,
            DateTime cachedTimeUtc)
        {
            #region Bit Conversion

            int intConfirmed;
            if (confirmed)
            {
                intConfirmed = 1;
            }
            else
            {
                intConfirmed = 0;
            }


            #endregion

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("INSERT INTO mp_RssFeedEntries (");
            sqlCommand.Append("RowGuid, ");
            sqlCommand.Append("ModuleGuid, ");
            sqlCommand.Append("FeedGuid, ");
            sqlCommand.Append("FeedId, ");
            sqlCommand.Append("PubDate, ");
            sqlCommand.Append("Title, ");
            sqlCommand.Append("Author, ");
            sqlCommand.Append("BlogUrl, ");
            sqlCommand.Append("Description, ");
            sqlCommand.Append("Link, ");
            sqlCommand.Append("Confirmed, ");
            sqlCommand.Append("EntryHash, ");
            sqlCommand.Append("CachedTimeUtc )");

            sqlCommand.Append(" VALUES (");
            sqlCommand.Append(":RowGuid, ");
            sqlCommand.Append(":ModuleGuid, ");
            sqlCommand.Append(":FeedGuid, ");
            sqlCommand.Append(":FeedId, ");
            sqlCommand.Append(":PubDate, ");
            sqlCommand.Append(":Title, ");
            sqlCommand.Append(":Author, ");
            sqlCommand.Append(":BlogUrl, ");
            sqlCommand.Append(":Description, ");
            sqlCommand.Append(":Link, ");
            sqlCommand.Append(":Confirmed, ");
            sqlCommand.Append(":EntryHash, ");
            sqlCommand.Append(":CachedTimeUtc )");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[13];

            arParams[0] = new SqliteParameter(":RowGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = rowGuid.ToString();

            arParams[1] = new SqliteParameter(":ModuleGuid", DbType.String, 36);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = moduleGuid.ToString();

            arParams[2] = new SqliteParameter(":FeedGuid", DbType.String, 36);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = feedGuid.ToString();

            arParams[3] = new SqliteParameter(":FeedId", DbType.Int32);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = feedId;

            arParams[4] = new SqliteParameter(":PubDate", DbType.DateTime);
            arParams[4].Direction = ParameterDirection.Input;
            arParams[4].Value = pubDate;

            arParams[5] = new SqliteParameter(":Title", DbType.String, 255);
            arParams[5].Direction = ParameterDirection.Input;
            arParams[5].Value = title;

            arParams[6] = new SqliteParameter(":Author", DbType.String, 100);
            arParams[6].Direction = ParameterDirection.Input;
            arParams[6].Value = author;

            arParams[7] = new SqliteParameter(":BlogUrl", DbType.String, 255);
            arParams[7].Direction = ParameterDirection.Input;
            arParams[7].Value = blogUrl;

            arParams[8] = new SqliteParameter(":Description", DbType.Object);
            arParams[8].Direction = ParameterDirection.Input;
            arParams[8].Value = description;

            arParams[9] = new SqliteParameter(":Link", DbType.String, 255);
            arParams[9].Direction = ParameterDirection.Input;
            arParams[9].Value = link;

            arParams[10] = new SqliteParameter(":Confirmed", DbType.Int32);
            arParams[10].Direction = ParameterDirection.Input;
            arParams[10].Value = intConfirmed;

            arParams[11] = new SqliteParameter(":EntryHash", DbType.Int32);
            arParams[11].Direction = ParameterDirection.Input;
            arParams[11].Value = entryHash;

            arParams[12] = new SqliteParameter(":CachedTimeUtc", DbType.DateTime);
            arParams[12].Direction = ParameterDirection.Input;
            arParams[12].Value = cachedTimeUtc;


            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(), 
                sqlCommand.ToString(), 
                arParams);

            return rowsAffected;

        }

        /// <summary>
        /// Updates a row in the mp_RssFeedEntries table. Returns true if row updated.
        /// </summary>
        /// <param name="moduleGuid"> moduleGuid </param>
        /// <param name="title"> title </param>
        /// <param name="author"> author </param>
        /// <param name="blogUrl"> blogUrl </param>
        /// <param name="description"> description </param>
        /// <param name="link"> link </param>
        /// <param name="entryHash"> entryHash </param>
        /// <param name="cachedTimeUtc"> cachedTimeUtc </param>
        /// <returns>bool</returns>
        public static bool UpdateEnry(
            Guid moduleGuid,
            string title,
            string author,
            string blogUrl,
            string description,
            string link,
            int entryHash,
            DateTime cachedTimeUtc)
        {

            StringBuilder sqlCommand = new StringBuilder();

            sqlCommand.Append("UPDATE mp_RssFeedEntries ");
            sqlCommand.Append("SET  ");
            sqlCommand.Append("Title = :Title, ");
            sqlCommand.Append("Author = :Author, ");
            sqlCommand.Append("BlogUrl = :BlogUrl, ");
            sqlCommand.Append("Description = :Description, ");
            sqlCommand.Append("Link = :Link, ");
            sqlCommand.Append("CachedTimeUtc = :CachedTimeUtc ");

            sqlCommand.Append("WHERE  ");
            sqlCommand.Append("ModuleGuid = :ModuleGuid ");
            sqlCommand.Append(" AND EntryHash = :EntryHash ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[8];

            arParams[0] = new SqliteParameter(":ModuleGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleGuid.ToString();

            arParams[1] = new SqliteParameter(":Title", DbType.String, 255);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = title;

            arParams[2] = new SqliteParameter(":Author", DbType.String, 100);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = author;

            arParams[3] = new SqliteParameter(":BlogUrl", DbType.String, 255);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = blogUrl;

            arParams[4] = new SqliteParameter(":Description", DbType.Object);
            arParams[4].Direction = ParameterDirection.Input;
            arParams[4].Value = description;

            arParams[5] = new SqliteParameter(":Link", DbType.String, 255);
            arParams[5].Direction = ParameterDirection.Input;
            arParams[5].Value = link;

            arParams[6] = new SqliteParameter(":EntryHash", DbType.Int32);
            arParams[6].Direction = ParameterDirection.Input;
            arParams[6].Value = entryHash;

            arParams[7] = new SqliteParameter(":CachedTimeUtc", DbType.DateTime);
            arParams[7].Direction = ParameterDirection.Input;
            arParams[7].Value = cachedTimeUtc;

            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(), 
                sqlCommand.ToString(), 
                arParams);

            return (rowsAffected > -1);

        }

        /// <summary>
        /// Updates a row in the mp_RssFeedEntries table. Returns true if row updated.
        /// </summary>
        /// <param name="moduleGuid"> moduleGuid </param>
        /// <param name="confirmed"> confirmed </param>
        /// <param name="entryHash"> entryHash </param>
        /// <returns>bool</returns>
        public static bool UpdatePublishing(
            Guid moduleGuid,
            bool confirmed,
            int entryHash)
        {
            #region Bit Conversion

            int intConfirmed;
            if (confirmed)
            {
                intConfirmed = 1;
            }
            else
            {
                intConfirmed = 0;
            }


            #endregion

            StringBuilder sqlCommand = new StringBuilder();

            sqlCommand.Append("UPDATE mp_RssFeedEntries ");
            sqlCommand.Append("SET  ");
            sqlCommand.Append("Confirmed = :Confirmed ");
           
            sqlCommand.Append("WHERE  ");
            sqlCommand.Append("ModuleGuid = :ModuleGuid ");
            sqlCommand.Append(" AND EntryHash = :EntryHash ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[3];

            arParams[0] = new SqliteParameter(":ModuleGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleGuid.ToString();

            arParams[1] = new SqliteParameter(":Confirmed", DbType.Int32);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = intConfirmed;

            arParams[2] = new SqliteParameter(":EntryHash", DbType.Int32);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = entryHash;

            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(), 
                sqlCommand.ToString(), 
                arParams);

            return (rowsAffected > -1);

        }


    }
}
