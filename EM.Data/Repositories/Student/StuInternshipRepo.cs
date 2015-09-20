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
    public class StuInternshipRepo : RepositoryBase<TB_Stu_Internship>, IStuInternshipRepo
    {
        public StuInternshipRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public async Task<StuInternshipDTO> FindAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select Id,Units,UnitsDept,UnitsPosition,UnitsWorkDesc,StartDate,EndDate,UserId from TB_Stu_Internship where Id = @id";
                var model = await conn.QueryAsync<StuInternshipDTO>(sql, new { @id = id });
                return model.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<StuInternshipDTO>> FindListByUserAsync(int userId)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"select Id,Units,UnitsDept,UnitsPosition,UnitsWorkDesc,StartDate,EndDate,UserId from TB_Stu_Internship where UserId = @userId";
                var model = await conn.QueryAsync<StuInternshipDTO>(sql, new { @userId = userId });
                return model.ToList();
            }
        }

        public async Task<int> CreateAsync(TB_Stu_Internship model)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                model.CreateDate = DateTime.Now;
                model.ModifyDate = DateTime.Now;
                return await conn.InsertAsync(model);
            }
        }

        public async Task UpdateAsync(StuInternshipDTO model)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var target = await conn.GetAsync<TB_Stu_Internship>(model.Id);
                target.Units = model.Units;
                target.UnitsDept = model.UnitsDept;
                target.UnitsPosition = model.UnitsPosition;
                target.UnitsWorkDesc = model.UnitsWorkDesc;
                target.StartDate = model.StartDate;
                target.EndDate = model.EndDate;
                target.ModifyDate = DateTime.Now;
                await conn.UpdateAsync(target);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"delete from TB_Stu_Internship where Id = @id";
                await conn.ExecuteAsync(sql, new { @id = id });
            }
        }

    }

    public interface IStuInternshipRepo : IRepository<TB_Stu_Internship>
    {
        Task<StuInternshipDTO> FindAsync(int id);
        Task<IEnumerable<StuInternshipDTO>> FindListByUserAsync(int userId);

        Task<int> CreateAsync(TB_Stu_Internship model);

        Task UpdateAsync(StuInternshipDTO model);

        Task DeleteAsync(int id);
    }

}
