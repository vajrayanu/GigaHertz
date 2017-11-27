using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;

namespace GH.DAL.SQLDAL
{
    public class ProductAccessorieManager
    {
        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.ProductAccessories.Where(m => m.sDescription.Contains(searching)).Count();
            }
        }

        public static int GetCountDuplicate(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.ProductAccessories
                        .Where(m => m.sDescription.Equals(searching))
                        .Count();
            }
        }

        public static List<ProductAccessorie> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.ProductAccessories
                        .OrderBy(m => m.sDescription)
                        .ToList();
            }
        }

        public static List<ProductAccessorie> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.ProductAccessories
                          .Where(m => m.sDescription.Contains(searching))
                          .OrderBy(m => m.sDescription)
                          .ToList();
            }
        }

        public static List<ProductAccessorie> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results = db.ProductAccessories
                               .Where(m => m.sDescription.Contains(searching))
                               .OrderByDescending(m => m.dtDateAdd).OrderByDescending(m => m.dtDateUpdate)
                               .Skip(startIndex).Take(pageSize)
                               .ToList();

                if (sorting.Contains("ASC"))
                {
                    if (sorting.Contains("sDescription"))
                    {
                        m_results = m_results.OrderBy(m => m.sDescription).ToList();
                    }
                }
                else
                {
                    if (sorting.Contains("sDescription"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sDescription).ToList();
                    }
                }

                return m_results;

            }
        }

        public static ProductAccessorie GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                ProductAccessorie model = db.ProductAccessories.Find(id);
                return model;
            }
        }

        public static void Create(ProductAccessorie model)
        {
            using (DataContext db = new DataContext())
            {
                db.ProductAccessories.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(ProductAccessorie model)
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
                ProductAccessorie model = db.ProductAccessories.Find(id);
                db.ProductAccessories.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
