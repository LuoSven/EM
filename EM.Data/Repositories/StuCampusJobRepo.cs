using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Model.Entities;
using System.Data.Entity;

namespace Topuc22Top.Data.Repositories
{
    public class StuCampusJobRepo : RepositoryBase<TB_Stu_CampusJob>, IStuCampusJobRepo
    {
        public StuCampusJobRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public TB_Stu_CampusJob GetByUserID(int userID)
        {
            var query = from m in DataContext.TB_Stu_CampusJob
                        where m.UserId == userID
                        select m;
            return query.FirstOrDefault();
        }
        public async Task<CampusJobDTO> FindLatestCampusJobByUserAsync(int userID)
        {
            string sql = string.Format(" select top(1)* from TB_Stu_CampusJob a    where a.UserId= {0} order by  startdate desc ", userID);//select*
            var query = DataContext.Database.SqlQuery<CampusJobDTO>(sql);
            return query.FirstOrDefault();
        }


        public async Task<CampusJobDTO> GetByUserIDAsync(int userID)
        {
            string sql = string.Format(" select * from TB_Stu_CampusJob a    where a.UserId= {0}  ", userID);//select*
            var query = DataContext.Database.SqlQuery<CampusJobDTO>(sql);
            return query.FirstOrDefault();
        }


        public async Task<CampusJobDTO> GetByIdAsync(int id)
        {
            string sql = string.Format(" select * from TB_Stu_CampusJob a    where a.Id= {0}  ", id);//select*
            var query = DataContext.Database.SqlQuery<CampusJobDTO>(sql);
            return query.FirstOrDefault();
        }

        public async Task UpdateAsync(CampusJobDTO model)
        {
            var target = DataContext.TB_Stu_CampusJob.Where(o => o.Id == model.Id).FirstOrDefault();
            target.JobDesc = model.JobDesc;
            target.JobName = model.JobName;
            target.SchoolName = model.SchoolName;
            target.SchoolNameExt = model.SchoolNameExt;
            target.StartDate = model.StartDate;
            target.EndDate = model.EndDate;
            target.ModifyDate = DateTime.Now;
            var result = await Dapper.DapperHelper.UpdateAsync(target);
            //DataContext.TB_Stu_CampusJob.Attach(target);
            //DataContext.Entry(target).State = EntityState.Modified;
            //DataContext.SaveChanges();

        }

        public async Task CreateAsync(CampusJobDTO model)
        {
            var target = new TB_Stu_CampusJob();
            target.JobDesc = model.JobDesc;
            target.JobName = model.JobName;
            target.SchoolName = model.SchoolName;
            target.SchoolNameExt = model.SchoolNameExt;
            target.StartDate = model.StartDate;
            target.EndDate = model.EndDate;
            target.ModifyDate = DateTime.Now;
            target.CreateDate = DateTime.Now;
            target.UserId = model.UserId;

            var result = await Dapper.DapperHelper.InsertAsync(target);
            //DataContext.TB_Stu_CampusJob.Add(target);
            //DataContext.SaveChanges();
        }


        public async Task DeleteAsync(int id)
        {
            string sql = string.Format("delete from TB_Stu_CampusJob where Id='{0}'", id);
            await DataContext.Database.ExecuteSqlCommandAsync(sql);
        }

        public async Task<List<CampusJobDTO>> GetListByUserId(int UserId)
        {
            string sql = string.Format(" select * from TB_Stu_CampusJob a    where a.UserId= {0}  ", UserId);//select*
            var query = await Dapper.DapperHelper.SqlQuery22Async<CampusJobDTO>(sql);
            return query.ToList();
        }
    }
    public interface IStuCampusJobRepo : IRepository<TB_Stu_CampusJob>
    {

        /// <summary>
        /// 根据用户获取列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<List<CampusJobDTO>> GetListByUserId(int UserId);

        /// <summary>
        /// 获取最新一笔的校内职务
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task< CampusJobDTO> FindLatestCampusJobByUserAsync(int userID);


        /// <summary>
        /// 通过UserID获取校内职务
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        TB_Stu_CampusJob GetByUserID(int userID);


       /// <summary>
       /// 更新校内职务
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
        Task UpdateAsync(CampusJobDTO model);

        /// <summary>
        /// 创建校内职务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task CreateAsync(CampusJobDTO model);


       /// <summary>
       /// 删除校内职务
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        Task DeleteAsync(int id);


        Task<CampusJobDTO> GetByUserIDAsync(int userID);
        

        Task<CampusJobDTO> GetByIdAsync(int id);
        

    }
}
