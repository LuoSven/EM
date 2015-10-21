using EM.Web.Core;
using EM.Model;
using EM.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;

namespace ProgramUpdateTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var Programs = ViewHelp.GetAllActionByAssembly();
            ISystemProgromRepo systemProgromRepo = new SystemProgromRepo(new DatabaseFactory());
            Console.Write(string.Format("共发现{0}个作业\r\n", Programs.Count));
            foreach (var item in Programs)
            {
               
                Console.Write(string.Format("当前更新{0}/{1},作业描述{2}",item.ControllerName,item.ActionName,item.ActionDescription));
               var result= systemProgromRepo.AddOrUpdateProgram(item);
               var message = "作业";
               switch(result)
               {
                   case 1:
                       message += "已新增\r\n";
                       break;
                   case 2:
                       message += "新增失败\r\n";
                       break;
                   case 3:
                       message += "已更新\r\n";
                       break;
                   case 4:
                       message += "更新失败\r\n";
                       break;
               }
               Console.Write(message);
            }
            Console.ReadKey();

        }
    }
}
