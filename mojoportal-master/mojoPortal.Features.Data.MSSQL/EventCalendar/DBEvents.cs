﻿/// Author:					
/// Created:				2007-11-03
/// Last Modified:			2010-07-01
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)  
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;

namespace mojoPortal.Data
{
    
    public static class DBEvents
    {
        
        /// <summary>
        /// Inserts a row in the mp_CalendarEvents table. Returns new integer id.
        /// </summary>
        /// <param name="itemGuid"> itemGuid </param>
        /// <param name="moduleGuid"> moduleGuid </param>
        /// <param name="moduleID"> moduleID </param>
        /// <param name="title"> title </param>
        /// <param name="description"> description </param>
        /// <param name="imageName"> imageName </param>
        /// <param name="eventDate"> eventDate </param>
        /// <param name="startTime"> startTime </param>
        /// <param name="endTime"> endTime </param>
        /// <param name="userID"> userID </param>
        /// <param name="userGuid"> userGuid </param>
        /// <param name="location"> location </param>
        /// <param name="requiresTicket"> requiresTicket </param>
        /// <param name="ticketPrice"> ticketPrice </param>
        /// <param name="createdDate"> createdDate </param>
        /// <returns>int</returns>
        public static int AddCalendarEvent(
            Guid itemGuid,
            Guid moduleGuid,
            int moduleId,
            string title,
            string description,
            string imageName,
            DateTime eventDate,
            DateTime startTime,
            DateTime endTime,
            int userId,
            Guid userGuid,
            string location,
            bool requiresTicket,
            decimal ticketPrice,
            DateTime createdDate)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "mp_CalendarEvents_Insert", 15);
            sph.DefineSqlParameter("@ItemGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, itemGuid);
            sph.DefineSqlParameter("@ModuleGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, moduleGuid);
            sph.DefineSqlParameter("@ModuleID", SqlDbType.Int, ParameterDirection.Input, moduleId);
            sph.DefineSqlParameter("@Title", SqlDbType.NVarChar, ParameterDirection.Input, title);
            sph.DefineSqlParameter("@Description", SqlDbType.NVarChar, -1, ParameterDirection.Input, description);
            sph.DefineSqlParameter("@ImageName", SqlDbType.NVarChar, ParameterDirection.Input, imageName);
            sph.DefineSqlParameter("@EventDate", SqlDbType.DateTime, ParameterDirection.Input, eventDate);
            sph.DefineSqlParameter("@StartTime", SqlDbType.SmallDateTime, ParameterDirection.Input, startTime);
            sph.DefineSqlParameter("@EndTime", SqlDbType.SmallDateTime, ParameterDirection.Input, endTime);
            sph.DefineSqlParameter("@UserID", SqlDbType.Int, ParameterDirection.Input, userId);
            sph.DefineSqlParameter("@UserGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, userGuid);
            sph.DefineSqlParameter("@Location", SqlDbType.NVarChar, -1, ParameterDirection.Input, location);

            sph.DefineSqlParameter("@RequiresTicket", SqlDbType.Bit, ParameterDirection.Input, requiresTicket);
            sph.DefineSqlParameter("@TicketPrice", SqlDbType.Decimal, ParameterDirection.Input, ticketPrice);
            sph.DefineSqlParameter("@CreatedDate", SqlDbType.DateTime, ParameterDirection.Input, createdDate);

            int newID = Convert.ToInt32(sph.ExecuteScalar());
            return newID;
        }


        /// <summary>
        /// Updates a row in the mp_CalendarEvents table. Returns true if row updated.
        /// </summary>
        /// <param name="itemID"> itemID </param>
        /// <param name="moduleID"> moduleID </param>
        /// <param name="title"> title </param>
        /// <param name="description"> description </param>
        /// <param name="imageName"> imageName </param>
        /// <param name="eventDate"> eventDate </param>
        /// <param name="startTime"> startTime </param>
        /// <param name="endTime"> endTime </param>
        /// <param name="location"> location </param>
        /// <param name="ticketPrice"> ticketPrice </param>
        /// <param name="requiresTicket"> requiresTicket </param>
        /// <param name="lastModUtc"> lastModUtc </param>
        /// <param name="lastModUserGuid"> lastModUserGuid </param>
        /// <returns>bool</returns>
        public static bool UpdateCalendarEvent(
            int itemId,
            int moduleId,
            string title,
            string description,
            string imageName,
            DateTime eventDate,
            DateTime startTime,
            DateTime endTime,
            string location,
            bool requiresTicket,
            decimal ticketPrice,
            DateTime lastModUtc,
            Guid lastModUserGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "mp_CalendarEvents_Update", 13);
            sph.DefineSqlParameter("@ItemID", SqlDbType.Int, ParameterDirection.Input, itemId);
            sph.DefineSqlParameter("@ModuleID", SqlDbType.Int, ParameterDirection.Input, moduleId);
            sph.DefineSqlParameter("@Title", SqlDbType.NVarChar, ParameterDirection.Input, title);
            sph.DefineSqlParameter("@Description", SqlDbType.NVarChar, -1, ParameterDirection.Input, description);
            sph.DefineSqlParameter("@ImageName", SqlDbType.NVarChar, ParameterDirection.Input, imageName);
            sph.DefineSqlParameter("@EventDate", SqlDbType.DateTime, ParameterDirection.Input, eventDate);
            sph.DefineSqlParameter("@StartTime", SqlDbType.SmallDateTime, ParameterDirection.Input, startTime);
            sph.DefineSqlParameter("@EndTime", SqlDbType.SmallDateTime, ParameterDirection.Input, endTime);
            sph.DefineSqlParameter("@Location", SqlDbType.NVarChar, -1, ParameterDirection.Input, location);
            sph.DefineSqlParameter("@RequiresTicket", SqlDbType.Bit, ParameterDirection.Input, requiresTicket);
            sph.DefineSqlParameter("@TicketPrice", SqlDbType.Decimal, ParameterDirection.Input, ticketPrice);
            sph.DefineSqlParameter("@LastModUtc", SqlDbType.DateTime, ParameterDirection.Input, lastModUtc);
            sph.DefineSqlParameter("@LastModUserGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, lastModUserGuid);

            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > -1);
        }

        public static bool DeleteCalendarEvent(int itemId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "mp_CalendarEvents_Delete", 1);
            sph.DefineSqlParameter("@ItemID", SqlDbType.Int, ParameterDirection.Input, itemId);
            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > -1);
        }

        public static bool DeleteByModule(int moduleId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "mp_CalendarEvents_DeleteByModule", 1);
            sph.DefineSqlParameter("@ModuleID", SqlDbType.Int, ParameterDirection.Input, moduleId);
            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > -1);
        }

        public static bool DeleteBySite(int siteId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "mp_CalendarEvents_DeleteBySite", 1);
            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteId);
            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > -1);
        }

        public static IDataReader GetCalendarEvent(int itemId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "mp_CalendarEvents_SelectOne", 1);
            sph.DefineSqlParameter("@ItemID", SqlDbType.Int, ParameterDirection.Input, itemId);
            return sph.ExecuteReader();
        }

        public static DataSet GetEvents(
                int moduleId,
                DateTime beginDate,
                DateTime endDate)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "mp_CalendarEvents_SelectByDate", 3);
            sph.DefineSqlParameter("@ModuleID", SqlDbType.Int, ParameterDirection.Input, moduleId);
            sph.DefineSqlParameter("@BeginDate", SqlDbType.DateTime, ParameterDirection.Input, beginDate);
            sph.DefineSqlParameter("@EndDate", SqlDbType.DateTime, ParameterDirection.Input, endDate);
            return sph.ExecuteDataset();
        }

        public static DataTable GetEventsTable(
                int moduleId,
                DateTime beginDate,
                DateTime endDate)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "mp_CalendarEvents_SelectByDate", 3);
            sph.DefineSqlParameter("@ModuleID", SqlDbType.Int, ParameterDirection.Input, moduleId);
            sph.DefineSqlParameter("@BeginDate", SqlDbType.DateTime, ParameterDirection.Input, beginDate);
            sph.DefineSqlParameter("@EndDate", SqlDbType.DateTime, ParameterDirection.Input, endDate);

            DataTable dt = new DataTable();

            dt.Columns.Add("ItemID", typeof(int));
            dt.Columns.Add("ModuleID", typeof(int));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("EventDate", typeof(DateTime));

            using (IDataReader reader = sph.ExecuteReader())
            {
                while (reader.Read())
                {
                    DataRow row = dt.NewRow();
                    row["ItemID"] = reader["ItemID"];
                    row["ModuleID"] = reader["ModuleID"];
                    row["Title"] = reader["Title"];
                    row["EventDate"] = reader["EventDate"];

                    dt.Rows.Add(row);

                }

            }
            return dt;
        }

        public static IDataReader GetEventsByPage(int siteId, int pageId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "mp_CalendarEvents_SelectByPage", 2);
            sph.DefineSqlParameter("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteId);
            sph.DefineSqlParameter("@PageID", SqlDbType.Int, ParameterDirection.Input, pageId);
            return sph.ExecuteReader();
        }




    }
}
