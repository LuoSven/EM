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
    public class StuEvaluationRepo : RepositoryBase<TB_Stu_Evaluation>, IStuEvaluationRepo
    {
        public StuEvaluationRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public TB_Stu_Evaluation GetByUserID(int userID)
        {
            var query = from m in DataContext.TB_Stu_Evaluation
                        where m.UserId == userID
                        select m;
            return query.FirstOrDefault();
        }


        public async Task<EvaluationDTO> GetByIdAsync(int userID)
        {
            string sql = string.Format(" select * from TB_Stu_Evaluation a    where a.Id= {0}  ", userID);
            var query = DataContext.Database.SqlQuery<EvaluationDTO>(sql);
            return query.FirstOrDefault();
        }


        public async Task<EvaluationDTO> GetByUserIDAsync(int userID)
        {
            string sql = string.Format(" select * from TB_Stu_Evaluation a    where a.UserId= {0}  ", userID);
            var query = DataContext.Database.SqlQuery<EvaluationDTO>(sql);
            return query.FirstOrDefault();
        }

        public async Task UpdateAsync(EvaluationDTO model)
        {
            var target = DataContext.TB_Stu_Evaluation.Where(o => o.UserId == model.UserId).FirstOrDefault();
            target.SelfEvaluation = model.SelfEvaluation;
            target.ModifyDate =DateTime.Now;
            DataContext.TB_Stu_Evaluation.Attach(target);
            DataContext.Entry(target).State = EntityState.Modified;
            DataContext.SaveChanges();
        }

        public async Task CreateAsync(EvaluationDTO model)
        {
            var target = new TB_Stu_Evaluation();
            target.SelfEvaluation = model.SelfEvaluation;
            target.ModifyDate = DateTime.Now;
            target.CreateDate = DateTime.Now;
            target.UserId = model.UserId;
            DataContext.TB_Stu_Evaluation.Add(target);
            DataContext.SaveChanges();
        }


        public async Task DeleteAsync(int id)
        {
            string sql = string.Format("delete from TB_Stu_Evaluation where Id='{0}'", id);
            await DataContext.Database.ExecuteSqlCommandAsync(sql);
        }
    }
    public interface IStuEvaluationRepo : IRepository<TB_Stu_Evaluation>
    {
        /// <summary>
        /// 通过ID获取自我评价
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<EvaluationDTO> GetByIdAsync(int id);


        /// <summary>
        /// 通过UserID获取自我评价
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        TB_Stu_Evaluation GetByUserID(int userID);


       /// <summary>
       /// 更新自我评价
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
        Task UpdateAsync(EvaluationDTO model);

        /// <summary>
        /// 创建自我评价
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task CreateAsync(EvaluationDTO model);


       /// <summary>
       /// 删除自我评价
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        Task DeleteAsync(int id);


        Task<EvaluationDTO> GetByUserIDAsync(int userID);
    }
}
