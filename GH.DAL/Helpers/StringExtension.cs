using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GH.DAL.Helpers
{
    public class StringExtension
    {
       public static string Date()
       {
           return "d MMM yyyy";
       }

       public static string DateBuddha()
       {
           //System.IFormatProvider format = new System.Globalization.CultureInfo("th-TH");
           //string sdate = dt.ToString();
           //DateTime newdate = System.DateTime.Parse(sdate, format);
           //string resultdate = newdate.ToString("dd/MM/yyyy");
           //return resultdate;
           return "dd/MM/yyyy";
       }
       public static string DateTime()
       {
           return "HH:mm";
       }
       public static string Money()
       {
           return "N2";
       }

       public static string MoneyFormat()
       {
           return "{0:0,0}";
       } 
    }
}
