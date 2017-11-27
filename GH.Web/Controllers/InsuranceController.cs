using System;
using System.Linq;
using System.Web.Mvc;
using GH.DAL.Model;
using GH.DAL.SQLDAL;
using GH.Web.Models;

namespace GH.Web.Controllers
{
    [Authorize]
    public class InsuranceController : Controller
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
                var items = InsuranceManager.GetBySearch(term);

                return Json(items.Select(m => m.sInsuranceName), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult Search2(string term)
        {
            try
            {
                var items = InsuranceManager.GetBySearch(term.Trim());

                return Json(items.Select(m => new
                {
                    label = m.sInsuranceName
                  ,
                    m.kInsuranceId
                  ,
                    m.sInsuranceName
                  ,
                    m.sAddress1
                  ,
                    m.sCity
                  ,
                    m.sZip
                  ,
                    m.sPhone
                  ,
                    m.sMobile
                  ,
                    m.sFax
                  ,
                    m.sEmailAddress
                })
                                                    , JsonRequestBehavior.AllowGet);
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
                var itemCount = InsuranceManager.GetCountByFiltering(jtSearching);
                var items = InsuranceManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting).ToList();

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
        public JsonResult Create(Insurance model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                var insCount = InsuranceManager.GetCountDuplicate(model.sInsuranceName.Trim());
                if (insCount.Count >= 1)
                {
                    return Json(new { Result = "ERROR", Message = "Item Exists." });
                }

                model.dtDateAdd = DateTime.Now;
                model.kInsuranceId = Guid.NewGuid();

                InsuranceManager.Create(model);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Edit(Insurance model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Insurance itemFound = InsuranceManager.GetById(model.kInsuranceId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                model.dtDateAdd = itemFound.dtDateAdd;
                model.dtDateUpdate = DateTime.Now;

                InsuranceManager.Edit(model);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid kInsuranceId)
        {
            try
            {
                //Thread.Sleep(50);
                InsuranceManager.Delete(kInsuranceId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
