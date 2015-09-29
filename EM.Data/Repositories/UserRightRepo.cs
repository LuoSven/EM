using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;
using EM.Model.Entities;
using EM.Data.Dapper;

namespace EM.Data.Repositories
{
    public class UserRightRepo : RepositoryBase<EM_User_Right>, IUserRightRepo
    {
        public UserRightRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public bool HasRight(int UserId,string ControllerName, string ActionName)
        {
            var result = DapperHelper.SqlQuery<EM_User_Right>(@"select * from EM_User_Right a 
join EM_System_Program b on a.ProgramId =b.Id 
where b.ActionName=@ActionName and b.ControllerName =@ControllerName and a.UserId=@UserId", new { ActionName = ActionName.ToLower(), ControllerName = ControllerName.ToLower(), UserId=UserId });
            return result == null ? false : true;
        }

    }
    public interface IUserRightRepo : IRepository<EM_User_Right>
    {

        bool HasRight(int UserId,string ControllerName, string ActionName);
    }
}
