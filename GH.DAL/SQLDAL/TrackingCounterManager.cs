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
    public class TrackingCounterManager
    {
        public static Int32 GetNextItemNo()
        {
            using (DataContext db = new DataContext())
            {
                var number = db.TrackingCounters.Select(m => m.iCounter);
              

                var next_number = number.SingleOrDefault();


                return next_number;
            }
        }

        public static void IncreaseNextItemNo()
        {
            using (DataContext db = new DataContext())
            {
                TrackingCounter model = db.TrackingCounters.Find(1);
                model.iCounter++;

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
