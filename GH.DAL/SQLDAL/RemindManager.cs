using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.Entity;
using System.Transactions;
using System.Data.Objects;
using GH.DAL.Helpers;


namespace GH.DAL.SQLDAL
{
    public class RemindManager
    {

        public static Remind GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Reminds.Find(id);
            }
        }

        public static void Create(Remind model)
        {
            using (DataContext db = new DataContext())
            {
                db.Reminds.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(Remind model)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static List<Remind> GetByRepairId(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Reminds
                    .Where(m => m.kRepairId == id)
                    .OrderByDescending(m => m.dtDateAdd)
                    .ToList();
            }
        }


        public static void Delete(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                Remind model = db.Reminds.Find(id);
                db.Reminds.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
