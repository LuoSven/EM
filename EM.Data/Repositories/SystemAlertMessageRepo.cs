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
    /// <summary>
    ///   
    /// </summary>
    public class SystemAlertMessageRepo : RepositoryBase<EM_System_AlertMessage>, ISystemAlertMessageRepo
    {
        public SystemAlertMessageRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }


        public async Task<List<SystemAlertMessageDTO>> GetAlertMessages(int userId)
        {
            var list = (await DapperHelper.SqlQueryAsync<SystemAlertMessageDTO>(@"select a.Id,a.MessagType, a.Message,a.CreateTime,case when c.RoleType=@admin then '系统管理员' else b.UserName end  as	Sender  from EM_System_AlertMessage a 
join EM_User_Account b on a.Sender=b.UserId
join EM_User_Role c on b.RoleId=c.Id
where a.Receiver=@userId and a.AlertedTime is null", new { userId, admin = (int)RoleType.Admin })).ToList();
            if(list!=null&&list.Count>0)
            {
                var ids = string.Join(",", list.Select(o => o.Id.ToString()).ToList());
                var updateResult =await DapperHelper.SqlExecuteAsync(string.Format("update EM_System_AlertMessage set AlertedTime=@Now where Id In ({0}) ", ids), new { Now = DateTime.Now });
            }
            return list;
        }

    }


    public interface ISystemAlertMessageRepo : IRepository<EM_System_AlertMessage>
    {
        Task<List<SystemAlertMessageDTO>> GetAlertMessages(int userId);
    }
}
