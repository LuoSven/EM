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
    [Description("角色管理")]
    [ControlType(SystemType.ZJ)]
    public class UserRoleController : BaseController
    {

        private readonly IUserRoleRepo userRoleRepo = new UserRoleRepo(new DatabaseFactory());
        private readonly IUserRightRepo userRightRepo = new UserRightRepo(new DatabaseFactory());
        private readonly ICompanyRepo CompanyRepo = new CompanyRepo(new DatabaseFactory());
        [Description("角色列表")]
        [ActionType(RightType.View)]
        public async Task< ActionResult> Index()
        {
            var Dtos = userRoleRepo.GetListDto();
            var Vms = Mapper.Map<List<UserRoleListVM>>(Dtos);
            if (Request.IsAjaxRequest())
                return PartialView("_List", Vms);
            return View(Vms);
        }
        [Description("新增角色")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> Add()
        {
            InitSelect();
            InitBody();
            var model = new EM_User_Role();
            return View("AddOrEdit", model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(EM_User_Role model, string CompanyIds, string ProgramIds)
        {
            model.CompanyIds = CompanyIds;
            userRoleRepo.Add(model);
            var result = userRoleRepo.SaveChanges();
            userRightRepo.UpdateUserRoleRight(model.Id, ProgramIds);
            if (result > 0)
                return Json(new { code = 1 });
            else
                return Json(new { code = 0, message = "保存失败，请重试" });
        }

        [Description("编辑角色")]
        [ActionType(RightType.Form)]
        public async Task<ActionResult> Edit(int Id)
        {
            var model = userRoleRepo.GetById(Id);
            InitSelect(model.RoleType, model.CompanyIds);
            InitBody(Id,model.CompanyIds);
            return View("AddOrEdit", model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(EM_User_Role model, string CompanyIds, string ProgramIds)
        {
            var entity = userRoleRepo.GetById(model.Id);
            entity = Mapper.Map<EM_User_Role, EM_User_Role>(model, entity);
            entity.CompanyIds = CompanyIds;
            userRightRepo.UpdateUserRoleRight(model.Id, ProgramIds);
            var result = userRoleRepo.SaveChanges();
               return Json(new { code = 1 });
        }
        [Description("删除角色")]
        [ActionType(RightType.Form)]
        public ActionResult Delete(int Id)
        {
            var RightList = userRightRepo.GetMany(o => o.RoleId == Id);
            RightList.ToList().ForEach(o => userRightRepo.Delete(o));
            userRightRepo.SaveChanges();
            var entity=userRoleRepo.GetById(Id);
            userRoleRepo.Delete(entity);
            userRoleRepo.SaveChanges();
            return Json(new { code = 1 },JsonRequestBehavior.AllowGet);
        }

        [Description("更新系统作业")]
        [ActionType(RightType.Form)]
        public ActionResult UpdateSystemPrograms()
        {
            var Programs = ViewHelp.GetAllActionByAssembly(AppDomain.CurrentDomain.BaseDirectory+"\\bin");
            var messageAll = "";
            ISystemProgromRepo systemProgromRepo = new SystemProgromRepo(new DatabaseFactory());
            messageAll+=string.Format("共发现{0}个作业\r\n", Programs.Count);
            foreach (var item in Programs)
            {

                var result = systemProgromRepo.AddOrUpdateProgram(item);
                var message = item.ActionDescription+"("+item.ControllerName+"/"+item.ActionName+")";
                switch (result)
                {
                    case 1:
                        message += "新增成功\r\n";
                        break;
                    case 2:
                        message += "新增失败\r\n";
                        break;
                    case 3:
                        message += "更新成功\r\n";
                        break;
                    case 4:
                        message += "更新失败\r\n";
                        break;
                }
               messageAll+=message;
            }

            return Json(new { code = 1, message = messageAll }, JsonRequestBehavior.AllowGet);
        }

        private void InitSelect(int roleType = 0, string CompanyIds = "")
        {
            var List=CompanyRepo.GetList();
            if (!string.IsNullOrEmpty(CompanyIds))
            {
                var  ids = CompanyIds.Split(',');
                var ListTemp =new List<KeyValueVM>();
                foreach (var CompanyId in  List.Where(o => !CompanyIds.Contains(o.Key)))
                {
                    ListTemp.Add(CompanyId);
                }
                List = ListTemp;
            }
            ViewBag.CompanySelect = new SelectList(List, "Key", "Value");
            ViewBag.RoleTypeList = new SelectList(RoleType.Admin.GetEnumList(), "Key", "Value", roleType);

            
        }

       

        private void InitBody(int Id=0,string CompanyIds="")
        {
            if (!string.IsNullOrEmpty(CompanyIds))
            {
                List<int> ids = CompanyIds.Split(',').Select(o => Convert.ToInt32(o)).ToList();
                var CompanyList = CompanyRepo.GetMany(o => ids.Contains(o.Id)).ToList();
                ViewBag.CompanyIdList = CompanyList;
            }
            else
            {
                ViewBag.CompanyIdList = new List<EM_Company>();
            }


            var Dtos =  userRoleRepo.GetPrograms(Id);
            var Vms = Mapper.Map<List<UserRoleProgramDTO>, List<UserRoleProgramVM>>(Dtos);
            var Systems = SystemType.YJ.GetEnumList().Where(o => o.Key != (int)SystemType.All).Select(o => new UserRoleTreeVM()//两个系统
            {
                ParentId = 0,
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
                    ParentId = system.Id,
                    Items = new List<UserRoleTreeVM>()
                }).ToList();
                for (int i = 0; i < Controls.Count(); i++)//遍历每个控制器
                {
                    var VirtualId = 999 + i;
                    Controls[i].Id = VirtualId;
                    var ItemList = Vms.Where(o => o.ControllerDescription == Controls[i].Name).Select(o => new UserRoleTreeVM()
                    {
                        Id = o.Id,
                        PerMit = o.PerMit,
                        ParentId = VirtualId,
                        Name = o.ActionDescription
                    }).ToList();
                    Controls[i].Items = ItemList;//最终节点。action
                }
                system.Items = Controls;
            }

            ViewBag.Systems = Systems;

        }


    }
}
