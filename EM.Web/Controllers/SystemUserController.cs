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

namespace EM.Web.Controllers
{
    [Description("系统用户管理")]
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
        public ActionResult Add()
        {
            return View();
        }

    }
}
