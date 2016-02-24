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
using System.IO;
using EM.Business;

namespace EM.Web.Controllers
{
    [Description("报销管理")]
    [ControlType(SystemType.All)]
    public class ExpenseAccountController : BaseController
    {

        private readonly IExpenseAccountRepo expenseAccountRepo = new ExpenseAccountRepo(new DatabaseFactory());
        private readonly IUserRoleRepo userRoleRepo = new UserRoleRepo(new DatabaseFactory());
        private readonly IChargeCateRepo changeCateRepo = new ChargeCateRepo(new DatabaseFactory());
        private readonly ICompanyRepo companyRepo = new CompanyRepo(new DatabaseFactory());
        private readonly ICompanyLimitRepo companyLimitRepo = new CompanyLimitRepo(new DatabaseFactory());
        private readonly IExpenseAccountFileRepo expenseAccountFileRepo = new ExpenseAccountFileRepo(new DatabaseFactory());
        private readonly IExpenseAccountDetailRepo expenseAccountDetailRepo = new ExpenseAccountDetailRepo(new DatabaseFactory());


        #region 增删改查

        [Description("我的报销单")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> Index(ExpenseAccountSM Sm,int Page=1,int PageSize=20)
        {
            Sm.CompanyIds = ViewHelp.GetCompanyIds();
            var Dtos =  expenseAccountRepo.GetListByDto(Sm, ViewHelp.UserInfo(), Page, PageSize);
            var Vms = new PagedResult<ExpenseAccountListVM>()
            {
                CurrentPage = Dtos.CurrentPage,
                PageSize = Dtos.PageSize,
                RowCount = Dtos.RowCount,
                Stats = Dtos.Stats

            };
            Vms.Results = Mapper.Map<IList<ExpenseAccountListDTO>, IList<ExpenseAccountListVM>>(Dtos.Results);
            if (Request.IsAjaxRequest())
                return PartialView("_List", Vms);
            InitSearchSelect();
            return View(Vms);
        }
        [Description("未通过报销单")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> FailApproved(ExpenseAccountSM Sm, int Page = 1, int PageSize = 20)
        {
            Sm.CompanyIds = ViewHelp.GetCompanyIds();
            if (!Request.IsAjaxRequest())
            Sm.ApproveStatus =(int)ExpenseAccountApproveStatus.FailApproved;
            var Dtos =  expenseAccountRepo.GetListByDto(Sm, ViewHelp.UserInfo(), Page, PageSize);
            var Vms = new PagedResult<ExpenseAccountListVM>()
            {
                CurrentPage = Dtos.CurrentPage,
                PageSize = Dtos.PageSize,
                RowCount = Dtos.RowCount,
                Stats = Dtos.Stats
            };
            Vms.Results = Mapper.Map<IList<ExpenseAccountListDTO>, IList<ExpenseAccountListVM>>(Dtos.Results);
            if (Request.IsAjaxRequest())
                return PartialView("_List", Vms);
            InitSearchSelect(false, (int)ExpenseAccountApproveStatus.FailApproved);
            return View("Index",Vms);
        }

        [Description("确认报销单")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> ApproveIndex(ExpenseAccountSM Sm, int Page = 1, int PageSize = 20)
        {
            if (!Request.IsAjaxRequest())
                Sm.ApproveStatus = (int)ExpenseAccountApproveStatus.WaitingApprove;
            Sm.CompanyIds = ViewHelp.GetCompanyIds();
            var Dtos =  expenseAccountRepo.GetListByDto(Sm, ViewHelp.UserInfo(), Page, PageSize,true);
            var Vms = new PagedResult<ExpenseAccountListVM>()
            {
                CurrentPage = Dtos.CurrentPage,
                PageSize = Dtos.PageSize,
                RowCount = Dtos.RowCount,
                Stats = Dtos.Stats

            };
            Vms.Results = Mapper.Map<IList<ExpenseAccountListDTO>, IList<ExpenseAccountListVM>>(Dtos.Results);
            if (Request.IsAjaxRequest())
                return PartialView("_ApproveList", Vms);
            InitSearchSelect(true, Sm.ApproveStatus.Value);
            return View(Vms);
        }
        [Description("新增报销单")]
        [ActionType(RightType.View)]
        public async Task<ActionResult> Add()
        {
            var model = new EM_ExpenseAccount();
            InitBodys();
            model.OccurDate = DateTime.Now;
            model.ApplyDate = DateTime.Now;
            model.Name = ViewHelp.GetUserName();
            return View("AddOrEdit",model);
        }

        

        [HttpPost]
        public async Task<ActionResult> Add(EM_ExpenseAccount model, string FileIds, string DetailIds)
        {
            var IsRepeat = expenseAccountRepo.GetMany(o => o.EANumber == model.EANumber).Any();
            if (IsRepeat)
            {
                return Json(new { code = 0, message = "报销单号已存在，请修改报销单号" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(DetailIds))
            {
                return Json(new { code = 0, message = "报销单明细上传失败，请重试" }, JsonRequestBehavior.AllowGet);
            }

            model.CreateDate = DateTime.Now;
            model.Creater = ViewHelp.GetUserName();
            model.ModifyDate = DateTime.Now;
            model.Modifier = ViewHelp.GetUserName();
            expenseAccountRepo.Add(model);
            var result = expenseAccountRepo.SaveChanges();
                //添加状态变更记录
            expenseAccountRepo.AddApproveHistory(model.Id,model.ApproveStatus,"",ViewHelp.GetUserName());
            if (result > 0)
            {
                //更新单身
                await expenseAccountDetailRepo.UpdateDetailExpenseAccountId(model.Id, DetailIds);
                await expenseAccountFileRepo.UpdateFileExpenseAccountId(model.Id, FileIds);
                return Json(new { code = 1, model = model }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { code = 0, message = "保存失败，请重试" }, JsonRequestBehavior.AllowGet);
        }

        [Description("编辑报销单")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> Edit(int Id)
        {
            var model = expenseAccountRepo.GetById(Id);
            ViewBag.ExpenseAccountId = Id;
            InitBodys(Id);
            return View("AddOrEdit", model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(EM_ExpenseAccount model, string FileIds, string DetailIds)
        {
            var entity = expenseAccountRepo.GetById(model.Id);
            if (model == null)
            {
                return Json(new { code = 0, message = "报销单不存在！" }, JsonRequestBehavior.AllowGet);
            }
            var IsRepeat = expenseAccountRepo.GetMany(o => o.EANumber == model.EANumber&&o.Id!=model.Id).Any();
            if (IsRepeat)
            {
                return Json(new { code = 0, message = "报销单号已存在，请修改报销单号" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(DetailIds))
            {
                return Json(new { code = 0, message = "报销单明细上传失败，请重试" }, JsonRequestBehavior.AllowGet);
            }
            entity = Mapper.Map<EM_ExpenseAccount, EM_ExpenseAccount>(model, entity);
            entity.ModifyDate = DateTime.Now;
            entity.Modifier = ViewHelp.GetUserName();
            var result = expenseAccountRepo.SaveChanges();
            //添加状态变更记录
            expenseAccountRepo.AddApproveHistory(model.Id, model.ApproveStatus, "", ViewHelp.GetUserName());
            if (result > 0)
            {
                //更新单身
                await expenseAccountDetailRepo.UpdateDetailExpenseAccountId(model.Id, DetailIds);
                await expenseAccountFileRepo.UpdateFileExpenseAccountId(model.Id, FileIds);
                Log(model);
                return Json(new { code = 1, model = model });
            }
            else
                return Json(new { code = 0, message = "保存失败，请重试" });
        }




        [Description("查看报销单")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> Browse(int Id,bool IsApprove=false)
        {
            var model = expenseAccountRepo.GetById(Id);
            ViewBag.ExpenseAccountId = Id;
            InitBodys(Id);
            if (IsApprove)
                return View("ApproveBrowse", model);
            return View(model);
        }
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteFile(int Id)
        {
            var result = await expenseAccountFileRepo.UpdateDeleteStatus(Id);
            return Json(new { code = result ? 1 : 0 }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除明细
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
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

        public ActionResult GetDetail(int Id)
        {
            var model = new EM_ExpenseAccount_Detail();
            if (Id != 0)
                model = expenseAccountDetailRepo.GetById(Id);
            else
                model.OccurDate = DateTime.Now;
            InitSelect(model.CateId, model.CompanyId);
            return PartialView("_AddOrEditDetail", model);
        }

        public ActionResult BrowseDetail(int Id)
        {
            var model = new EM_ExpenseAccount_Detail();
            if (Id != 0)
                model = expenseAccountDetailRepo.GetById(Id);
            else
                model.OccurDate = DateTime.Now;
            InitSelect(model.CateId, model.CompanyId);
            return PartialView("_BrowseDetail", model);
        }
        public ActionResult SaveDetail(EM_ExpenseAccount_Detail model)
        {
            model.Modifier = ViewHelp.GetUserName();
            model.ModifyTime = DateTime.Now;
            if(model.Id==0)
            {
                model.Creater = ViewHelp.GetUserName();
                model.CreateTime = DateTime.Now;
                expenseAccountDetailRepo.Add(model);
            }
            else
            {
                var entity = expenseAccountDetailRepo.GetById(model.Id);
                Log(entity);
                entity = Mapper.Map<EM_ExpenseAccount_Detail, EM_ExpenseAccount_Detail>(model, entity);
            }
            var result=expenseAccountDetailRepo.SaveChanges();
            var dto = Mapper.Map<EM_ExpenseAccount_Detail, ExpenseAccountDetailListDTO>(model);
            return Json(new { code = result, message = "保存失败，请重试", model = dto });
        }

        public async Task<ActionResult> BrowseFile(int Id)
        {
            var Dto = await expenseAccountFileRepo.GetDtos(Id);
            if (Dto.UpLoader != ViewHelp.GetUserName() && !ViewHelp.IsAdmin())
                return RedirectToAction("noright", "error");
            var Vm = Mapper.Map<ExpenseAccountFileVM>(Dto);
            return View(Vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ActionResult"></typeparam>
        /// <param name="?"></param>
        /// <returns></returns> 
        [Description("导出报销单")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> ToExcel(ExpenseAccountSM Sm)
        {
            Sm.CompanyIds = ViewHelp.GetCompanyIds();
            var Dtos = expenseAccountRepo.GetExcelListByDto(Sm, ViewHelp.UserInfo());
            var Vms = Mapper.Map<List<ExpenseAccountExcelVM>>(Dtos);
            var FilePath = ExpenseAccountManager.Instance.ToExcel(Vms);
            Log(Vms, "路径:" + FilePath);
            return File(new FileStream(FilePath, FileMode.Open), "application/octet-stream","报销清单_"+ViewHelp.GetUserName() +"_" +DateTime.Now.ToString("yyyyMMddss")+".xls");
        }

        #endregion


        #region 表单操作
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

        [Description("审核报销单")]
        [ActionType(RightType.Form, "ApproveIndex")]
        public async Task<ActionResult> UpdataApproveStatus(int Id, int ApproveStatus, string Message)
        {

            var result = expenseAccountRepo.UpdataApproveStatus(Id, ApproveStatus, Message, ViewHelp.GetUserName());
            //发送
            return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
        }

        [Description("提交报销单")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> SumbitExpenseAccount(int Id)
        {

            var result = expenseAccountRepo.UpdataApproveStatus(Id, (int)ExpenseAccountApproveStatus.WaitingApprove, "", ViewHelp.GetUserName());
            return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
        }

        [Description("撤销报销单")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> CancelExpenseAccount(int Id)
        {

            var result = expenseAccountRepo.UpdataApproveStatus(Id,(int) ExpenseAccountApproveStatus.Created, "", ViewHelp.GetUserName());
            return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
        }
        #endregion
      
        #region 批量操作
        [Description("批量删除报销单")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> Deletes(string  Ids)
        {
            var IdsInt = Ids.ToInts();
            foreach (var Id in IdsInt)
            {
                var model = expenseAccountRepo.GetById(Id);
                if (model == null)
                {
                    return Json(new { code = 0, message = "报销单不存在！,单据号" + model.EANumber }, JsonRequestBehavior.AllowGet);
                }
                expenseAccountRepo.Delete(model);
                if (expenseAccountRepo.SaveChanges() > 0)
                {
                    Log(model);
                }
            }
            return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
            
        }

        [Description("批量审核报销单")]
        [ActionType(RightType.Form, "ApproveIndex")]
        public async Task<ActionResult> UpdataApproveStatuss(string Ids, int ApproveStatus, string Message)
        {
            var IdsInt = Ids.ToInts();
            foreach (var Id in IdsInt)
            { 
              var result = expenseAccountRepo.UpdataApproveStatus(Id, ApproveStatus, Message, ViewHelp.GetUserName());
            }
            //发送
            return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
        }

        [Description("批量提交报销单")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> SumbitExpenseAccounts(string Ids)
        {
            var IdsInt = Ids.ToInts();
            foreach (var Id in IdsInt)
            {
             var result = expenseAccountRepo.UpdataApproveStatus(Id,(int) ExpenseAccountApproveStatus.WaitingApprove, "", ViewHelp.GetUserName());
            }
            return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
        }

        [Description("批量撤销报销单")]
        [ActionType(RightType.Form, "Index")]
        public async Task<ActionResult> CancelExpenseAccounts(string Ids)
        {
            var IdsInt = Ids.ToInts();
            foreach (var Id in IdsInt)
            {
                var result = expenseAccountRepo.UpdataApproveStatus(Id,(int) ExpenseAccountApproveStatus.Created, "", ViewHelp.GetUserName());
            }
            return Json(new { code = 1 }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 私有函数
        

        private void InitSelect(int CateId=0,int CompanyId=0)
        {
            var CateList = changeCateRepo.GetList(ViewHelp.GetRoleType(), CateDropType.Form);
          ViewBag.CateList = new SelectList(CateList, "Key", "Value", CateId);
          //录入人和amdin在录入的时候可以录入所有人的公司，
          //也只有录入人和admin可以编辑和新增，所以目前都是可以选所有的
          var CompanyList = companyRepo.GetList();
          ViewBag.CompanyList = new SelectList(CompanyList, "Key", "Value", CompanyId);
        }


        private void InitSearchSelect(bool IsFromApprove = false, int ApproveStatus=0)
        {
            var DateTypeList = ExpenseAccountDateType.OccurDate.GetEnumList();
            ViewBag.DateTypeList = new SelectList(DateTypeList, "Key", "Value");
            var CateList = changeCateRepo.GetList(ViewHelp.GetRoleType(),CateDropType.Search);
            ViewBag.CateList = new SelectList(CateList, "Key", "Value");
            var ApproveStatusList = ExpenseAccountApproveStatus.Created.GetEnumList();
            if (IsFromApprove)
            {

                ApproveStatusList = ApproveStatusList.Where(o => o.Key != (int)ExpenseAccountApproveStatus.Created).ToList();
            }

            ViewBag.ApproveStatusList = new SelectList(ApproveStatusList, "Key", "Value", ApproveStatus);
          
            var CompanyList = companyRepo.GetList(ViewHelp.GetRoleId());
            ViewBag.CompanyList = new SelectList(CompanyList, "Key", "Value");
        }
        private void InitBodys(int ExpenseAccountId=0)
        {
            ViewBag.ExpenseAccountId = ExpenseAccountId;
            if (ExpenseAccountId==0)
            {
                ViewBag.Files = new List<EM_ExpenseAccount_File>();
                ViewBag.Details =new List<ExpenseAccountDetailListDTO>();

            }
            else
            {
                ViewBag.Files = expenseAccountFileRepo.GetListByExpenseAccountId(ExpenseAccountId);
                var entitys = expenseAccountDetailRepo.GetListByExpenseAccountId(ExpenseAccountId, ViewHelp.GetDetailCompanyIds());
                var Details = Mapper.Map<List<EM_ExpenseAccount_Detail>, List<ExpenseAccountDetailListDTO>>(entitys);
                ViewBag.Details = Details;

            }
        }

        public async Task< JsonResult> GetNewPublicId()
        {
            var Num=  await expenseAccountRepo.GetNewPublicId();
            return Json(new { code = 1, EANumber = Num }, JsonRequestBehavior.AllowGet);
        }


        #endregion


    }
}
