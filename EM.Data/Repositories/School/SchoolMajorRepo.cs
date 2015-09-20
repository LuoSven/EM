using System.Collections.Generic;
using System.Linq;
using Topuc.Framework.Cache;
using Topuc22Top.Data.Dapper;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

using Dapper;
using System.Data;
using Dapper.Contrib.Extensions;
using Topuc22Top.Model.DTOs;

namespace Topuc22Top.Data.Repositories
{
    public class SchoolMajorRepo : RepositoryBase<School_Major>, ISchoolMajorRepo
    {
        private readonly ICache cache;

        public SchoolMajorRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public IList<School_Major> GetSchoolMajorList(int schoolId)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select * from School_Major where SchoolId = @schoolId and ISNULL(MajorName,'') <> ''";
                var result = conn.Query<School_Major>(sql, new { @schoolId = schoolId });
                return result.ToList();
            }
        }

        public bool UpdateSchoolMajors(int schoolId, string[] codeArr, string[] nameArr)
        {
            //if (nameArr.Length == 0 || codeArr.Length != nameArr.Length) return false;
            using (var conn = DapperHelper.Get22Connection())
            {
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var sql = "delete from School_Major where SchoolId = @schoolId";
                    conn.Execute(sql, new { @schoolId = schoolId }, transaction);

                    for (int i = 0; i < nameArr.Length; i++)
                    {
                        conn.Insert<SchoolMajorDTO>(new SchoolMajorDTO()
                        {
                            SchoolId = schoolId,
                            MajorCode = codeArr[i],
                            MajorName = nameArr[i]
                        }, transaction);
                    }

                    transaction.Commit();
                    return true;
                }
                catch 
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

    }

    public interface ISchoolMajorRepo : IRepository<School_Major>
    {
        IList<School_Major> GetSchoolMajorList(int schoolId);
        bool UpdateSchoolMajors(int schoolId, string[] codeArr, string[] nameArr);
    }
}
