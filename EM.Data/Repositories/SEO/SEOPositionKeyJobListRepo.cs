using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;

namespace Topuc22Top.Data.Repositories
{
    public class SEOPositionKeyJobListRepo : RepositoryBase<TB_SEO_PositionKey_JobList>, ISEOPositionKeyJobListRepo
    {
        public SEOPositionKeyJobListRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<IList<TB_SEO_PositionKey_JobList>> GetListByKeyIdAsync(int keyId)
        {
            using (TopucDB context = new TopucDB())
            {
                var query = from m in context.TB_SEO_PositionKey_JobList
                            where m.KeyId == keyId
                            select m;
                return await query.ToListAsync();
            }
        }
    }
    public interface ISEOPositionKeyJobListRepo : IRepository<TB_SEO_PositionKey_JobList>
    {
        Task<IList<TB_SEO_PositionKey_JobList>> GetListByKeyIdAsync(int keyId);
    }
}
