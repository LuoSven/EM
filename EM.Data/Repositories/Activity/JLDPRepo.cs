using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class JLDPRepo : RepositoryBase<TB_Activity_JLDP>, IJLDPRepo
    {
        public JLDPRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

    }
    public interface IJLDPRepo : IRepository<TB_Activity_JLDP>
    {
    }
}
