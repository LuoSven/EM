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
            //根据每个日期合计金额,下方报表用
            var DateCostList = DapperHelper.SqlQuery<CompanyCateLimitDateDTO>(@"select case when SUM(Money) is null then 0 else SUM(Money) end  as Money  ,a.OccurDate 
from EM_ExpenseAccount_Detail a
join EM_ExpenseAccount b on a.ExpenseAccountId=b.Id
where a.CompanyId=@CompanyId and a.ExpenseAccountId<>0 and a.CateId in ( select * from dbo.FC_GetChildCateIds(@CateId) )
and a.OccurDate >@SDate and a.OccurDate<@EDate
and b.ApproveStatus=4 and b.IsNotAccount <>1
group by a.OccurDate", SM).ToList();


            //根据每个日期预计合计金额,下方报表用
            var DateExpectCostList = DapperHelper.SqlQuery<CompanyCateLimitDateDTO>(@"select case when SUM(Money) is null then 0 else SUM(Money) end  as Money  ,a.OccurDate 
from EM_ExpenseAccount_Detail a
join EM_ExpenseAccount b on a.ExpenseAccountId=b.Id
where a.CompanyId=@CompanyId and a.ExpenseAccountId<>0 and a.CateId in ( select * from dbo.FC_GetChildCateIds(@CateId) )
and a.OccurDate >@SDate and a.OccurDate<@EDate
and b.ApproveStatus<>4 and b.IsNotAccount <>1
group by a.OccurDate", SM).ToList();
            //获取公司类型，获取绩效额度的时候不一样
            var NowCompanyType = DapperHelper.SqlQuery<int?>("select a.CompanyType from EM_Company a where a.Id=@CompanyId", SM).FirstOrDefault();


            var TotalCost = DateCostList.Sum(o => o.Money);
            var ExpectTotalCost = DateExpectCostList.Sum(o => o.Money);
            //获取大类的额度类型和名称
            var MainCate = DapperHelper.SqlQuery<EM_Charge_Cate>(@"select * from dbo.FC_GetParentCateInfo(@CateId)", SM).FirstOrDefault();
            decimal TotalLimit = 0;
            //根据大类类型不同获取不同的额度
            var cateType = (CateTypeEnum)Enum.ToObject(CateTypeEnum.KPIAbout.GetType(), MainCate.CateType);
            switch (cateType)
            {
                case CateTypeEnum.KPIAbout:
                    switch (NowCompanyType) {
                        case null:
                        case (int)CompanyType.Other:  //汇总KPI额度
                            var KpiSum = GetPerformance(SM.CompanyId.ToString());
                            //绩效有关额度计算=（公司目前绩效/所有全部目标绩效）*全部报销额
                            TotalLimit = Math.Round((KpiSum.FinishPerformance / (decimal)1900000) * 700000, 2); ;
                            break;
                        case (int)CompanyType.City:  //汇总KPI额度
                            //获取城市公司的子公司
                            var ChildrenCompanyIdList = DapperHelper.SqlQuery<int>("select a.Id from EM_Company a where a.ParentCompanyId=@CompanyId", SM);
                            var KpiSumChildren = (decimal)0;
                            //汇总kpi
                            foreach (var ChildrenCompanyId in ChildrenCompanyIdList)
                            {
                                KpiSumChildren += GetPerformance(ChildrenCompanyId.ToString()).FinishPerformance;
                            }
                            //得出报销额/10
                            //绩效有关额度计算=（公司目前绩效/所有全部目标绩效）*全部报销额
                            TotalLimit = Math.Round((KpiSumChildren / (decimal)2010000) * 400000, 2) / 10;
                            break;
                    }
                  
                    break;
                case CateTypeEnum.YearlyLimit:
                    //年度额度=当前公司，当前大类，所有季度额度的汇总，因为上季度用不掉的会延续到下季度，所以不需要判断时间
                    TotalLimit = DapperHelper.SqlQuery<int>(" select  case when  SUM(LimitSum) is null then 0 else SUM(LimitSum) end from EM_Company_Limit where CateId=@CateId and SeasonType =0 and CompanyId=@CompanyId ", new { CateId = MainCate.Id, CompanyId = CompanyId }).FirstOrDefault();
                    break;
                case CateTypeEnum.SeasonlyLimit:
                    //季度额度=当前公司，当前大类，所有季度额度的汇总，因为上季度用不掉的会延续到下季度，所以不需要判断时间
                    TotalLimit = DapperHelper.SqlQuery<int>(" select  case when  SUM(LimitSum) is null then 0 else SUM(LimitSum) end from EM_Company_Limit where CateId=@CateId and SeasonType in (1,2,3,4) and CompanyId=@CompanyId ", new { CateId = MainCate.Id, CompanyId = CompanyId }).FirstOrDefault();
                    break;

            }
          

            var result = new CompanyCateLimitDTO()
            {
                CateName=MainCate.CateName,
                TotalCost = TotalCost,
                TotalLimit = TotalLimit,
                ExpectTotalCost=ExpectTotalCost,
                DateDetails = DateCostList,
                ExpectDateDetails = DateExpectCostList,
            };
            return result;
            
        }

        public  CompanyCateLimitDTO GetCompanysLimit(string CompanyIds, int CateId, DateTime? SDate = null, DateTime? EDate = null)
        {
            var companyIds = CompanyIds.ToInts();
            var result = new CompanyCateLimitDTO();
            foreach (var companyId in companyIds)
            {
                var tempLimit = GetCompanyLimit(companyId, CateId, SDate, EDate);
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
        /// 获取传入的公司业绩
        /// </summary>
        /// <param name="CompanyIds"></param>
        /// <returns></returns>
        public CompanyPerformanceSumDTO GetPerformance(string CompanyIds)
        {
            var result = new CompanyPerformanceSumDTO();
            var Performances = DapperHelper.SqlQuery<EM_Company_Performance>(string.Format(@" select CompanyId,SalesPerformance,UploadDate from EM_Company_Performance 
  where CompanyId in ({0}) order by UploadDate ",CompanyIds)).ToList();
            var KPIValue = DapperHelper.SqlQuery<decimal>(string.Format(@"select Sum(KPIValue) from EM_Company 
  where Id in ({0}) ", CompanyIds)).FirstOrDefault();
            if (Performances != null && Performances.Count > 0)
            {
                //根据每个公司分组，拿每组的最后一个业绩，再合成
                var KpiSum = Performances.GroupBy(o=>o.CompanyId).Select(o => o.ToList().Last().SalesPerformance).Sum();
                var EndDate = Performances.Last().UploadDate;
                result.EndDate = EndDate;
                result.FinishPerformance = KpiSum;
                result.TotalPerformance = KPIValue/10000;
            }

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

         CompanyCateLimitDTO GetCompanysLimit(string CompanyIds, int CateId, DateTime? SDate = null, DateTime? EDate = null);

         CompanyPerformanceSumDTO GetPerformance(string CompanyIds);

    }
}
