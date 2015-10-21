using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;
using EM.Model.Entities;
using EM.Data.Dapper;
using EM.Model;

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
            var result = DapperHelper.SqlQuery<EM_User_Right>(@"select * from EM_User_Account a 
join EM_User_Role b on a.RoleId=b.id
join EM_User_Right c on c.RoleId=b.id
join EM_System_Program d on d.Id=c.ProgramId and d.ActionName=@ActionName and d.ControllerName=@ControllerName and c.Permit=1
where a.UserId=@UserId", new { ActionName = ActionName.ToLower(), ControllerName = ControllerName.ToLower(), UserId = UserId });
            return result == null ? false : true;
        }

      public   List<string> GetActions(int UserId, string ControllerName)
        {
            var result = DapperHelper.SqlQuery<string>(@"select d.ActionName from EM_User_Account a 
join EM_User_Role b on a.RoleId=b.id
join EM_User_Right c on c.RoleId=b.id
join EM_System_Program d on d.Id=c.ProgramId  and d.ControllerName=@ControllerName and c.Permit=1
where a.UserId=@UserId", new { UserId = UserId, ControllerName = ControllerName }).Select(o=>o.ToLower()).ToList();
            return result;
        }

    }
    public interface IUserRightRepo : IRepository<EM_User_Right>
    {

        bool HasRight(int UserId,string ControllerName, string ActionName);

        List<string> GetActions(int UserId, string ControllerName);


    }
}
