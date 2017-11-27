using System;
using System.Linq;
using System.Web.Mvc;
using GH.DAL.Model;
using GH.DAL.SQLDAL;
using GH.Web.Models;


namespace GH.Web.Controllers
{
    [Authorize]
    public class ProductTypeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Setting = "first active";

            BookingRepairViewModel model = new BookingRepairViewModel();
           
            return View(model);
        }

        [HttpPost]
        public JsonResult GetAll()
        {
            try
            {
                var items = ProductTypeManager.GetAll();

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kProductTypeId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult Search(string term)
        {
            try
            {
                var items = ProductTypeManager.GetBySearch(term);

                return Json(items.Select(m => m.sDescription), JsonRequestBehavior.AllowGet);
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
                var items = ProductTypeManager.GetBySearch(term.Trim());

                return Json(items.Select(m => new { label = String.Format("{0}", m.sDescription), id = m.kProductTypeId })
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
                var itemCount = ProductTypeManager.GetCountByFiltering(jtSearching);
                var items = ProductTypeManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting).ToList();

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
        public JsonResult Create(ProductType model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                var productTypeCount = ProductTypeManager.GetCountDuplicate(model.sDescription.Trim());
                if (productTypeCount.Count >= 1)
                {
                    return Json(new { Result = "ERROR", Message = "Item Exists." });
                }

                model.dtDateAdd = DateTime.Now;
                model.kProductTypeId = Guid.NewGuid();

                ProductTypeManager.Create(model);
                return Json(new { Result = "OK", Record = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Edit(ProductType model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                ProductType itemFound = ProductTypeManager.GetById(model.kProductTypeId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                model.dtDateAdd = itemFound.dtDateAdd;
                model.dtDateUpdate = DateTime.Now;

                ProductTypeManager.Edit(model);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid kProductTypeId)
        {
            try
            {
                //Thread.Sleep(50);
                ProductTypeManager.Delete(kProductTypeId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
