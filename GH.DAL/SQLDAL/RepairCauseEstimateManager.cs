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
    public class RepairCauseEstimateManager
    {
        public static RepairCauseEstimate GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.RepairCauseEstimates.Find(id);
            }
        }

        public static void Create(RepairCauseEstimate model)
        {
            using (DataContext db = new DataContext())
            {
                db.RepairCauseEstimates.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(RepairCauseEstimate model)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static List<RepairCauseEstimate> GetByRepairId(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.RepairCauseEstimates
                    .Where(m => m.kRepairId == id)
                    .OrderByDescending(m => m.dtDateAdd)
                    .ToList();
            }
        }


        public static void Delete(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                RepairCauseEstimate model = db.RepairCauseEstimates.Find(id);
                db.RepairCauseEstimates.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
