using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    class AdvisorRepo : RepositoryBase<TB_Advisor>, IAdvisorRepo
    {
        public AdvisorRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<int> GetMinStuCountAdvisor(string city = "")
        {
            int advisorId = 0;

            if (!string.IsNullOrEmpty(city))
            {
                advisorId = (from a in DataContext.TB_Advisor
                             join b in DataContext.TB_Advisor_Stu on a.AdvisorId equals b.AdvisorId into g
                             where ("," + a.City + ",").Contains("," + city + ",")
                             orderby g.Count()
                             select a.AdvisorId).FirstOrDefault();
            }

            if (advisorId > 0)
            {
                return advisorId;
            }
            else
            {

                advisorId = (from a in DataContext.TB_Advisor
                             join b in DataContext.TB_Advisor_Stu on a.AdvisorId equals b.AdvisorId into g
                             orderby g.Count()
                             select a.AdvisorId).FirstOrDefault();
                return advisorId;
            }

        }
    }

    public interface IAdvisorRepo : IRepository<TB_Advisor>
    {
        Task<int> GetMinStuCountAdvisor(string city = "");
    }

}
