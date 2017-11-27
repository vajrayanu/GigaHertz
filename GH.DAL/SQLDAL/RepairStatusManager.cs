using System.Collections.Generic;
using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System;
using System.Data.Entity;
using System.Data;

namespace GH.DAL.SQLDAL
{
    public class RepairStatusManager
    {
        public static void Create(RepairStatus model)
        {
            using (DataContext db = new DataContext())
            {
                db.RepairStatuies.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(RepairStatus model)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static List<RepairStatus> GetByRepairId(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.RepairStatuies
                    .Include(m=>m.Staff)
                    .Include(m=>m.WorkingStatus)
                    .Where(m=>m.kRepairId==id)
                    .OrderByDescending(m=>m.dtDateAdd)
                    .ToList();
            }
        }
    }
}
