using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class StuObjectiveRepo : RepositoryBase<TB_Stu_Objective>, IStuObjectiveRepo
    {
        public StuObjectiveRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public TB_Stu_Objective GetByUserID(int userID)
        {
            return DataContext.TB_Stu_Objective.Where(x => x.UserId == userID).FirstOrDefault();
        }

        public async Task<ObjectiveDTO> FindLatestObjectiveByUserAsync(int userID)
        {
            string sql = string.Format(" select top(1)* from TB_Stu_Objective a    where a.UserId= {0} order by  ModifyDate desc ", userID);
            var query = DataContext.Database.SqlQuery<ObjectiveDTO>(sql);
            return query.FirstOrDefault();
        }


        public async Task<ObjectiveDTO> FindObjectiveByUserIDAsync(int userID)
        {
            string sql = string.Format(" select * from TB_Stu_Objective a    where a.UserId= {0}  ", userID);
            var query = DataContext.Database.SqlQuery<ObjectiveDTO>(sql);
            return query.FirstOrDefault();
        }


        public async Task<ObjectiveDTO> FindObjectiveAsync(int id)
        {
            string sql = string.Format(" select * from TB_Stu_Objective a    where a.ObjectiveId= {0}  ", id);
            var query = DataContext.Database.SqlQuery<ObjectiveDTO>(sql);
            return query.FirstOrDefault();
        }

        public async Task UpdateAsync(ObjectiveDTO model)
        {
            var target = DataContext.TB_Stu_Objective.Where(o => o.ObjectiveId == model.ObjectiveId).FirstOrDefault();
            target.ObjectiveType = model.ObjectiveType;
            target.ObjectLocation = model.ObjectLocation;
            target.ObjectiveSalary = model.ObjectiveSalary;
            target.InternShipLocation = model.InternShipLocation;
            target.InternShipSalary = model.InternShipSalary;
            target.Keyword = model.Keyword;
            target.ModifyDate = DateTime.Now;
            DataContext.TB_Stu_Objective.Attach(target);
            DataContext.Entry(target).State = EntityState.Modified;
            DataContext.SaveChanges();
        }

        public async Task CreateAsync(ObjectiveDTO model)
        {
            var target = DataContext.TB_Stu_Objective.Where(o => o.UserId == model.UserId).FirstOrDefault();
            if(target==null)
            {
                target = new TB_Stu_Objective();
                target.ObjectiveType = model.ObjectiveType;
                target.ObjectLocation = model.ObjectLocation;
                target.ObjectiveSalary = model.ObjectiveSalary;
                target.InternShipLocation = model.InternShipLocation;
                target.InternShipSalary = model.InternShipSalary;
                target.Keyword = model.Keyword;
                target.ModifyDate = DateTime.Now;
                target.CreateDate = DateTime.Now;
                target.UserId = model.UserId;
                DataContext.TB_Stu_Objective.Add(target);
                DataContext.SaveChanges();
            }
          
        }


        public async Task DeleteAsync(int id)
        {
            string sql = string.Format("delete from TB_Stu_Objective where ObjectiveId='{0}'", id);
            await DataContext.Database.ExecuteSqlCommandAsync(sql);
        }

    }

    public interface IStuObjectiveRepo : IRepository<TB_Stu_Objective>
    {
        /// <summary>
        /// 通过UserID获取该学生职业理想
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        TB_Stu_Objective GetByUserID(int userID);


        /// <summary>
        /// 获取最新一笔的职业理想
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<ObjectiveDTO> FindLatestObjectiveByUserAsync(int userID);


        /// <summary>
        /// 更新职业理想
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateAsync(ObjectiveDTO model);

        /// <summary>
        /// 创建职业理想
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task CreateAsync(ObjectiveDTO model);


        /// <summary>
        /// 删除职业理想
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(int id);


        Task<ObjectiveDTO> FindObjectiveByUserIDAsync(int userID);


        Task<ObjectiveDTO> FindObjectiveAsync(int id);
    }
}
