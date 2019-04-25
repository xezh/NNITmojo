/// Author:					
/// Created:				2007-11-03
/// Last Modified:			2009-05-12
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
using System.Data.Common;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web;
using Mono.Data.Sqlite;

namespace mojoPortal.Data
{
   
    public static class DBBannedIP
    {
        
        public static String DBPlatform()
        {
            return "SQLite";
        }

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
        /// Inserts a row in the mp_BannedIPAddresses table. Returns new integer id.
        /// </summary>
        /// <param name="bannedIP"> bannedIP </param>
        /// <param name="bannedUTC"> bannedUTC </param>
        /// <param name="bannedReason"> bannedReason </param>
        /// <returns>int</returns>
        public static int Add(
            string bannedIP,
            DateTime bannedUtc,
            string bannedReason)
        {
            #region Bit Conversion


            #endregion

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("INSERT INTO mp_BannedIPAddresses (");
            sqlCommand.Append("BannedIP, ");
            sqlCommand.Append("BannedUTC, ");
            sqlCommand.Append("BannedReason )");

            sqlCommand.Append(" VALUES (");
            sqlCommand.Append(":BannedIP, ");
            sqlCommand.Append(":BannedUTC, ");
            sqlCommand.Append(":BannedReason );");

            sqlCommand.Append("SELECT LAST_INSERT_ROWID();");

            SqliteParameter[] arParams = new SqliteParameter[3];

            arParams[0] = new SqliteParameter(":BannedIP", DbType.String, 50);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = bannedIP;

            arParams[1] = new SqliteParameter(":BannedUTC", DbType.DateTime);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = bannedUtc;

            arParams[2] = new SqliteParameter(":BannedReason", DbType.String, 255);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = bannedReason;


            int newID = 0;
            newID = Convert.ToInt32(SqliteHelper.ExecuteScalar(GetConnectionString(), sqlCommand.ToString(), arParams).ToString());
            return newID;

        }


        /// <summary>
        /// Updates a row in the mp_BannedIPAddresses table. Returns true if row updated.
        /// </summary>
        /// <param name="rowID"> rowID </param>
        /// <param name="bannedIP"> bannedIP </param>
        /// <param name="bannedUTC"> bannedUTC </param>
        /// <param name="bannedReason"> bannedReason </param>
        /// <returns>bool</returns>
        public static bool Update(
            int rowId,
            string bannedIP,
            DateTime bannedUtc,
            string bannedReason)
        {
            #region Bit Conversion


            #endregion

            StringBuilder sqlCommand = new StringBuilder();

            sqlCommand.Append("UPDATE mp_BannedIPAddresses ");
            sqlCommand.Append("SET  ");
            sqlCommand.Append("BannedIP = :BannedIP, ");
            sqlCommand.Append("BannedUTC = :BannedUTC, ");
            sqlCommand.Append("BannedReason = :BannedReason ");

            sqlCommand.Append("WHERE  ");
            sqlCommand.Append("RowID = :RowID ;");

            SqliteParameter[] arParams = new SqliteParameter[4];

            arParams[0] = new SqliteParameter(":RowID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = rowId;

            arParams[1] = new SqliteParameter(":BannedIP", DbType.String, 50);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = bannedIP;

            arParams[2] = new SqliteParameter(":BannedUTC", DbType.DateTime);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = bannedUtc;

            arParams[3] = new SqliteParameter(":BannedReason", DbType.String, 255);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = bannedReason;


            int rowsAffected = SqliteHelper.ExecuteNonQuery(GetConnectionString(), sqlCommand.ToString(), arParams);
            return (rowsAffected > -1);

        }

        /// <summary>
        /// Deletes a row from the mp_BannedIPAddresses table. Returns true if row deleted.
        /// </summary>
        /// <param name="rowID"> rowID </param>
        /// <returns>bool</returns>
        public static bool Delete(
            int rowId)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_BannedIPAddresses ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("RowID = :RowID ;");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":RowID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = rowId;


            int rowsAffected = SqliteHelper.ExecuteNonQuery(GetConnectionString(), sqlCommand.ToString(), arParams);
            return (rowsAffected > 0);

        }

        /// <summary>
        /// Gets an IDataReader with one row from the mp_BannedIPAddresses table.
        /// </summary>
        /// <param name="rowID"> rowID </param>
        public static IDataReader GetOne(int rowId)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  * ");
            sqlCommand.Append("FROM	mp_BannedIPAddresses ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("RowID = :RowID ;");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":RowID", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = rowId;

            return SqliteHelper.ExecuteReader(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

        }

        /// <summary>
        /// Gets an IDataReader with rows from the mp_BannedIPAddresses table.
        /// </summary>
        /// <param name="ipAddress"> ipAddress </param>
        public static IDataReader GeByIpAddress(string ipAddress)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  * ");
            sqlCommand.Append("FROM	mp_BannedIPAddresses ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("BannedIP = :BannedIP ;");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":BannedIP", DbType.String, 50);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = ipAddress;

            return SqliteHelper.ExecuteReader(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);

        }

        /// <summary>
        /// Gets an IDataReader with all rows in the mp_BannedIPAddresses table.
        /// </summary>
        public static IDataReader GetAll()
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  * ");
            sqlCommand.Append("FROM	mp_BannedIPAddresses ;");

            return SqliteHelper.ExecuteReader(
                GetConnectionString(),
                sqlCommand.ToString(),
                null);
        }

        /// <summary>
        /// Returns true if the passed in address is banned
        /// </summary>
        /// <param name="rowID"> rowID </param>
        /// <returns>bool</returns>
        public static bool IsBanned(string ipAddress)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  Count(*) ");
            sqlCommand.Append("FROM	mp_BannedIPAddresses ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("BannedIP = :BannedIP ;");

            SqliteParameter[] arParams = new SqliteParameter[1];

            arParams[0] = new SqliteParameter(":BannedIP", DbType.String, 50);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = ipAddress;

            int foundRows = Convert.ToInt32(SqliteHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams));

            return (foundRows > 0);

        }

        /// <summary>
        /// Gets a count of rows in the mp_BannedIPAddresses table.
        /// </summary>
        public static int GetCount()
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  Count(*) ");
            sqlCommand.Append("FROM	mp_BannedIPAddresses ;");

            return Convert.ToInt32(SqliteHelper.ExecuteScalar(
                GetConnectionString(),
                sqlCommand.ToString(),
                null));
        }

        /// <summary>
        /// Gets a page of data from the mp_BannedIPAddresses table.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="totalPages">total pages</param>
        public static IDataReader GetPage(
            int pageNumber,
            int pageSize,
            out int totalPages)
        {
            int pageLowerBound = (pageSize * pageNumber) - pageSize;
            totalPages = 1;
            int totalRows = GetCount();

            if (pageSize > 0) totalPages = totalRows / pageSize;

            if (totalRows <= pageSize)
            {
                totalPages = 1;
            }
            else
            {
                int remainder;
                Math.DivRem(totalRows, pageSize, out remainder);
                if (remainder > 0)
                {
                    totalPages += 1;
                }
            }

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT	* ");
            sqlCommand.Append("FROM	mp_BannedIPAddresses  ");
            sqlCommand.Append("ORDER BY  BannedIP ");
            sqlCommand.Append("LIMIT " + pageLowerBound.ToString() + ", :PageSize  ; ");

            SqliteParameter[] arParams = new SqliteParameter[2];

            arParams[0] = new SqliteParameter(":PageNumber", DbType.Int32);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = pageNumber;

            arParams[1] = new SqliteParameter(":PageSize", DbType.Int32);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = pageSize;

            return SqliteHelper.ExecuteReader(
                GetConnectionString(),
                sqlCommand.ToString(),
                arParams);
        }

        

    }
}
