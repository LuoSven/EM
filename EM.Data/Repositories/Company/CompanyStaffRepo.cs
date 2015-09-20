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


    public class CompanyStaffRepo : RepositoryBase<TB_Enterprise_StaffGrowth>, ICompanyStaffRepo
    {
        public CompanyStaffRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<TB_Enterprise_StaffGrowth> GetFirstStaffGrowthAsync(int enterpriseid)
        {
            using (TopucDB context = new TopucDB())
            {
                return await context.TB_Enterprise_StaffGrowth.Where(c => c.EnterpriseId == enterpriseid).FirstOrDefaultAsync();
            }
        }

        public async Task<IList<TB_Enterprise_StaffGrowth>> GetStaffGrowthAsync(int enterpriseid)
        {
            using (TopucDB DataContext = new TopucDB())
            {
                return await DataContext.TB_Enterprise_StaffGrowth.Where(c => c.EnterpriseId == enterpriseid).ToListAsync();
            }
        }


        public IList<TB_Enterprise_StaffGrowth> GetStaffList(int enterpriseid)
        {
            return DataContext.TB_Enterprise_StaffGrowth.OrderByDescending(c => c.CreateTime).Where(c => c.EnterpriseId == enterpriseid).ToList();
        }

        public async Task<IList<TB_Enterprise_StaffGrowth>> GetStaffListAsync(int enterpriseid)
        {
            using (TopucDB DataContext = new TopucDB())
            {
                return await DataContext.TB_Enterprise_StaffGrowth.Where(c => c.EnterpriseId == enterpriseid).OrderByDescending(c => c.CreateTime).ToListAsync();
            }
        }
    }
    public interface ICompanyStaffRepo : IRepository<TB_Enterprise_StaffGrowth>
    {

        IList<TB_Enterprise_StaffGrowth> GetStaffList(int enterpriseid);
        Task<IList<TB_Enterprise_StaffGrowth>> GetStaffListAsync(int enterpriseid);

        Task<TB_Enterprise_StaffGrowth> GetFirstStaffGrowthAsync(int enterpriseid);
        Task<IList<TB_Enterprise_StaffGrowth>> GetStaffGrowthAsync(int enterpriseid);
    }
}
