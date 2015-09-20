using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class StuNetworkLinkRepo : RepositoryBase<TB_Stu_NetworkLink>, IStuNetworkLinkRepo
    {
        public StuNetworkLinkRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }


        public TB_Stu_NetworkLink GetByUserId(int userId)
        {
            var query = from m in DataContext.TB_Stu_NetworkLink
                        where m.UserId == userId
                        select m;
            return query.FirstOrDefault();
        }
    }

    public interface IStuNetworkLinkRepo : IRepository<TB_Stu_NetworkLink>
    {
        TB_Stu_NetworkLink GetByUserId(int userId);
    }
}
