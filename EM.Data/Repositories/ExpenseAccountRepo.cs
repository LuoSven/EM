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
    public class ExpenseAccountRepo : RepositoryBase<EM_ExpenseAccount>, IExpenseAccountRepo
    {
        public ExpenseAccountRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<PagedResult<ExpenseAccountListDTO>> GetListByDtoAsync(ExpenseAccountSM sm, string UserName, int Page, int PageSize)
        {
            var dateSql = "and b.{0} >= @SDate and b.{0} <=@EDate";
            var sql = string.Format(@"select  distinct a.EANumber, a.Id,a.ApproveStatus,a.ModifyDate,a.Name,a.SumMoney ,a.ApplyDate  from EM_ExpenseAccount a
join EM_ExpenseAccount_Detail b on a.Id=b.ExpenseAccountId
where a.Creater='{0}'  ", UserName);

            if (sm.CompanyId.HasValue)
                sql += " and a.CompanyId = @CompanyId ";
            if (!string.IsNullOrEmpty(sm.Creater))
                sql += " and a.Creater like '%'+@Creater+'%' ";
            if (!string.IsNullOrEmpty(sm.EANumber))
                sql += " and a.EANumber like '%'+@EANumber+'%' ";
            if (!string.IsNullOrEmpty(sm.Name))
                sql += " and a.Name like '%'+@Name+'%' ";
            if (sm.CateId.HasValue)
                sql += " and a.CateId = @CateId ";

            sm.SDate = sm.SDate.HasValue ? sm.SDate : DateTime.Now.AddYears(-10);
            sm.EDate = sm.EDate.HasValue ? sm.EDate : DateTime.Now.AddYears(10);
            var dateType = (ExpenseAccountDateType)Enum.ToObject(typeof(ExpenseAccountDateType), sm.DateType);
            switch (dateType)
            {
                case ExpenseAccountDateType.CreateDate:
                    sql += string.Format(dateSql, "CreateDate");
                    break;
                case ExpenseAccountDateType.OccurDate:
                    sql += string.Format(dateSql, "OccurDate");
                    break;
                case ExpenseAccountDateType.ApplyDate:
                    sql += string.Format(dateSql, "ApplyDate");
                    break;
                case ExpenseAccountDateType.ModifyDate:
                    sql += string.Format(dateSql, "ModifyDate");
                    break;
            }




            var list = await DapperHelper.QueryWithPageAsync<ExpenseAccountListDTO>(sql, sm, " ModifyDate desc ", Page, PageSize);
            return list;
        }

        public async Task<string> GetNewPublicId()
        {
           var  EANumber=(await  DapperHelper.SqlQueryAsync<string>("select top 1 EANumber from EM_ExpenseAccount where EANumber like '对公%' order by Id desc")).FirstOrDefault();
           EANumber=EANumber??"";
            var MaxNo = 0;
           int.TryParse(EANumber.Replace("对公", ""), out MaxNo);
            var NewNo="对公"+( MaxNo + 1).ToString();
            return NewNo;
        }
    }


    public interface IExpenseAccountRepo : IRepository<EM_ExpenseAccount>
    {
        Task<PagedResult<ExpenseAccountListDTO>> GetListByDtoAsync(ExpenseAccountSM sm, string UserName, int Page, int PageSize);

        Task<string> GetNewPublicId();


    }
}