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
using EM.Model.SMs;

namespace EM.Data.Repositories
{
    /// <summary>
    /// 主要用来执行一些系统任务，比如备份
    /// </summary>
    public class SystemRepo : RepositoryBase<EM_System_Program>, ISystemRepo
    {
        public SystemRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public void SystemBackUp()
        {
            try
            {
                
            DapperHelper.SqlExecute("exec SystemBackup");
            }
            catch (Exception EX)
            {
                
                throw EX;
            }
        }


    }


    public interface ISystemRepo : IRepository<EM_System_Program>
    {
       
        void SystemBackUp();
    }
}
