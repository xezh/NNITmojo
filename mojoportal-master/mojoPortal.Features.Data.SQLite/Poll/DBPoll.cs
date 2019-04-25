﻿/// Author:				
/// Created:			2008-08-29
/// Last Modified:		2008-11-12
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using System.Data;
using System.Configuration;
using Mono.Data.Sqlite;
using mojoPortal.Data;

namespace PollFeature.Data
{
    
    public static class DBPoll
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

        /// <summary>
        /// Inserts a row in the mp_Polls table. Returns rows affected count.
        /// </summary>
        /// <param name="pollGuid"> pollGuid </param>
        /// <param name="siteGuid"> siteGuid </param>
        /// <param name="question"> question </param>
        /// <param name="active"> active </param>
        /// <param name="anonymousVoting"> anonymousVoting </param>
        /// <param name="allowViewingResultsBeforeVoting"> allowViewingResultsBeforeVoting </param>
        /// <param name="showOrderNumbers"> showOrderNumbers </param>
        /// <param name="showResultsWhenDeactivated"> showResultsWhenDeactivated </param>
        /// <param name="activeFrom"> activeFrom </param>
        /// <param name="activeTo"> activeTo </param>
        /// <returns>int</returns>
        public static int Add(
            Guid pollGuid,
            Guid siteGuid,
            string question,
            bool active,
            bool anonymousVoting,
            bool allowViewingResultsBeforeVoting,
            bool showOrderNumbers,
            bool showResultsWhenDeactivated,
            DateTime activeFrom,
            DateTime activeTo)
        {
            #region Bit Conversion

            int intActive;
            if (active)
            {
                intActive = 1;
            }
            else
            {
                intActive = 0;
            }

            int intAnonymousVoting;
            if (anonymousVoting)
            {
                intAnonymousVoting = 1;
            }
            else
            {
                intAnonymousVoting = 0;
            }

            int intAllowViewingResultsBeforeVoting;
            if (allowViewingResultsBeforeVoting)
            {
                intAllowViewingResultsBeforeVoting = 1;
            }
            else
            {
                intAllowViewingResultsBeforeVoting = 0;
            }

            int intShowOrderNumbers;
            if (showOrderNumbers)
            {
                intShowOrderNumbers = 1;
            }
            else
            {
                intShowOrderNumbers = 0;
            }

            int intShowResultsWhenDeactivated;
            if (showResultsWhenDeactivated)
            {
                intShowResultsWhenDeactivated = 1;
            }
            else
            {
                intShowResultsWhenDeactivated = 0;
            }


            #endregion

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("INSERT INTO mp_Polls (");
            sqlCommand.Append("PollGuid, ");
            sqlCommand.Append("SiteGuid, ");
            sqlCommand.Append("Question, ");
            sqlCommand.Append("Active, ");
            sqlCommand.Append("AnonymousVoting, ");
            sqlCommand.Append("AllowViewingResultsBeforeVoting, ");
            sqlCommand.Append("ShowOrderNumbers, ");
            sqlCommand.Append("ShowResultsWhenDeactivated, ");
            sqlCommand.Append("ActiveFrom, ");
            sqlCommand.Append("ActiveTo )");

            sqlCommand.Append(" VALUES (");
            sqlCommand.Append(":PollGuid, ");
            sqlCommand.Append(":SiteGuid, ");
            sqlCommand.Append(":Question, ");
            sqlCommand.Append(":Active, ");
            sqlCommand.Append(":AnonymousVoting, ");
            sqlCommand.Append(":AllowViewingResultsBeforeVoting, ");
            sqlCommand.Append(":ShowOrderNumbers, ");
            sqlCommand.Append(":ShowResultsWhenDeactivated, ");
            sqlCommand.Append(":ActiveFrom, ");
            sqlCommand.Append(":ActiveTo )");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[10];

            arParams[0] = new SqliteParameter(":PollGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pollGuid.ToString();

            arParams[1] = new SqliteParameter(":SiteGuid", DbType.String, 36);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = siteGuid.ToString();

            arParams[2] = new SqliteParameter(":Question", DbType.String, 255);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = question;

            arParams[3] = new SqliteParameter(":Active", DbType.Int32);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = intActive;

            arParams[4] = new SqliteParameter(":AnonymousVoting", DbType.Int32);
            arParams[4].Direction = ParameterDirection.Input;
            arParams[4].Value = intAnonymousVoting;

            arParams[5] = new SqliteParameter(":AllowViewingResultsBeforeVoting", DbType.Int32);
            arParams[5].Direction = ParameterDirection.Input;
            arParams[5].Value = intAllowViewingResultsBeforeVoting;

            arParams[6] = new SqliteParameter(":ShowOrderNumbers", DbType.Int32);
            arParams[6].Direction = ParameterDirection.Input;
            arParams[6].Value = intShowOrderNumbers;

            arParams[7] = new SqliteParameter(":ShowResultsWhenDeactivated", DbType.Int32);
            arParams[7].Direction = ParameterDirection.Input;
            arParams[7].Value = intShowResultsWhenDeactivated;

            arParams[8] = new SqliteParameter(":ActiveFrom", DbType.DateTime);
            arParams[8].Direction = ParameterDirection.Input;
            arParams[8].Value = activeFrom;

            arParams[9] = new SqliteParameter(":ActiveTo", DbType.DateTime);
            arParams[9].Direction = ParameterDirection.Input;
            arParams[9].Value = activeTo;


            int rowsAffected = 0;
            rowsAffected = SqliteHelper.ExecuteNonQuery(GetConnectionString(), sqlCommand.ToString(), arParams);
            return rowsAffected;

        }

        /// <summary>
        /// Updates a row in the mp_Polls table. Returns true if row updated.
        /// </summary>
        /// <param name="pollGuid"> pollGuid </param>
        /// <param name="question"> question </param>
        /// <param name="active"> active </param>
        /// <param name="anonymousVoting"> anonymousVoting </param>
        /// <param name="allowViewingResultsBeforeVoting"> allowViewingResultsBeforeVoting </param>
        /// <param name="showOrderNumbers"> showOrderNumbers </param>
        /// <param name="showResultsWhenDeactivated"> showResultsWhenDeactivated </param>
        /// <param name="activeFrom"> activeFrom </param>
        /// <param name="activeTo"> activeTo </param>
        /// <returns>bool</returns>
        public static bool Update(
            Guid pollGuid,
            String question,
            bool anonymousVoting,
            bool allowViewingResultsBeforeVoting,
            bool showOrderNumbers,
            bool showResultsWhenDeactivated,
            bool active,
            DateTime activeFrom,
            DateTime activeTo)
        {
            #region Bit Conversion

            int intActive;
            if (active)
            {
                intActive = 1;
            }
            else
            {
                intActive = 0;
            }

            int intAnonymousVoting;
            if (anonymousVoting)
            {
                intAnonymousVoting = 1;
            }
            else
            {
                intAnonymousVoting = 0;
            }

            int intAllowViewingResultsBeforeVoting;
            if (allowViewingResultsBeforeVoting)
            {
                intAllowViewingResultsBeforeVoting = 1;
            }
            else
            {
                intAllowViewingResultsBeforeVoting = 0;
            }

            int intShowOrderNumbers;
            if (showOrderNumbers)
            {
                intShowOrderNumbers = 1;
            }
            else
            {
                intShowOrderNumbers = 0;
            }

            int intShowResultsWhenDeactivated;
            if (showResultsWhenDeactivated)
            {
                intShowResultsWhenDeactivated = 1;
            }
            else
            {
                intShowResultsWhenDeactivated = 0;
            }


            #endregion

            StringBuilder sqlCommand = new StringBuilder();

            sqlCommand.Append("UPDATE mp_Polls ");
            sqlCommand.Append("SET  ");
            sqlCommand.Append("Question = :Question, ");
            sqlCommand.Append("Active = :Active, ");
            sqlCommand.Append("AnonymousVoting = :AnonymousVoting, ");
            sqlCommand.Append("AllowViewingResultsBeforeVoting = :AllowViewingResultsBeforeVoting, ");
            sqlCommand.Append("ShowOrderNumbers = :ShowOrderNumbers, ");
            sqlCommand.Append("ShowResultsWhenDeactivated = :ShowResultsWhenDeactivated, ");
            sqlCommand.Append("ActiveFrom = :ActiveFrom, ");
            sqlCommand.Append("ActiveTo = :ActiveTo ");

            sqlCommand.Append("WHERE  ");
            sqlCommand.Append("PollGuid = :PollGuid ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[9];

            arParams[0] = new SqliteParameter(":PollGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pollGuid.ToString();

            arParams[1] = new SqliteParameter(":Question", DbType.String, 255);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = question;

            arParams[2] = new SqliteParameter(":Active", DbType.Int32);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = intActive;

            arParams[3] = new SqliteParameter(":AnonymousVoting", DbType.Int32);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = intAnonymousVoting;

            arParams[4] = new SqliteParameter(":AllowViewingResultsBeforeVoting", DbType.Int32);
            arParams[4].Direction = ParameterDirection.Input;
            arParams[4].Value = intAllowViewingResultsBeforeVoting;

            arParams[5] = new SqliteParameter(":ShowOrderNumbers", DbType.Int32);
            arParams[5].Direction = ParameterDirection.Input;
            arParams[5].Value = intShowOrderNumbers;

            arParams[6] = new SqliteParameter(":ShowResultsWhenDeactivated", DbType.Int32);
            arParams[6].Direction = ParameterDirection.Input;
            arParams[6].Value = intShowResultsWhenDeactivated;

            arParams[7] = new SqliteParameter(":ActiveFrom", DbType.DateTime);
            arParams[7].Direction = ParameterDirection.Input;
            arParams[7].Value = activeFrom;

            arParams[8] = new SqliteParameter(":ActiveTo", DbType.DateTime);
            arParams[8].Direction = ParameterDirection.Input;
            arParams[8].Value = activeTo;


            int rowsAffected = SqliteHelper.ExecuteNonQuery(GetConnectionString(), sqlCommand.ToString(), arParams);
            return (rowsAffected > -1);

        }

        /// <summary>
        /// Deletes a row from the mp_Polls table. Returns true if row deleted.
        /// </summary>
        /// <param name="pollGuid"> pollGuid </param>
        /// <returns>bool</returns>
        public static bool Delete(Guid pollGuid)
        {
            StringBuilder sqlCommand = new StringBuilder();

            sqlCommand.Append("DELETE FROM mp_PollModules ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("PollGuid = :PollGuid ");
            sqlCommand.Append(";");

            sqlCommand.Append("DELETE FROM mp_PollUsers ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("PollGuid = :PollGuid ");
            sqlCommand.Append(";");

            sqlCommand.Append("DELETE FROM mp_PollOptions ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("PollGuid = :PollGuid ");
            sqlCommand.Append(";");

            sqlCommand.Append("DELETE FROM mp_Polls ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("PollGuid = :PollGuid ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":PollGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pollGuid.ToString();


            int rowsAffected = SqliteHelper.ExecuteNonQuery(GetConnectionString(), sqlCommand.ToString(), arParams);
            return (rowsAffected > 0);
        }

        public static bool DeleteBySite(int siteId)
        {
            StringBuilder sqlCommand = new StringBuilder();

            sqlCommand.Append("DELETE FROM mp_PollModules ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ModuleID IN (SELECT ModuleID FROM mp_Modules WHERE SiteID = :SiteID) ");
            sqlCommand.Append(";");

            sqlCommand.Append("DELETE FROM mp_PollUsers ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("PollGuid IN (SELECT PollGuid FROM mp_Polls WHERE SiteGuid IN (SELECT SiteGuid FROM mp_Sites WHERE SiteID = :SiteID)) ");
            sqlCommand.Append(";");

            sqlCommand.Append("DELETE FROM mp_PollOptions ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("PollGuid IN (SELECT PollGuid FROM mp_Polls WHERE SiteGuid IN (SELECT SiteGuid FROM mp_Sites WHERE SiteID = :SiteID)) ");
            sqlCommand.Append(";");

            sqlCommand.Append("DELETE FROM mp_Polls ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("PollGuid IN (SELECT PollGuid FROM mp_Polls WHERE SiteGuid IN (SELECT SiteGuid FROM mp_Sites WHERE SiteID = :SiteID)) ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":SiteID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = siteId;


            int rowsAffected = SqliteHelper.ExecuteNonQuery(
                GetConnectionString(), 
                sqlCommand.ToString(), 
                arParams);

            return (rowsAffected > 0);

        }

        /// <summary>
        /// Clears the vote count a row from the mp_Polls table. 
        /// </summary>
        /// <param name="pollGuid"> pollGuid </param>
        /// <returns>bool</returns>
        public static bool ClearVotes(Guid pollGuid)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_PollUsers ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("PollGuid = :PollGuid ");
            sqlCommand.Append("; ");

            sqlCommand.Append("UPDATE mp_PollUsers ");
            sqlCommand.Append("SET Votes = 0 ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("PollGuid = :PollGuid ");
            sqlCommand.Append("; ");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":PollGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pollGuid.ToString();


            int rowsAffected = SqliteHelper.ExecuteNonQuery(GetConnectionString(), sqlCommand.ToString(), arParams);
            return (rowsAffected > 0);

        }

        /// <summary>
        /// Gets an IDataReader with one row from the mp_Polls table.
        /// </summary>
        /// <param name="pollGuid"> pollGuid </param>
        public static IDataReader GetPoll(Guid pollGuid)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  p.*, ");
            sqlCommand.Append("(SELECT SUM(Votes) FROM mp_PollOptions WHERE mp_PollOptions.PollGuid = :PollGuid) As TotalVotes ");

            sqlCommand.Append("FROM	mp_Polls p ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("p.PollGuid = :PollGuid ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":PollGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pollGuid.ToString();

            return SqliteHelper.ExecuteReader(
                GetConnectionString(), 
                sqlCommand.ToString(), 
                arParams);
            

        }

        /// <summary>
        /// Gets an IDataReader with one row from the mp_Polls table.
        /// </summary>
        /// <param name="pollGuid"> pollGuid </param>
        public static IDataReader GetPollByModuleID(int moduleId)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  p.*, ");
            sqlCommand.Append("(SELECT SUM(po.Votes) FROM mp_PollOptions po WHERE po.PollGuid = p.PollGuid) As TotalVotes ");

            sqlCommand.Append("FROM	mp_Polls p ");

            sqlCommand.Append("JOIN mp_PollModules pm ");
            sqlCommand.Append("ON p.PollGuid = pm.PollGuid ");

            sqlCommand.Append("WHERE ");
            sqlCommand.Append("pm.ModuleID = :ModuleID ");
            sqlCommand.Append(";");

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
        /// Gets an IDataReader with all rows from the mp_Polls table matching the siteGuid.
        /// </summary>
        /// <param name="siteGuid"> siteGuid </param>
        public static IDataReader GetPolls(Guid siteGuid)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  * ");
            sqlCommand.Append("FROM	mp_Polls ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("SiteGuid = :SiteGuid ");
            sqlCommand.Append("ORDER BY ActiveFrom DESC, Question ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":SiteGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = siteGuid.ToString();


            return SqliteHelper.ExecuteReader(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

        }

        /// <summary>
        /// Gets an IDataReader with all rows from the mp_Polls table matching the siteGuid.
        /// </summary>
        /// <param name="siteGuid"> siteGuid </param>
        public static IDataReader GetActivePolls(Guid siteGuid)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  * ");
            sqlCommand.Append("FROM	mp_Polls ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("SiteGuid = :SiteGuid ");
            //sqlCommand.Append("AND Active = 1 ");
            sqlCommand.Append("AND ActiveFrom <= :CurrentTime ");
            sqlCommand.Append("AND ActiveTo >= :CurrentTime ");
            sqlCommand.Append("ORDER BY Question ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[2];

            arParams[0] = new SqliteParameter(":SiteGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = siteGuid.ToString();

            arParams[1] = new SqliteParameter(":CurrentTime", DbType.DateTime);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = DateTime.UtcNow;


            return SqliteHelper.ExecuteReader(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);
        }

        /// <summary>
        /// Gets an IDataReader with all rows from the mp_Polls table matching the siteGuid.
        /// </summary>
        /// <param name="siteGuid"> siteGuid </param>
        public static IDataReader GetPollsByUserGuid(Guid userGuid)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  p.*, ");
            sqlCommand.Append("po.OptionGuid, ");
            sqlCommand.Append("po.Answer ");

            sqlCommand.Append("FROM	mp_Polls p ");

            sqlCommand.Append("JOIN mp_PollUsers pu ");
            sqlCommand.Append("ON p.PollGuid = pu.PollGuid ");

            sqlCommand.Append("JOIN mp_PollOptions po ");
            sqlCommand.Append("ON pu.OptionGuid = po.OptionGuid ");

            sqlCommand.Append("WHERE ");
            sqlCommand.Append("pu.UserGuid = :UserGuid ");

            sqlCommand.Append("ORDER BY ActiveFrom DESC, Question ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":UserGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = userGuid.ToString();

           
            return SqliteHelper.ExecuteReader(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

        }

        /// <summary>
        /// Gets a count of rows in the mp_Polls table.
        /// </summary>
        public static bool UserHasVoted(Guid pollGuid, Guid userGuid)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  Count(*) ");
            sqlCommand.Append("FROM	mp_PollUsers ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("UserGuid = :UserGuid ");
            sqlCommand.Append("AND PollGuid = :PollGuid ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[2];

            arParams[0] = new SqliteParameter(":UserGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = userGuid.ToString();

            arParams[1] = new SqliteParameter(":PollGuid", DbType.String, 36);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = pollGuid.ToString();

            int count = Convert.ToInt32(SqliteHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            return (count > 0);

        }

        /// <summary>
        /// to comment
        /// </summary>
        /// <param name="pollGuid"> pollGuid </param>
        /// <returns>bool</returns>
        public static bool AddToModule(Guid pollGuid, int moduleId)
        {
            StringBuilder sqlCommand = new StringBuilder();

            sqlCommand.Append("DELETE FROM mp_PollModules ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ModuleID = :ModuleID ");
            sqlCommand.Append(";");

            sqlCommand.Append("INSERT INTO mp_PollModules (PollGuid, ModuleID) ");
            sqlCommand.Append("VALUES( ");
            sqlCommand.Append(":PollGuid, :ModuleID ");
            sqlCommand.Append(");");

            SqliteParameter[] arParams = new SqliteParameter[2];

            arParams[0] = new SqliteParameter(":PollGuid", DbType.String, 36);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pollGuid.ToString();

            arParams[1] = new SqliteParameter(":ModuleID", DbType.Int32);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = moduleId;

            int rowsAffected = SqliteHelper.ExecuteNonQuery(GetConnectionString(), sqlCommand.ToString(), arParams);
            return (rowsAffected > 0);

        }

        /// <summary>
        /// to comment
        /// </summary>
        /// <param name="moduleID"> moduleID </param>
        /// <returns>bool</returns>
        public static bool RemoveFromModule(int moduleId)
        {
            StringBuilder sqlCommand = new StringBuilder();

            sqlCommand.Append("DELETE FROM mp_PollModules ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("ModuleID = :ModuleID ");
            sqlCommand.Append(";");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":ModuleID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleId;

            int rowsAffected = SqliteHelper.ExecuteNonQuery(GetConnectionString(), sqlCommand.ToString(), arParams);
            return (rowsAffected > 0);

        }


    }
}
