using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EM.Web.Core;
using EM.Common;
using System.ComponentModel;
using EM.Data.Repositories;
using EM.Data.Infrastructure;
using EM.Web.Core.Base;
using AutoMapper;
using EM.Model.VMs;
using EM.Model.DTOs;
using System.Threading.Tasks;
using EM.Model.Entities;
using EM.Model.SMs;

namespace EM.Web.Controllers
{
    [Description("统计报表")]
    [ControlType(SystemType.All)]
    public class ReportController : BaseController
    {

        private readonly IExpenseAccountRepo expenseAccountRepo = new ExpenseAccountRepo(new DatabaseFactory());
        private readonly IUserRoleRepo userRoleRepo = new UserRoleRepo(new DatabaseFactory());
        private readonly IChargeCateRepo changeCateRepo = new ChargeCateRepo(new DatabaseFactory());
        private readonly ICompanyRepo companyRepo = new CompanyRepo(new DatabaseFactory());
        private readonly ICompanyLimitRepo companyLimitRepo = new CompanyLimitRepo(new DatabaseFactory());
        private readonly IExpenseAccountFileRepo expenseAccountFileRepo = new ExpenseAccountFileRepo(new DatabaseFactory());
        private readonly IExpenseAccountDetailRepo expenseAccountDetailRepo = new ExpenseAccountDetailRepo(new DatabaseFactory());
        
        [Description("月度费用统计")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> MonthExpenseStatistics(MonthExpenseStatisticsSM sm)
        {
          
            if(Request.IsAjaxRequest())
            {
                var NameList = new List<string>();
                if (!sm.SDate.HasValue)
                {
                    sm.SDate = DateTime.Now.AddYears(-1);
                }
                if (!sm.EDate.HasValue)
                {
                    sm.EDate = DateTime.Now;
                }
                var CateList = changeCateRepo.GetList(ViewHelp.GetRoleType(), CateDropType.Report, ViewHelp.GetCateIds());
                sm.CompanyIds = ViewHelp.GetCompanyIds();
                sm.RoleType = ViewHelp.GetRoleType();
                var Dtos = expenseAccountRepo.GetMonthCateList(sm);
                var List = new Dictionary<string, List<ExpenseAccountMonthCateDTO>>();
                if (Dtos.Count > 0)
                {
                    //补全信息
                    foreach (var Cate in CateList)
                    {
                        var BeginDate = new DateTime(sm.SDate.Value.Year, sm.SDate.Value.Month, 1);
                        var EndDate = new DateTime(sm.EDate.Value.Year, sm.EDate.Value.Month, 1);
                        List.Add(Cate.Value, new List<ExpenseAccountMonthCateDTO>());
                        if (BeginDate != EndDate)
                        {
                            //不是同月的
                            while (BeginDate != EndDate)
                            {
                                var Dto = new ExpenseAccountMonthCateDTO() { CateName = Cate.Value, ECMonth = BeginDate.Month, ECYear = BeginDate.Year, SumMoney = 0 };
                                Dto.SumMoney = Dtos.Where(o => o.CateName == Cate.Value && o.ECYear == BeginDate.Year && o.ECMonth == BeginDate.Month).Select(o => o.SumMoney).FirstOrDefault();
                                Dto.SumMoney = Dto.SumMoney ?? 0;
                                List[Cate.Value].Add(Dto);
                                NameList.Add(BeginDate.GetMonthName());
                                BeginDate = BeginDate.AddMonths(1);
                            }
                        }
                        else
                        {
                            //同月的
                            var Dto = new ExpenseAccountMonthCateDTO() { CateName = Cate.Value, ECMonth = BeginDate.Month, ECYear = BeginDate.Year, SumMoney = 0 };
                            Dto.SumMoney = Dtos.Where(o => o.CateName == Cate.Value && o.ECYear == BeginDate.Year && o.ECMonth == BeginDate.Month).Select(o => o.SumMoney).FirstOrDefault();
                            Dto.SumMoney = Dto.SumMoney ?? 0;
                            List[Cate.Value].Add(Dto);
                            NameList.Add(BeginDate.GetMonthName());
                            BeginDate = BeginDate.AddMonths(1);
                        }
                        

                    }
                }
                return Json(new { NameList, List  }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.MonthList = ViewHelp.GetRecentlyMonthList();
            }  
            return View();
        }


        [Description("额度使用明细")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> CompanyLimitDetail(ExpenseAccountSM Sm, int Page = 1, int PageSize = 20)
        {
            if (!Sm.CompanyId.HasValue)
            {
                var CompanyList = companyRepo.GetList(ViewHelp.GetRoleId());
                Sm.CompanyId = CompanyList.FirstOrDefault().Key.ToInt();
            }
            if (!Sm.CateId.HasValue)
            {
                var CateList = changeCateRepo.GetList(ViewHelp.GetRoleType(), CateDropType.Search, ViewHelp.GetCateIds());
                Sm.CateId = CateList.FirstOrDefault().Key.ToInt();
            }
            Sm.IsNotAccount = 0;
            Sm.ApproveStatus =(int) ExpenseAccountApproveStatus.PassApproved;
            var Dtos = expenseAccountRepo.GetListByDto(Sm, ViewHelp.UserInfo(), Page, PageSize);
            var Vms = new PagedResult<ExpenseAccountListVM>()
            {
                CurrentPage = Dtos.CurrentPage,
                PageSize = Dtos.PageSize,
                RowCount = Dtos.RowCount,
                Stats = Dtos.Stats

            };
            ViewBag.limit = companyLimitRepo.GetCompanyLimit(Sm.CompanyId.Value, Sm.CateId.Value, Sm.SDate, Sm.EDate);
            Vms.Results = Mapper.Map<IList<ExpenseAccountListDTO>, IList<ExpenseAccountListVM>>(Dtos.Results);
            if (Request.IsAjaxRequest())
                return PartialView("_CompanyLimitDetailList", Vms);
            CompanyLimitInitSearchSelect();
            return View(Vms);
        }
        private void CompanyLimitInitSearchSelect()
        {
            var CateList = changeCateRepo.GetList(ViewHelp.GetRoleType(), CateDropType.Report, ViewHelp.GetCateIds());
            ViewBag.CateList = new SelectList(CateList, "Key", "Value");
            var CompanyList = companyRepo.GetList(ViewHelp.GetRoleId());
            ViewBag.CompanyList = new SelectList(CompanyList, "Key", "Value");
        }
    }
}
