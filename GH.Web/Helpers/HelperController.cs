using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using GH.DAL.SQLDAL;
using GH.DAL.Model;

namespace GH.Web.Helpers
{
    public class HelperController
    {
        public static void CloseStatusUpdateValue(Repair model)
        {
            //var closeStatusId = CloseStatusManager.GetAll().
        }
        public static SelectList DayWarranty()
        {
            var items = new List<string>();

            for (int i = 1; i <= 100; i++)
            {
                items.Add(i.ToString());
            }
            return new SelectList(items, "Value");
        }

        public static SelectList Remind()
        {
            var items = new List<string>();

            for (int i = 0; i <= 10; i++)
            {
                items.Add(i.ToString());
            }
            return new SelectList(items, "Value");
        }

        public static SelectList SuperUser()
        {
            var items = StaffManager.GetAll().Where(m => m.vStaffPositionDescription == "หัวหน้าช่าง").ToList();

            var item = new SelectList(items, items.ConvertAll(m => m.sStaffName));

            return new SelectList(items, new { @Value = item.SelectedValue });
        }

        public static DateTime ConvertDateTimeFormat(DateTime dt){
            var m = dt.Month;
            var d = dt.Day;
            var y = dt.Year;
            return new DateTime(y, m, d);
        }
        public static string DateThaiFormatShort()
        {
            return "dd mm yy";
        }
        public static string DateThaiFormatLong()
        {
            return "dd MM yy";
        }
        public static string StaffName(Guid staffId)
        {
            if (staffId != Guid.Empty)
                return StaffManager.GetById(staffId).sStaffName;
            else
                return "";
        }

       
    }
}
