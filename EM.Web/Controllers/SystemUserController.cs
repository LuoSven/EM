using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EM.Web.Core;
using EM.Common;
using  System.ComponentModel;
using EM.Data.Repositories;
using EM.Data.Infrastructure;
using EM.Web.Core.Base;
using AutoMapper;
using EM.Model.VMs;
using EM.Model.DTOs;
using System.Threading.Tasks;
using EM.Model.Entities;
using EM.Utils;

namespace EM.Web.Controllers
{
    [Description("用户管理")]
    [ControlType(SystemType.All)]
    public class SystemUserController : BaseController
    {

        private readonly IUserAccountRepo userAccountRepo = new UserAccountRepo(new DatabaseFactory());
        private readonly IUserRoleRepo userRoleRepo = new UserRoleRepo(new DatabaseFactory());
        [Description("用户列表")]
        [ActionType(RightType.View)]
        public async Task< ActionResult> Index(string UserName = "", string LoginEmail = "", string RoleId = "")
        {
          var Dtos= await  userAccountRepo.GetUserList(UserName, LoginEmail, RoleId);
          var Vms = Mapper.Map<List<AccountDetailDTO>, List<AccountDetailVM>>(Dtos);
            
          if (Request.IsAjaxRequest())
              return PartialView("_List", Vms);
          ViewBag.RoleList = userRoleRepo.GetList();
          return View(Vms);
        }
        [Description("新增用户")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> Add()
        {
            var model = new EM_User_Account();
            InitSelect(model.RoleId, model.Status);
            return View("AddOrEdit", model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(EM_User_Account model)
        {
            if(userAccountRepo.IsEmailRepeat(model.LoginEmail,0))
            {
                return Json(new { code = 0, message = "邮箱已存在，请重新输入" });
            }


            model.Password = DESEncrypt.Encrypt(model.Password);
            model.CreateTime = DateTime.Now;
            model.ModifyTime = DateTime.Now;
            userAccountRepo.Add(model);
            var result = userAccountRepo.SaveChanges();
            if (result > 0)
                return Json(new { code = 1 });
            else
                return Json(new { code = 0, message = "保存失败，请重试" });
        }

        [Description("编辑用户")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> Edit(int Id)
        {
            var model = userAccountRepo.GetById(Id);
            model.Password = DESEncrypt.Decrypt(model.Password);
            InitSelect(model.RoleId, model.Status);
           return View("AddOrEdit",model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(EM_User_Account model)
        {
            if (userAccountRepo.IsEmailRepeat(model.LoginEmail, model.UserId))
            {
                return Json(new { code = 0, message = "邮箱已存在，请重新输入" });
            }
            var entity=userAccountRepo.GetById(model.UserId);
            model.Password = DESEncrypt.Encrypt(model.Password);
            entity = Mapper.Map<EM_User_Account, EM_User_Account>(model, entity);
            entity.CreateTime = entity.CreateTime == DateTime.MinValue ? DateTime.Now : entity.CreateTime;
            entity.ModifyTime = DateTime.Now;
           var result= userAccountRepo.SaveChanges();
           if (result > 0)
               return Json(new { code = 1 });
           else
               return Json(new { code = 0,message="保存失败，请重试" });
        }

        [Description("删除用户")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> Delete(int Id)
        {
            var model = userAccountRepo.GetById(Id);
            if (model == null)
            {
                return Json(new { code = 0, message = "报销单不存在！" }, JsonRequestBehavior.AllowGet);
            }
            userAccountRepo.Delete(model);
            if (userAccountRepo.SaveChanges() > 0)
            {
                Log(model);
                return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { code = 0, message = "删除失败，请重试" }, JsonRequestBehavior.AllowGet);
        }


         public async Task<ActionResult> ChangePassword()
        {
            return View();
        }
        [HttpPost]
         public async Task<JsonResult> ChangePassword(string OPassword,string NPassword)
         {
             var result=userAccountRepo.ChangePassword(ViewHelp.GetUserId(), OPassword, NPassword);
             return Json(new { code = result == "" ? 1 : 0,message= result });
         }

        public ActionResult CheckLoginEmail(int UserId,string LoginEmail)
        {
               var result= userAccountRepo.IsEmailRepeat(LoginEmail, UserId);
               return Json(!result,JsonRequestBehavior.AllowGet);
        }
        private void InitSelect(int? RoleId,int AccountStatusId)
        {
         var roleList  = userRoleRepo.GetList();
             ViewBag.roleList =new SelectList(roleList,"Key","Value",RoleId.HasValue?RoleId.Value:0);
         var AccountStatusList  = AccountStatus.Allow.GetEnumList();
         ViewBag.AccountStatusList = new SelectList(AccountStatusList, "Key", "Value", AccountStatusId);
        }

    }
}
