using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    class AdvisorStuRelationRepo : RepositoryBase<TB_Advisor_Stu>, IAdvisorStuRelationRepo
    {
        public AdvisorStuRelationRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task CreateAsync(int advisroId, int userId)
        {
            TB_Advisor_Stu advisor = new TB_Advisor_Stu();
            advisor.UserId = userId;
            advisor.AdvisorId = advisroId;
            advisor.CreateTime = DateTime.Now;
            advisor.IsApply = false;
            advisor.IsInterview = false;
            advisor.IsResume = false;
   
            DataContext.TB_Advisor_Stu.Add(advisor);
            DataContext.SaveChanges();
        }

    }

    public interface IAdvisorStuRelationRepo : IRepository<TB_Advisor_Stu>
    {

        Task CreateAsync(int advisroId, int userId);
    }

}
