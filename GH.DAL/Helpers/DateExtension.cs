using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace GH.DAL.Helpers
{
    public class DateExtension
    {
        public static DateTime ThaiDate(DateTime dt)
        {
            DateTime d2 = DateTime.Parse(dt.ToString("yyyy/MM/dd hh:mm:ss", new CultureInfo("th-TH")));
            return d2;
            //var thTH = new System.Globalization.CultureInfo("th-TH");
            //var resultingDate = DateTime.ParseExact(dt.ToString(), "yyyy/MM/dd", thTH);
            //return resultingDate;
        }
        public static string DateFormat()
        {
            return "d MMM yyyy";
        }

        public static string DateNumber()
        {
            return "M/d/yyyy";
        }

        public static string TimeFormat()
        {
            return "H:mm";
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

        public static String DateThaiFormat(DateTime dt)
        {
            if (dt.Month == 1)
                return String.Format("{0} มกราคม {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 2)
                return String.Format("{0} กุมภาพันธ์ {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 3)
                return String.Format("{0} มีนาคม {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 4)
                return String.Format("{0} เมษายน {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 5)
                return String.Format("{0} พฤษภาคม {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 6)
                return String.Format("{0} มิถุนายน {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 7)
                return String.Format("{0} กรกฏาคม {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 8)
                return String.Format("{0} สิงหาคม {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 9)
                return String.Format("{0} กันยายน {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 10)
                return String.Format("{0} ตุลาคม {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 11)
                return String.Format("{0} พฤศจิกายน {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 12)
                return String.Format("{0} ธันวาคม {1}", dt.Day, dt.Year + 543);
            else
                return "";
        }

        public static String DateThaiFormat2(DateTime dt)
        {
            if (dt.Month == 1)
                return String.Format("มกราคม {0}",  dt.Year + 543);
            else if (dt.Month == 2)
                return String.Format("กุมภาพันธ์ {0}", dt.Year + 543);
            else if (dt.Month == 3)
                return String.Format("มีนาคม {0}",  dt.Year + 543);
            else if (dt.Month == 4)
                return String.Format("เมษายน {0}",  dt.Year + 543);
            else if (dt.Month == 5)
                return String.Format("พฤษภาคม {0}", dt.Year + 543);
            else if (dt.Month == 6)
                return String.Format("มิถุนายน {0}", dt.Year + 543);
            else if (dt.Month == 7)
                return String.Format("กรกฏาคม {0}", dt.Year + 543);
            else if (dt.Month == 8)
                return String.Format("สิงหาคม {0}", dt.Year + 543);
            else if (dt.Month == 9)
                return String.Format("กันยายน {0}", dt.Year + 543);
            else if (dt.Month == 10)
                return String.Format("ตุลาคม {0}", dt.Year + 543);
            else if (dt.Month == 11)
                return String.Format("พฤศจิกายน {0}", dt.Year + 543);
            else if (dt.Month == 12)
                return String.Format("ธันวาคม {0}", dt.Year + 543);
            else
                return "";
        }


        public static String DateThaiFormatShort(DateTime dt)
        {
            if (dt.Month == 1)
                return String.Format("{0} มค. {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 2)
                return String.Format("{0} กพ. {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 3)
                return String.Format("{0} มีค. {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 4)
                return String.Format("{0} เมย. {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 5)
                return String.Format("{0} พค. {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 6)
                return String.Format("{0} มิย. {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 7)
                return String.Format("{0} กค. {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 8)
                return String.Format("{0} สค. {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 9)
                return String.Format("{0} กย. {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 10)
                return String.Format("{0} ตค. {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 11)
                return String.Format("{0} พย. {1}", dt.Day, dt.Year + 543);
            else if (dt.Month == 12)
                return String.Format("{0} ธค. {1}", dt.Day, dt.Year + 543);
            else
                return "";
        }

        public static DateTime DateThai(DateTime dt)
        {
            //DateTime d2 = DateTime.Parse(dt.ToString("yyyy/MM/dd hh:mm:ss", new CultureInfo("th-TH")));
            return dt.AddYears(-543);
        }

        public static DateTime FirstDayOfMonthFromDateTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime LastDayOfMonthFromDateTime(DateTime dateTime)
        {
            DateTime firstDayOfTheMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }


        public static int Compare(DateTime dt1, DateTime dt2)
        {
            // return -1 = less
            // return 0 = equal
            // return 1 = more
            int allDay1 = dt1.DayOfYear;
            int allDay2 = dt2.DayOfYear;


            if (allDay1 > allDay2)
                return 1;
            else if (allDay1 < allDay2)
                return -1;
            else
                return 0;
        }
    }
}
