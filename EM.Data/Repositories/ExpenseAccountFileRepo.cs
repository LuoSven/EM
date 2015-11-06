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
    public class ExpenseAccountFileRepo : RepositoryBase<EM_ExpenseAccount_File>, IExpenseAccountFileRepo
    {
        public ExpenseAccountFileRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<EM_ExpenseAccount_File> GetListByExpenseAccountId(int Id)
        {
            var List = DapperHelper.SqlQuery<EM_ExpenseAccount_File>(@"select * from EM_ExpenseAccount_File where ExpenseAccountId=@ExpenseAccountId", new { ExpenseAccountId = Id }).ToList();
            return List;
        }
    }


    public interface IExpenseAccountFileRepo : IRepository<EM_ExpenseAccount_File>
    {
        List<EM_ExpenseAccount_File> GetListByExpenseAccountId(int Id);

    }
}
