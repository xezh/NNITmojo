﻿/// Author:					
/// Created:				2008-06-22
/// Last Modified:			2012-08-11
/// 
/// The use and distribution terms for this software are covered by the 
/// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)  
/// which can be found in the file CPL.TXT at the root of this distribution.
/// By using this software in any fashion, you are agreeing to be bound by 
/// the terms of this license.
///
/// You must not remove this notice, or any other, from this software.

using System;
using System.Configuration;
using System.Data;
using System.Text;
using Npgsql;

namespace mojoPortal.Data
{
	
	public static class DBCurrency
    {
		
		/// <summary>
		/// Inserts a row in the mp_Currency table. Returns rows affected count.
		/// </summary>
		/// <param name="guid"> guid </param>
		/// <param name="title"> title </param>
		/// <param name="code"> code </param>
		/// <param name="symbolLeft"> symbolLeft </param>
		/// <param name="symbolRight"> symbolRight </param>
		/// <param name="decimalPointChar"> decimalPointChar </param>
		/// <param name="thousandsPointChar"> thousandsPointChar </param>
		/// <param name="decimalPlaces"> decimalPlaces </param>
		/// <param name="value"> value </param>
		/// <param name="lastModified"> lastModified </param>
		/// <param name="created"> created </param>
		/// <returns>int</returns>
		public static int Create(
			Guid guid, 
			string title, 
			string code, 
			string symbolLeft, 
			string symbolRight, 
			string decimalPointChar, 
			string thousandsPointChar, 
			string decimalPlaces, 
			decimal value, 
			DateTime lastModified, 
			DateTime created) 
		{
			NpgsqlParameter[] arParams = new NpgsqlParameter[11];
		
			arParams[0] = new NpgsqlParameter("guid", NpgsqlTypes.NpgsqlDbType.Char, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = guid.ToString();
			
			arParams[1] = new NpgsqlParameter("title", NpgsqlTypes.NpgsqlDbType.Varchar,50);
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = title;
			
			arParams[2] = new NpgsqlParameter("code", NpgsqlTypes.NpgsqlDbType.Text,3);
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = code;
			
			arParams[3] = new NpgsqlParameter("symbolleft", NpgsqlTypes.NpgsqlDbType.Varchar,15);
			arParams[3].Direction = ParameterDirection.Input;
			arParams[3].Value = symbolLeft;
			
			arParams[4] = new NpgsqlParameter("symbolright", NpgsqlTypes.NpgsqlDbType.Varchar,15);
			arParams[4].Direction = ParameterDirection.Input;
			arParams[4].Value = symbolRight;
			
			arParams[5] = new NpgsqlParameter("decimalpointchar", NpgsqlTypes.NpgsqlDbType.Text,1);
			arParams[5].Direction = ParameterDirection.Input;
			arParams[5].Value = decimalPointChar;
			
			arParams[6] = new NpgsqlParameter("thousandspointchar", NpgsqlTypes.NpgsqlDbType.Text,1);
			arParams[6].Direction = ParameterDirection.Input;
			arParams[6].Value = thousandsPointChar;
			
			arParams[7] = new NpgsqlParameter("decimalplaces", NpgsqlTypes.NpgsqlDbType.Text,1);
			arParams[7].Direction = ParameterDirection.Input;
			arParams[7].Value = decimalPlaces;
			
			arParams[8] = new NpgsqlParameter("value", NpgsqlTypes.NpgsqlDbType.Numeric);
			arParams[8].Direction = ParameterDirection.Input;
			arParams[8].Value = value;
			
			arParams[9] = new NpgsqlParameter("lastmodified", NpgsqlTypes.NpgsqlDbType.Timestamp);
			arParams[9].Direction = ParameterDirection.Input;
			arParams[9].Value = lastModified;
			
			arParams[10] = new NpgsqlParameter("created", NpgsqlTypes.NpgsqlDbType.Timestamp);
			arParams[10].Direction = ParameterDirection.Input;
			arParams[10].Value = created;
			
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("INSERT INTO mp_currency (");
			sqlCommand.Append("guid, ");
			sqlCommand.Append("title, ");
			sqlCommand.Append("code, ");
			sqlCommand.Append("symbolleft, ");
			sqlCommand.Append("symbolright, ");
			sqlCommand.Append("decimalpointchar, ");
			sqlCommand.Append("thousandspointchar, ");
			sqlCommand.Append("decimalplaces, ");
			sqlCommand.Append("value, ");
			sqlCommand.Append("lastmodified, ");
			sqlCommand.Append("created )"); 
			
			sqlCommand.Append(" VALUES (");
			sqlCommand.Append(":guid, ");
			sqlCommand.Append(":title, ");
			sqlCommand.Append(":code, ");
			sqlCommand.Append(":symbolleft, ");
			sqlCommand.Append(":symbolright, ");
			sqlCommand.Append(":decimalpointchar, ");
			sqlCommand.Append(":thousandspointchar, ");
			sqlCommand.Append(":decimalplaces, ");
			sqlCommand.Append(":value, ");
			sqlCommand.Append(":lastmodified, ");
			sqlCommand.Append(":created "); 
			sqlCommand.Append(")");
			sqlCommand.Append(";");
			
			int rowsAffected = NpgsqlHelper.ExecuteNonQuery(ConnectionString.GetWriteConnectionString(),
				CommandType.Text, 
				sqlCommand.ToString(),
				arParams);
			
	
			return rowsAffected;
			
		}
	
	
		/// <summary>
		/// Updates a row in the mp_Currency table. Returns true if row updated.
		/// </summary>
		/// <param name="guid"> guid </param>
		/// <param name="title"> title </param>
		/// <param name="code"> code </param>
		/// <param name="symbolLeft"> symbolLeft </param>
		/// <param name="symbolRight"> symbolRight </param>
		/// <param name="decimalPointChar"> decimalPointChar </param>
		/// <param name="thousandsPointChar"> thousandsPointChar </param>
		/// <param name="decimalPlaces"> decimalPlaces </param>
		/// <param name="value"> value </param>
		/// <param name="lastModified"> lastModified </param>
		/// <param name="created"> created </param>
		/// <returns>bool</returns>
		public static bool Update(
			Guid  guid, 
			string title, 
			string code, 
			string symbolLeft, 
			string symbolRight, 
			string decimalPointChar, 
			string thousandsPointChar, 
			string decimalPlaces, 
			decimal value, 
			DateTime lastModified) 
		{
			NpgsqlParameter[] arParams = new NpgsqlParameter[10];
			
			arParams[0] = new NpgsqlParameter("guid", NpgsqlTypes.NpgsqlDbType.Char, 36); 
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = guid.ToString();
			
			arParams[1] = new NpgsqlParameter("title", NpgsqlTypes.NpgsqlDbType.Varchar,50); 
			arParams[1].Direction = ParameterDirection.Input;
			arParams[1].Value = title;
			
			arParams[2] = new NpgsqlParameter("code", NpgsqlTypes.NpgsqlDbType.Text,3); 
			arParams[2].Direction = ParameterDirection.Input;
			arParams[2].Value = code;
			
			arParams[3] = new NpgsqlParameter("symbolleft", NpgsqlTypes.NpgsqlDbType.Varchar,15); 
			arParams[3].Direction = ParameterDirection.Input;
			arParams[3].Value = symbolLeft;
			
			arParams[4] = new NpgsqlParameter("symbolright", NpgsqlTypes.NpgsqlDbType.Varchar,15); 
			arParams[4].Direction = ParameterDirection.Input;
			arParams[4].Value = symbolRight;
			
			arParams[5] = new NpgsqlParameter("decimalpointchar", NpgsqlTypes.NpgsqlDbType.Text,1); 
			arParams[5].Direction = ParameterDirection.Input;
			arParams[5].Value = decimalPointChar;
			
			arParams[6] = new NpgsqlParameter("thousandspointchar", NpgsqlTypes.NpgsqlDbType.Text,1); 
			arParams[6].Direction = ParameterDirection.Input;
			arParams[6].Value = thousandsPointChar;
			
			arParams[7] = new NpgsqlParameter("decimalplaces", NpgsqlTypes.NpgsqlDbType.Text,1); 
			arParams[7].Direction = ParameterDirection.Input;
			arParams[7].Value = decimalPlaces;
			
			arParams[8] = new NpgsqlParameter("value", NpgsqlTypes.NpgsqlDbType.Numeric); 
			arParams[8].Direction = ParameterDirection.Input;
			arParams[8].Value = value;
			
			arParams[9] = new NpgsqlParameter("lastmodified", NpgsqlTypes.NpgsqlDbType.Timestamp); 
			arParams[9].Direction = ParameterDirection.Input;
			arParams[9].Value = lastModified;
			
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("UPDATE mp_currency ");
			sqlCommand.Append("SET  ");
			sqlCommand.Append("title = :title, ");
			sqlCommand.Append("code = :code, ");
			sqlCommand.Append("symbolleft = :symbolleft, ");
			sqlCommand.Append("symbolright = :symbolright, ");
			sqlCommand.Append("decimalpointchar = :decimalpointchar, ");
			sqlCommand.Append("thousandspointchar = :thousandspointchar, ");
			sqlCommand.Append("decimalplaces = :decimalplaces, ");
			sqlCommand.Append("value = :value, ");
			sqlCommand.Append("lastmodified = :lastmodified ");
			
			sqlCommand.Append("WHERE  ");
			sqlCommand.Append("guid = :guid "); 
			sqlCommand.Append(";");

            int rowsAffected = NpgsqlHelper.ExecuteNonQuery(ConnectionString.GetWriteConnectionString(),
				CommandType.Text, 
				sqlCommand.ToString(), 
				arParams);
	
			return (rowsAffected > -1);
			
		}
		
		/// <summary>
		/// Deletes a row from the mp_Currency table. Returns true if row deleted.
		/// </summary>
		/// <param name="guid"> guid </param>
		/// <returns>bool</returns>
		public static bool Delete(Guid  guid) 
		{
			NpgsqlParameter[] arParams = new NpgsqlParameter[1];
			
			arParams[0] = new NpgsqlParameter("guid", NpgsqlTypes.NpgsqlDbType.Char, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = guid.ToString();
				
			
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("DELETE FROM mp_currency ");
			sqlCommand.Append("WHERE ");
			sqlCommand.Append("guid = :guid "); 
			sqlCommand.Append(";");
            int rowsAffected = NpgsqlHelper.ExecuteNonQuery(ConnectionString.GetWriteConnectionString(),
				CommandType.Text, 
				sqlCommand.ToString(), 
				arParams);
	
			return (rowsAffected > -1);
			
		}
		
		/// <summary>
		/// Gets an IDataReader with one row from the mp_Currency table.
		/// </summary>
		/// <param name="guid"> guid </param>
		public static IDataReader GetOne(Guid  guid)  
		{
			NpgsqlParameter[] arParams = new NpgsqlParameter[1];
			
			arParams[0] = new NpgsqlParameter("guid", NpgsqlTypes.NpgsqlDbType.Char, 36);
			arParams[0].Direction = ParameterDirection.Input;
			arParams[0].Value = guid.ToString();
			
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT  * ");
			sqlCommand.Append("FROM	mp_currency ");
			sqlCommand.Append("WHERE ");
			sqlCommand.Append("guid = :guid "); 
			sqlCommand.Append(";");
			
			return NpgsqlHelper.ExecuteReader(
                ConnectionString.GetReadConnectionString(),
				CommandType.Text, 
				sqlCommand.ToString(), 
				arParams);
		
				
		}
		
		/// <summary>
		/// Gets an IDataReader with all rows in the mp_Currency table.
		/// </summary>
		public static IDataReader GetAll()
		{
			
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT  * ");
			sqlCommand.Append("FROM	mp_currency ");
			sqlCommand.Append(";");
			
			return NpgsqlHelper.ExecuteReader(
                ConnectionString.GetReadConnectionString(),
				CommandType.Text, 
				sqlCommand.ToString(),
				null);
			
	
		}
		


	}
}
