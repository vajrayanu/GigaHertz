using System.Collections.Generic;
using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System;
using System.Data.Entity;

namespace GH.DAL.SQLDAL
{
    public class ClaimStatusManager
    {
        public static void Create(ClaimStatus model)
        {
            using (DataContext db = new DataContext())
            {
                db.ClaimStatuies.Add(model);
                db.SaveChanges();
            }
        }

        public static List<ClaimStatus> GetByClaimId(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.ClaimStatuies
                    .Include(m=>m.Staff)
                    .Include(m=>m.WorkingStatus)
                    .Where(m=>m.kClaimId==id)
                    .OrderByDescending(m=>m.dtDateAdd)
                    .ToList();
            }
        }

       
    }
}
