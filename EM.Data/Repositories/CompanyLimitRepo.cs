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
    public class CompanyLimitRepo : RepositoryBase<EM_Company_Limit>, ICompanyLimitRepo
    {
        public CompanyLimitRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<CompanyLimitDTO> GetList(CompanyLimitSM sm)
        {

            var sql = @"select a.Id,b.CompanyName,c.CateName,a.LimitSum,a.ModifyDate,a.Modifier from EM_Company_Limit a
join EM_Company b on a.CompanyId=b.Id
join EM_Charge_Cate c on a.CateId=c.Id
 where 1=1 ";

            if (sm.CompanyId != 0)
            {
                sql += " and a.CompanyId=@CompanyId";
            } if (sm.CateId != 0)
            {
                sql += " and a.CateId=@CateId";
            } if (sm.SeasonType != 0)
            {
                sql += " and a.SeasonType=@SeasonType";
            }
            if (sm.SDate.HasValue)
            {
                sql += " and a.ModifyDate>=@SDate";
            }
            if (sm.EDate.HasValue)
            {
                sql += " and a.ModifyDate<=@EDate";
            }
            var result = DapperHelper.SqlQuery<CompanyLimitDTO>(sql, sm).ToList();
            return result;
        }

        public  CompanyCateLimitDTO GetCompanyLimit(int CompanyId, int CateId,DateTime? SDate=null,DateTime? EDate=null)
        {
            var SM = new CompanyCateLimitSM()
            {
                CompanyId = CompanyId,
                CateId = CateId,
                SDate = SDate.HasValue?SDate.Value:DateTime.Now.AddYears(-10),
                EDate = EDate.HasValue?EDate.Value:DateTime.Now.AddYears(10),
            };
            //先获取大类的已使用额度
            var TotalCost = DapperHelper.SqlQuery<int>(@"select case when SUM(Money) is null then 0 else SUM(Money) end    from EM_ExpenseAccount_Detail a
where a.CompanyId=@CompanyId and a.ExpenseAccountId<>0 and a.CateId in
(
select a.ParentId from EM_Charge_Cate a 
 where a.Id=@CateId
union all
select b.Id from EM_Charge_Cate a
left join EM_Charge_Cate b on  a.ParentId=b.ParentId and b.ParentId<>-1
left join EM_Charge_Cate c on a.ParentId=c.Id
 where a.Id=@CateId
 )
 and a.OccurDate >@SDate and a.OccurDate<@EDate", SM).FirstOrDefault();

           //根据传入的值获取大类的额度类型和名称
            var MainCate = DapperHelper.SqlQuery<EM_Charge_Cate>(@"select b.Id,b.CateName,b.CateType from EM_Charge_Cate a
  join EM_Charge_Cate b on a.ParentId=b.Id
  where a.Id=@CateId
 ", SM).FirstOrDefault();
            var TotalLimit = 0;
            //根据大类类型不同获取不同的额度
            var cateType = (CateTypeEnum)Enum.ToObject(CateTypeEnum.KPIAbout.GetType(), MainCate.CateType);
            switch (cateType)
            {
                case CateTypeEnum.KPIAbout:
                    //汇总KPI额度
                    var KpiSum = DapperHelper.SqlQuery<int>(@"select (case when SUM(SalesPerformance) is null then 0 else SUM(SalesPerformance) end )*10000  from EM_Company_Performance 
  where CompanyId=@CompanyId and UploadDate>@SDate and UploadDate<@EDate", SM).FirstOrDefault();
                    //绩效有关额度计算=（公司目前绩效/所有全部目标绩效）*全部报销额
                    TotalLimit = (KpiSum / 1130000) * 400000;
                    break;
                case CateTypeEnum.SeasonlyLimit:
                    //季度额度=当前公司，当前大类，所有季度额度的汇总，因为上季度用不掉的会延续到下季度，所以不需要判断时间
                    TotalLimit = DapperHelper.SqlQuery<int>(" select  case when  SUM(LimitSum) is null then 0 else SUM(LimitSum) end from EM_Company_Limit where CateId=@CateId and SeasonType in (1,2,3,4) and CompanyId=@CompanyId ", new { CateId = MainCate.Id, CompanyId = CompanyId }).FirstOrDefault();
                    break;
                case CateTypeEnum.YearlyLimit:
                    //年度额度=当前公司，当前大类，所有季度额度的汇总，因为上季度用不掉的会延续到下季度，所以不需要判断时间
                    TotalLimit = DapperHelper.SqlQuery<int>(" select  case when  SUM(LimitSum) is null then 0 else SUM(LimitSum) end from EM_Company_Limit where CateId=@CateId and SeasonType =0 and CompanyId=@CompanyId ", new { CateId = MainCate.Id, CompanyId = CompanyId }).FirstOrDefault();
                    break;

            }

            var result = new CompanyCateLimitDTO()
            {
                CateName=MainCate.CateName,
                TotalCost = TotalCost,
                TotalLimit = TotalLimit,
            };
            return result;
            
        }

    }


    public interface ICompanyLimitRepo : IRepository<EM_Company_Limit>
    {
         List<CompanyLimitDTO> GetList(CompanyLimitSM sm);

        /// <summary>
        /// 根据公司和分类Id获取公司的额度，只计算大类
        /// </summary>
        /// <param name="Company"></param>
        /// <param name="CateId"></param>
        /// <returns></returns>
         CompanyCateLimitDTO GetCompanyLimit(int CompanyId, int CateId, DateTime? SDate = null, DateTime? EDate = null);

    }
}
