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
    public class StuITSkillRepo : RepositoryBase<TB_Stu_ITSkill>, IStuITSkillRepo
    {
        public StuITSkillRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public async Task<StuITSkillDTO> FindAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select Id,SkillName,SkillLevel,isnull(SkillTime,0) SkillTime,UserId from TB_Stu_ITSkill where Id = @id";
                var model = await conn.QueryAsync<StuITSkillDTO>(sql, new { @id = id });
                return model.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<StuITSkillDTO>> FindListByUserAsync(int userId)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select Id,SkillName,SkillLevel,isnull(SkillTime,0) SkillTime,UserId from TB_Stu_ITSkill where UserId = @userId";
                var model = await conn.QueryAsync<StuITSkillDTO>(sql, new { @userId = userId });
                return model.ToList();
            }
        }

        public async Task<int> CreateAsync(TB_Stu_ITSkill model)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                model.CreateDate = DateTime.Now;
                model.ModifyDate = DateTime.Now;
                return await conn.InsertAsync(model);
            }
        }

        public async Task UpdateAsync(StuITSkillDTO model)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var target = await conn.GetAsync<TB_Stu_ITSkill>(model.Id);
                target.SkillName = model.SkillName;
                target.SkillLevel = model.SkillLevel;
                target.SkillTime = model.SkillTime;
                target.ModifyDate = DateTime.Now;
                await conn.UpdateAsync(target);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"delete from TB_Stu_ITSkill where Id = @id";
                await conn.ExecuteAsync(sql, new { @id = id });
            }
        }

        public async Task<bool> IsExistAsync(TB_Stu_ITSkill model)
        {
            var id =( await DapperHelper.SqlQuery22Async<int>(@"select Id from TB_Stu_ITSkill  a where  a.UserId=@UserId and a.SkillName=@SkillName", new { UserId = model.UserId, SkillName = model.SkillName })).FirstOrDefault();
            if (id == null || id == 0)
                return false;
            return true;
        }

    }

    public interface IStuITSkillRepo : IRepository<TB_Stu_ITSkill>
    {
        Task<StuITSkillDTO> FindAsync(int id);
        Task<IEnumerable<StuITSkillDTO>> FindListByUserAsync(int userId);

        Task<int> CreateAsync(TB_Stu_ITSkill model);

        Task UpdateAsync(StuITSkillDTO model);

        Task DeleteAsync(int id);
        Task<bool> IsExistAsync(TB_Stu_ITSkill model);
    }

}
