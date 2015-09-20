using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class PositionLinkRepo : RepositoryBase<TB_Position_Link>, IPositionLinkRepo
    {
        public PositionLinkRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public  IList<TB_Position_Link> GetPositionLinks(int posId)
        {

            return DataContext.TB_Position_Link.Where(l => l.PositionId == posId).ToList();
        }
    }

    public interface IPositionLinkRepo : IRepository<TB_Position_Link>
    {
        /// <summary>
        /// 获取特定职位的内链对象列表
        /// </summary>
        /// <param name="posId">职位ID</param>
        /// <returns>职位的内链对象列表（包含关键词和URL）</returns>
        IList<TB_Position_Link> GetPositionLinks(int posId);
    }
}
