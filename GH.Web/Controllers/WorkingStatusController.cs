using System;
using System.Linq;
using System.Web.Mvc;
using GH.DAL.SQLDAL;
using GH.DAL.Model;
using System.Threading.Tasks;
using SignalR.Hubs;
using GH.Web.Models;
using GH.DAL.Helpers;
using System.Web.Security;

namespace GH.Web.Controllers
{
    [Authorize]
    public class WorkingStatusController : Controller
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
                var items = WorkingStatusManager.GetAll();

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kWorkingStatusId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //[HttpPost]
        //public JsonResult GetByPosition()
        //{
        //    try
        //    {
        //        var id = (Guid)Membership.GetUser().ProviderUserKey;
        //        var staff = StaffManager.GetById(id);
        //        var items = WorkingStatusManager.GetByPosition(staff.StaffPosition.sDescription);

        //        return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kWorkingStatusId }) });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult GetByRole()
        //{
        //    try
        //    {
        //        var user = StaffUserManager.GetStaffByName(User.Identity.Name);
        //        var role = user.Roles.FirstOrDefault().RoleName;
        //        var items = WorkingStatusManager.GetByRole(role);

        //        return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kWorkingStatusId }) });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult GetForRepair(Boolean? isComplete)
        //{
        //    try
        //    {
        //        var items = WorkingStatusManager.GetAll();
        //        if (isComplete==true)
        //        {
        //            items.Where(m => m.iDefault == (int)Close.Back).ToList();
        //        }


        //        return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kWorkingStatusId }) });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult ForClaim()
        //{
        //    try
        //    {
        //        var items = WorkingStatusManager.GetAll().Where(m=>m.iDefault<=(int)Working.Claim);

        //        return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kWorkingStatusId }) });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        [HttpPost]
        public JsonResult EditStatus(RepairStatus model)
        {
            try
            {
                WorkingStatusManager.EditRepairStatus(model);


                //re-initial data
                Repair model2 = RepairManager.GetById(model.kRepairId);


                //Inform all connected clients
                var clientName = User.Identity.Name;
                Task.Factory.StartNew(() =>
                {
                    var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                    clients.RecordUpdated(clientName, model2);
                });


                return Json(new { Result = "OK" });
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
                var items = WorkingStatusManager.GetBySearch(term);

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
                var itemCount = WorkingStatusManager.GetCountByFiltering(jtSearching);
                var items = WorkingStatusManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting).ToList();

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
        public JsonResult Create(WorkingStatus model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                model.kWorkingStatusId = Guid.NewGuid();

                WorkingStatusManager.Create(model);
                return Json(new { Result = "OK", Record = model});
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Edit(WorkingStatus model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                WorkingStatus itemFound = WorkingStatusManager.GetById(model.kWorkingStatusId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }

                WorkingStatusManager.Edit(model);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid kWorkingStatusId)
        {
            try
            {
                //Thread.Sleep(50);
                WorkingStatusManager.Delete(kWorkingStatusId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "Cann't Delete" });
            }
        }
    }
}
