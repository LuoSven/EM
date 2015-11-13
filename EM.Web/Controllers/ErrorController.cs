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
    public class ErrorController : Controller
    {
        public ActionResult NoRight()
        {
            return View();
        }
    }
}
