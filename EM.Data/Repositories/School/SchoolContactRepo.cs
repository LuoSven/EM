using Topuc.Framework.Cache;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using Topuc22Top.Data.Dapper;
using System.Linq;

using Dapper;
using Topuc22Top.Model.DTOs;

namespace Topuc22Top.Data.Repositories
{
    class SchoolContactRepo : RepositoryBase<School_Contact>, ISchoolContactRepo
    {
        private readonly ICache cache;

        public SchoolContactRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public bool DeleteContact(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var sql = "delete from School_Contact where Id = @id";
                var result = conn.Execute(sql, new { @id = id});
                return result > 1;
            }
        }


        public IList<School_Contact> GetSchoolContactList(int schoolId)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select * from School_Contact where SchoolId = @schoolId order by ModifyDate desc, CreateDate desc";
                var result = conn.Query<School_Contact>(sql, new { @schoolId = schoolId });
                return result.ToList();
            }
        }

        public int AddSchoolContact(SchoolContactDTO schoolContact)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var result = conn.Insert<SchoolContactDTO>(schoolContact);
                return (int)result;
            }
        }

        public bool UpdateSchoolContact(SchoolContactDTO schoolContact)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var result = conn.Update<SchoolContactDTO>(schoolContact);
                return result;
            }
        }


    }

    public interface ISchoolContactRepo : IRepository<School_Contact>
    {
        bool DeleteContact(int id);
        IList<School_Contact> GetSchoolContactList(int schoolId);
        int AddSchoolContact(SchoolContactDTO schoolContact);
        bool UpdateSchoolContact(SchoolContactDTO schoolContact);
    }

}
