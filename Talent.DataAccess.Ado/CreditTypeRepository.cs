using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talent.Domain;
using UclaExt.Common.ExtensionMethods;
using UclaExt.Common.Interfaces;

namespace Talent.DataAccess.Ado
{
    public class CreditTypeRepository : IRepository<CreditType>
    {
        #region IRepository interface

        public IEnumerable<CreditType> Fetch(object criteria = null)
        {
            var data = new List<CreditType>();
            var connString = ConfigurationManager
                .ConnectionStrings["AppConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    if (criteria == null)
                    {
                        cmd.CommandText = "select * from CreditType";
                    }
                    else if (criteria is int)
                    {
                        cmd.CommandText = "select * from CreditType where Id = @Id";
                        cmd.Parameters.AddWithValue("@Id", (int)criteria);
                    }
                    else
                    {
                        var msg = String.Format(
                            "CreditTypeRepository: Unknown criteria type: {0}",
                            criteria);
                        throw new InvalidOperationException(msg);
                    }
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var g = new CreditType();
                        g.Id = dr.AsInt32("Id");
                        g.Code = dr.AsString("Code");
                        g.Name = dr.AsString("Name");
                        g.IsInactive = dr.AsBoolean("IsInactive");
                        g.DisplayOrder = dr.AsInt32("DisplayOrder");
                        g.IsDirty = false;

                        data.Add(g);
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// Saves entity changes to the database
        /// </summary>
        /// <param name="item"></param>
        /// <returns>updated entity, or null if the entity is deleted</returns>
        public CreditType Persist(CreditType item)
        {
            if (item.Id == 0 && item.IsMarkedForDeletion)
            {
                return null;
            }

            var connString = ConfigurationManager.ConnectionStrings["AppConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                if (item.IsMarkedForDeletion)
                {
                    DeleteEntity(item, conn);
                    item = null;
                }
                else if (item.Id == 0)
                {
                    InsertEntity(item, conn);
                    item.IsDirty = false;
                }
                else if (item.IsDirty)
                {
                    UpdateEntity(item, conn);
                    item.IsDirty = false;
                }
            }
            return item;
        }

        #endregion

        #region SQL methods

        internal static void InsertEntity(CreditType item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("insert CreditType (Code, Name, IsInactive, DisplayOrder)");
                sql.Append("values (@Code, @Name, @IsInactive, @DisplayOrder);");
                sql.Append("select cast( scope_identity() as int);");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);

                item.Id = (int)cmd.ExecuteScalar();
            }
        }

        internal static void UpdateEntity(CreditType item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("update CreditType set ");
                sql.Append(" Code = @Code, ");
                sql.Append(" Name = @Name, ");
                sql.Append(" IsInactive = @IsInactive, ");
                sql.Append(" DisplayOrder = @DisplayOrder ");
                sql.Append("where Id = @Id");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);
                cmd.Parameters.AddWithValue("@Id", item.Id);

                cmd.ExecuteNonQuery();
            }
        }

        internal static void DeleteEntity(CreditType item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "delete CreditType where Id = @Id";
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.ExecuteNonQuery();
            }
        }



        private static void SetCommonParameters(CreditType item, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Code", item.Code);
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@IsInactive", item.IsInactive);
            cmd.Parameters.AddWithValue("@DisplayOrder", item.DisplayOrder);
        }

        #endregion

    }
}


