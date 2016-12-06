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
using System.Transactions;

namespace Talent.DataAccess.Ado
{
    public class PersonRepository : IRepository<Person>
    {
        #region IRepository<Person> Members

        public IEnumerable<Person> Fetch(object criteria = null)
        {
            var data = new List<Person>();
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
                        var sql = new StringBuilder();
                        sql.Append("select * from Person; \r\n");
                        sql.Append("select * from Credit; \r\n");
                        sql.Append("select * from PersonAttachment \r\n");
                        cmd.CommandText = sql.ToString();
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            data.Add(LoadPersonFromDataReader(dr));
                        }

                        dr.NextResult();
                        while (dr.Read())
                        {
                            var c = LoadCreditFromDataReader(dr);
                            data.Where(o => o.Id == c.PersonId)
                                .Single().Credits.Add(c);
                        }
                        dr.NextResult();
                        while (dr.Read())
                        {
                            var c = LoadPersonAttachmentFromDataReader(dr);
                            data.Where(o => o.Id == c.PersonId)
                                .Single().PersonAttachments.Add(c);
                        }
                    }
                    else if (criteria is PersonCriteria)
                    {
                        var crit = criteria as PersonCriteria;
                        var sql = new StringBuilder();
                        sql.Append(@"select * 
                            from Person 
                            where FirstName like '%' + @Name + '%' 
                                or LastName like '%' + @Name + '%';"
                        );
                        sql.Append(@"select c.* 
                            from Credit c 
                            join Person p on c.PersonId = p.Id 
                            where p.FirstName like  '%' + @Name + '%' 
                                or p.LastName like '%' + @Name + '%';"
                        );
                        sql.Append(@"select pa.* 
                            from PersonAttachment pa 
                            join Person p on pa.PersonId = p.Id 
                            where p.FirstName like  '%' + @Name + '%' 
                                or p.LastName like '%' + @Name + '%';"
                        );
                        cmd.CommandText = sql.ToString();
                        cmd.Parameters.AddWithValue("@Name", crit.Name);
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            data.Add(LoadPersonFromDataReader(dr));
                        }
                        dr.NextResult();
                        while (dr.Read())
                        {
                            var c = LoadCreditFromDataReader(dr);
                            data.Where(o => o.Id == c.PersonId)
                                .Single().Credits.Add(c);
                        }
                        dr.NextResult();
                        while (dr.Read())
                        {
                            var c = LoadPersonAttachmentFromDataReader(dr);
                            data.Where(o => o.Id == c.PersonId)
                                .Single().PersonAttachments.Add(c);
                        }
                    }
                    else if (criteria is int)
                    {
                        var sql = new StringBuilder();
                        sql.Append("select * from Person where "
                            + "Id = @Id; \r\n");
                        sql.Append("select * from Credit where "
                            + "PersonId = @Id; ");
                        sql.Append("select * from PersonAttachment where "
                            + "PersonId = @Id; ");
                        cmd.CommandText = sql.ToString();
                        cmd.Parameters.AddWithValue("@Id", (int)criteria);
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            data.Add(LoadPersonFromDataReader(dr));
                        }
                        dr.NextResult();
                        while (dr.Read())
                        {
                            data[0].Credits.Add(LoadCreditFromDataReader(dr));
                        }
                        dr.NextResult();
                        while (dr.Read())
                        {
                            data[0].PersonAttachments.Add(LoadPersonAttachmentFromDataReader(dr));
                        }
                    }
                    else
                    {
                        var msg = String
                            .Format("PersonRepository: Unknown criteria type: {0}", 
                                criteria);
                        throw new InvalidOperationException(msg);
                    }
                }
            }
            return data;
        }

        private static Person LoadPersonFromDataReader(SqlDataReader dr)
        {
            var s = new Person();
            s.Id = dr.MapTo<int>("Id");
            s.Salutation = dr.MapTo<string>("Salutation");
            s.FirstName = dr.MapTo<string>("FirstName");
            s.MiddleName = dr.MapTo<string>("MiddleName");
            s.LastName = dr.MapTo<string>("LastName");
            s.Suffix = dr.MapTo<string>("Suffix");
            s.DateOfBirth = dr.MapTo<DateTime?>("DateOfBirth");
            s.Height = dr.MapTo<double?>("Height");
            s.HairColorId = dr.MapTo<int>("HairColorId");
            s.EyeColorId = dr.MapTo<int>("EyeColorId");

            s.IsDirty = false;
            return s;
        }

        private static Credit LoadCreditFromDataReader(SqlDataReader dr)
        {
            var c = new Credit();
            c.Id = dr.MapTo<int>("Id");
            c.PersonId = dr.MapTo<int>("PersonId");
            c.ShowId = dr.MapTo<int>("ShowId");
            c.CreditTypeId = dr.MapTo<int>("CreditTypeId");
            c.Character = dr.MapTo<string>("Character");

            c.IsDirty = false;
            return c;
        }

        private static PersonAttachment LoadPersonAttachmentFromDataReader(SqlDataReader dr)
        {
            var c = new PersonAttachment();
            c.Id = dr.MapTo<int>("Id");
            c.PersonId = dr.MapTo<int>("PersonId");
            c.Caption = dr.MapTo<string>("Caption");
            c.FileName = dr.MapTo<string>("FileName");
            c.FileExtension = dr.MapTo<string>("FileExtension");
            c.FileBytes = dr.MapTo<Byte[]>("FileBytes");

            c.IsDirty = false;
            return c;
        }

        /// <summary>
        /// Saves entity changes to the database
        /// </summary>
        /// <param name="item"></param>
        /// <returns>updated entity, or null if the entity is deleted</returns>
        public Person Persist(Person item)
        {
            if (item.Id == 0 && item.IsMarkedForDeletion)
            {
                return null;
            }

            var connString = ConfigurationManager
                .ConnectionStrings["AppConnection"].ConnectionString;
            using (TransactionScope ts = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    if (item.IsMarkedForDeletion)
                    {
                        // Also Deletes Children
                        DeleteEntity(item, conn);
                        item = null;
                    }
                    else if (item.Id == 0)
                    {
                        InsertEntity(item, conn);
                        PersistChildren(item, conn);
                        item.IsDirty = false;
                    }
                    else if (item.IsDirty)
                    {
                        UpdateEntity(item, conn);
                        PersistChildren(item, conn);
                        item.IsDirty = false;
                    }
                    else
                    {
                        // No changes to Person, but might be changes to children
                        PersistChildren(item, conn);
                    }
                }
                ts.Complete();
            }
            return item;
        }


        private static void PersistChildren(Person Person, SqlConnection conn)
        {
            foreach (var credit in Person.Credits)
            {
                credit.PersonId = Person.Id;
                CreditHelper.Persist(credit, conn);
            }
            foreach (var attachment in Person.PersonAttachments)
            {
                attachment.PersonId = Person.Id;
                PersonAttachmentHelper.Persist(attachment, conn);
            }
        }

        #endregion

        #region SQL Methods

        internal static void InsertEntity(Person item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("insert Person (Salutation, FirstName, MiddleName, "
                + "LastName, Suffix, DateOfBirth, Height, "
                + "HairColorId, EyeColorId)");
                sql.Append("values ( @Salutation, @FirstName, @MiddleName, "
                + "@LastName, @Suffix, @DateOfBirth, @Height,  "
                + "@HairColorId, @EyeColorId); ");
                sql.Append("select cast ( scope_identity() as int);");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);

                item.Id = (int)cmd.ExecuteScalar();
            }
        }

        internal static void UpdateEntity(Person item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("update Person set ");
                sql.Append(" Salutation = @Salutation, ");
                sql.Append(" FirstName = @FirstName, ");
                sql.Append(" MiddleName = @MiddleName, ");
                sql.Append(" LastName = @LastName, ");
                sql.Append(" Suffix = @Suffix, ");
                sql.Append(" DateOfBirth = @DateOfBirth, ");
                sql.Append(" Height = @Height, ");
                sql.Append(" HairColorId = @HairColorId, ");
                sql.Append(" EyeColorId = @EyeColorId ");
                sql.Append("where Id = @Id");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);
                cmd.Parameters.AddWithValue("@Id", item.Id);

                cmd.ExecuteNonQuery();
            }
        }

        internal static void DeleteEntity(Person item, SqlConnection conn)
        {

            // Cascade delete Credits
            foreach (var credit in item.Credits)
            {
                CreditHelper.DeleteEntity(credit, conn);
            }

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "delete Person where Id = @Id";
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.ExecuteNonQuery();
            }
        }

        private static void SetCommonParameters(Person person, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Salutation", person.Salutation ?? "");
            cmd.Parameters.AddWithValue("@FirstName", person.FirstName ?? "");
            cmd.Parameters.AddWithValue("@MiddleName", person.MiddleName ?? "");
            cmd.Parameters.AddWithValue("@LastName", person.LastName ?? "");
            cmd.Parameters.AddWithValue("@Suffix", person.Suffix ?? "");
            cmd.Parameters.AddWithValue("@DateOfBirth",
                person.DateOfBirth.AsSqlParameterValue());
            cmd.Parameters.AddWithValue("@Height", 
                person.Height.AsSqlParameterValue());
            cmd.Parameters.AddWithValue("@HairColorId", 
                person.HairColorId);
            cmd.Parameters.AddWithValue("@EyeColorId", 
                person.EyeColorId);
        }

        #endregion
    }
}

