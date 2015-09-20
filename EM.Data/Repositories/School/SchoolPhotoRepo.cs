using Topuc.Framework.Cache;
using Topuc22Top.Data.Dapper;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Model.Entities;

using Dapper;
using Dapper.Contrib.Extensions;

namespace Topuc22Top.Data.Repositories
{
    class SchoolPhotoRepo : RepositoryBase<School_Photo>, ISchoolPhotoRepo
    {
        private readonly ICache cache;

        public SchoolPhotoRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public SchoolPhotoDTO GetSchoolPhotoById(int id) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var result = conn.Get<SchoolPhotoDTO>(id);
                return result;
            }
        }

        public int AddSchoolPhoto(SchoolPhotoDTO schoolPhoto)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var result = conn.Insert<SchoolPhotoDTO>(schoolPhoto);
                return (int)result;
            }
        }

        public bool UpdateSchoolPhoto(SchoolPhotoDTO schoolPhoto) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var result = conn.Update<SchoolPhotoDTO>(schoolPhoto);
                return result;
            }
        }

        public bool DeletePhoto(int id) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var sql = "delete from School_Photo where Id = @id";
                var result = conn.Execute(sql, new { @id = id });
                return result > 1;
            }
        }

        public PagedResult<School_Photo> GetSchoolPhotoList(int schoolId, int page, int pageSize)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select * from School_Photo where schoolId = @schoolId";

                var result = conn.QueryWithPage<School_Photo>(sql, new
                {
                    @schoolId = schoolId
                }, "UploadTime desc", page, pageSize);

                return result;
            }
        }

    }

    public interface ISchoolPhotoRepo : IRepository<School_Photo>
    {
        SchoolPhotoDTO GetSchoolPhotoById(int id);
        int AddSchoolPhoto(SchoolPhotoDTO schoolPhoto);
        bool UpdateSchoolPhoto(SchoolPhotoDTO schoolPhoto);
        bool DeletePhoto(int id);

        PagedResult<School_Photo> GetSchoolPhotoList(int schoolId, int page, int pageSize);
    }

}
