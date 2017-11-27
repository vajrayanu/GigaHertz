using System;
using System.Linq;
using System.Web.Mvc;
using GH.DAL.Model;
using GH.DAL.SQLDAL;
using GH.Web.Models;


namespace GH.Web.Controllers
{
    [Authorize]
    public class ProductAccessorieController : Controller
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
                var items = ProductAccessorieManager.GetAll();
               
                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kProductAccessorieId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //input jquery มันจะ pass parameter ด้วยตัว q เท่านั้น 
        public JsonResult Search(string q)
        {
            try
            {
                q = q.Trim();
                var items= ProductAccessorieManager.GetAll();

                if (!string.IsNullOrEmpty(q))
                {
                    items = ProductAccessorieManager.GetBySearch(q);
                }
                return Json(items.Select(m => new { name = m.sDescription, id = m.kProductAccessorieId })
                 , JsonRequestBehavior.AllowGet);

                //return Json(items.Select(m => new { name = m.sDescription, id = m.sDescription })
                //   , JsonRequestBehavior.AllowGet);
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
                var items = ProductAccessorieManager.GetAll();
                
                if (!string.IsNullOrEmpty(term))
                {
                    items = ProductAccessorieManager.GetBySearch(term);
                }

                return Json(items.Select(m => m.sDescription ), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult Search3()
        {
            try
            {
                var items = ProductAccessorieManager.GetAll();

                return Json(items.Select(m => m.sDescription), JsonRequestBehavior.AllowGet);
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
                var itemCount = ProductAccessorieManager.GetCountByFiltering(jtSearching);
                var items = ProductAccessorieManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting).ToList();

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
        public JsonResult Create(ProductAccessorie model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                var productAccessorieCount = ProductAccessorieManager.GetCountDuplicate(model.sDescription.Trim());
                if (productAccessorieCount >= 1)
                {
                    return Json(new { Result = "ERROR", Message = "Item Exists." });
                }


                model.dtDateAdd = DateTime.Now;
                model.kProductAccessorieId = Guid.NewGuid();

                ProductAccessorieManager.Create(model);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Edit(ProductAccessorie model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                ProductAccessorie itemFound = ProductAccessorieManager.GetById(model.kProductAccessorieId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                model.dtDateAdd = itemFound.dtDateAdd;
                model.dtDateUpdate = DateTime.Now;

                ProductAccessorieManager.Edit(model);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid kProductAccessorieId)
        {
            try
            {
                //Thread.Sleep(50);
                ProductAccessorieManager.Delete(kProductAccessorieId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
