using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;

namespace Topuc22Top.Data.Repositories
{
    public class CompanyAccountRepo : RepositoryBase<TB_Enterprise_Account>, ICompanyAccountRepo
    {
        public CompanyAccountRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public bool HasAccount(int enterpriseId)
        {
            return DataContext.TB_Enterprise_Account.Where(m => m.EnterpriseId == enterpriseId).Any();
        }

        public async Task<bool> HasAccountAsync(int enterpriseId)
        {
            using (TopucDB DataContext =new TopucDB())
            {
                return await DataContext.TB_Enterprise_Account.AnyAsync(m => m.EnterpriseId == enterpriseId);
            }
        }

        public bool IsActiveEtp(int enterpriseId)
        {
            return DataContext.TB_Enterprise_Account.Where(m => m.EnterpriseId == enterpriseId).Any();
        }

        /// <summary>
        /// 判断待审核的账号和审核已通过的账号是否存在相同邮件地址的企业
        /// </summary>
        public bool IsEmailExist(string email)
        {
            var query = from e in DataContext.TB_Enterprise
                        join a in DataContext.TB_Enterprise_Account
                        on e.EnterpriseId equals a.EnterpriseId
                        where a.LoginEmail == email &&
                        (e.ProcessStatus == (int)EtpProcessStatus.AccountApproved
                        || e.ProcessStatus == (int)EtpProcessStatus.AccountGenerated)
                        select e;
            return query.Any();
        }

        public string GetLoginEmail(int enterpriserId)
        {
            var query = from m in DataContext.TB_Enterprise_Account
                        where m.EnterpriseId == enterpriserId
                        select m.LoginEmail;
            return query.FirstOrDefault();
        }

        public bool IsAccountNameExist(string accountName) 
        {
            return (from entity in DataContext.TB_Enterprise_Account where entity.UserName == accountName select 1).Any();
        }

        public bool IsAccountIdExist(int accountId) 
        {
            return (from entity in DataContext.TB_Enterprise_Account where entity.AccountId == accountId select 1).Any();
        }

        public int GetEnterpriseIdByAccountId(int accountId)
        {
            return (from entity in DataContext.TB_Enterprise_Account where entity.AccountId == accountId select entity.EnterpriseId).FirstOrDefault();
        }
        public int GetAccountIdByEnterpriseId(int enterpriseId) 
        {
            return (from entity in DataContext.TB_Enterprise_Account where entity.EnterpriseId == enterpriseId select entity.AccountId).FirstOrDefault();
        }

        public TB_Enterprise_Account GetAccountByEnterpriseId(int enterpriseId) 
        {
            return (from entity in DataContext.TB_Enterprise_Account where entity.EnterpriseId == enterpriseId select entity).FirstOrDefault();
        }

        public string GetAccountNameByAccountId(int accountId) 
        {
            return (from entity in DataContext.TB_Enterprise_Account where entity.AccountId == accountId select entity.UserName).FirstOrDefault();
        }

        public string GetEmailByAccountId(int accountId) 
        {
            return (from entity in DataContext.TB_Enterprise_Account where entity.AccountId == accountId select entity.LoginEmail).FirstOrDefault();
        }

        public TB_Enterprise_Account GetByAccountId(int accountId) 
        {
            return (from entity in DataContext.TB_Enterprise_Account where entity.AccountId == accountId select entity).FirstOrDefault();
        }

        public TB_Enterprise_Account GetByAccountName(string accountName) 
        {
            return (from entity in DataContext.TB_Enterprise_Account where entity.UserName == accountName select entity).FirstOrDefault();
        }

        public string GetLoginEmailByAccountId(int accountId) 
        {
            return (from entity in DataContext.TB_Enterprise_Account where entity.AccountId == accountId select entity.LoginEmail).FirstOrDefault();
        }

    }
    public interface ICompanyAccountRepo : IRepository<TB_Enterprise_Account>
    {
        /// <summary>
        /// 判断企业是否激活
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        bool HasAccount(int enterpriseId);
        Task<bool> HasAccountAsync(int enterpriseId);
        /// <summary>
        /// 返回是否是激活企业
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        bool IsActiveEtp(int enterpriseId);

        bool IsEmailExist(string email);
        /// <summary>
        /// 获取企业登陆邮箱
        /// </summary>
        /// <param name="enterpriserId"></param>
        /// <returns></returns>
        string GetLoginEmail(int enterpriserId);

        bool IsAccountNameExist(string accountName);
        bool IsAccountIdExist(int accountId);
        int GetEnterpriseIdByAccountId(int accountId);
        int GetAccountIdByEnterpriseId(int enterpriseId);
        TB_Enterprise_Account GetAccountByEnterpriseId(int enterpriseId);
        string GetAccountNameByAccountId(int accountId);
        string GetEmailByAccountId(int accountId);
        TB_Enterprise_Account GetByAccountId(int accountId);
        TB_Enterprise_Account GetByAccountName(string accountName);
        string GetLoginEmailByAccountId(int accountId);
    }
}
