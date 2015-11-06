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

        public ActionResult PostExpenseAccountFile(int id)
        {
            var message = "";
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                var FileType = file.FileName.Split('.').Last().ToLower();
                message = CheckFileType(FileType);
                if (string.IsNullOrEmpty(message))
                {
                    var BasePath = Server.MapPath("~");
                    var FileName = DateTime.Now.ToLongTimeString();
                    var FilePath = BasePath + "Upload/ExpenseAccountFile/" + id + "/";
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                    var FullFilePath = FilePath + FileName + "." + FileType;
                    file.SaveAs(FullFilePath);
                    var model = new EM_ExpenseAccount_File()
                    {
                        ExpenseAccountId = id,
                        FileName = file.FileName,
                        FileId = FileName,
                        FilePath = FullFilePath,
                        CreateDate = DateTime.Now,
                        UpLoader = ViewHelp.GetUserName(),
                        Status = (int)ExpenseAccountFileStatus.Defult

                    };
                    expenseAccountFileRepo.Add(model);
                    expenseAccountFileRepo.SaveChanges();

                    return Json(new { code = 1, model = model });
                }

            }
            return Json(new { code = 0, message = message });
        }

        public string CheckFileType(string FileType)
        {
            string[] FileTypeList = { "pdf", "jpg", "png" };
            if (FileTypeList.Contains(FileType))
                return string.Format("请上传以下格式的的文件：{0}", string.Join(",", FileTypeList));
                return "";
        }

    }
}
