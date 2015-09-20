using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc22Top.Data.Dapper;

using Dapper;

namespace Topuc22Top.Data.Repositories
{
    public class SEOPositionKeyRepo : RepositoryBase<TB_SEO_PositionKey>, ISEOPositionKeyRepo
    {
        public SEOPositionKeyRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<IList<TB_SEO_PositionKey>> GetSimilarFunctionsAsync(string key, int? count)
        {
            using (TopucDB context = new TopucDB())
            {
                var query = from m in context.TB_SEO_PositionKey
                            where m.Position.Contains(key)
                            select m;
                if (count.HasValue)
                {
                    return await query.Take(count.Value).ToListAsync();
                }

                return await query.ToListAsync();
            }
        }

        public async Task<IList<TB_SEO_PositionKey>> GetRelatedFunctionsAsync(int keyId)
        {
            using (TopucDB context = new TopucDB())
            {
                if (keyId != 0)
                {
                    var query = from m in context.TB_SEO_PositionKey
                                where m.Id >= (keyId - 6) && m.Id <= (keyId + 6) && m.Id != keyId
                                select m;

                    return await query.ToListAsync();
                }
                else
                {
                    return new List<TB_SEO_PositionKey>();
                }
            }
        }


        public IList<TB_SEO_PositionKey> GetSimilarFunctions(string key, int? count)
        {
            using (TopucDB context = new TopucDB())
            {
                var query = from m in context.TB_SEO_PositionKey
                            where m.Position.Contains(key)
                            select m;
                if (count.HasValue)
                {
                    return query.Take(count.Value).ToList();
                }

                return query.ToList();
            }
        }

        public IList<TB_SEO_PositionKey> GetRelatedFunctions(int keyId)
        {
            using (TopucDB context = new TopucDB())
            {
                if (keyId != 0)
                {
                    var query = from m in context.TB_SEO_PositionKey
                                where m.Id >= (keyId - 6) && m.Id <= (keyId + 6) && m.Id != keyId
                                select m;

                    return query.ToList();
                }
                else
                {
                    return new List<TB_SEO_PositionKey>();
                }
            }
        }

        public TB_SEO_PositionKey GetPositonInfoById(int id)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"select * from TB_SEO_PositionKey where Id = {0}", id);
                var list = conn.Query<TB_SEO_PositionKey>(sql);
                return list.FirstOrDefault();
            }
        }

    }
    public interface ISEOPositionKeyRepo : IRepository<TB_SEO_PositionKey>
    {
        Task<IList<TB_SEO_PositionKey>> GetSimilarFunctionsAsync(string key, int? count);
        Task<IList<TB_SEO_PositionKey>> GetRelatedFunctionsAsync(int keyId);


        IList<TB_SEO_PositionKey> GetSimilarFunctions(string key, int? count);
        IList<TB_SEO_PositionKey> GetRelatedFunctions(int keyId);

        TB_SEO_PositionKey GetPositonInfoById(int id);

    }
}
