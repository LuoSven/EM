using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Logger;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class JobFavoriteRepo : RepositoryBase<Job_Favorite>, IJobFavoriteRepo
    {
        public JobFavoriteRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public bool IsFavorite(int jobID, int stuID)
        {
            var query = from it in DataContext.Job_Favorite
                        where it.UserId == stuID && it.JobID == jobID && it.FavorStatus == 1
                        select it;
            return query.Any();
        }

        public OperationResult AddFavorite(int jobID, int stuID)
        {
            try
            {
                var fav = (from it in DataContext.Job_Favorite
                            where it.UserId == stuID && it.JobID == jobID
                            select it).OrderByDescending(p => p.CreateDate).FirstOrDefault();
                if (fav != null && fav.FavorStatus == 1)
                {
                    return OperationResult.RepeatOperation;
                }
                else 
                {
                    fav = new Job_Favorite()
                    {
                        JobID = jobID,
                        UserId = stuID,
                        FavorStatus = 1,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now
                    };
                    DataContext.Job_Favorite.Add(fav);
                    DataContext.SaveChanges();
                    return OperationResult.Success;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex.Message);
                return OperationResult.Fail;
            }
        }

        public int RemoveFavorite(int jobID, int stuID)
        {
            try
            {
                var query = from it in DataContext.Job_Favorite
                            where it.UserId == stuID && it.JobID == jobID
                            select it;
                var fav = query.OrderByDescending(p => p.CreateDate).FirstOrDefault();
                if (fav == null) return 0;
                fav.FavorStatus = 0;
                fav.ModifyDate = DateTime.Now;
                DataContext.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex.Message);
                return 0;
            }
        }

        public int RemoveFavorite(int id)
        {
            try
            {
                var query = from it in DataContext.Job_Favorite
                            where it.ID == id
                            select it;
                Job_Favorite fav = query.FirstOrDefault();
                if (fav == null) return 0;
                fav.FavorStatus = 0;
                fav.ModifyDate = DateTime.Now;
                DataContext.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex.Message);
                return 0;
            }
        }

        public PagedResult<JobInFavorite> GetFavoriteJobs(int stuID, int pageNo, int pageSize)
        {
            var favList = (from it in DataContext.Job_Favorite
                           where it.UserId == stuID && it.FavorStatus == 1
                           select it.ID).ToList();
            var query = from f in DataContext.Job_Favorite
                        join p in DataContext.TB_Position_Element
                        on f.JobID equals p.PositionId
                        join e in DataContext.TB_Enterprise
                        on p.EnterpriseId equals e.EnterpriseId
                        join c in (DataContext.DictItem.Where(a => a.Type == "City"))
                        on p.CityId equals c.ItemId
                        where favList.Contains(f.ID)
                        orderby f.ID descending
                        select new JobInFavorite()
                        {
                            ID = f.ID,
                            JobID = f.JobID,
                            JobName = p.Position,
                            EtpID = p.EnterpriseId,
                            EtpName = e.Name,
                            CreateDate = f.CreateDate,
                            City = c.ItemName,
                            FavoriteStatus = f.FavorStatus,
                            //--------2014-7-28 Green
                            PositionStatus = p.PositionStatus,
                            DeadLine = p.Deadline,
                            SalaryMin = p.SalaryMin,
                            SalaryMax = p.SalaryMax,
                            InternSalaryType = p.InternSalaryType,
                            Industry = e.Industry
                        };
            return new PagedResult<JobInFavorite>()
            {
                Results = query.OrderByDescending(x => x.CreateDate).Skip(pageSize * (pageNo - 1)).Take(pageSize).ToList(),
                PageSize = pageSize,
                CurrentPage = pageNo,
                RowCount = query.Count()
            };
        }
    }

    /// <summary>
    /// 收藏职位相关
    /// </summary>
    public interface IJobFavoriteRepo : IRepository<Job_Favorite>
    {
        /// <summary>
        /// 判断职位是否被学生收藏
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="stuID"></param>
        /// <returns></returns>
        bool IsFavorite(int jobID, int stuID);

        /// <summary>
        ///  添加收藏
        /// </summary>
        /// <param name="jobID">职位ID</param>
        /// <param name="stuID">学生ID</param>
        /// <returns>1 成功 2 重复操作 0 失败</returns>
        OperationResult AddFavorite(int jobID, int stuID);

        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="jobID">职位ID</param>
        /// <param name="stuID">学生ID</param>
        /// <returns>1 成功  0 失败</returns>
        int RemoveFavorite(int jobID, int stuID);

        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="id">职位收藏记录ID</param>
        /// <returns>1 成功  0 失败</returns>
        int RemoveFavorite(int id);
        
        /// <summary>
        /// 获取学生收藏的所有职位
        /// </summary>
        /// <param name="stuID"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PagedResult<JobInFavorite> GetFavoriteJobs(int stuID, int pageNo, int pageSize);
    }
}
