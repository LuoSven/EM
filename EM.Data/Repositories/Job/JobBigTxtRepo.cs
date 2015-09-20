using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class JobBigTxtRepo : RepositoryBase<TB_Position_BigTxt>, IJobBigTxtRepo
    {
        public JobBigTxtRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public string GetPositionDesc(int posid)
        {
            var context = ObjectContextHelper.TopUObjectContext;
            return context.TB_Position_BigTxt.Where(a => a.PositionId == posid).Select(a => a.PosDescription).FirstOrDefault() ?? "";
        }

    }
    public interface IJobBigTxtRepo : IRepository<TB_Position_BigTxt>
    {
        string GetPositionDesc(int posid);
    }
}
