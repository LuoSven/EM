using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EM.Web.Core;
using EM.Data.Repositories;
using EM.Web.Core.Base;
using EM.Data.Infrastructure;
using EM.Common;
using EM.Model.VMs;
using EM.Model.DTOs;
using EM.Model.SMs;
namespace EM.Web.Controllers
{
    public class HomeController : BaseController
    {

        private readonly ISystemProgromRepo systemProgromRepo = new SystemProgromRepo(new DatabaseFactory());
        private readonly IExpenseAccountRepo expenseAccountRepo = new ExpenseAccountRepo(new DatabaseFactory());
        private readonly IUserRoleRepo userRoleRepo = new UserRoleRepo(new DatabaseFactory());
        private readonly IChargeCateRepo changeCateRepo = new ChargeCateRepo(new DatabaseFactory());
        private readonly ICompanyRepo companyRepo = new CompanyRepo(new DatabaseFactory());
        private readonly ICompanyLimitRepo companyLimitRepo = new CompanyLimitRepo(new DatabaseFactory());
        private readonly IExpenseAccountFileRepo expenseAccountFileRepo = new ExpenseAccountFileRepo(new DatabaseFactory());
        private readonly IExpenseAccountDetailRepo expenseAccountDetailRepo = new ExpenseAccountDetailRepo(new DatabaseFactory());

        
        public ActionResult Index(string Id)
        {
            var SysTypeId = Id == "ZJ" ? 1 : 2;
            var MemuList = systemProgromRepo.GetMenu(ViewHelp.GetUserId(), SysTypeId);
            for (int i = 0; i < MemuList.Count; i++)
            {
                MemuList[i].Items = GetMenuCount(MemuList[i].Items);
            }
            return View(MemuList);
        }


        public ActionResult WelCome()
        {
            var RoleTypeId=(RoleType)ViewHelp.GetRoleType();
            switch(RoleTypeId)
            {
                case RoleType.Admin:
                  return  RedirectToAction("AdminWelcome");
                case RoleType.CompanyManager:
                    return RedirectToAction("CompanyManagerWelcome");
                case RoleType.Staff:
                case RoleType.Area:
                    return RedirectToAction("StaffWelcome");
            }
            return View();
        }

        public ActionResult AdminWelcome(AdminWelcomeSM SM, int Page = 1, int PageSize = 40)
        {
            var CompanyLimits = new List<CompanyCateLimitVM>();
            //公司列表
            var Companys = companyRepo.GetListDto();
            if(SM.CompanyId.HasValue)
            {
                Companys = Companys.Where(o => o.Id == SM.CompanyId).ToList();
            }
            //分类列表
            var Cates = changeCateRepo.GetList(ViewHelp.GetRoleType(), CateDropType.Report, ViewHelp.GetCateIds());
            if (SM.CateId.HasValue)
            {
                Cates = Cates.Where(o => o.Key == SM.CateId.ToString()).ToList();
            }
            ViewBag.Cates=Cates;
            foreach (var Company in Companys)
            {
                var CompanyLimit = new CompanyCateLimitVM();
                CompanyLimit.CompanyId = Company.Id;
                CompanyLimit.CompanyName = Company.CompanyName;
                CompanyLimit.CompanyCateLimits = new List<CompanyCateLimitDTO>();
                foreach (var Cate in Cates)
                {
                   CompanyLimit.CompanyCateLimits.Add(companyLimitRepo.GetCompanyLimit(Company.Id, Cate.Key.ToInt()));
                }
                CompanyLimits.Add(CompanyLimit);
            }

            var Vms = new PagedResult<CompanyCateLimitVM>()
            {
                CurrentPage = Page,
                PageSize = PageSize,
                RowCount = CompanyLimits.Count,
                Results = CompanyLimits.Skip((Page-1)*PageSize).Take(PageSize).ToList()

            };
            if (Request.IsAjaxRequest())
                return PartialView("_List", Vms);
            InitSearchSelect();
            return View(Vms);
        }
        public ActionResult CompanyManagerWelcome(DateTime? SDate=null,DateTime? EDate=null)
        {
            ViewBag.isAdmin = false;
            if (!string.IsNullOrEmpty(Request.QueryString["companyIds"]))
            {
                
                var replaceList=new List<string>(){"一","二","三","四","五","六","七","八","九"};
                ViewBag.isAdmin = true;
                var companys = Request.QueryString["companyIds"];
                ViewBag.companys = companys;
                var companyName = companyRepo.GetCompanysName(companys, ",");
                replaceList.ForEach(o=>{
                    companyName=companyName.Replace(o,"第"+o);
                });
                companyName += "分公司";
                ViewBag.companyName = companyName;
            }
            return View();
        }
        [HttpGet]
        public ActionResult GetCompanyLimit(DateTime? SDate = null, DateTime? EDate = null)
        {
            var model = new CompanyManagerWelcomeVM();
            var companys = ViewHelp.GetCompanyIds();
            var roleType = ViewHelp.GetRoleType();
            var cateIds = ViewHelp.GetCateIds();
            if (!string.IsNullOrEmpty(Request.QueryString["companyIds"]))
            {
                companys = Request.QueryString["companyIds"];
                roleType =(int) RoleType.CompanyManager;
                ViewBag.companyName = companyRepo.GetCompanysName(companys, ",");
                cateIds = "";
            }
            model.CompanyCateLimits = new List<CompanyCateLimitDTO>();
            var CateList = changeCateRepo.GetList(roleType, CateDropType.Report, cateIds);
            if (!string.IsNullOrEmpty(Request.QueryString["companyIds"]))
            {
                CateList.Add(new KeyValueVM() { Key = "20", Value = "室内交通费" });
            }
            foreach (var Cate in CateList)
            {
                var Limit = companyLimitRepo.GetCompanysLimit(companys, Cate.Key.ToInt(), SDate, EDate);
                model.CompanyCateLimits.Add(Limit);
            }

            model.Performance = companyLimitRepo.GetPerformance(companys);
            return  Json(model,JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffWelcome()
        {
            return View();
        }

        private List<MenuVM> GetMenuCount(List<MenuVM> Items)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                switch (item.ProgramId)
                {
                    case "expenseaccount_approveindex":
                        item.Count = expenseAccountRepo.GetListByDto(new ExpenseAccountSM() { ApproveStatus = (int)ExpenseAccountApproveStatus.WaitingApprove }, ViewHelp.UserInfo(), 1, 100).RowCount;
                        break;
                    case "expenseaccount_failapproved":
                        item.Count = expenseAccountRepo.GetListByDto(new ExpenseAccountSM() { ApproveStatus = (int)ExpenseAccountApproveStatus.FailApproved }, ViewHelp.UserInfo(), 1, 100).RowCount;
                        break;
                };
                Items[i] = item;
            }
            return Items;
        }

        private void InitSearchSelect()
        {
            var CompanyList = companyRepo.GetList(ViewHelp.GetRoleId());
            ViewBag.CompanyList = new SelectList(CompanyList, "Key", "Value");
            var CateList = changeCateRepo.GetList(ViewHelp.GetRoleType(), CateDropType.Report, ViewHelp.GetCateIds());
            ViewBag.CateList = new SelectList(CateList, "Key", "Value");
        }

    }
}
