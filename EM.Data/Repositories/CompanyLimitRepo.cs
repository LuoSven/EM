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


        private static readonly decimal totalSale = 1900000;
        private static readonly decimal totalSaleExpenseAccount = 700000;
        public List<CompanyLimitDTO> GetList(CompanyLimitSM sm, string companyIds, string cateIds)
        {

            var sql = @"select a.Id,b.CompanyName,c.CateName,a.LimitSum,a.ModifyDate,a.Modifier,a.SeasonType from EM_Company_Limit a
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

            if (!string.IsNullOrEmpty(companyIds))
            {
                sql += "  and a.CompanyId in ("+companyIds+")";
            } 
            if (!string.IsNullOrEmpty(cateIds))
            {
                sql += "  and a.CateId in (" + cateIds + ")";
            }
            var result = DapperHelper.SqlQuery<CompanyLimitDTO>(sql, sm).ToList();
            return result;
        }

        public  CompanyCateLimitDTO GetCompanyLimit(int companyId, int cateId,int year)
        {
            DateTime sDate, eDate;
             year.GetDateByYear(out sDate, out eDate); 
            var sm = new CompanyCateLimitSM()
            {
                CompanyId = companyId,
                CateId = cateId,
                SDate =sDate,
                EDate = eDate,
            };
            var costList = GetCostList(companyId, cateId, sDate, eDate);
            //根据每个日期合计金额,下方报表用
            var datelyCostList = from cost in costList
                               group cost by new { cost.OccurDate } into m
                               select new CompanyCateLimitDateDTO()
                               {
                                   OccurDate = m.First().OccurDate,
                                   Money = m.Sum(o => o.Money)

                               };


            //根据每个日期预计合计金额,下方报表用
            var dateExpectCostList = DapperHelper.SqlQuery<CompanyCateLimitDateDTO>(@"select case when SUM(Money) is null then 0 else SUM(Money) end  as Money  ,a.OccurDate 
from EM_ExpenseAccount_Detail a
join EM_ExpenseAccount b on a.ExpenseAccountId=b.Id
where a.CompanyId=@CompanyId and a.ExpenseAccountId<>0 and a.CateId in ( select * from dbo.FC_GetChildCateIds(@CateId) )
and a.OccurDate >@SDate and a.OccurDate<@EDate
and b.ApproveStatus<>4
and( b.IsNotAccount is null or b.IsNotAccount =0)
group by a.OccurDate", sm).ToList();
            //获取公司类型，获取绩效额度的时候不一样


            var totalCost = datelyCostList.Sum(o => o.Money);
            var expectTotalCost = dateExpectCostList.Sum(o => o.Money);


            //获取大类的额度类型和名称
            var mainCate = DapperHelper.SqlQuery<EM_Charge_Cate>(@"select * from dbo.FC_GetParentCateInfo(@CateId)", sm).FirstOrDefault();

            //获取额度
            var totalLimit = GetLimit(companyId, cateId, year);
       
          
            
          

            var result = new CompanyCateLimitDTO()
            {
                CateId=cateId,
                CateName=mainCate.CateName,
                TotalCost = totalCost,
                TotalLimit = totalLimit,
                ExpectTotalCost=expectTotalCost,
                DateDetails = datelyCostList.ToList(),
                ExpectDateDetails = dateExpectCostList,
            };
            return result;
            
        }
        public List<EM_ExpenseAccount_Detail>  GetCostList(int companyId, int cateId, DateTime sDate , DateTime eDate )
        {
             var getCostList = DapperHelper.SqlQuery<EM_ExpenseAccount_Detail>(@"select a.* from EM_ExpenseAccount_Detail a
join EM_ExpenseAccount b on a.ExpenseAccountId=b.Id
where a.CompanyId=@companyId and a.ExpenseAccountId<>0 and a.CateId in ( select * from dbo.FC_GetChildCateIds(@cateId) )
and a.OccurDate >@sDate and a.OccurDate<@eDate
and b.ApproveStatus=4 
and ( b.IsNotAccount is null or b.IsNotAccount =0)", new {  companyId,  cateId,  sDate, eDate }).ToList();
             return getCostList;
        }


        public  CompanyCateLimitDTO GetCompanysLimit(string companyIds, int cateId, int year)
        { 
            var result = new CompanyCateLimitDTO();
            foreach (var companyId in companyIds.ToInts())
            {
                var tempLimit = GetCompanyLimit(companyId, cateId, year);
                result.CateName = tempLimit.CateName;
                result.TotalCost += tempLimit.TotalCost;
                result.ExpectTotalCost += tempLimit.ExpectTotalCost;
                result.TotalLimit += tempLimit.TotalLimit == 0 ? 1 : tempLimit.TotalLimit;
                result.DateDetails = tempLimit.DateDetails;
                result.ExpectDateDetails = tempLimit.ExpectDateDetails;
            }
            return result;
        }

        /// <summary>
        ///  获取传入的公司业绩
        /// </summary>
        /// <param name="companyIds"></param> 
        /// <param name="year"></param> 
        /// <returns></returns>
        public CompanyPerformanceSumDTO GetPerformance(string companyIds,int year)
        {
            var result = new CompanyPerformanceSumDTO();
            DateTime startTime, endTime; 
            year.GetDateByYear(out startTime, out endTime); 

            var sm = new { startTime, endTime, year }; 
            var performances = DapperHelper.SqlQuery<EM_Company_Performance>(string.Format(@" select CompanyId,SalesPerformance,UploadDate from EM_Company_Performance 
  where CompanyId in ({0}) and  UploadDate >=@startTime and  UploadDate<=@endTime  order by UploadDate  ", companyIds), sm).ToList();

            var kpiValueSql = string.Format(@"select Sum(KPIValue) from EM_Company 
  where Id in ({0}) ", companyIds);

            if (startTime >= new DateTime(2017, 1, 1))
            {
                kpiValueSql = string.Format(@"select Sum(KPIValue) from EM_Company_Kpi 
  where CompanyId in ({0})  and Year=@year", companyIds);
            }
            var kpiValue = DapperHelper.SqlQuery<decimal>(kpiValueSql, sm).FirstOrDefault();

            if (performances.Count == 0)
            {
                return result;
            }
            //根据每个公司分组，拿每组的最后一个业绩，再合成
            var kpiSum = performances.GroupBy(o => o.CompanyId).Select(o => o.ToList().Last().SalesPerformance).Sum();
            var endDate = performances.Last().UploadDate;
            result.EndDate = endDate;
            result.FinishPerformance = kpiSum;
            result.TotalPerformance = kpiValue / 10000;


            return result;
        }
       /// <summary>
       /// 获取公司和相关分类的额度
       /// </summary>
       /// <param name="companyId"></param>
        /// <param name="cateId"></param>
        /// <param name="year"></param>
       /// <returns></returns>
        private decimal GetLimit(int companyId, int cateId,int year)
        {
         
            var sm = new { cateId, companyId, year };
            decimal totalLimit = 0;
            //根据大类类型不同获取不同的额度
            //获取大类的额度类型和名称
            var parentCateType = DapperHelper.SqlQuery<int>(@"select CateType from dbo.FC_GetParentCateInfo(@cateId)", sm).FirstOrDefault(); 

            var cateType = (CateTypeEnum)Enum.ToObject(CateTypeEnum.KPIAbout.GetType(), parentCateType);
            //当前公司的类型
            var companyType = DapperHelper.SqlQuery<int?>("select a.CompanyType from EM_Company a where a.Id=@companyId", sm).FirstOrDefault();


            //城市分公司所有额度都是子公司和10%
            if (companyType == (int)CompanyType.City)
            {

                var childrenCompanyIdList = DapperHelper.SqlQuery<int>("select a.Id from EM_Company a where a.ParentCompanyId=@companyId", sm);
                foreach (var childrenCompanyId in childrenCompanyIdList)
                {
                    //嵌套把所有子公司的和起来
                    var childreLimit = GetLimit(childrenCompanyId, cateId, year);
                    totalLimit += childreLimit;
                }
                totalLimit = totalLimit / 10;
            }
            else
            {
                switch (cateType)
                {
                    case CateTypeEnum.KPIAbout:
                        var kpiSum = GetPerformance(companyId.ToString(), year);
                        //绩效有关额度计算=（公司目前绩效/所有全部目标绩效）*全部报销额
                        totalLimit = Math.Round((kpiSum.FinishPerformance / totalSale) * totalSaleExpenseAccount, 2); ;
                        break;
                    case CateTypeEnum.YearlyLimit:
                        //年度额度=当前公司，当前大类，所有季度额度的汇总，因为上季度用不掉的会延续到下季度，所以不需要判断时间
                        totalLimit = DapperHelper.SqlQuery<int>(" select  case when  SUM(LimitSum) is null then 0 else SUM(LimitSum) end from EM_Company_Limit where CateId=@cateId and SeasonType =0 and CompanyId=@companyId and Year=@year  ", sm).FirstOrDefault();
                        break;
                    case CateTypeEnum.SeasonlyLimit:
                        //季度额度=当前公司，当前大类，所有季度额度的汇总，因为上季度用不掉的会延续到下季度，所以不需要判断时间
                        totalLimit = DapperHelper.SqlQuery<int>(" select  case when  SUM(LimitSum) is null then 0 else SUM(LimitSum) end from EM_Company_Limit where CateId=@cateId and SeasonType in (1,2,3,4) and CompanyId=@companyId and Year=@year ", sm).FirstOrDefault();
                        break;
                    case CateTypeEnum.Other:
                    case CateTypeEnum.None:
                    default:
                           //默认，其他和无类型都只要把目前的都和起来就可以了
                        totalLimit = DapperHelper.SqlQuery<int>(" select  case when  SUM(LimitSum) is null then 0 else SUM(LimitSum) end from EM_Company_Limit where CateId=@cateId  and CompanyId=@companyId and Year=@year ", sm).FirstOrDefault();
                        break;


                }
            }
            return totalLimit;
        }

    }


    public interface ICompanyLimitRepo : IRepository<EM_Company_Limit>
    {
         List<CompanyLimitDTO> GetList(CompanyLimitSM sm,string companyIds,string cateIds);

    /// <summary>
         /// 根据公司和分类Id获取公司的额度，只计算大类
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="cateId"></param>
    /// <param name="sDate"></param>
    /// <param name="eDate"></param>
    /// <returns></returns>
         CompanyCateLimitDTO GetCompanyLimit(int companyId, int cateId, int year);

         CompanyCateLimitDTO   GetCompanysLimit(string companyIds, int cateId, int year);

         CompanyPerformanceSumDTO GetPerformance(string companyIds, int year);

        List<EM_ExpenseAccount_Detail> GetCostList(int companyId, int cateId, DateTime sDate, DateTime eDate);

    }
}
