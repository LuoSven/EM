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
        private readonly IUserLoginRecordRepo userLoginRecordRepo=new UserLoginRecordRepo(new DatabaseFactory()); 
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

        [Description("回复反馈信息")]
        [ActionType(RightType.Form)]
        public ActionResult UpdateFeedback(int id,string message)
        {
            var feedback = systemFeedbackRepo.GetById(id);
            feedback.ReplyDate = DateTime.Now;
            feedback.ReplyMessage = message;
            systemFeedbackRepo.SaveChanges();
            var ReplyMessage="管理员回复了你的反馈:{0}<br>{1}";
            ReplyMessage=string.Format(ReplyMessage,feedback.Message.Omit(),message);
            var systemAlertMessage = new EM_System_AlertMessage()
            {
                Message = ReplyMessage,
                MessageType = (int)MessageType.Alert,
                Receiver = feedback.Creater,
                Sender = ViewHelp.GetUserId(),
                CreateTime = DateTime.Now,
                 
            };
            systemAlertMessageRepo.Add(systemAlertMessage);
            systemAlertMessageRepo.SaveChanges();
            return Json(new { code = 1 });
        }


        [Description("系统信息管理")]
        [ActionType(RightType.View)]
        public ActionResult SystemMessageManage(SystemAlertMessageSM sm, int Page = 1, int PageSize = 20)
        {
            var list = systemAlertMessageRepo.GetPagedList(sm, Page, PageSize);
            var vms = Mapper.Map<IList<SystemAlertMessageVM>>(list.Results);
            var result = new PagedResult<SystemAlertMessageVM>(vms, Page, PageSize, list.RowCount);
            if (Request.IsAjaxRequest())
                return PartialView("_ListSystemMessageManage", result);
            ViewBag.userList = SelectHelper.GetUserList();
            ViewBag.messageTypeList = SelectHelper.GetEnumList(MessageType.Notification);
            ViewBag.alertDateTypeList = SelectHelper.GetEnumList(SystemMessageDateTimeType.CreateDate);
            ViewBag.alertTypeList = SelectHelper.GetEnumList(AlertedStstusType.Alerted);  
            return View(result);
        }

        [Description("删除系统消息")]
        [ActionType(RightType.Form)]
        public ActionResult DeleteSystemMessageManage(int Id)
        {
            var entity = systemAlertMessageRepo.GetById(Id);
            Log(entity);
            systemAlertMessageRepo.Delete(entity);
            systemAlertMessageRepo.SaveChanges();
            return Json(new {code=1 },JsonRequestBehavior.AllowGet);
        }

        [Description("重新发送系统消息")]
        [ActionType(RightType.Form)]
        public ActionResult ResendSystemMessageManage(int Id)
        {
            var entity = systemAlertMessageRepo.GetById(Id);
            Log(entity);
            entity.AlertedTime=null;
            systemAlertMessageRepo.SaveChanges();
            return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
        }




    }
}
