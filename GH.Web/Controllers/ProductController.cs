using System;
using System.Linq;
using System.Web.Mvc;
using GH.DAL.Model;
using GH.DAL.SQLDAL;
using GH.DAL.Helpers;
using GH.Web.Models;

namespace GH.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
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
                var items = ProductManager.GetAll();

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sProductName, Value = m.kProductId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

       
        public ActionResult GetAuthors(string q)
        {
            return View();
        }

        public JsonResult Search(string term)
        {
            try
            {
                term = term.Trim();

                var items = ProductManager.GetBySearch(term);

                return Json(items.Select(m => m.sProductName), JsonRequestBehavior.AllowGet);
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
                var items = ProductManager.GetAll();

                if (!string.IsNullOrEmpty(term))
                {
                    items = ProductManager.GetBySearch(term);
                }
                return Json(items.Select(m => new
                {
                    label = m.sProductName,
                    m.kProductId ,
                    m.sProductName ,
                    m.kBrandId,
                    sBrandDescription = m.vBrandDescription ,
                    m.kProductTypeId,
                    sProductTypeDescription = m.vProductTypeDescription //,
                   // m.sProductModel
                })
                , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult SearchJobID(string term)
        {
            try
            {
                term = term.Trim();

                var items = RepairManager.GetBySearch(term);
                //items = items.Where(m => m.RepairStatuies[0].WorkingStatus.iDefault == (int)Working.Claim).ToList();


                return Json(items.Select(m => new
                {
                    label = m.sRepairNo + " - " + m.Product.sProductName,
                    m.kRepairId  ,
                    m.sRepairNo ,
                    sProductName = m.vProductName  ,
                    kProductTypeId = m.Product.kProductTypeId,
                    kBrandId = m.Product.kBrandId,
                    sBrandDescription = m.Product.vBrandDescription  ,
                    sProductTypeDescription = m.Product.vProductTypeDescription ,
                    m.Product.kProductId ,
                    m.Product.sProductModel ,
                    m.sSerial,
                    dtInsuranceExpire =m.dtInsuranceExpire !=null ? m.dtInsuranceExpire.GetValueOrDefault().AddYears(543).ToString(DateExtension.DateNumber()) : ""
                })
                , JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult SearchJobIDForClaim(string term)
        {
            try
            {
                term = term.Trim();

                var items = RepairManager.GetBySearchForClaim(term);
                //items = items.Where(m => m.RepairStatuies[0].WorkingStatus.iDefault == (int)Working.Claim).ToList();


                return Json(items.Select(m => new
                {
                    label = m.sRepairNo + " - " + m.Product.sProductName,
                    m.kRepairId,
                    m.sRepairNo,
                    sProductName = m.vProductName,
                    kProductTypeId = m.Product.kProductTypeId,
                    kBrandId = m.Product.kBrandId,
                    sBrandDescription = m.Product.vBrandDescription,
                    sProductTypeDescription = m.Product.vProductTypeDescription,
                    m.Product.kProductId,
                    m.Product.sProductModel,
                    m.sSerial,
                    dtInsuranceExpire = m.dtInsuranceExpire != null ? m.dtInsuranceExpire.GetValueOrDefault().AddYears(543).ToString(DateExtension.DateNumber()) : ""
                })
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
                Product itemFound = ProductManager.GetById(Id);

                return Json(itemFound, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult GetFromRepair(Guid Id)
        {
            try
            {
                var itemFounds = ProductManager.GetFromRepair(Id);
                return Json(new { Result = "OK", Records = itemFounds });
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
                Product itemFound = ProductManager.GetByName(Name.Trim());

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
                var itemCount = ProductManager.GetCountByFiltering(jtSearching);
                var items = ProductManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting).ToList();

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
        public JsonResult Create(Product model)
        {
            try
            {
                //check model is valid
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                //check dupliate when create item before save to database , itemcount should be no over 1
                var itemCount = ProductManager.GetCountDuplicate(
                                    model.sProductModel.Trim()
                                    //, model.Brand.sBrandName.Trim()
                                    //, model.ProductType.sDescription.Trim()
                );

                if (itemCount.Count >= 1)
                {
                    return Json(new { Result = "ERROR", Message = "Item Exists." });
                }

                model.dtDateAdd = DateTime.Now;
                model.kProductId = Guid.NewGuid();

                //if everthing is ok , then save to database 
                ProductManager.Create(model);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Edit(Product model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                //check dupliate when edit item before save to database , itemcount should be no over 2
                var itemCount = ProductManager.GetCountDuplicate(
                                     model.sProductModel.Trim()
                                     //, model.Brand.sBrandName.Trim()
                                     //, model.ProductType.sDescription.Trim()
                );

                if (itemCount.Count > 1)
                {
                    return Json(new { Result = "ERROR", Message = "Product already exists." });
                }

                //check item found
                Product itemFound = ProductManager.GetById(model.kProductId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                model.dtDateAdd = itemFound.dtDateAdd;
                model.dtDateUpdate = DateTime.Now;

                ProductManager.Edit(model);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid kProductId)
        {
            try
            {
                //Thread.Sleep(50);
                ProductManager.Delete(kProductId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
