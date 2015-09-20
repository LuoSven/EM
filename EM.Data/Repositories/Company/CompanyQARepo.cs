using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc22Top.Data.ResultModel;
using Topuc.Framework.Logger;
using System.Diagnostics;

namespace Topuc22Top.Data.Repositories
{
    public class CompanyQARepo : RepositoryBase<TB_Enterprise_QA>, ICompanyQARepo
    {
        public CompanyQARepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<IList<TB_Enterprise_QA>> TakeEnterpriseQAListAsync(int enterpriseid, int? count = null)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (TopucDB context = new TopucDB())
            {
                var qalist = from q in context.TB_Enterprise_QA
                             where q.EnterpriseId == enterpriseid
                             select q;

                if (count.HasValue)
                {
                    AppLogger.Info("查询公司QA共花费：" + sw.ElapsedMilliseconds + "ms, 公司id:" + enterpriseid.ToString());
                    return await qalist.OrderByDescending(a => a.CreateTime).Take(count.Value).ToListAsync();
                }
                else
                {
                    AppLogger.Info("查询公司QA共花费：" + sw.ElapsedMilliseconds + "ms, 公司id:" + enterpriseid.ToString());
                    return await qalist.OrderByDescending(a => a.CreateTime).ToListAsync();
                }


            }
        }

        public IList<TB_Enterprise_QA> TakeEnterpriseQAList(int enterpriseid, int? count = null)
        {
            var qalist = from q in DataContext.TB_Enterprise_QA
                         where q.EnterpriseId == enterpriseid
                         select q;

            if (count.HasValue)
            {
                return qalist.OrderByDescending(a => a.CreateTime).Take(count.Value).ToList();
            }
            else
            {
                return qalist.OrderByDescending(a => a.CreateTime).ToList();
            }
        }

        public int GetQuestionCount(int enterpriseId, bool? isAnswered)
        {
            var query = from q in DataContext.TB_Enterprise_QA
                        where q.EnterpriseId == enterpriseId
                        select q;
            if (isAnswered.HasValue)
            {
                if (isAnswered.Value)
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.Answer));
                }
                else
                {
                    query = query.Where(x => string.IsNullOrEmpty(x.Answer));
                }
            }

            return query.Count();
        }

        public bool CheckQuestionExisted(string question, int enterpriseId, DateTime? startDate)
        {
            DateTime startDt = startDate ?? new DateTime().AddMonths(-2);
            var query = from q in DataContext.TB_Enterprise_QA
                        where q.EnterpriseId == enterpriseId && q.Question == question.Trim() && q.CreateTime >= startDt
                        select 1;
            return query.Any();
        }

        public IList<CompanyQAResult> GetUnReplyQAList(int page, int pagesize, out int totalCnt) 
        {
            var query = from qa in DataContext.TB_Enterprise_QA

                        join etp in DataContext.TB_Enterprise
                        on qa.EnterpriseId equals etp.EnterpriseId

                        join sAccount in DataContext.TB_S_Account
                        on qa.StuId equals sAccount.UserId into gSAccount

                        where (qa.Answer == null || qa.Answer == "")

                        select new CompanyQAResult {
                            QA = qa,
                            EnterpriseName = etp.Name,
                            EtpCertificateStatus = etp.CertificationStatus,
                            StuName = gSAccount.Select(p => p.UserName).FirstOrDefault()
                        };
            totalCnt = query.Count();
            return query.OrderByDescending(p => p.QA.CreateTime).Skip((page - 1) * pagesize).Take(pagesize).ToList();
        }

        public IList<CompanyQAResult> GetQAList(int enterpriseId, int page, int pagesize, out int totalCnt)
        {
            var query = from qa in DataContext.TB_Enterprise_QA

                        join sAccount in DataContext.TB_S_Account
                        on qa.StuId equals sAccount.UserId into gSAccount

                        where qa.EnterpriseId == enterpriseId && !string.IsNullOrEmpty(qa.Question)

                        select new CompanyQAResult
                        {
                            QA = qa,
                            StuName = gSAccount.Select(p => p.UserName).FirstOrDefault()
                        };
            totalCnt = query.Count();
            return query.OrderByDescending(p => (p.QA.Answer == null || p.QA.Answer == "")).ThenByDescending(p => p.QA.CreateTime).Skip((page - 1) * pagesize).Take(pagesize).ToList();
        }

    }
    public interface ICompanyQARepo : IRepository<TB_Enterprise_QA>
    {
        IList<TB_Enterprise_QA> TakeEnterpriseQAList(int enterpriseid, int? count = null);

        Task<IList<TB_Enterprise_QA>> TakeEnterpriseQAListAsync(int enterpriseid, int? count = null);

        int GetQuestionCount(int enterpriseId, bool? isAnswered);
        bool CheckQuestionExisted(string question, int enterpriseId, DateTime? startDate);

        /// <summary>
        /// Admin 未回复对话HR列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="totalCnt"></param>
        /// <returns></returns>
        IList<CompanyQAResult> GetUnReplyQAList(int page, int pagesize, out int totalCnt);
        /// <summary>
        /// 企业端QA列表 学生端对话HR
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="totalCnt"></param>
        /// <returns></returns>
        IList<CompanyQAResult> GetQAList(int enterpriseId, int page, int pagesize, out int totalCnt);
    }
}
