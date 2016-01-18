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

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View(); 
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
                    return RedirectToAction("StaffWelcome");
            }
            return View();
        }

        public ActionResult AdminWelcome()
        {
            return View();
        }
        public ActionResult CompanyManagerWelcome(DateTime? SDate=null,DateTime? EDate=null)
        {
           
            return View();
        }
        [HttpGet]
        public ActionResult GetCompanyLimit(DateTime? SDate = null, DateTime? EDate = null)
        {
            var model = new CompanyManagerWelcomeVM();
            model.CompanyCateLimits = new List<CompanyCateLimitDTO>();
            var CateList = changeCateRepo.GetList(ViewHelp.GetRoleType(), CateDropType.Report);
            foreach (var Cate in CateList)
            {
                var Limit = companyLimitRepo.GetCompanysLimit(ViewHelp.GetCompanyIds(), Cate.Key.ToInt(), SDate, EDate);
                model.CompanyCateLimits.Add(Limit);
                model.Performance = companyLimitRepo.GetPerformance(ViewHelp.GetCompanyIds());
            }

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
                switch(item.ProgramId)
                {
                    case "expenseaccount_failapproved":
                        var sm=new ExpenseAccountSM();
                        var result=expenseAccountRepo.GetListByDto(sm,ViewHelp.UserInfo(),1,100);
                        item.Count = result.RowCount;//result.RowCount;
                            break;

                };
                Items[i] = item;
            }
            return Items;
        }


    }
}
