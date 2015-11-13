﻿using System;
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
    public class ExpenseAccountDetailRepo : RepositoryBase<EM_ExpenseAccount_Detail>, IExpenseAccountDetailRepo
    {
        public ExpenseAccountDetailRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }


        public async Task UpdateFileExpenseAccountId(int ExpenseAccountId, string Ids)
        {
            var result = await DapperHelper.SqlExecuteAsync(string.Format(@"update EM_ExpenseAccount_Detail set ExpenseAccountId=@ExpenseAccountId where Id in ({0})",Ids), new { ExpenseAccountId = ExpenseAccountId});
        }


    }


    public interface IExpenseAccountDetailRepo : IRepository<EM_ExpenseAccount_Detail>
    {
        Task UpdateFileExpenseAccountId(int ExpenseAccountId,string Ids);
    }
}
