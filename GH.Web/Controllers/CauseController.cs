using System;
using System.Linq;
using System.Web.Mvc;
using GH.DAL.Model;
using GH.DAL.SQLDAL;
using GH.Web.Models;

namespace GH.Web.Controllers
{
    [Authorize]
    public class CauseController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Setting = "first active";

            BookingRepairViewModel model = new BookingRepairViewModel();
          
            return View(model);
        }

        public JsonResult GetAll()
        {
            try
            {
                var items = CauseManager.GetAll();
                //return Json(items.Select(m => m.sDescription), JsonRequestBehavior.AllowGet);
                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kCauseId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult Search(string q)
        {
            try
            {
                q = q.Trim();
                var items = CauseManager.GetAll();

                if (!string.IsNullOrEmpty(q))
                {
                    items = CauseManager.GetBySearch(q);
                }
                return Json(items.Select(m => new { name = String.Format("[{0}] {1} ราคา {2} บาท",m.sCode, m.sDescription, m.dPrice), id = m.sDescription, Readonly = "true" })
                 , JsonRequestBehavior.AllowGet);
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
                term = term.Trim();
                var items = CauseManager.GetAll();

                if (!string.IsNullOrEmpty(term))
                {
                    items = CauseManager.GetBySearch(term);
                }
                return Json(items.Select(m => new { label = String.Format("[{0}] {1} ราคา {2} บาท",m.sCode ,m.sDescription, m.dPrice),description = m.sDescription, price = m.dPrice })
                 , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult Search2Prefix(string term)
        {
            try
            {
                term = term.Trim();
                var items = CauseManager.GetAll();

                if (!string.IsNullOrEmpty(term))
                {
                    items = CauseManager.GetBySearch(term);
                }
                return Json(items.Select(m => new { label = String.Format("[{0}] {1} ราคา {2}",m.sCode, m.sDescription, m.dPrice), description = m.sDescription, price = m.dPrice })
                 , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //For Create Cause on Claim Module
        public JsonResult Search3(string term)
        {
            try
            {
                term = term.Trim();
                var items = CauseManager.GetAll();

                if (!string.IsNullOrEmpty(term))
                {
                    items = CauseManager.GetBySearch(term);
                }
                return Json(items.Select(m => new { label = String.Format("[{0}] {1}",m.sCode, m.sDescription), description = m.sDescription, price = m.dPrice })
                 , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult GetById(Guid Id)
        {
            try
            {
                Cause itemFound = CauseManager.GetById(Id);

                return Json(itemFound, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult GetByName(String Name)
        {
            try
            {
                Cause itemFound = CauseManager.GetByName(Name.Trim());

                return Json(itemFound, JsonRequestBehavior.AllowGet);
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
                var itemCount = CauseManager.GetCountByFiltering(jtSearching);
                var items = CauseManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting).ToList();

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
        public JsonResult Create(Cause model)
        {
            try
            {
                //check model is valid
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                //check dupliate when create item before save to database , itemcount should be no over 1
                var itemCount = CauseManager.GetCountDuplicate(model.sDescription.Trim());
                if (itemCount != 0)
                {
                    return Json(new { Result = "ERROR", Message = "Name already exists." });
                }
                model.kCauseId = Guid.NewGuid();
                model.dtDateUpdate = DateTime.Now;
                //if everthing is ok , then save to database 
                CauseManager.Create(model);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Edit(Cause model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                //check dupliate when edit item before save to database , itemcount should be no over 2
                var itemCount = CauseManager.GetCountDuplicate(model.sDescription.Trim());
                if (itemCount > 1)
                {
                    return Json(new { Result = "ERROR", Message = "Cause already exists." });
                }

                //check item found
                Cause itemFound = CauseManager.GetById(model.kCauseId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                model.dtDateAdd = itemFound.dtDateAdd;
                model.dtDateUpdate = DateTime.Now;

                CauseManager.Edit(model);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid kCauseId)
        {
            try
            {
                //Thread.Sleep(50);
                CauseManager.Delete(kCauseId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
