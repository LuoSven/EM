using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class AppreciationEtpApplyRepo : RepositoryBase<Appreciation_EtpApply>, IAppreciationEtpApplyRepo
    {
        public AppreciationEtpApplyRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
    public interface IAppreciationEtpApplyRepo : IRepository<Appreciation_EtpApply>
    {

    }
}
