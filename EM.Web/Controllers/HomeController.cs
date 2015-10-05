using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EM.Web.Core;
using EM.Data.Repositories;
using EM.Web.Core.Base;
using EM.Data.Infrastructure;
namespace EM.Web.Controllers
{
    public class HomeController : BaseController
    {

        private readonly ISystemProgromRepo systemProgromRepo = new SystemProgromRepo(new DatabaseFactory());

        
        public ActionResult Index(string Id)
        {
            var SysTypeId = Id == "ZJ" ? 1 : 2;
            var MemuList = systemProgromRepo.GetMenu(ViewHelp.GetUserId(), SysTypeId);

            return View(MemuList);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
