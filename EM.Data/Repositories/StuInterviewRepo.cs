using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class StuInterviewRepo : RepositoryBase<Stu_Interview>, IStuInterviewRepo
    {
        public StuInterviewRepo(IDatabaseFactory databaseFActory)
            : base(databaseFActory)
        {
        }
        public Stu_Interview GetByApplyId(int applyId)
        {
            var query = from m in DataContext.Stu_Interview
                        where m.ApplyId == applyId
                        select m;
            return query.FirstOrDefault();
        }
    }
    public interface IStuInterviewRepo : IRepository<Stu_Interview>
    {
        Stu_Interview GetByApplyId(int applyId);
    }
}
