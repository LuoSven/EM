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
using  Topuc22Top.Model.VMs;
using Topuc22Top.Model.ExtendEntities;

namespace Topuc22Top.Data.Repositories
{
    public class StuForwardResumeEmailRecordRepo : RepositoryBase<TB_Stu_ForwardResume_Email_Record>, IStuForwardResumeEmailRecordRepo
    {
        public StuForwardResumeEmailRecordRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }


      public   List<StuForwordResumeRecordDTO> GetListByUserId(int UserId)
        {
            return Dapper.DapperHelper.SqlQuery22<StuForwordResumeRecordDTO>(@"
select  a.SendTo,a.SendSubject,a.SendTime from TB_Stu_ForwardResume_Email_Record a 
where UserId=@UserId and a.SendTime>=@SendTime
order by a.SendTime desc ", new { UserId = UserId, SendTime = DateTime.Now.AddMonths(-1) }).ToList();
        }

        public     bool Add(StuForwordResumeRecordDTO record)
      {
          var result = Dapper.DapperHelper.SqlExecute22("insert into TB_Stu_ForwardResume_Email_Record values(@UserId,@SendTo,@SendSubject,@SendBody,@SendTime)", record);
          return result > 0 ? true : false;
        }


    }
    public interface IStuForwardResumeEmailRecordRepo : IRepository<TB_Stu_ForwardResume_Email_Record>
    {
        List<StuForwordResumeRecordDTO> GetListByUserId(int UserId);

        bool Add(StuForwordResumeRecordDTO record);

    }
}
