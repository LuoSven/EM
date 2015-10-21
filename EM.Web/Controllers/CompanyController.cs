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
    [Description("公司管理")]
    [ControlType(SystemType.ZJ)]
    public class CompanyController : BaseController
    {

        private readonly IUserRoleRepo userRoleRepo = new UserRoleRepo(new DatabaseFactory());
        private readonly IUserRightRepo userRightRepo = new UserRightRepo(new DatabaseFactory());
        [Description("公司列表")]
        [ActionType(RightType.View)]
        public async Task< ActionResult> Index()
        {
            var Dtos =  userRoleRepo.GetAll().ToList();
            if (Request.IsAjaxRequest())
              return PartialView("_List", Dtos);
            return View(Dtos);
        }
        [Description("新增公司")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> Add()
        {
            InitSelect();
            ViewBag.roleList = userRoleRepo.GetList();
            var accountDetailVM = new AccountDetailVM();
            return View(accountDetailVM);
        }

        [HttpPost]
        public async Task<ActionResult> Add(AccountDetailVM model)
        {
            InitSelect();
            var accountDetailVM = new AccountDetailVM();
            return View(accountDetailVM);
        }

        [Description("编辑公司")]
        [ActionType(RightType.Form)]
        public async Task<ActionResult> Edit(int Id)
        {
            InitSelect();
            var Role =  userRoleRepo.GetById(Id);
            var Dtos =await userRoleRepo.GetPrograms(Id);
            var Vms = Mapper.Map<List<UserRoleProgramDTO>, List<UserRoleProgramVM>>(Dtos);
            var Systems = SystemType.YJ.GetEnumList().Where(o=>o.Key!=(int)SystemType.All).Select(o => new UserRoleTreeVM()//两个系统
            {
                ParentId=0,
                Id = o.Key,
                Name = o.Value,
                Items = new List<UserRoleTreeVM>()
            }).ToList();
            foreach (var system in Systems)//遍历系统
            {
                var Controls = Vms.Select(o => o.ControllerDescription).Distinct().Select(o => new UserRoleTreeVM()
                {
                    Id = 0,
                    Name = o,
                    ParentId=system.Id,
                    Items = new List<UserRoleTreeVM>()
                }).ToList();
                for (int i = 0; i < Controls.Count(); i++)//遍历每个控制器
                {
                    var VirtualId=999+i;
                    Controls[i].Id = VirtualId;
                    var ItemList = Vms.Where(o => o.ControllerDescription == Controls[i].Name).Select(o => new UserRoleTreeVM()
                    {
                        Id = o.Id,
                        PerMit = o.PerMit,
                        ParentId=VirtualId,
                        Name = o.ActionDescription
                    }).ToList();
                    Controls[i].Items = ItemList;//最终节点。action
                }
                system.Items = Controls;
            }

            ViewBag.Systems = Systems;

            return View(Role);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(EM_User_Role model,List<int> Ids)
        {
            ViewBag.AccountStatusList = AccountStatus.Allow.GetEnumList();
            var entity = userRoleRepo.GetById(model.id);
            entity = Mapper.Map<EM_User_Role, EM_User_Role>(model, entity);
            var result = userRoleRepo.SaveChanges();
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
