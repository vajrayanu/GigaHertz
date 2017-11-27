using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;

namespace GH.DAL.SQLDAL
{
    public class BrandManager
    {
        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Brands.Where(m => m.sBrandName.Contains(searching)).Count();
            }
        }

        public static List<Brand> GetCountDuplicate(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Brands
                        .Where(m => m.sBrandName.Equals(searching))
                        .ToList();
            }
        }

        public static List<Brand> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Brands
                        .OrderBy(m => m.sBrandName)
                        .ToList();
            }
        }

        public static List<Brand> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Brands
                          .Where(m => m.sBrandName.Contains(searching))
                          .ToList();
            }
        }

        public static List<Brand> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";
                
                var m_results = db.Brands
                               .Where(m => m.sBrandName.Contains(searching))
                               .OrderByDescending(m => m.dtDateAdd).OrderByDescending(m => m.dtDateUpdate)
                               .Skip(startIndex).Take(pageSize)
                               .ToList();

                if (sorting.Contains("ASC"))
                {
                    if (sorting.Contains("sBrandName"))
                    {
                        m_results = m_results.OrderBy(m => m.sBrandName).ToList();
                    }
                }
                else
                {
                    if (sorting.Contains("sBrandName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sBrandName).ToList();
                    }
                }

                return m_results;
            }
        }

        public static Brand GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                Brand model = db.Brands.Find(id);
                return model;
            }
        }

        public static void Create(Brand model)
        {
            using (DataContext db = new DataContext())
            {
                db.Brands.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(Brand model)
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
                Brand model = db.Brands.Find(id);
                db.Brands.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
