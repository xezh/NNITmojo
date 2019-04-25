﻿/// Author:					
/// Created:				2007-11-03
/// Last Modified:			2012-08-12
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)  
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.

using System;
using System.Data;
using System.Text;
using Npgsql;

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
            NpgsqlParameter[] arParams = new NpgsqlParameter[15];
            
            arParams[0] = new NpgsqlParameter("moduleid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleId;

            arParams[1] = new NpgsqlParameter("title", NpgsqlTypes.NpgsqlDbType.Text, 255);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = title;

            arParams[2] = new NpgsqlParameter("description", NpgsqlTypes.NpgsqlDbType.Text);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = description;

            arParams[3] = new NpgsqlParameter("imagename", NpgsqlTypes.NpgsqlDbType.Text, 100);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = imageName;

            arParams[4] = new NpgsqlParameter("eventdate", NpgsqlTypes.NpgsqlDbType.Date);
            arParams[4].Direction = ParameterDirection.Input;
            arParams[4].Value = eventDate;

            arParams[5] = new NpgsqlParameter("starttime", NpgsqlTypes.NpgsqlDbType.Timestamp);
            arParams[5].Direction = ParameterDirection.Input;
            arParams[5].Value = startTime;

            arParams[6] = new NpgsqlParameter("endtime", NpgsqlTypes.NpgsqlDbType.Timestamp);
            arParams[6].Direction = ParameterDirection.Input;
            arParams[6].Value = endTime;

            arParams[7] = new NpgsqlParameter("userid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[7].Direction = ParameterDirection.Input;
            arParams[7].Value = userId;

            arParams[8] = new NpgsqlParameter("itemguid", NpgsqlTypes.NpgsqlDbType.Char, 36);
            arParams[8].Direction = ParameterDirection.Input;
            arParams[8].Value = itemGuid.ToString();

            arParams[9] = new NpgsqlParameter("moduleguid", NpgsqlTypes.NpgsqlDbType.Char, 36);
            arParams[9].Direction = ParameterDirection.Input;
            arParams[9].Value = moduleGuid.ToString();

            arParams[10] = new NpgsqlParameter("userguid", NpgsqlTypes.NpgsqlDbType.Char, 36);
            arParams[10].Direction = ParameterDirection.Input;
            arParams[10].Value = userGuid.ToString();

            arParams[11] = new NpgsqlParameter("location", NpgsqlTypes.NpgsqlDbType.Text);
            arParams[11].Direction = ParameterDirection.Input;
            arParams[11].Value = location;

            arParams[12] = new NpgsqlParameter("createddate", NpgsqlTypes.NpgsqlDbType.Timestamp);
            arParams[12].Direction = ParameterDirection.Input;
            arParams[12].Value = createdDate;

            arParams[13] = new NpgsqlParameter("ticketprice", NpgsqlTypes.NpgsqlDbType.Numeric);
            arParams[13].Direction = ParameterDirection.Input;
            arParams[13].Value = ticketPrice;

            arParams[14] = new NpgsqlParameter("requiresticket", NpgsqlTypes.NpgsqlDbType.Boolean);
            arParams[14].Direction = ParameterDirection.Input;
            arParams[14].Value = requiresTicket;

            int newID = Convert.ToInt32(NpgsqlHelper.ExecuteScalar(
                ConnectionString.GetWriteConnectionString(),
                CommandType.StoredProcedure,
                "mp_calendarevents_insert(:moduleid,:title,:description,:imagename,:eventdate,:starttime,:endtime,:userid,:itemguid,:moduleguid,:userguid,:location,:createddate,:ticketprice,:requiresticket)",
                arParams));

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
            NpgsqlParameter[] arParams = new NpgsqlParameter[14];
            
            arParams[0] = new NpgsqlParameter("itemid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = itemId;

            arParams[1] = new NpgsqlParameter("moduleid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = moduleId;

            arParams[2] = new NpgsqlParameter("title", NpgsqlTypes.NpgsqlDbType.Text, 255);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = title;

            arParams[3] = new NpgsqlParameter("description", NpgsqlTypes.NpgsqlDbType.Text);
            arParams[3].Direction = ParameterDirection.Input;
            arParams[3].Value = description;

            arParams[4] = new NpgsqlParameter("imagename", NpgsqlTypes.NpgsqlDbType.Text, 100);
            arParams[4].Direction = ParameterDirection.Input;
            arParams[4].Value = imageName;

            arParams[5] = new NpgsqlParameter("eventdate", NpgsqlTypes.NpgsqlDbType.Date);
            arParams[5].Direction = ParameterDirection.Input;
            arParams[5].Value = eventDate;

            arParams[6] = new NpgsqlParameter("starttime", NpgsqlTypes.NpgsqlDbType.Timestamp);
            arParams[6].Direction = ParameterDirection.Input;
            arParams[6].Value = startTime;

            arParams[7] = new NpgsqlParameter("endtime", NpgsqlTypes.NpgsqlDbType.Timestamp);
            arParams[7].Direction = ParameterDirection.Input;
            arParams[7].Value = endTime;

            arParams[8] = new NpgsqlParameter("lastmoduserguid", NpgsqlTypes.NpgsqlDbType.Char, 36);
            arParams[8].Direction = ParameterDirection.Input;
            arParams[8].Value = lastModUserGuid.ToString();

            arParams[9] = new NpgsqlParameter("location", NpgsqlTypes.NpgsqlDbType.Text);
            arParams[9].Direction = ParameterDirection.Input;
            arParams[9].Value = location;

            arParams[10] = new NpgsqlParameter("lastmodutc", NpgsqlTypes.NpgsqlDbType.Timestamp);
            arParams[10].Direction = ParameterDirection.Input;
            arParams[10].Value = lastModUtc;

            arParams[11] = new NpgsqlParameter("ticketprice", NpgsqlTypes.NpgsqlDbType.Numeric);
            arParams[11].Direction = ParameterDirection.Input;
            arParams[11].Value = ticketPrice;

            arParams[13] = new NpgsqlParameter("requiresticket", NpgsqlTypes.NpgsqlDbType.Boolean);
            arParams[13].Direction = ParameterDirection.Input;
            arParams[13].Value = requiresTicket;

            int rowsAffected = Convert.ToInt32(NpgsqlHelper.ExecuteScalar(
                ConnectionString.GetWriteConnectionString(),
                CommandType.StoredProcedure,
                "mp_calendarevents_update(:itemid,:moduleid,:title,:description,:imagename,:eventdate,:starttime,:endtime,:lastmoduserguid,:location,:lastmodutc,:ticketprice,:requiresticket)",
                arParams));

            return (rowsAffected > -1);

        }

        public static bool DeleteCalendarEvent(int itemId)
        {
            NpgsqlParameter[] arParams = new NpgsqlParameter[1];
            
            arParams[0] = new NpgsqlParameter("itemid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = itemId;

            int rowsAffected = Convert.ToInt32(NpgsqlHelper.ExecuteScalar(
                ConnectionString.GetWriteConnectionString(),
                CommandType.StoredProcedure,
                "mp_calendarevents_delete(:itemid)",
                arParams));

            return (rowsAffected > -1);

        }

        public static bool DeleteByModule(int moduleId)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_calendarevents ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("moduleid  = :moduleid ");
            sqlCommand.Append(";");

            NpgsqlParameter[] arParams = new NpgsqlParameter[1];

            arParams[0] = new NpgsqlParameter("moduleid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleId;

            int rowsAffected = NpgsqlHelper.ExecuteNonQuery(ConnectionString.GetWriteConnectionString(),
                CommandType.Text,
                sqlCommand.ToString(),
                arParams);

            return (rowsAffected > -1);

        }

        public static bool DeleteBySite(int siteId)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("DELETE FROM mp_calendarevents ");
            sqlCommand.Append("WHERE ");
            sqlCommand.Append("moduleid IN (SELECT moduleid FROM mp_modules WHERE siteid = :siteid) ");
            sqlCommand.Append(";");

            NpgsqlParameter[] arParams = new NpgsqlParameter[1];

            arParams[0] = new NpgsqlParameter("siteid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = siteId;
            int rowsAffected = NpgsqlHelper.ExecuteNonQuery(ConnectionString.GetWriteConnectionString(),
                CommandType.Text,
                sqlCommand.ToString(),
                arParams);

            return (rowsAffected > -1);

        }

        public static IDataReader GetCalendarEvent(int itemId)
        {
            NpgsqlParameter[] arParams = new NpgsqlParameter[1];
            
            arParams[0] = new NpgsqlParameter("itemid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = itemId;

            return NpgsqlHelper.ExecuteReader(
                ConnectionString.GetReadConnectionString(),
                CommandType.StoredProcedure,
                "mp_calendarevents_select_one(:itemid)",
                arParams);
        }



        public static DataSet GetEvents(
            int moduleId,
            DateTime beginDate,
            DateTime endDate)
        {
            NpgsqlParameter[] arParams = new NpgsqlParameter[3];

            arParams[0] = new NpgsqlParameter("moduleid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleId;

            arParams[1] = new NpgsqlParameter("begindate", NpgsqlTypes.NpgsqlDbType.Date);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = beginDate;

            arParams[2] = new NpgsqlParameter("enddate", NpgsqlTypes.NpgsqlDbType.Date);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = endDate;
           
            return NpgsqlHelper.ExecuteDataset(
                ConnectionString.GetReadConnectionString(),
                CommandType.StoredProcedure,
                "mp_calendarevents_select_bydate(:moduleid,:begindate,:enddate)",
                arParams);
        }

        public static DataTable GetEventsTable(
            int moduleId,
            DateTime beginDate,
            DateTime endDate)
        {
            NpgsqlParameter[] arParams = new NpgsqlParameter[3];

            arParams[0] = new NpgsqlParameter("moduleid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = moduleId;

            arParams[1] = new NpgsqlParameter("begindate", NpgsqlTypes.NpgsqlDbType.Date);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = beginDate;

            arParams[2] = new NpgsqlParameter("enddate", NpgsqlTypes.NpgsqlDbType.Date);
            arParams[2].Direction = ParameterDirection.Input;
            arParams[2].Value = endDate;
            
            DataTable dt = new DataTable();

            dt.Columns.Add("ItemID", typeof(int));
            dt.Columns.Add("ModuleID", typeof(int));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("EventDate", typeof(DateTime));

            using (IDataReader reader = NpgsqlHelper.ExecuteReader(
                ConnectionString.GetReadConnectionString(),
                CommandType.StoredProcedure,
                "mp_calendarevents_select_bydate(:moduleid,:begindate,:enddate)",
                arParams))
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
            NpgsqlParameter[] arParams = new NpgsqlParameter[2];

            arParams[0] = new NpgsqlParameter("siteid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[0].Direction = ParameterDirection.Input;
            arParams[0].Value = siteId;

            arParams[1] = new NpgsqlParameter("pageid", NpgsqlTypes.NpgsqlDbType.Integer);
            arParams[1].Direction = ParameterDirection.Input;
            arParams[1].Value = pageId;

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append("SELECT  ce.*, ");

            sqlCommand.Append("m.moduletitle, ");
            sqlCommand.Append("m.viewroles , ");
            sqlCommand.Append("md.featurename  ");

            sqlCommand.Append("FROM	mp_calendarevents ce ");

            sqlCommand.Append("JOIN	mp_modules m ");
            sqlCommand.Append("ON ce.moduleid = m.moduleid ");

            sqlCommand.Append("JOIN	mp_moduledefinitions md ");
            sqlCommand.Append("ON m.moduledefid = md.moduledefid ");

            sqlCommand.Append("JOIN	mp_pagemodules pm ");
            sqlCommand.Append("ON m.moduleid = pm.moduleid ");

            sqlCommand.Append("JOIN	mp_pages p ");
            sqlCommand.Append("ON p.pageid = pm.pageid ");

            sqlCommand.Append("WHERE ");
            sqlCommand.Append("p.siteid = :siteid ");
            sqlCommand.Append("AND pm.pageid = :pageid ");
            sqlCommand.Append(" ; ");

            
            return NpgsqlHelper.ExecuteReader(
                ConnectionString.GetReadConnectionString(),
                CommandType.Text,
                sqlCommand.ToString(),
                arParams);

        }







    }
}
