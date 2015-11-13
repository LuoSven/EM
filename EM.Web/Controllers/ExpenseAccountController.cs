using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EM.Web.Core;
using EM.Common;
using System.ComponentModel;
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
    [Description("报销管理")]
    [ControlType(SystemType.All)]
    public class ExpenseAccountController : BaseController
    {

        private readonly IExpenseAccountRepo expenseAccountRepo = new ExpenseAccountRepo(new DatabaseFactory());
        private readonly IUserRoleRepo userRoleRepo = new UserRoleRepo(new DatabaseFactory());
        private readonly IChangeCateRepo changeCateRepo = new ChangeCateRepo(new DatabaseFactory());
        private readonly ICompanyRepo companyRepo = new CompanyRepo(new DatabaseFactory());
        private readonly IExpenseAccountFileRepo expenseAccountFileRepo = new ExpenseAccountFileRepo(new DatabaseFactory());
        private readonly IExpenseAccountDetailRepo expenseAccountDetailRepo = new ExpenseAccountDetailRepo(new DatabaseFactory());


        #region 增删改查

        [Description("我的报销单")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> Index(ExpenseAccountSM Sm)
        {
            Sm.CompanyIds = ViewHelp.GetCompanyIds();
            var Dtos = await expenseAccountRepo.GetListByDtoAsync(Sm);
            var Vms = Mapper.Map<List<ExpenseAccountListDTO>, List<ExpenseAccountListVM>>(Dtos);
            if (Request.IsAjaxRequest())
                return PartialView("_List", Vms);
            ViewBag.CateList = changeCateRepo.GetList();
            return View(Vms);
        }
        [Description("新增报销单")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> Add()
        {
            InitSelect(0, 0);
            var model = new EM_ExpenseAccount();
            ViewBag.Files = new List<EM_ExpenseAccount_File>();
            ViewBag.ExpenseAccountId = 0;
            model.OccurDate = DateTime.Now;
            model.ApplyDate = DateTime.Now;
            model.Name = ViewHelp.GetUserName();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(EM_ExpenseAccount model, string FileIds, string DetailIds)
        {
            model.CreateDate = DateTime.Now;
            model.Creater = ViewHelp.GetUserName();
            model.ModifyDate = DateTime.Now;
            model.Modifier = ViewHelp.GetUserName();
            var result = expenseAccountRepo.SaveChanges();
            if (result > 0)
            {
                //更新单身
                await expenseAccountDetailRepo.UpdateFileExpenseAccountId(model.Id, DetailIds);
                await expenseAccountFileRepo.UpdateFileExpenseAccountId(model.Id, FileIds);
                return Json(new { code = 1, model = model }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { code = 0, messgage = "保存失败，请重试" }, JsonRequestBehavior.AllowGet);
        }

        [Description("编辑报销单")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> Edit(int Id)
        {
            var model = expenseAccountRepo.GetById(Id);
            ViewBag.ExpenseAccountId = Id;

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(EM_ExpenseAccount model, string FileIds, string DetailIds)
        {
            var entity = expenseAccountRepo.GetById(model.Id);
            if (model == null)
            {
                return Json(new { code = 0, message = "报销单不存在！" }, JsonRequestBehavior.AllowGet);
            }

            entity = Mapper.Map<EM_ExpenseAccount, EM_ExpenseAccount>(model, entity);
            entity.ModifyDate = DateTime.Now;
            entity.Modifier = ViewHelp.GetUserName();
            var result = expenseAccountRepo.SaveChanges();
            if (result > 0)
            {
                //更新单身
                await expenseAccountDetailRepo.UpdateFileExpenseAccountId(model.Id, DetailIds);
                await expenseAccountFileRepo.UpdateFileExpenseAccountId(model.Id, FileIds);
                Log(model);
                return Json(new { code = 1, model = model });
            }
            else
                return Json(new { code = 0, message = "保存失败，请重试" });
        }

        [Description("删除报销单")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> Delete(int Id)
        {
            var model = expenseAccountRepo.GetById(Id);
            if (model == null)
            {
                return Json(new { code = 0, message = "报销单不存在！" }, JsonRequestBehavior.AllowGet);
            }
            expenseAccountRepo.Delete(model);
            if (expenseAccountRepo.SaveChanges() > 0)
            {
                Log(model);
                return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { code = 0, message = "删除失败，请重试" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DeleteFile(int Id)
        {
            var result = await expenseAccountFileRepo.UpdateDeleteStatus(Id);
            return Json(new { code = result ? 1 : 0 }, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> DeleteDetail(int Id)
        {
            var result = 1;
            var model = expenseAccountDetailRepo.GetById(Id);
            if (model != null)
            {
                expenseAccountDetailRepo.Delete(model);
                result = expenseAccountDetailRepo.SaveChanges();
            }
            return Json(new { code = result }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> EditDetail(int Id)
        {
            var model = expenseAccountFileRepo.GetById(Id);

            return View(model);
        }
        public async Task<ActionResult> ViewFile(int Id)
        {
            var Dto = await expenseAccountFileRepo.GetDtos(Id);
            if (Dto.UpLoader != ViewHelp.GetUserName() && !ViewHelp.IsAdmin())
                return RedirectToAction("noright", "error");
            var Vm = Mapper.Map<ExpenseAccountFileVM>(Dto);
            return View(Vm);
        }

        #endregion

        #region 私有函数
        

        private void InitSelect(int CateId,int CompanyId)
        {
          var CateList = changeCateRepo.GetList();
          ViewBag.CateList = new SelectList(CateList, "Key", "Value", CateId);
          var CompanyList = companyRepo.GetList(ViewHelp.GetRoleId());
          ViewBag.CompanyList = new SelectList(CompanyList, "Key", "Value", CompanyId);
        }

        private void InitBodys(int )

        public async Task< JsonResult> GetNewPublicId()
        {
            var Num=  await expenseAccountRepo.GetNewPublicId();
            return Json(new { code = 1, EANumber = Num }, JsonRequestBehavior.AllowGet);
        }


        #endregion


    }
}
