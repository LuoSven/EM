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

    public class CompanyNewsRepo : RepositoryBase<TB_Enterprise_News>, ICompanyNewsRepo
    {
        public CompanyNewsRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<IList<TB_Enterprise_News>> GetEnterpriseNewsAsync(int enterpriseid, int? count = null)
        {
            using (TopucDB context = new TopucDB())
            {
                var news = from q in context.TB_Enterprise_News
                           where q.EnterpriseId == enterpriseid
                           select q;

                if (count.HasValue)
                {
                    return await news.OrderByDescending(a => a.CreateTime).Take(count.Value).ToListAsync();
                }
                else
                {
                    return await news.OrderByDescending(a => a.CreateTime).ToListAsync();
                }
            }
        }

        public IList<TB_Enterprise_News> GetList(int enterpriseid, int? count = null)
        {
            using (TopucDB context = new TopucDB())
            {
                var news = from q in DataContext.TB_Enterprise_News
                           where q.EnterpriseId == enterpriseid
                           select q;

                if (count.HasValue)
                {
                    return news.OrderByDescending(a => a.CreateTime).Take(count.Value).ToList();
                }
                else
                {
                    return news.OrderByDescending(a => a.CreateTime).ToList();
                }
            }
        }



    }
    public interface ICompanyNewsRepo : IRepository<TB_Enterprise_News>
    {

        Task<IList<TB_Enterprise_News>> GetEnterpriseNewsAsync(int enterpriseid, int? count = null);

        IList<TB_Enterprise_News> GetList(int enterpriseid, int? count = null);
    }
}
