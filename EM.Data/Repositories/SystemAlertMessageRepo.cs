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


        public  List<SystemAlertMessageDTO> GetAlertMessages(int userId)
        {
            var list =  DapperHelper.SqlQuery<SystemAlertMessageDTO>(@"select a.Id,a.MessageType, a.Message,a.CreateTime,case when c.RoleType=@admin then '系统管理员' else b.UserName end  as	Sender  from EM_System_AlertMessage a 
join EM_User_Account b on a.Sender=b.UserId
join EM_User_Role c on b.RoleId=c.Id
where a.Receiver=@userId and a.AlertedTime is null", new { userId, admin = (int)RoleType.Admin }).ToList();
            if(list!=null&&list.Count>0)
            {
                var ids = string.Join(",", list.Select(o => o.Id.ToString()).ToList());
                var updateResult = DapperHelper.SqlExecute(string.Format("update EM_System_AlertMessage set AlertedTime=@Now where Id In ({0}) ", ids), new { Now = DateTime.Now });
            }
            return list;
        }

        public PagedResult<EM_System_AlertMessage> GetPagedList(SystemAlertMessageSM sm, int Page, int PageSize)
        {
            var sql = @"select  * from EM_System_AlertMessage 
where 1=1   ";
            if (sm.Sender.HasValue)
                sql += " and a.Sender=@Sender ";
            if (sm.Receiver.HasValue)
                sql += " and a.Receiver=@Receiver ";
            if (sm.MessageType.HasValue)
                sql += " and a.MessageType=@MessageType ";

            if (!string.IsNullOrWhiteSpace(sm.Message))
                sql += " and a.Message like '%'+ @Message +'%' ";
            if(sm.AlertedStstus.HasValue)
            {
                if(sm.AlertedStstus.Value==(int)AlertedStstusType.Alerted)
                {
                    sql += " and a.AlertedTime  is not null ";
                }
                else if (sm.AlertedStstus.Value == (int)AlertedStstusType.Waiting)
                {
                    sql += " and a.AlertedTime  is  null ";
                }
            }

            if (sm.DateTimeType.HasValue)
            {
                var dateField = "CreateTime";
                var systemMessageDateTimeType=(SystemMessageDateTimeType)Enum.ToObject(typeof(SystemMessageDateTimeType), sm.DateTimeType.Value);
                switch (systemMessageDateTimeType)
                {
                    case SystemMessageDateTimeType.AlertDate:
                        dateField = "AlertTime";
                        break;
                    case SystemMessageDateTimeType.AlertedDate:
                        dateField = "AlertedTime";
                        break; 
                }
                if (sm.SDate.HasValue)
                    sql += " and a."+dateField+">=@SDate ";
                if (sm.EDate.HasValue)
                    sql += " and a." + dateField + " <=@EDate ";
            }



            var list = DapperHelper.QueryWithPage<EM_System_AlertMessage>(sql, sm, " CreateTime desc  ", Page, PageSize);
            return list;
        }

    }


    public interface ISystemAlertMessageRepo : IRepository<EM_System_AlertMessage>
    {
        List<SystemAlertMessageDTO> GetAlertMessages(int userId);

        PagedResult<EM_System_AlertMessage> GetPagedList(SystemAlertMessageSM sm, int Page, int PageSize);
    }
}
