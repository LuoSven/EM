using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;
using EM.Model.Entities;

namespace EM.Data.Repositories
{
    public class UserAccountRepo : RepositoryBase<EM_User_Account>, IUserAccountRepo
    {
        public UserAccountRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

    }
    public interface IUserAccountRepo : IRepository<EM_User_Account>
    {
    }
}
