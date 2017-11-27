using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GH.DAL.SQLDAL;

namespace GH.Web.Controllers
{
    [Authorize]
    public class StaffPositionController : Controller
    {
        public JsonResult GetAll()
        {
            try
            {
                var items = StaffPositionManager.GetAll();

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sDescription, Value = m.kStaffPositionId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}
