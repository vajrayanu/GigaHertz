using System;
using System.Linq;
using System.Web.Mvc;
using GH.DAL.SQLDAL;
using GH.Web.Helpers;
using GH.DAL.Model;
using System.Web.Security;
using GH.Web.Controllers;
using SignalR.Hubs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GH.DAL.Controllers
{
    public class RemindController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetNotes(int count)
        {
            bool isMore = true;
            string message = string.Empty;
            var items = RemindHistoryManager.GetAll();
            try
            {

                isMore = (from x in items select x.kRemindHistoryId).Take(count + 1).Count() - count > 0;
                items = (from x in items orderby x.dtDateAdd descending select x).Take(count).ToList();

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new
            {
                Html = this.RenderPartialView("Lists", items),
                IsMore = isMore,
                Message = message
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetOlderNotes(int id, int count = 5)
        {
            bool isMore = true;
            string message = string.Empty;
            var items = RemindHistoryManager.GetAll();
            try
            {
                isMore = (from x in items where x.kRemindHistoryId < id select x.kRemindHistoryId).Take(count + 1).Count() - count > 0;
                items = (from x in items where x.kRemindHistoryId < id orderby x.dtDateAdd descending select x).Take(count).ToList();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new
            {
                Html = this.RenderPartialView("Lists", items),
                IsMore = isMore,
                Message = message
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLatestNotes(DateTime? lastDate)
        {
            var items = RemindHistoryManager.GetLasts(lastDate.Value);
            string message = string.Empty;
            bool IsNew = false;
            if (items.Count > 0)
                IsNew = true;


            return Json(new
            {
                Html = this.RenderPartialView("Lists", items),
                Message = message,
                IsNew = IsNew
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PostNote(string content)
        {
            RemindHistory model = new RemindHistory
            {
                sRemind = content,
                kStaffId = (Guid)Membership.GetUser().ProviderUserKey
            };
            RemindHistoryManager.Create(model);

            //List<RemindHistory> items = new List<RemindHistory>();
            //items.Add(model);
            var items = RemindHistoryManager.GetLasts(model.dtDateAdd);
            string message = string.Empty;
            bool IsNew = false;
            if (items.Count > 0)
                IsNew = true;


            var staff = StaffManager.GetById((Guid)Membership.GetUser().ProviderUserKey);
            var clientName = staff.sStaffName;
            Task.Factory.StartNew(() =>
            {
                var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                clients.RecordUpdated(clientName, content);
            });

            return Json(new
            {
                Html = this.RenderPartialView("Lists", items),
                Message = message,
                IsNew = IsNew
            }, JsonRequestBehavior.AllowGet);
        }

    }


 
}
