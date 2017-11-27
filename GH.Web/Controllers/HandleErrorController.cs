using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GH.Web.Controllers
{
    public class HandleErrorController : Controller
    {
        //
        // GET: /HandleError/

        public ActionResult Error()
        {
            return View();
        }

    }
}
