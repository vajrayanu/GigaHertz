using System;
using System.Linq;
using System.Web.Mvc;
using GH.DAL.Model;
using GH.DAL.SQLDAL;
using GH.Web.Models;

namespace GH.Web.Controllers
{
    [Authorize]
    public class TransportController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Setting = "first active";

            BookingRepairViewModel model = new BookingRepairViewModel();

         
            return View(model);
        }

        public JsonResult Search(string term)
        {
            try
            {
                var items = TransportManager.GetBySearch(term);

                return Json(items.Select(m => m.sTransportName), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Lists(string jtSearching = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                //Thread.Sleep(200);
                var itemCount = TransportManager.GetCountByFiltering(jtSearching);
                var items = TransportManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting).ToList();

                return Json(new
                {
                    Result = "OK",
                    Records = items,
                    TotalRecordCount = itemCount
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Create(Transport model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                model.dtDateAdd = DateTime.Now;
                model.kTransportId = Guid.NewGuid();

                TransportManager.Create(model);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Edit(Transport model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Transport itemFound = TransportManager.GetById(model.kTransportId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }

                /*---- bind exists data -------------------------------------*/
                model.dtDateAdd = itemFound.dtDateAdd;
                model.dtDateUpdate = DateTime.Now;
                /*----------------------------------------------------------*/

                TransportManager.Edit(model);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid kTransportId)
        {
            try
            {
                //Thread.Sleep(50);
                TransportManager.Delete(kTransportId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
