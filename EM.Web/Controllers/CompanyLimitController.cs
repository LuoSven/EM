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
    [Description("额度管理")]
    [ControlType(SystemType.ZJ)]
    public class CompanyLimitController : BaseController
    {

        private readonly IUserRoleRepo userRoleRepo = new UserRoleRepo(new DatabaseFactory());
        private readonly IUserRightRepo userRightRepo = new UserRightRepo(new DatabaseFactory());
        private readonly ICompanyLimitRepo companyLimitRepo = new CompanyLimitRepo(new DatabaseFactory());
        private readonly ICompanyRepo companyRepo = new CompanyRepo(new DatabaseFactory());
        private readonly IChargeCateRepo changeCateRepo = new ChargeCateRepo(new DatabaseFactory());
        [Description("额度列表")]
        [ActionType(RightType.View)]
        public async Task< ActionResult> Index(CompanyLimitSM sm)
        {
            var Dtos = companyLimitRepo.GetList(sm);
            var models = Mapper.Map<List<CompanyLimitVM>>(Dtos);
            if (Request.IsAjaxRequest())
                return PartialView("_List", models);
            InitSelect();
            return View(models);
        }
        [Description("新增额度")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> Add()
        {
            InitSelect();
            var model = new EM_Company_Limit();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(EM_Company_Limit model)
        {
            model.CreateDate = DateTime.Now;
            model.Creater = ViewHelp.GetUserName();
            model.ModifyDate = DateTime.Now;
            model.Modifier = ViewHelp.GetUserName();
            companyLimitRepo.Add(model);
            var result = companyLimitRepo.SaveChanges();
            if (result > 0)
                return Json(new { code = 1, model = model }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { code = 0, messgage = "保存失败，请重试" }, JsonRequestBehavior.AllowGet);
        }

        [Description("编辑额度")]
        [ActionType(RightType.Form)]
        public async Task<ActionResult> Edit(int Id)
        {
            var model = companyLimitRepo.GetById(Id);
            InitSelect( model.CompanyId,model.CateId,model.SeasonType);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(EM_Company_Limit model)
        {
            var entity = companyLimitRepo.GetById(model.Id);
            if (model == null)
            {
                return Json(new { code = 0, message = "额度不存在！" }, JsonRequestBehavior.AllowGet);
            }

            entity = Mapper.Map<EM_Company_Limit, EM_Company_Limit>(model, entity);
            entity.ModifyDate = DateTime.Now;
            entity.Modifier = ViewHelp.GetUserName();
            var result = companyLimitRepo.SaveChanges();
            if (result > 0)
            {
                Log(model);
                return Json(new { code = 1, model = model });
            }
            else
                return Json(new { code = 0, message = "保存失败，请重试" });
        }


        [Description("删除额度")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> Delete(int Id)
        {
            var model = companyLimitRepo.GetById(Id);
            if (model == null)
            {
                return Json(new { code = 0, message = "额度不存在！" }, JsonRequestBehavior.AllowGet);
            }
            companyLimitRepo.Delete(model);
            if (companyLimitRepo.SaveChanges() > 0)
            {
                Log(model);
                return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { code = 0, message = "删除失败，请重试" }, JsonRequestBehavior.AllowGet);
        }

        private void InitSelect(int CateId = 0, int CompanyId = 0,int SeasonType =0)
        {
            var CateList = changeCateRepo.GetList(ViewHelp.GetRoleType());
            ViewBag.CateList = new SelectList(CateList, "Key", "Value", CateId);
            var CompanyList = companyRepo.GetList(ViewHelp.GetRoleId());
            ViewBag.CompanyList = new SelectList(CompanyList, "Key", "Value", CompanyId);
            var SeasonTypeList = SeasonTypeEnum.Autumn.GetEnumList();
            ViewBag.SeasonTypeList = new SelectList(SeasonTypeList, "Key", "Value", SeasonType);
        }

    }
}
