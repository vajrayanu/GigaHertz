using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

namespace GH.DAL.SQLDAL
{
    public class ClaimNextItemNoManager
    {
        public static Int32 GetNextItemNo()
        {
            using (DataContext db = new DataContext())
            {
                var number = db.ClaimNextItemNo.Select(m => m.kNextItemNo);
                var year = DateTime.Now.AddYears(543).Year;

                var next_number = number.SingleOrDefault() + 1;
                var temp_number = year.ToString().Substring(2, 2);
                temp_number = temp_number + next_number;
                Int32 yearSuffix = Convert.ToInt32(temp_number);

                return yearSuffix;
            }
        }

        public static void IncreaseNextItemNo()
        {
            using (DataContext db = new DataContext())
            {
                var number = db.ClaimNextItemNo.Select(m => m.kNextItemNo);
                var next_number = number.SingleOrDefault() + 1;
                ClaimNextItemNo obj = new ClaimNextItemNo();
                obj.kNextItemNo = Convert.ToInt32(next_number);
                obj.dtLastMod = DateTime.Now;


                db.Database.ExecuteSqlCommand("IncreaseClaimNextItemNo @ItemNo", new SqlParameter("ItemNo", obj.kNextItemNo));

                //db.SaveChanges();
            }
        }
    }
}
