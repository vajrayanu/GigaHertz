using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;


namespace GH.DAL.SQLDAL
{
    public class ClaimCourseManager
    {

        public static ClaimCause GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.ClaimCauses.Find(id);
            }
        }

        public static void Create(ClaimCause model)
        {
            using (DataContext db = new DataContext())
            {
                db.ClaimCauses.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(ClaimCause model)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static List<ClaimCause> GetByClaimId(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.ClaimCauses
                    .Where(m => m.kClaimId == id)
                    .OrderByDescending(m => m.dtDateAdd)
                    .ToList();
            }
        }


        public static void Delete(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                ClaimCause model = db.ClaimCauses.Find(id);
                db.ClaimCauses.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
