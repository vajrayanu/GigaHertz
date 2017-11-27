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
    public class RepairCauseManager
    {

        public static RepairCause GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.RepairCauses.Find(id);
            }
        }

        public static void Create(RepairCause model)
        {
            using (DataContext db = new DataContext())
            {
                db.RepairCauses.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(RepairCause model)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static List<RepairCause> GetByRepairId(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.RepairCauses
                    .Where(m => m.kRepairId == id)
                    .OrderByDescending(m => m.dtDateAdd)
                    .ToList();
            }
        }


        public static void Delete(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                RepairCause model = db.RepairCauses.Find(id);
                db.RepairCauses.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
