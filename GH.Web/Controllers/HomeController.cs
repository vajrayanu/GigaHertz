using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GH.DAL.SQLDAL;
using GH.DAL.Model;
using System.Threading.Tasks;
using SignalR.Hubs;
using GH.Web.Models;
using System.Web.Security;
using GH.DAL.Helpers;

namespace GH.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Error()
        {
            BookingRepairViewModel model = new BookingRepairViewModel();
            return View("Error",model);
        }
        

        public JsonResult GetCloseStatusByPosition()
        {
            try
            {
                Staff staff = new Staff{ kStaffId=Guid.Empty, sStaffName="--ผลงานช่าง--" };
                List<Staff> Staffs = new List<Staff>();
                Staffs.Add(staff);
                

                var items = StaffManager.GetAll().Where(m=>m.StaffPosition.sDescription=="ช่าง").ToList();
                foreach (var s in items)
                {
                    Staffs.Add(s);
                }
                return Json(new { Result = "OK", Options = Staffs.Select(m => new { DisplayText = m.sStaffName, Value = m.kStaffId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //pass jobid
        public JsonResult GetStaffRelatedInJob(Guid Id)
        {
            try
            {
                var items = RepairStatusManager.GetByRepairId(Id);

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.Staff.sStaffName, Value = m.kStaffId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        public JsonResult GetDayWarranty()
        {
            try
            {
                var items = Helpers.HelperController.DayWarranty();

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.Text, Value = m.Text }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        
        public JsonResult GetStaffByPosition()
        {
            try
            {
                var id = (Guid)Membership.GetUser().ProviderUserKey;
                var staff = StaffManager.GetById(id);
                var items = StaffManager.GetByPosition(staff.StaffPosition.sDescription).OrderBy(m=>m.vStaffPositionDescription);
              

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sStaffName, Value = m.kStaffId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public JsonResult GetClaimStaffByPosition()
        {
            try
            {

                var items = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "ช่าง"
                            || m.StaffPosition.sDescription == "ฝ่ายเครม"
                            || m.StaffPosition.sDescription == "ตรวจสอบคุณภาพ").OrderBy(m => m.vStaffPositionDescription);


                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sStaffName, Value = m.kStaffId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult GetWorkingStatusByPosition()
        {
            try
            {
                var id = (Guid)Membership.GetUser().ProviderUserKey;
                var staff = StaffManager.GetById(id);
                var items = WorkingStatusManager
                                .GetByPosition(staff.StaffPosition.sDescription)
                                .Where(m => m.iDefault != (int)Working.Claiming)
                                .OrderBy(m=>m.iDefault);
               



                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kWorkingStatusId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        public JsonResult GetClaimWorkingStatusByPosition()
        {
            try
            {
                var items = WorkingStatusManager.GetAll()
                            .Where(m => m.iDefault == (int)Working.Claim || m.iDefault == (int)Working.Claiming || m.iDefault == (int)Working.ConfirmRepair)
                            .ToList().OrderBy(m => m.sDescription);

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kWorkingStatusId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

       
        public JsonResult GetWorkingStatusByStep(int step)
        {
            try
            {
                var items = WorkingStatusManager.GetAll();

                //ช่าง
                if (step == 1)
                {
                    items = items.Where(m => m.iDefault == (int)Working.Claim || m.iDefault == (int)Working.QC || m.iDefault == (int)Working.Repair).ToList();
                }
                //
                else if (step == 2)
                {
                    items = items.Where(m => m.iDefault == (int)Working.Claim || m.iDefault == (int)Working.QC || m.iDefault == (int)Working.Repair).ToList();
                }

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kWorkingStatusId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        public JsonResult GetRemind()
        {
            var items = Helpers.HelperController.Remind();
            return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.Text, Value = m.Text }) });
        }

        public ActionResult Claim()
        {
            return RedirectToAction("Create", "Claim");
        }
        public ActionResult Repair()
        {
            return RedirectToAction("Create", "Repair");
        }


 
        private static readonly Random _rnd = new Random();


        public ActionResult Index()
        {
            //var a = CurrentUserId;
            //ViewBag.ClientName = User.Identity.Name; //"user-" + _rnd.Next(10000, 99999);
            //return View();

            return RedirectToAction("Index","Repair");
        }
        
        public ActionResult textboxlist()
        {
            return View();
        }
        public ActionResult autocompletefb()
        {
            return View();
        }
        public ActionResult thaidate()
        {
            ViewBag.Date = "03/02/2555";//DateTime.Now.AddYears(543);
            return View();
        }
        public ActionResult addRow()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addRow(FormCollection collection)
        {
            
            string test = collection["desc_0"];
            return View();
        }

        public ActionResult addCause()
        {
            return View();
        }

        public ActionResult tokeninput()
        {
            return View();
        }


        public ActionResult visualsearch()
        {
            return View();
        }

        public ActionResult AddCustomer()
        {
            return View();
        }
        
        public ActionResult TestAddRepair()
        {
            ViewBag.Message = "Gigahertz claim & repair service";

            return View();
        }
        
        public ActionResult datetime()
        {
            ViewBag.Message = "Gigahertz claim & repair service";

            return View();
        }

        public ContentResult Territories(string q, int regionId, int limit, Int64 timestamp)
        {
            var products = new List<tProduct>
            {
                new tProduct { Id = 1, Name= "Prod1" }
                ,new tProduct { Id = 2, Name= "Prod2" }
                ,new tProduct { Id = 3, Name= "Crod3" }
            };
            var prodlist = from p in products.Where(a => a.Name.StartsWith("Pro")) select p.Name;
            return Content(prodlist.ToString());
        }

        public ActionResult Validation()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Confirm(BookingRepairViewModel model, FormCollection COLLECTION)
        {
            if (!ModelState.IsValid)
            {
                 return View();
            }
            return PartialView(model);            
        }   

        [HttpPost]
        public ActionResult TestAddRepair(BookingRepairViewModel model, FormCollection COLLECTION)
        {
            if (!ModelState.IsValid)
            {
               
            }
            return View();
        }   


        [Authorize]
        public ActionResult About()
        {
            return View();
        }

        public ActionResult autocomplete()
        {
           

            return View();
        }

        public class tProduct
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public JsonResult Search(string query)
        {
            try
            {
                //var items = BrandManager.GetBySearch("3M");

                var products = new List<tProduct>
                {
                    new tProduct { Id = 1, Name= "Prod1" }
                    ,new tProduct { Id = 2, Name= "Prod2" }
                    ,new tProduct { Id = 3, Name= "Crod3" }
                };

                //var prodlist = from p in products.Where(a => a.Name.StartsWith("Pro")) select p.Name;

                return Json(products.Select(m=>m.Name), JsonRequestBehavior.AllowGet);
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

                //Inform all connected clients
                var clientName = Request["clientName"];
                Task.Factory.StartNew(
                    () =>
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
        public JsonResult Lists(string jtSearching = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                //Thread.Sleep(200);
                if (string.IsNullOrEmpty(jtSearching))
                    jtSearching = "";

                var itemCount = BrandManager.GetCountByFiltering(jtSearching.Trim());
                var items = BrandManager.GetAll();

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
    }
}
