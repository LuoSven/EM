using EM.Data.Infrastructure;
using EM.Data.Repositories;
using EM.Model.Entities;
using EM.Web.Core;
using EM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace EM.Web.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/
        private readonly IExpenseAccountFileRepo expenseAccountFileRepo = new ExpenseAccountFileRepo(new DatabaseFactory());
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PostExpenseAccountFile(int id,string remark="")
        {
            var message = "";
            if(string.IsNullOrEmpty(remark))
            {
                return Json(new { code = 0, message = "必须填写附件说明" }, "text/html;charset=utf-8");
            }
            var FilePathId = id == 0 ? "temp" : id.ToString();
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                var FileType = file.FileName.Split('.').Last().ToLower();
                message = CheckFileType(FileType);
                if (string.IsNullOrEmpty(message))
                {
                    var BasePath = Server.MapPath("~");
                    var FileName = DateTime.Now.ToBinary().ToString();
                    var FilePath =  "Upload\\ExpenseAccountFile\\" + FilePathId + "\\";
                    if (!Directory.Exists(BasePath+FilePath))
                    {
                        Directory.CreateDirectory(BasePath+FilePath);
                    }
                    var FullFilePath = BasePath+FilePath + FileName + "." + FileType;
                    file.SaveAs(FullFilePath);
                    var model = new EM_ExpenseAccount_File()
                    {
                        ExpenseAccountId = id,
                        FileName = file.FileName,
                        FileId = FileName,
                        FilePath = FilePath + FileName + "." + FileType,
                        CreateDate = DateTime.Now,
                        UpLoader = ViewHelp.GetUserName(),
                        Status = (int)ExpenseAccountFileStatus.NoRelated,
                        Remark=remark

                    };
                    expenseAccountFileRepo.Add(model);
                    expenseAccountFileRepo.SaveChanges();

                    return Json(new { code = 1, model = new { Id = model.Id, FileName = model.FileName, CreateDate = model.CreateDate.ToString("yyyy-MM-dd HH:mm:ss") } }, "text/html;charset=utf-8");
                }

            }
            return Json(new { code = 0, message = message }, "text/html;charset=utf-8");
        }

        public string CheckFileType(string FileType)
        {
            string[] FileTypeList = { "pdf", "jpg", "png" };
            if (!FileTypeList.Contains(FileType))
                return string.Format("请上传以下格式的的文件：{0}", string.Join(",", FileTypeList));
                return "";
        }

    }
}
