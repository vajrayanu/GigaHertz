using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GH.DAL.Model;
using GH.DAL.SQLDAL;
using SignalR.Hubs;
using GH.Web.Models;

namespace GH.Web.Controllers
{
    [Authorize]
    public class BrandController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Setting = "first active";

            ViewBag.ClientName = User.Identity.Name;
            
            BookingRepairViewModel model = new BookingRepairViewModel();
           
            return View(model);
        }

        public JsonResult GetAll()
        {
            try
            {
                var items = BrandManager.GetAll();
                
                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sBrandName, Value = m.kBrandId }) });
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
                var items = BrandManager.GetBySearch(term);

                return Json(items.Select(m => m.sBrandName), JsonRequestBehavior.AllowGet);
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
                var items = BrandManager.GetBySearch(term.Trim());

                return Json(items.Select(m => new { label = String.Format("{0}", m.sBrandName), id = m.kBrandId })
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
                var itemCount = BrandManager.GetCountByFiltering(jtSearching);
                var items = BrandManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting).ToList();
                
                return Json(new 
                {
                    Result = "OK", Records = items, TotalRecordCount = itemCount 
                },  JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Create(Brand model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                var brandCount = BrandManager.GetCountDuplicate(model.sBrandName.Trim());
                if (brandCount.Count >= 1)
                {
                    return Json(new { Result = "ERROR", Message = "Item Exists." });
                }

                model.kBrandId = Guid.NewGuid();
                BrandManager.Create(model);

                var clientName = User.Identity.Name;
                Task.Factory.StartNew(() =>
                {
                    var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                    clients.RecordCreated(clientName, model);
                });
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Edit(Brand model)
        {
           
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Brand itemFound = BrandManager.GetById(model.kBrandId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                model.dtDateAdd = itemFound.dtDateAdd;
                model.dtDateUpdate = DateTime.Now;

                BrandManager.Edit(model);

                var clientName = User.Identity.Name;
                Task.Factory.StartNew(() =>
                {
                    var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                    clients.RecordUpdated(clientName, model);
                });
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid kBrandId)
        {
            try
            {
                //Thread.Sleep(50);
                BrandManager.Delete(kBrandId);

                var clientName = User.Identity.Name;
                Task.Factory.StartNew(() =>
                {
                    var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                    clients.RecordDeleted(clientName, kBrandId);
                });
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
