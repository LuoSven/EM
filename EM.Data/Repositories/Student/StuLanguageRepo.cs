using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Model.Entities;

using Dapper;
using Topuc22Top.Data.Dapper;
using Dapper.Contrib.Extensions;
using System;

namespace Topuc22Top.Data.Repositories
{
    public class StuLanguageRepo : RepositoryBase<TB_Stu_Language>, IStuLanguageRepo
    {
        public StuLanguageRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public async Task<StuLanguageDTO> FindAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select Id,Name,Hear,ReadWrite,Certifications,UserId from TB_Stu_Language where Id = @id";
                var model = await conn.QueryAsync<StuLanguageDTO>(sql, new { @id = id });
                return model.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<StuLanguageDTO>> FindListByUserAsync(int userId)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select Id,Name,Hear,ReadWrite,Certifications,UserId from TB_Stu_Language where UserId = @userId";
                var model = await conn.QueryAsync<StuLanguageDTO>(sql, new { @userId = userId });
                return model.ToList();
            }
        }

        public async Task<int> CreateAsync(TB_Stu_Language model)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                model.CreateDate = DateTime.Now;
                model.ModifyDate = DateTime.Now;
                return await conn.InsertAsync(model);
            }
        }

        public async Task UpdateAsync(StuLanguageDTO model)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var target = await conn.GetAsync<TB_Stu_Language>(model.Id);
                target.Name = model.Name;
                target.Hear = model.Hear;
                target.ReadWrite = model.ReadWrite;
                target.Certifications = model.Certifications;
                target.ModifyDate = DateTime.Now;
                await conn.UpdateAsync(target);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"delete from TB_Stu_Language where Id = @id";
                await conn.ExecuteAsync(sql, new { @id = id });
            }
        }

        public async Task<bool> IsExistAsync(TB_Stu_Language model)
        {
            var id = (await DapperHelper.SqlQuery22Async<int>(@"select Id from TB_Stu_Language  a where  a.UserId=@UserId and a.Name=@Name", new { UserId = model.UserId, Name = model.Name })).FirstOrDefault();
            if (id == null || id == 0)
                return false;
            return true;
        }

    }

    public interface IStuLanguageRepo : IRepository<TB_Stu_Language>
    {
        Task<StuLanguageDTO> FindAsync(int id);
        Task<IEnumerable<StuLanguageDTO>> FindListByUserAsync(int userId);

        Task<int> CreateAsync(TB_Stu_Language model);

        Task UpdateAsync(StuLanguageDTO model);

        Task DeleteAsync(int id);
        Task<bool> IsExistAsync(TB_Stu_Language model);
    }

}
