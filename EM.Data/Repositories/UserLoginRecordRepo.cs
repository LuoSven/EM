using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;
using EM.Model.Entities;
using EM.Model.VMs;
using EM.Utils;
using EM.Common;
using EM.Model.DTOs;
using EM.Data.Dapper;
using EM.Model.SMs;

namespace EM.Data.Repositories
{
    public class UserLoginRecordRepo : RepositoryBase<EM_User_Login_Record>, IUserLoginRecordRepo
    {
        public UserLoginRecordRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public PagedResult<UserLoginRecordDTO> GetList(LoginManageSM sm, int Page, int PageSize)
        {
            var sql=@" select a.id,a.UserId,b.UserName,a.LoginInfo,a.LoginTime,a.IsLogin,a.ErrorInfo from EM_User_Login_Record a
left join EM_User_Account b on a.UserId=b.UserId 
where 1=1 ";
            if (sm.UserId.HasValue)
            {
                sql += " and a.UserId=@UserId ";
            } 
            if (sm.SDate.HasValue)
            {
                sql += " and a.LoginTime >=@SDate ";
            }
            if (sm.EDate.HasValue)
            {
                sql += " and a.LoginTime <=@EDate  ";
            }

            var list = DapperHelper.QueryWithPage<UserLoginRecordDTO>(sql, sm, " id desc ", Page, PageSize);
            return list;
        }
}


    public interface IUserLoginRecordRepo : IRepository<EM_User_Login_Record>
    {
        /// <summary>
        /// 获取登陆日志
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        PagedResult<UserLoginRecordDTO> GetList(LoginManageSM sm, int Page, int PageSize);
    }
}
