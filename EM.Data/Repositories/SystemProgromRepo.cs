using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;
using EM.Model.Entities;
using EM.Data.Dapper;
using EM.Model;
using EM.Model.VMs;

namespace EM.Data.Repositories
{
    public class SystemProgromRepo : RepositoryBase<EM_System_Program>, ISystemProgromRepo
    {
        public SystemProgromRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public int AddOrUpdateProgram(EM_System_Program program)
        {
            var source = DataContext.EM_System_Program.Where(o => o.ActionName == program.ActionName && o.ControllerName == program.ControllerName).FirstOrDefault();
            if(source==null)
            {
                program.ModifeTime = DateTime.Now;
                program.CreateTime = DateTime.Now;

                DataContext.EM_System_Program.Add(program);
                var ProgramResult=DataContext.SaveChanges();

                int[] AdminRoleList = { 1 };
                foreach (var id in AdminRoleList)
                {
                    var right = new EM_User_Right()
                    {
                        RoleId = id,
                        ProgramId=program.Id,
                        Permit=true,
                        ModifeTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                    };
                  DataContext.EM_User_Right.Add(right);
                 ProgramResult+= DataContext.SaveChanges();
                }



                if (ProgramResult == (1 + AdminRoleList.Length))
                return 1;
               return 2;
            }
            else
            {
                source.ControllerName = program.ControllerName;
                source.ControllerDescription = program.ControllerDescription;
                source.SystemType=program.SystemType;
                source.ActionName = program.ActionName;
                source.ActionDescription = program.ActionDescription;
                source.ModifeTime = DateTime.Now;
                source.RightType = program.RightType;
                source.ParentAction = program.ParentAction;


                if (DataContext.SaveChanges() == 1)
                return 3;
                return 4;
            }
            

        }


        public List<MenuVM> GetMenu(int UserId,int SysTypeId)
        {
            var MenuList = new List<MenuVM>();
            var ProgramList = DapperHelper.SqlQuery<EM_System_Program>(@"select b.SystemType ,b.ControllerName,b.ControllerDescription,b.ActionName,b.ActionDescription,b.Id from EM_User_Right a 
join EM_System_Program b on a.ProgramId=b.Id and b.RightType=1   and ( b.SystemType=@SysTypeId or b.SystemType=3)
and a.RoleId =(select top(1) a.RoleId from EM_User_Account a
join EM_User_Role b on a.RoleId=b.id
where a.UserId=@UserId ) and a.Permit=1", new { UserId = UserId, SysTypeId = SysTypeId });

            var Controls = ProgramList.Select(o =>new Tuple<string,string>(o.ControllerName,o.ControllerDescription)).Distinct();
            foreach (var Control in Controls)
            {
                var menu = new MenuVM();
                menu.ProgramId = Control.Item1;
                menu.Name = Control.Item2;
                menu.Items = ProgramList.Where(o => o.ControllerName == Control.Item1).Select(o => new MenuVM() {
                    ProgramId = o.ControllerName + "_" + o.ActionName,
                    Url = o.ControllerName + "/" + o.ActionName,
                    Name = o.ActionDescription,
                }).ToList();
                MenuList.Add(menu);
            }
            return MenuList;
        }

       
        public void DeleteAllProgram()
        {
            DapperHelper.SqlExecute(@"delete from EM_System_Program");
        }
    }
    public interface ISystemProgromRepo : IRepository<EM_System_Program>
    {


       int AddOrUpdateProgram(EM_System_Program program);

       List<MenuVM> GetMenu(int UserId, int SysTypeId);
       void DeleteAllProgram();
    }
}
