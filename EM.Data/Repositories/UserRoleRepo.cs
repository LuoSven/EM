using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;
using EM.Model.Entities;
using EM.Model.VMs;
using EM.Utils;
using EM.Common;
using EM.Model.DTOs;
using EM.Data.Dapper;

namespace EM.Data.Repositories
{
    public class UserRoleRepo : RepositoryBase<EM_User_Role>, IUserRoleRepo
    {
        public UserRoleRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }


        public  List<KeyValueVM> GetList()
        {
           var result=   DataContext.EM_User_Role.Select(o =>new KeyValueVM(){Key=o.Id.ToString(),Value=o.Name}).ToList();
           return result;
        }
        public List<UserRoleProgramDTO> GetPrograms(int RoleId=0)
        {
           

            var sql = @"select c.Id,c.ActionDescription ,c.RightType,c.ControllerDescription,b.Permit from EM_User_Role a 
join EM_User_Right b on b.RoleId=a.id
join EM_System_Program c on c.Id=b.ProgramId  where a.id=@RoleId";
            if(RoleId==0)
            {
                sql = "select c.Id,c.ActionDescription ,c.RightType,c.ControllerDescription,0 as Permit from  EM_System_Program c";
            }
            var list = DapperHelper.SqlQuery<UserRoleProgramDTO>(sql, new { RoleId = RoleId });
          return list.ToList();
        }

        public List<UserRoleListDTO> GetListDto()
        {
            var query = DapperHelper.SqlQuery<UserRoleListDTO>("select * from EM_User_Role").ToList();
            return query;
        }
        public void UpdateRightByIdAndProgromId(int Id, string ProgromIds)
        {

        }
}

    
    public interface IUserRoleRepo : IRepository<EM_User_Role>
    {

        List<KeyValueVM> GetList();
        List<UserRoleListDTO> GetListDto();

      List<UserRoleProgramDTO> GetPrograms(int RoleId=0);

      void UpdateRightByIdAndProgromId(int Id, string ProgromIds);
    }
}
