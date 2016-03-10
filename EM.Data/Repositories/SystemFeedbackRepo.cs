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
    public class SystemFeedbackRepo : RepositoryBase<EM_System_Feedback>, ISystemFeedbackRepo
    {
        public SystemFeedbackRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }


        public List<EM_System_Feedback> GetFeedbacks(int userId)
        {
            var list = Dapper.DapperHelper.SqlQuery<EM_System_Feedback>("select * from EM_System_Feedback where Creater =@userId  ", new { userId }).ToList();
            return list;
        }

        public PagedResult<SystemFeedbackDTO> GetList(SystemFeedbackSM sm, int Page, int PageSize)
        {
            var sql=@"select a.*,b.UserName,c.Description from EM_System_Feedback a
join EM_User_Account b on b.UserId=a.Creater
join EM_User_Role c on b.RoleId=c.Id
where 1=1   ";
                if(sm.UserId.HasValue)
                    sql += " and a.Creater=@Creater ";
                if (sm.IsRepert.HasValue)
                {
                    if(sm.IsRepert.Value==1)
                    {
                        sql += " and (a.ReplyDate is not null or a.ReplyMessage<>'')  ";
                    }
                    else
                    {
                        sql += " and (a.ReplyDate is null or a.ReplyMessage='')  ";
                    }
                }
                if(sm.SDate.HasValue)
                 sql+=" and a.CreateDate >=@SDate ";
                if(sm.EDate.HasValue)
                 sql+=" and a.CreateDate <=@EDate ";
                var list = DapperHelper.QueryWithPage<SystemFeedbackDTO>(sql, sm, " CreateDate desc  ", Page, PageSize);
            return list;
        }

    }


    public interface ISystemFeedbackRepo : IRepository<EM_System_Feedback>
    {
        List<EM_System_Feedback> GetFeedbacks(int userId);


        PagedResult<SystemFeedbackDTO> GetList(SystemFeedbackSM sm,int Page,int PageSize);
    }
}
