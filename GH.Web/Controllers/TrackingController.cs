using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GH.DAL.SQLDAL;

namespace GH.DAL.Controllers
{
    public class TrackingController : Controller
    {
        //
        // GET: /Tracking/

        public ActionResult Index()
        {
            ViewBag.Counter = TrackingCounterManager.GetNextItemNo();

            return View();
        }

        public JsonResult ProductTracking(string term)
        {
            try
            {
                var items = RepairManager.GetByTracking(term.Trim());

                if (items == null)
                {
                    return Json(new { result = "ไม่พบข้อมูล/not found." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TrackingCounterManager.IncreaseNextItemNo();
                    return Json(new { result = items.vProductTracking }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = "ไม่พบข้อมูล/not found." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
