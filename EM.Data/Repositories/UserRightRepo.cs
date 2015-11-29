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
      public  void UpdateUserRoleRight(int RoleId, string ProgramIds)
      {
          //补全全部权限信息

          #region 补全权限信息
          var userRight = new EM_User_Right();
          userRight.RoleId = RoleId;
          userRight.CreateTime = DateTime.Now;
          userRight.ModifeTime = DateTime.Now;
          userRight.Permit = false;
          var programIds = DapperHelper.SqlQuery<int>(@"  select Id from EM_System_Program 
where Id not in (select ProgramId from EM_User_Right where RoleId=@RoleId )", new { RoleId = RoleId }).ToList();
          foreach (var item in programIds)
          {
              userRight.ProgramId = item;
              DapperHelper.SqlExecute(@"INSERT INTO EM_User_Right
           (RoleId
           ,ProgramId
           ,Permit
           ,ModifeTime
           ,CreateTime)
     VALUES
           (@RoleId
           ,@ProgramId
           ,@Permit
           ,@ModifeTime
           ,@CreateTime)", userRight);
          }
          #endregion
         

          //全部置为否
          DapperHelper.SqlExecute("update EM_User_Right set Permit=0 where RoleId=@RoleId ", new { RoleId = RoleId });

          if (!string.IsNullOrWhiteSpace(ProgramIds))
          {
              //选中的置为是
              DapperHelper.SqlExecute(string.Format("update EM_User_Right set Permit=1 where RoleId=@RoleId and  ProgramId in ({0})", ProgramIds), new { RoleId = RoleId });
          }
      }

    }
    public interface IUserRightRepo : IRepository<EM_User_Right>
    {

        bool HasRight(int UserId,string ControllerName, string ActionName);

        List<string> GetActions(int UserId, string ControllerName);

        void UpdateUserRoleRight(int RoleId, string ProgramIds);


    }
}
