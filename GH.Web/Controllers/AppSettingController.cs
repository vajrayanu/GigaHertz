using System.Web.Mvc;
using GH.Web.Models;
using GH.DAL.SQLDAL;
using System;
using System.Web.Security;
using GH.DAL.Model;
using System.IO;
using System.Web;
namespace GH.Web.Controllers
{
    [Authorize(Roles = "Admin, SuperUser, User")]
    public class AppSettingController : Controller
    {
        private Staff Staff
        {
            get
            {
                var userId = (Guid)Membership.GetUser().ProviderUserKey;
                var staff = StaffManager.GetById(userId);
                return staff;
            }
        }
        private User Users
        {
            get
            {
                var user = StaffUserManager.GetStaffByName(User.Identity.Name);
                return user;
            }
        }
        public ActionResult Index()
        {
            ViewBag.Setting = "first active";

            ApplicationHeaderViewModel model = new ApplicationHeaderViewModel();


            if (!User.IsInRole("Admin"))
                return RedirectToAction("UserProfile");
            
            return View(model);
        }
        
        [HttpPost]
        public ActionResult Index(BookingRepairViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (string file in Request.Files)
                {
                    var hpf = Request.Files[file] as HttpPostedFileBase;
                    if (hpf.ContentLength == 0)
                        continue;

                    string path = string.Format("/Content/uploads/{0}", Path.GetFileName(hpf.FileName));
                    string savedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + path);
                    model.ApplicationSetting.sLogoUrl = Url.Content(path);

                    hpf.SaveAs(savedFileName);
                }
                ApplicationSettingManager.Save(model.ApplicationSetting);

                ModelState.AddModelError("ApplicationSetting", "success.");
            }
            else
            {
                ModelState.AddModelError("ApplicationSetting", "failed.");
            }

            return View(model);
        }

        public ActionResult UserProfile()
        {
            ViewBag.Settinguser = "first active";

            ApplicationHeaderViewModel model= new ApplicationHeaderViewModel();

            Guid id = (Guid)Membership.GetUser().ProviderUserKey;

            model.Staff = StaffManager.GetById(id);
            var users = StaffUserManager.GetUser(id);

            if (users != null)
            {
                model.User = new UserProfileChangePasswordModel
                {
                    UserName = users[0].Username,
                    OldPassword = users[0].Password
                };
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult UserProfile(ApplicationHeaderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var staff = StaffManager.GetById(model.Staff.kStaffId);

                model.Staff.kStaffPositionId = staff.kStaffPositionId;
                model.Staff.dtDateUpdate = DateTime.Now;
                StaffManager.Edit(model.Staff);
                ModelState.AddModelError("UserProfile", "success.");
            }
            else
            {
                ModelState.AddModelError("UserProfile", "failed.");
            }
            return View(model);
        }

        public ActionResult UserPassword()
        {
            ViewBag.Setting = "first active";

            BookingRepairViewModel model = new BookingRepairViewModel();

            Guid id = (Guid)Membership.GetUser().ProviderUserKey;

            var users = StaffUserManager.GetUser(id);

            if (users != null)
            {
                model.User = new UserProfileChangePasswordModel
                {
                    UserName = users[0].Username
                };
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult UserPassword(ApplicationHeaderViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded;

                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);

                    changePasswordSucceeded = currentUser.ChangePassword(model.User.OldPassword, model.User.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                    ModelState.AddModelError("", "failed.");
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("LogOff", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            else
            {
                ModelState.AddModelError("", "failed.");
            }

            Guid id = (Guid)Membership.GetUser().ProviderUserKey;
            var users = StaffUserManager.GetUser(id);

            if (users != null)
            {
                model.User = new UserProfileChangePasswordModel
                {
                    UserName = users[0].Username,
                    OldPassword = users[0].Password
                };
            }
            return View(model);
        }

    }
}
