using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using System.Diagnostics;
using Topuc.Framework.Logger;
namespace Topuc22Top.Data.Repositories
{
    public class JobDetailRepo : RepositoryBase<TB_Position_Detail>, IJobDetailRepo
    {
        public JobDetailRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public async Task<bool> HasApplyEmailAsync(int enterpriseId, int positionId)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (TopucDB DataContext = new TopucDB())
            {
                bool result = false;
                if (await DataContext.TB_Position_Detail.AnyAsync(p => p.PositionId == positionId && !string.IsNullOrEmpty(p.ContactEmail)))
                {
                    result = true;

                    AppLogger.Info("查询公司和职位信息共花费：" + sw.ElapsedMilliseconds + "ms, 公司id:" + enterpriseId.ToString()+"职位Id"+positionId.ToString());
                }
                else
                {
                    if (await DataContext.TB_Enterprise_Contact.AnyAsync(p => p.EnterpriseId == enterpriseId && !string.IsNullOrEmpty(p.ContactEmail)))
                    {
                        result = true;
                    }
                    else
                    {
                        //如果企业已激活且明确标明不想接收申请邮件提醒，则返回空
                        if (await DataContext.TB_Enterprise_Account.AnyAsync(m => m.EnterpriseId == enterpriseId && m.AccountStatus == (int)EtpAccountStatus.Approved && m.AcceptResumeEDM.HasValue && m.AcceptResumeEDM.Value))
                        {
                            result = true;
                        }
                    }
                }
                return result;
            }
        }

        public bool HasApplyEmail(int enterpriseId, int positionId)
        {
            bool result = false;
                if (DataContext.TB_Position_Detail.Any(p => p.PositionId == positionId && !string.IsNullOrEmpty(p.ContactEmail)))
                {
                    result = true;
                }
                else
                {
                    if (DataContext.TB_Enterprise_Contact.Any(p => p.EnterpriseId == enterpriseId && !string.IsNullOrEmpty(p.ContactEmail)))
                    {
                        result = true;
                    }
                    else
                    {
                        //如果企业已激活且明确标明不想接收申请邮件提醒，则返回空
                        if (DataContext.TB_Enterprise_Account.Any(m => m.EnterpriseId == enterpriseId && m.AccountStatus == (int)EtpAccountStatus.Approved && m.AcceptResumeEDM.HasValue && m.AcceptResumeEDM.Value))
                        {
                            result = true;
                        }
                    }
                }
                return result;
         }
    }

    public interface IJobDetailRepo : IRepository<TB_Position_Detail>
    {
        /// <summary>
        /// 是否有职位申请邮箱
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        Task<bool> HasApplyEmailAsync(int enterpriseId, int positionId);

        bool HasApplyEmail(int enterpriseId, int positionId);
    }
}
