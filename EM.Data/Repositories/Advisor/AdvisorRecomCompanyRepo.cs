using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc22Top.Data.ResultModel;

namespace Topuc22Top.Data.Repositories
{

    public class AdvisorRecomCompanyRepo : RepositoryBase<TB_Advisor_RecomCompany>, IAdvisorRecomCompanyRepo
    {
        public AdvisorRecomCompanyRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<AdvisorComResult> GetEnterpriseRecomAsync(int enterpriseid)
        {
            using (TopucDB context = new TopucDB())
            {
                var recomcompany = from q in context.TB_Advisor_RecomCompany
                                   join a in context.TB_Advisor on q.AdvisorId equals a.AdvisorId
                                   where q.CompanyId == enterpriseid
                                   select new AdvisorComResult
                                   {
                                       Advisorid = a.AdvisorId,
                                       AdvisorName = a.Name,
                                       RecomReason = q.RecomReason

                                   };
                return await recomcompany.FirstOrDefaultAsync();

            }

        }


    }
    public interface IAdvisorRecomCompanyRepo : IRepository<TB_Advisor_RecomCompany>
    {
        Task<AdvisorComResult> GetEnterpriseRecomAsync(int enterpriseid);

    }
}
