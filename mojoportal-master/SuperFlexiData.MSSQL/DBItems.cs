﻿// Author:					i7MEDIA
// Created:					2015-03-06
// Last Modified:			2019-04-03
// You must not remove this notice, or any other, from this software.

using mojoPortal.Data;
using System;
using System.Data;

namespace SuperFlexiData
{

	public static class DBItems
    {


        /// <summary>
        /// Inserts a row in the i7_sflexi_items table. Returns new integer id.
        /// </summary>
        /// <returns>int</returns>
        public static int Create(
            Guid siteGuid,
            Guid featureGuid,
            Guid moduleGuid,
            int moduleID,
            Guid definitionGuid,
            Guid itemGuid,
            int sortOrder,
            DateTime createdUtc,
            DateTime lastModUtc,
			string viewRoles,
			string editRoles)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "i7_sflexi_items_Insert", 11);
            sph.DefineSqlParameter("@ItemGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, itemGuid);
            sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
            sph.DefineSqlParameter("@FeatureGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, featureGuid);
            sph.DefineSqlParameter("@ModuleGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, moduleGuid);
            sph.DefineSqlParameter("@ModuleID", SqlDbType.Int, ParameterDirection.Input, moduleID);
            sph.DefineSqlParameter("@DefinitionGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, definitionGuid);            
            sph.DefineSqlParameter("@SortOrder", SqlDbType.Int, ParameterDirection.Input, sortOrder);
            sph.DefineSqlParameter("@CreatedUtc", SqlDbType.DateTime, ParameterDirection.Input, createdUtc);
            sph.DefineSqlParameter("@LastModUtc", SqlDbType.DateTime, ParameterDirection.Input, lastModUtc);
			sph.DefineSqlParameter("@ViewRoles", SqlDbType.NVarChar, -1, ParameterDirection.Input, viewRoles);
			sph.DefineSqlParameter("@EditRoles", SqlDbType.NVarChar, -1, ParameterDirection.Input, editRoles);
			int newID = Convert.ToInt32(sph.ExecuteScalar());
            return newID;
        }


        /// <summary>
        /// Updates a row in the i7_sflexi_items table. Returns true if row updated.
        /// </summary>
        /// <returns>bool</returns>
        public static bool Update(
            Guid siteGuid,
            Guid featureGuid,
            Guid moduleGuid,
            int moduleID,
            Guid definitionGuid,
            Guid itemGuid,
            int sortOrder,
            DateTime createdUtc,
            DateTime lastModUtc,
			string viewRoles,
			string editRoles)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "i7_sflexi_items_Update", 11);
            sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
            sph.DefineSqlParameter("@FeatureGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, featureGuid);
            sph.DefineSqlParameter("@ModuleGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, moduleGuid);
            sph.DefineSqlParameter("@ModuleID", SqlDbType.Int, ParameterDirection.Input, moduleID);
            sph.DefineSqlParameter("@DefinitionGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, definitionGuid);
            sph.DefineSqlParameter("@ItemGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, itemGuid);
            sph.DefineSqlParameter("@SortOrder", SqlDbType.Int, ParameterDirection.Input, sortOrder);
            sph.DefineSqlParameter("@CreatedUtc", SqlDbType.DateTime, ParameterDirection.Input, createdUtc);
            sph.DefineSqlParameter("@LastModUtc", SqlDbType.DateTime, ParameterDirection.Input, lastModUtc);
			sph.DefineSqlParameter("@ViewRoles", SqlDbType.NVarChar, -1, ParameterDirection.Input, viewRoles);
			sph.DefineSqlParameter("@EditRoles", SqlDbType.NVarChar, -1, ParameterDirection.Input, editRoles);

			int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > 0);

        }

        /// <summary>
        /// Deletes a row from the i7_sflexi_items table. Returns true if row deleted.
        /// </summary>
        /// <returns>bool</returns>
        public static bool Delete(int itemID)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "i7_sflexi_items_Delete", 1);
            sph.DefineSqlParameter("@ItemID", SqlDbType.Int, ParameterDirection.Input, itemID);
            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > 0);

        }

        /// <summary>
        /// Deletes rows from the i7_sflexi_items table. Returns true if rows deleted.
        /// </summary>
        /// <returns>bool</returns>
        public static bool DeleteBySite(Guid siteGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "i7_sflexi_items_DeleteBySite", 1);
            sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > 0);
        }

        /// <summary>
        /// Deletes rows from the i7_sflexi_items table. Returns true if rows deleted.
        /// </summary>
        /// <returns>bool</returns>
        public static bool DeleteByModule(Guid moduleGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "i7_sflexi_items_DeleteByModule", 1);
            sph.DefineSqlParameter("@ModuleGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, moduleGuid);
            int rowsAffected = sph.ExecuteNonQuery();
            return (rowsAffected > 0);
        }

        /// <summary>
        /// Deletes items and values used by definition guid. Returns true if rows deleted.
        /// </summary>
        /// <param name="definitionGuid"> guid </param>
        /// <returns>bool</returns>
        //public static bool DeleteByDefinition(Guid definitionGuid)
        //{
        //    SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetWriteConnectionString(), "i7_sflexi_items_DeleteByDefinition", 1);
        //    sph.DefineSqlParameter("@DefinitionGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, definitionGuid);
        //    int rowsAffected = sph.ExecuteNonQuery();
        //    return (rowsAffected > 0);
        //}

        /// <summary>
        /// Gets an IDataReader with one row from the i7_sflexi_items table.
        /// </summary>
        public static IDataReader GetOne(int itemID)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectOne", 1);
            sph.DefineSqlParameter("@ItemID", SqlDbType.Int, ParameterDirection.Input, itemID);
            return sph.ExecuteReader();

        }

        /// <summary>
        /// Gets an IDataReader with one row from the i7_sflexi_items table.
        /// </summary>
        public static IDataReader GetOne(
            Guid itemGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectOneByGuid", 1);
            sph.DefineSqlParameter("@ItemGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, itemGuid);
            return sph.ExecuteReader();

        }

        /// <summary>
        /// Gets a count of rows in the i7_sflexi_items table.
        /// </summary>
        public static int GetCount()
        {

            return Convert.ToInt32(SqlHelper.ExecuteScalar(
                ConnectionString.GetReadConnectionString(),
                CommandType.StoredProcedure,
                "i7_sflexi_items_GetCount",
                null));
        }

		public static int GetCountForModule(int moduleId)
		{
			SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_GetCountForModule", 1);
			sph.DefineSqlParameter("@ModuleID", SqlDbType.Int, ParameterDirection.Input, moduleId);
			return Convert.ToInt32(sph.ExecuteScalar());
		}

		/// <summary>
		/// Gets an IDataReader with all rows in the i7_sflexi_items table.
		/// </summary>
		public static IDataReader GetAll(Guid siteGuid)
        {
			SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectAll", 1);
			sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
			return sph.ExecuteReader();
        }


        /// <summary>
        /// Gets an IDataReader with all items for module.
        /// </summary>
        /// <param name="itemID"> itemID </param>
        public static IDataReader GetModuleItems(int moduleId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectAllForModule", 1);
            sph.DefineSqlParameter("@ModuleID", SqlDbType.Int, ParameterDirection.Input, moduleId);
            return sph.ExecuteReader();
        }

		/// <summary>
		/// Gets and IDataReader for a page of items for a module
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetPageOfModuleItems(
			Guid moduleGuid,
			int pageNumber,
			int pageSize,
			string searchTerm = "",
			string searchField = "",
			//string sortField = "",
			bool descending = false)
		{
			SqlParameterHelper sph = null;

			if (String.IsNullOrWhiteSpace(searchField) && !String.IsNullOrWhiteSpace(searchTerm))
			{
				sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectPageForModuleWithTerm", 5);
				sph.DefineSqlParameter("@ModuleGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, moduleGuid);
				sph.DefineSqlParameter("@PageNumber", SqlDbType.Int, ParameterDirection.Input, pageNumber);
				sph.DefineSqlParameter("@PageSize", SqlDbType.Int, ParameterDirection.Input, pageSize);
				sph.DefineSqlParameter("@SearchTerm", SqlDbType.NVarChar, -1, ParameterDirection.Input, searchTerm);
				//sph.DefineSqlParameter("@sortField", SqlDbType.NVarChar, -1, ParameterDirection.Input, sortField);
				sph.DefineSqlParameter("@SortDirection", SqlDbType.VarChar, 4, ParameterDirection.Input, descending ? "desc" : "asc");
			}
			else if (!String.IsNullOrWhiteSpace(searchField) && !String.IsNullOrWhiteSpace(searchTerm))
			{
				sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectPageForModuleWithTermAndField", 6);
				sph.DefineSqlParameter("@ModuleGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, moduleGuid);
				sph.DefineSqlParameter("@PageNumber", SqlDbType.Int, ParameterDirection.Input, pageNumber);
				sph.DefineSqlParameter("@PageSize", SqlDbType.Int, ParameterDirection.Input, pageSize);
				sph.DefineSqlParameter("@SearchTerm", SqlDbType.NVarChar, -1, ParameterDirection.Input, searchTerm);
				sph.DefineSqlParameter("@SearchField", SqlDbType.NVarChar, -1, ParameterDirection.Input, searchField);
				//sph.DefineSqlParameter("@sortField", SqlDbType.NVarChar, -1, ParameterDirection.Input, sortField);
				sph.DefineSqlParameter("@SortDirection", SqlDbType.VarChar, 4, ParameterDirection.Input, descending ? "desc" : "asc");
			}
			else
			{
				sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectPageForModule", 4);
				sph.DefineSqlParameter("@ModuleGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, moduleGuid);
				sph.DefineSqlParameter("@PageNumber", SqlDbType.Int, ParameterDirection.Input, pageNumber);
				sph.DefineSqlParameter("@PageSize", SqlDbType.Int, ParameterDirection.Input, pageSize);
				//sph.DefineSqlParameter("@sortField", SqlDbType.NVarChar, -1, ParameterDirection.Input, sortField);
				sph.DefineSqlParameter("@SortDirection", SqlDbType.VarChar, 4, ParameterDirection.Input, descending ? "desc" : "asc");
			}
			return sph.ExecuteReader();
		}
		/// <summary>
		/// Gets and IDataReader for a page of items for a definition
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetPageForDefinition(
			Guid defGuid,
			Guid siteGuid,
			int pageNumber,
			int pageSize,
			string searchTerm = "",
			string searchField = "",
			//string sortField = "",
			bool descending = false
			)
		{
			SqlParameterHelper sph = null;

			if (String.IsNullOrWhiteSpace(searchField) && !String.IsNullOrWhiteSpace(searchTerm))
			{
				sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectPageForDefinitionWithTerm", 6);
				sph.DefineSqlParameter("@DefGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, defGuid);
				sph.DefineSqlParameter("@PageNumber", SqlDbType.Int, ParameterDirection.Input, pageNumber);
				sph.DefineSqlParameter("@PageSize", SqlDbType.Int, ParameterDirection.Input, pageSize);
				sph.DefineSqlParameter("@SearchTerm", SqlDbType.NVarChar, -1, ParameterDirection.Input, searchTerm);
				//sph.DefineSqlParameter("@sortField", SqlDbType.NVarChar, -1, ParameterDirection.Input, sortField);
				sph.DefineSqlParameter("@SortDirection", SqlDbType.VarChar, 4, ParameterDirection.Input, descending ? "desc" : "asc");
				sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
			}
			else if (!String.IsNullOrWhiteSpace(searchField) && !String.IsNullOrWhiteSpace(searchTerm))
			{
				sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectPageForDefinitionWithTermAndField", 7);
				sph.DefineSqlParameter("@DefGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, defGuid);
				sph.DefineSqlParameter("@PageNumber", SqlDbType.Int, ParameterDirection.Input, pageNumber);
				sph.DefineSqlParameter("@PageSize", SqlDbType.Int, ParameterDirection.Input, pageSize);
				sph.DefineSqlParameter("@SearchTerm", SqlDbType.NVarChar, -1, ParameterDirection.Input, searchTerm);
				sph.DefineSqlParameter("@SearchField", SqlDbType.NVarChar, -1, ParameterDirection.Input, searchField);
				//sph.DefineSqlParameter("@sortField", SqlDbType.NVarChar, -1, ParameterDirection.Input, sortField);
				sph.DefineSqlParameter("@SortDirection", SqlDbType.VarChar, 4, ParameterDirection.Input, descending ? "desc" : "asc");
				sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
			}
			else
			{
				sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectPageForDefinition", 5);
				sph.DefineSqlParameter("@DefGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, defGuid);
				sph.DefineSqlParameter("@PageNumber", SqlDbType.Int, ParameterDirection.Input, pageNumber);
				sph.DefineSqlParameter("@PageSize", SqlDbType.Int, ParameterDirection.Input, pageSize);
				//sph.DefineSqlParameter("@sortField", SqlDbType.NVarChar, -1, ParameterDirection.Input, sortField);
				sph.DefineSqlParameter("@SortDirection", SqlDbType.VarChar, 4, ParameterDirection.Input, descending ? "desc" : "asc");
				sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
			}
			return sph.ExecuteReader();
		}
		/// <summary>
		/// Gets an IDataReader with all items for a single definition.
		/// </summary>
		public static IDataReader GetAllForDefinition(Guid definitionGuid, Guid siteGuid)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectAllForDefinition", 2);
            sph.DefineSqlParameter("@DefinitionGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, definitionGuid);
            sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
			return sph.ExecuteReader();
        }

        /// <summary>
        /// Gets a page of data from the i7_sflexi_items table.
        /// </summary>
        public static IDataReader GetPage(
            int pageNumber,
            int pageSize,
            out int totalPages)
        {
            totalPages = 1;
            int totalRows
                = GetCount();

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

            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectPage", 2);
            sph.DefineSqlParameter("@PageNumber", SqlDbType.Int, ParameterDirection.Input, pageNumber);
            sph.DefineSqlParameter("@PageSize", SqlDbType.Int, ParameterDirection.Input, pageSize);
            return sph.ExecuteReader();

        }

		/// <summary>
		/// Gets all superflexi items from all superflexi modules on a page
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetByCMSPage(Guid siteGuid, int pageId)
        {
            SqlParameterHelper sph = new SqlParameterHelper(ConnectionString.GetReadConnectionString(), "i7_sflexi_items_SelectByCMSPage", 2);
            sph.DefineSqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier, ParameterDirection.Input, siteGuid);
            sph.DefineSqlParameter("@PageID", SqlDbType.Int, ParameterDirection.Input, pageId);
            return sph.ExecuteReader();
        }
    }

}


