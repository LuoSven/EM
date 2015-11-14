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

    }


    public interface IExpenseAccountApproveHistoryRepo : IRepository<EM_ExpenseAccount_ApproveHistory>
    {
    }
}
