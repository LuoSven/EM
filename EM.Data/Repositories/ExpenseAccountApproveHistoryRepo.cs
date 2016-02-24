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
    public class ExpenseAccountApproveHistoryRepo : RepositoryBase<EM_ExpenseAccount_ApproveHistory>, IExpenseAccountApproveHistoryRepo
    {
        public ExpenseAccountApproveHistoryRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public   List<ExpenseAccountApproveHistoryListDTO> GetListStringByECId(int Id)
        {
            var list = DapperHelper.SqlQuery<ExpenseAccountApproveHistoryListDTO>("select * from EM_ExpenseAccount_ApproveHistory where ExpenseAccountId=@Id ", new { Id }).ToList();
            return list;
        }
    }


    public interface IExpenseAccountApproveHistoryRepo : IRepository<EM_ExpenseAccount_ApproveHistory>
    {
        List<ExpenseAccountApproveHistoryListDTO> GetListStringByECId(int Id);
    }
}
