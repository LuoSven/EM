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
    public class ExpenseAccountDetailRepo : RepositoryBase<EM_ExpenseAccount_Detail>, IExpenseAccountDetailRepo
    {
        public ExpenseAccountDetailRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }


        public async Task UpdateDetailExpenseAccountId(int ExpenseAccountId, string Ids)
        {
                  if(!string.IsNullOrEmpty(Ids))
                  await DapperHelper.SqlExecuteAsync(string.Format(@"update EM_ExpenseAccount_Detail set ExpenseAccountId=@ExpenseAccountId where Id in ({0})",Ids), new { ExpenseAccountId = ExpenseAccountId});
        }

        public  List<ExpenseAccountDetailListDTO> GetListByExpenseAccountId(int ExpenseAccountId)
        {
            var result = DapperHelper.SqlQuery<ExpenseAccountDetailListDTO>(@"select a.Id,b.CompanyName,c.CateName,a.Money,a.Remark,a.OccurDate from EM_ExpenseAccount_Detail a
join EM_Company b on a.CompanyId=b.Id
join EM_Charge_Cate c on a.CateId=c.Id
where ExpenseAccountId=@ExpenseAccountId",new{ExpenseAccountId=ExpenseAccountId});
            return result.ToList();
        }
    }


    public interface IExpenseAccountDetailRepo : IRepository<EM_ExpenseAccount_Detail>
    {
        Task UpdateDetailExpenseAccountId(int ExpenseAccountId, string Ids);

        List<ExpenseAccountDetailListDTO> GetListByExpenseAccountId(int ExpenseAccountId);
    }
}
