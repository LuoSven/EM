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

        public List<ExpenseAccountDetailListDTO> GetListDtoByExpenseAccountId(int ExpenseAccountId, string CompanyIds, string UserName)
        {
            var sql = @"select a.Id,b.CompanyName,c.CateName,a.Money,a.Remark,a.OccurDate from EM_ExpenseAccount_Detail a
join EM_Company b on a.CompanyId=b.Id
join EM_Charge_Cate c on a.CateId=c.Id
join EM_ExpenseAccount d on a.ExpenseAccountId=d.Id
where a.ExpenseAccountId=@ExpenseAccountId ";
            if (!string.IsNullOrEmpty(CompanyIds))
            {
            CompanyIds = string.IsNullOrEmpty(CompanyIds) ? "0" : CompanyIds;
            sql += " and (  a.CompanyId in (" + CompanyIds + ") or d.Creater=@UserName) ";

            };
            var result = DapperHelper.SqlQuery<ExpenseAccountDetailListDTO>(sql, new { ExpenseAccountId = ExpenseAccountId, UserName });
            return result.ToList();
        }
        public List<EM_ExpenseAccount_Detail> GetListByExpenseAccountId(int ExpenseAccountId, string CompanyIds)
        {
            var companyIds = CompanyIds.ToInts();
            return DataContext.EM_ExpenseAccount_Detail.Where(o => o.ExpenseAccountId == ExpenseAccountId &&(string.IsNullOrEmpty(CompanyIds)|| companyIds.Contains(o.CompanyId))).ToList();
        }
    }


    public interface IExpenseAccountDetailRepo : IRepository<EM_ExpenseAccount_Detail>
    {
        Task UpdateDetailExpenseAccountId(int ExpenseAccountId, string Ids);

        List<ExpenseAccountDetailListDTO> GetListDtoByExpenseAccountId(int ExpenseAccountId, string CompanyIds,string UserName);

        List<EM_ExpenseAccount_Detail> GetListByExpenseAccountId(int ExpenseAccountId, string CompanyIds);

    }
}
