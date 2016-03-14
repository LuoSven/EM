using AutoMapper;
using EM.Common;
using EM.Data.Infrastructure;
using EM.Data.Repositories;
using EM.Model.DTOs;
using EM.Model.Entities;
using EM.Model.SMs;
using EM.Model.VMs;
using EM.Web.Core;
using EM.Web.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using EM.Web.Core.Helper;

namespace EM.Web.Controllers
{
    [Description("系统管理")]
    [ControlType(SystemType.All)]
    public class SystemController : BaseController
    {
        //
        // GET: /System/

        private readonly ISystemFeedbackRepo systemFeedbackRepo = new SystemFeedbackRepo(new DatabaseFactory());
        private readonly ISystemAlertMessageRepo systemAlertMessageRepo = new SystemAlertMessageRepo(new DatabaseFactory());
        private readonly IExpenseAccountRepo expenseAccountRepo = new ExpenseAccountRepo(new DatabaseFactory());
        private readonly IUserLoginRecordRepo userLoginRecordRepo=new UserLoginRecordRepo(new DatabaseFactory());
        private readonly IUserAccountRepo userAccountRepo = new UserAccountRepo(new DatabaseFactory());
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddFeedBack(string message, string url)
        {
            var systemFeedback = new EM_System_Feedback()
            {
                Creater = ViewHelp.GetUserId(),
                Message = message,
                ModefyDate = DateTime.Now,
                CreateDate = DateTime.Now,
                Priority = (int)FeedBackPriority.L0,
                Url = url
            };
            systemFeedbackRepo.Add(systemFeedback);
            systemFeedbackRepo.SaveChanges();
            var Feedbacks = systemFeedbackRepo.GetFeedbacks(ViewHelp.GetUserId());
            return PartialView("_ListFeedbacks", Feedbacks);
        }

        [HttpGet]
        public ActionResult GetFeedBack()
        {
            var Feedbacks= systemFeedbackRepo.GetFeedbacks(ViewHelp.GetUserId());
            return PartialView(Feedbacks);
        }


        [HttpGet]
        public  ActionResult AlertMessage()
        {
            var alertMessage = systemAlertMessageRepo.GetAlertMessages(ViewHelp.GetUserId());
            return Json(new { messages = alertMessage }, JsonRequestBehavior.AllowGet);
        }

        
        [Description("登陆信息管理")]
        [ActionType(RightType.View)]
        public ActionResult LoginManage(LoginManageSM sm, int Page=1, int PageSize=20)
        {
            var list = userLoginRecordRepo.GetList(sm,Page,PageSize);
            var vms = Mapper.Map<IList<UserLoginRecordVM>>(list.Results);
            var result = new PagedResult<UserLoginRecordVM>(vms, Page, PageSize, list.RowCount);
            if (Request.IsAjaxRequest())
                return PartialView("_ListLoginManage", result);
            ViewBag.userList = SelectHelper.GetUserList();
            return View(result);
        }

        [Description("反馈信息管理")]
        [ActionType(RightType.View)]
        public ActionResult FeedbackManage(SystemFeedbackSM sm, int Page = 1, int PageSize = 20)
        {
            var list = systemFeedbackRepo.GetList(sm, Page, PageSize);
            var vms = Mapper.Map<IList<SystemFeedbackVM>>(list.Results);
            var result = new PagedResult<SystemFeedbackVM>(vms, Page, PageSize, list.RowCount);
            if (Request.IsAjaxRequest())
                return PartialView("_ListFeedbackManage", result);
            ViewBag.userList = SelectHelper.GetUserList();
            return View(result);
        }





    }
}
