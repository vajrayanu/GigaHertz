using System;
using System.Linq;
using System.Web.Mvc;
using GH.DAL.Model;
using GH.DAL.SQLDAL;
using GH.Web.Models;


namespace GH.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
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
                var items = CustomerManager.GetAll();

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sCustomerName, Value = m.kCustomerId }) });
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
                var items = CustomerManager.GetBySearch(term.Trim());

                //return Json(items.Select(m => new { value = m.kCustomerId, label = m.sCustomerName }), JsonRequestBehavior.AllowGet);

                return Json(items.Select(m => m.sCustomerName), JsonRequestBehavior.AllowGet);
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
                var items = CustomerManager.GetBySearch(term.Trim());

                return Json(items.Select(m => new { label = m.sCustomerName
                                                    , m.kCustomerId
                                                    , m.sCustomerName
                                                    , m.sAddress1
                                                    , m.sCity
                                                    , m.sZip
                                                    , m.sPhone
                                                    , m.sMobile
                                                    , m.sFax
                                                    , m.sEmailAddress })
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
                Customer itemFound = CustomerManager.GetById(Id);

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
                Customer itemFound = CustomerManager.GetByName(Name.Trim());

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
                if (string.IsNullOrEmpty(jtSearching))
                    jtSearching = "";

                var itemCount = CustomerManager.GetCountByFiltering(jtSearching.Trim());
                var items = CustomerManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting).ToList();

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
        public JsonResult Create(Customer model)
        {
            try
            {
                //check model is valid
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                //check dupliate when create item before save to database , itemcount should be no over 1
                var itemCount = CustomerManager.GetCountDuplicate(model.sCustomerName.Trim());
                if(itemCount.Count >= 1)
                {
                    return Json(new { Result = "ERROR", Message = "Item Exists." });
                }

                model.sCustomerName = model.sCustomerName.Trim();
                model.dtDateAdd = DateTime.Now;
                model.kCustomerId = Guid.NewGuid();

                //if everthing is ok , then save to database 
                CustomerManager.Create(model);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult Create2(Customer model)
        {
            try
            {
                var customerCount = CustomerManager.GetCountDuplicate(model.sCustomerName.Trim());
                if (customerCount.Count <= 0)
                {
                    model.kCustomerId = Guid.NewGuid();
                    model.dtDateAdd = DateTime.Now;
                    CustomerManager.Create(model);
                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
     

        [HttpPost]
        public JsonResult Edit(Customer model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                //check dupliate when edit item before save to database , itemcount should be no over 1
                var itemCount = CustomerManager.GetCountDuplicate(model.sCustomerName.Trim());
                if (itemCount.Count > 1)
                {
                    return Json(new { Result = "ERROR", Message = "Name already exists." });
                }

                //check item found
                Customer itemFound = CustomerManager.GetById(model.kCustomerId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                model.sCustomerName = model.sCustomerName.Trim();
                model.dtDateAdd = itemFound.dtDateAdd;
                model.dtDateUpdate = DateTime.Now;

                //if everthing is ok , then save to database 
                CustomerManager.Edit(model);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid kCustomerId)
        {
            try
            {
                //Thread.Sleep(50);
                CustomerManager.Delete(kCustomerId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        public JsonResult GetRepairs(Guid id)
        {
            try
            {
                var customer = CustomerManager.GetById(id);

                var items = CustomerManager.GetRepairs(customer);
                return Json(new
                {
                    Result = "OK",
                    Records = items.Select(m => new
                    {
                        RepairNo = m.sRepairNo
                        , Product = m.Product.sProductName
                        , Status = m.RepairStatuies.FirstOrDefault().vWorkingStatus
                        , WorkingDate = m.RepairStatuies.FirstOrDefault().vWorkingDate
                        , ClosingDate = m.vDateClose
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
