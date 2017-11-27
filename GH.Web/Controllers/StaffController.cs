using System;
using System.Linq;
using System.Web.Mvc;
using GH.DAL.Model;
using GH.DAL.SQLDAL;
using System.Web.Security;
using GH.Web.Models;
using GH.DAL.Helpers;

namespace GH.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        //public Object CurrentUserId
        //{
        //    get
        //    {
        //        if (User.Identity.IsAuthenticated)
        //            return (Guid)Membership.GetUser().ProviderUserKey;
        //        else
        //            return null;
        //    }
        //}

        public ActionResult Index()
        {
            ViewBag.Setting = "first active";

            BookingRepairViewModel model = new BookingRepairViewModel();
         
            return View(model);
        }

        //[HttpPost]
        public JsonResult GetAll()
        {
            try
            {
                var items = StaffManager.GetAll();

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sStaffName, Value = m.kStaffId }) });
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
        //        var items = StaffManager.GetByPosition(staff.StaffPosition.sDescription);

        //        return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sStaffName, Value = m.kStaffId }) });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        public JsonResult GetById(Guid Id)
        {
            try
            {
                var itemFounds = StaffUserManager.GetUser(Id);
                return Json(new 
                { 
                    Result = "OK", 
                    Records = itemFounds.Select(m => new { Username = m.Username, Password = m.Password, Role=m.Roles.FirstOrDefault().RoleName }) }
                , JsonRequestBehavior.AllowGet);
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
                var items = StaffManager.GetBySearch(term);

                return Json(items.Select(m => m.sStaffName), JsonRequestBehavior.AllowGet);
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
                var itemCount = StaffManager.GetCountByFiltering(jtSearching);
                var items = StaffManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting).ToList();

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
        public JsonResult Create(Staff model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                var itemCount = StaffManager.GetCountDuplicate(model.sStaffName.Trim());
                if (itemCount.Count >= 1)
                {
                    return Json(new { Result = "ERROR", Message = "Item Exists." });
                }


                MembershipCreateStatus createStatus;
                var password = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();

                var position = StaffPositionManager.GetById(model.kStaffPositionId);
               
                if (position.sDescription == "หัวหน้าช่าง")
                {
                    var insert = Membership.CreateUser(model.UserName[0].ToString(), password, model.sEmailAddress, null, null, true, "true", out createStatus);
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        model.dtDateAdd = DateTime.Now;
                        model.kStaffId = new Guid(insert.ProviderUserKey.ToString());
                        StaffManager.Create(model);
                    }
                }
                else
                {
                    var insert = Membership.CreateUser(model.UserName[0].ToString(), password, model.sEmailAddress, null, null, true, "false", out createStatus);
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        model.dtDateAdd = DateTime.Now;
                        model.kStaffId = new Guid(insert.ProviderUserKey.ToString());
                        StaffManager.Create(model);
                    }
                }
                //var insert = Membership.CreateUser(model.UserName[0].ToString(), password, model.sEmailAddress, null, null, true, null, out createStatus);

                
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Edit(Staff model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Staff itemFound = StaffManager.GetById(model.kStaffId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                model.dtDateAdd = itemFound.dtDateAdd;
                model.dtDateUpdate = DateTime.Now;
                
                StaffManager.Edit(model);

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(Staff model)
        {
            try
            {
                StaffManager.Delete(model);
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
                Staff satff = StaffManager.GetById(id);
                var items = StaffManager.GetRepairs(satff);



                //Records = itemFounds.Select(m => new { Username = m.Username, Password = m.Password, Role=m.Roles.FirstOrDefault().RoleName }) }
                return Json(new
                {
                    Result = "OK",
                    Records = items.Select(m => new { 
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
