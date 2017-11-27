using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;
namespace GH.DAL.SQLDAL
{
    public class InsuranceManager
    {
        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Insurances.Where(m => m.sInsuranceName.Contains(searching)).Count();
            }
        }

        public static List<Insurance> GetCountDuplicate(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Insurances
                        .Where(m => m.sInsuranceName.Equals(searching))
                        .ToList();
            }
        }

        public static List<Insurance> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Insurances
                        .OrderBy(m => m.sInsuranceName)
                        .ToList();
            }
        }

        public static List<Insurance> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Insurances
                          .Where(m => m.sInsuranceName.Contains(searching))
                          .ToList();
            }
        }

        public static List<Insurance> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results = db.Insurances
                               .Where(m => m.sInsuranceName.Contains(searching))
                               .OrderByDescending(m => m.dtDateAdd).OrderByDescending(m => m.dtDateUpdate)
                               .Skip(startIndex).Take(pageSize)
                               .ToList();

                if (sorting.Contains("ASC"))
                {
                    if (sorting.Contains("sFullName"))
                    {
                        m_results = m_results.OrderBy(m => m.vFullName).ToList();
                    }
                    if (sorting.Contains("sPhone"))
                    {
                        m_results = m_results.OrderBy(m => m.sPhone).ToList();
                    }
                    if (sorting.Contains("sMobile"))
                    {
                        m_results = m_results.OrderBy(m => m.sMobile).ToList();
                    }
                    if (sorting.Contains("sFax"))
                    {
                        m_results = m_results.OrderBy(m => m.sFax).ToList();
                    }
                    if (sorting.Contains("sEmailAddress"))
                    {
                        m_results = m_results.OrderBy(m => m.sEmailAddress).ToList();
                    }
                }
                else
                {
                    if (sorting.Contains("sFullName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vFullName).ToList();
                    }
                    if (sorting.Contains("sPhone"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sPhone).ToList();
                    }
                    if (sorting.Contains("sMobile"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sMobile).ToList();
                    }
                    if (sorting.Contains("sFax"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sFax).ToList();
                    }
                    if (sorting.Contains("sEmailAddress"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sEmailAddress).ToList();
                    }
                }

                return m_results;
            }
        }

        public static Insurance GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                Insurance model = db.Insurances.Find(id);
                return model;
            }
        }

        public static void Create(Insurance model)
        {
            using (DataContext db = new DataContext())
            {
                db.Insurances.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(Insurance model)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                Insurance model = db.Insurances.Find(id);
                db.Insurances.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
