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


    public class CompanySloganRepo : RepositoryBase<TB_Enterprise_Slogan>, ICompanySloganRepo
    {
        public CompanySloganRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<TB_Enterprise_Slogan> GetEnterpriseSloganAsync(int enterpriseid)
        {
            using (TopucDB context = new TopucDB())
            {
                return await context.TB_Enterprise_Slogan.Where(c => c.EnterpriseId == enterpriseid).FirstOrDefaultAsync();
            }

        }

        public async Task<string> GetEnterpriseSloganContentAsync(int enterpriseid)
        {
            using (TopucDB context = new TopucDB())
            {
                var slogan= await context.TB_Enterprise_Slogan.Where(c => c.EnterpriseId == enterpriseid).FirstOrDefaultAsync();
                if (slogan != null)
                {
                    return slogan.Content;
                }
                return "";
            }

        }
        public TB_Enterprise_Slogan GetEnterpriseSlogan(int enterpriseid)
        {
            return DataContext.TB_Enterprise_Slogan.Where(c => c.EnterpriseId == enterpriseid).FirstOrDefault();
        }
    }
    public interface ICompanySloganRepo : IRepository<TB_Enterprise_Slogan>
    {
        Task<TB_Enterprise_Slogan> GetEnterpriseSloganAsync(int enterpriseid);

        Task<string> GetEnterpriseSloganContentAsync(int enterpriseid);

        TB_Enterprise_Slogan GetEnterpriseSlogan(int enterpriseid);
    }
}
