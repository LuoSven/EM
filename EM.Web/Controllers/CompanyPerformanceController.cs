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
using EM.Model.SMs;

namespace EM.Web.Controllers
{
    [Description("业绩管理")]
    [ControlType(SystemType.ZJ)]
    public class CompanyPerformanceController : BaseController
    {

        private readonly IUserRoleRepo userRoleRepo = new UserRoleRepo(new DatabaseFactory());
        private readonly IUserRightRepo userRightRepo = new UserRightRepo(new DatabaseFactory());
        private readonly ICompanyPerformanceRepo companyPerformanceRepo = new CompanyPerformanceRepo(new DatabaseFactory());
        private readonly ICompanyRepo companyRepo = new CompanyRepo(new DatabaseFactory());
        [Description("业绩列表")]
        [ActionType(RightType.View)]
        public async Task< ActionResult> Index(CompanyPerformanceSM sm)
        {
            var CompanyList = companyRepo.GetList();
            ViewBag.CompanyList = new SelectList(CompanyList, "Key", "Value");
            var Dtos = companyPerformanceRepo.GetList(sm);
            var models = Mapper.Map<List<CompanyPerformanceVM>>(Dtos);
            if (Request.IsAjaxRequest())
                return PartialView("_List", models);
            return View(models);
        }
        [Description("新增业绩")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> Add()
        {
            InitSelect();
            ViewBag.roleList = userRoleRepo.GetList();
            var model = new EM_Company_Performance();
            model.UploadDate = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(EM_Company_Performance model)
        {
            model.CreateDate = DateTime.Now;
            model.Creater = ViewHelp.GetUserName();
            model.ModifyDate = DateTime.Now;
            model.Modifier = ViewHelp.GetUserName();
            companyPerformanceRepo.Add(model);
            var result = companyPerformanceRepo.SaveChanges();
            if (result > 0)
                return Json(new { code = 1, model = model }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { code = 0, messgage = "保存失败，请重试" }, JsonRequestBehavior.AllowGet);
        }

        [Description("编辑业绩")]
        [ActionType(RightType.Form)]
        public async Task<ActionResult> Edit(int Id)
        {
            var model = companyPerformanceRepo.GetById(Id);
            InitSelect( model.CompanyId);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(EM_Company_Performance model)
        {
            var entity = companyPerformanceRepo.GetById(model.Id);
            if (model == null)
            {
                return Json(new { code = 0, message = "业绩不存在！" }, JsonRequestBehavior.AllowGet);
            }

            entity = Mapper.Map<EM_Company_Performance, EM_Company_Performance>(model, entity);
            entity.ModifyDate = DateTime.Now;
            entity.Modifier = ViewHelp.GetUserName();
            var result = companyPerformanceRepo.SaveChanges();
            if (result > 0)
            {
                Log(model);
                return Json(new { code = 1, model = model });
            }
            else
                return Json(new { code = 0, message = "保存失败，请重试" });
        }


        [Description("删除业绩")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> Delete(int Id)
        {
            var model = companyPerformanceRepo.GetById(Id);
            if (model == null)
            {
                return Json(new { code = 0, message = "业绩不存在！" }, JsonRequestBehavior.AllowGet);
            }
            companyPerformanceRepo.Delete(model);
            if (companyPerformanceRepo.SaveChanges() > 0)
            {
                Log(model);
                return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { code = 0, message = "删除失败，请重试" }, JsonRequestBehavior.AllowGet);
        }
        private void InitSelect(int CompanyId=0)
        {
            var CompanyList = companyRepo.GetList(ViewHelp.GetRoleId());
            ViewBag.CompanyList = new SelectList(CompanyList, "Key", "Value", CompanyId);
        }

    }
}
