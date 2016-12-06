using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talent.Domain;
using UclaExt.Common.Interfaces;
using UclaExt.Common.ExtensionMethods;

namespace Talent.DataAccess.Ado
{
    internal static class CreditHelper
    {

        #region PersistChild

        public static Credit Persist(Credit credit, SqlConnection conn)
        {

            if (credit.Id == 0 && credit.IsMarkedForDeletion)
            {
                credit = null;
            }
            else if (credit.IsMarkedForDeletion)
            {
                DeleteEntity(credit, conn);
                credit = null;
            }
            else if (credit.Id == 0)
            {
                InsertEntity(credit, conn);
                credit.IsDirty = false;
            }
            else if (credit.IsDirty)
            {
                UpdateEntity(credit, conn);
                credit.IsDirty = false;
            }
            return credit;

        }

        #endregion

        #region SQL

        internal static void InsertEntity(Credit item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("insert Credit (ShowId, PersonId, CreditTypeId, Character)");
                sql.Append("values ( @ShowId, @PersonId, @CreditTypeId, @Character);");
                sql.Append("select cast ( scope_identity() as int);");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);

                item.Id = (int)cmd.ExecuteScalar();
            }
        }

        internal static void UpdateEntity(Credit item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("update Credit set ");
                sql.Append(" ShowId = @ShowId, ");
                sql.Append(" PersonId = @PersonId, ");
                sql.Append(" CreditTypeId = @CreditTypeId, ");
                sql.Append(" Character = @Character ");
                sql.Append("where Id = @Id ");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);
                cmd.Parameters.AddWithValue("@Id", item.Id);

                cmd.ExecuteNonQuery();
            }
        }

        internal static void DeleteEntity(Credit item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "delete Credit where Id = @Id";
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.ExecuteNonQuery();
            }
        }

        private static void SetCommonParameters(Credit item, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@ShowId", item.ShowId);
            cmd.Parameters.AddWithValue("@PersonId", item.PersonId);
            cmd.Parameters.AddWithValue("@CreditTypeId", item.CreditTypeId);
            cmd.Parameters.AddWithValue("@Character", 
                    item.Character.AsSqlParameterValue());
        }

        #endregion

    }
}

