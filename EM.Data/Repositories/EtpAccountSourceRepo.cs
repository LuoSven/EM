using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class EtpAccountSourceRepo : RepositoryBase<Etp_Account_Source>, IEtpAccountSourceRepo
    {
        public EtpAccountSourceRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IEtpAccountSourceRepo : IRepository<Etp_Account_Source>
    {
        
    }

}
