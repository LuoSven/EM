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
        //[Description("新增用户")]
        //[ActionType(RightType.View)]
        //public async Task<ActionResult> Add()
        //{
        //    InitSelect();
        //    ViewBag.roleList = userRoleRepo.GetList();
        //    var accountDetailVM = new AccountDetailVM();
        //    return View(accountDetailVM);
        //}

        //[HttpPost]
        //public async Task<ActionResult> Add(AccountDetailVM model)
        //{
        //    InitSelect();
        //    var accountDetailVM = new AccountDetailVM();
        //    return View(accountDetailVM);
        //}

        [Description("编辑用户")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> Edit(int Id)
        {
            InitSelect();
            var account = await userAccountRepo.GetByIdDto(Id);
           var model= Mapper.Map<AccountDetailDTO, AccountDetailVM>(account);
           return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(AccountDetailVM model)
        {
            ViewBag.AccountStatusList = AccountStatus.Allow.GetEnumList();
            var entity=userAccountRepo.GetById(model.UserId);
            entity = Mapper.Map<AccountDetailVM, EM_User_Account>(model, entity);
            entity.ModifyTime = DateTime.Now;
           var result= userAccountRepo.SaveChanges();
           if (result > 0)
               return Json(new { code = 1 });
           else
               return Json(new { code = 0,messgage="保存失败，请重试" });
        }
        private void InitSelect()
        {
            ViewBag.roleList = userRoleRepo.GetList();
        }

    }
}
