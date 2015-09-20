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
    public class StuRewardRepo : RepositoryBase<TB_Stu_Reward>, IStuRewardRepo
    {
        public StuRewardRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public async Task<StuRewardDTO> FindAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select Id,RewardTime,RewardName,RewardSchoolExt,RewardDesc,UserId from TB_Stu_Reward where Id = @id";
                var model = await conn.QueryAsync<StuRewardDTO>(sql, new { @id = id });
                return model.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<StuRewardDTO>> FindListByUserAsync(int userId)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select Id,RewardTime,RewardName,RewardSchoolExt,RewardDesc,UserId from TB_Stu_Reward where UserId = @userId";
                var model = await conn.QueryAsync<StuRewardDTO>(sql, new { @userId = userId });
                return model.ToList();
            }
        }

        public async Task<int> CreateAsync(TB_Stu_Reward model)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                //model.RewardSchool = 
                model.CreateDate = DateTime.Now;
                model.ModifyDate = DateTime.Now;
                return await conn.InsertAsync(model);
            }
        }

        public async Task UpdateAsync(StuRewardDTO model)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var target = await conn.GetAsync<TB_Stu_Reward>(model.Id);
                target.RewardTime = model.RewardTime;
                target.RewardName = model.RewardName;
                target.RewardSchoolExt = model.RewardSchoolExt;
                target.RewardDesc = model.RewardDesc;
                //target.RewardSchool = 
                target.ModifyDate = DateTime.Now;
                await conn.UpdateAsync(target);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"delete from TB_Stu_Reward where Id = @id";
                await conn.ExecuteAsync(sql, new { @id = id });
            }
        }

    }

    public interface IStuRewardRepo : IRepository<TB_Stu_Reward>
    {
        Task<StuRewardDTO> FindAsync(int id);
        Task<IEnumerable<StuRewardDTO>> FindListByUserAsync(int userId);

        Task<int> CreateAsync(TB_Stu_Reward model);

        Task UpdateAsync(StuRewardDTO model);

        Task DeleteAsync(int id);
    }

}
