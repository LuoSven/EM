using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc22Top.Common;

namespace Topuc22Top.Data.Repositories
{
    public class CompanyBusinessRepo : RepositoryBase<TB_Enterprise_Business>, ICompanyBusinessRepo
    {
        public CompanyBusinessRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<TB_Enterprise_Business> GetFirstBusinessAsync(int enterpriseid)
        {
            using (TopucDB context = new TopucDB())
            {
                var bussiness = from b in context.TB_Enterprise_Business
                                where b.EnterpriseId == enterpriseid
                                select b;
                return await bussiness.FirstOrDefaultAsync();

            }
        }

        public async Task<IList<TB_Enterprise_Business>> GetBusinessAsync(int enterpriseid, int? count = null)
        {
            using (TopucDB context = new TopucDB())
            {
                var bussiness = from b in context.TB_Enterprise_Business
                                where b.EnterpriseId == enterpriseid
                                select b;
                if (count.HasValue)
                {
                    return await bussiness.OrderByDescending(a => a.CreateTime).Take(count.Value).ToListAsync();
                }
                else
                {

                    return await bussiness.OrderByDescending(a => a.CreateTime).ToListAsync();
                }

            }
        }





        public IList<TB_Enterprise_Business> GetList(int enterpriseid, int? count = null)
        {
            var bussiness = from b in DataContext.TB_Enterprise_Business
                            where b.EnterpriseId == enterpriseid
                            select b;
            if (count.HasValue)
            {
                return bussiness.OrderByDescending(a => a.CreateTime).Take(count.Value).ToList();
            }
            else
            {

                return bussiness.OrderByDescending(a => a.CreateTime).ToList();
            }
        }
        public bool CheckBusinessTitleUseable(int enterpriseid, string title)
        {
            var bussiness = from b in DataContext.TB_Enterprise_Business
                            where b.EnterpriseId == enterpriseid && b.Title == title
                            select 1;
            return !bussiness.Any();
        }
    }
    public interface ICompanyBusinessRepo : IRepository<TB_Enterprise_Business>
    {
        Task<TB_Enterprise_Business> GetFirstBusinessAsync(int enterpriseid);
        Task<IList<TB_Enterprise_Business>> GetBusinessAsync(int enterpriseid, int? count = null);

        IList<TB_Enterprise_Business> GetList(int enterpriseid, int? count = null);
        bool CheckBusinessTitleUseable(int enterpriseid, string title);
    }
}
