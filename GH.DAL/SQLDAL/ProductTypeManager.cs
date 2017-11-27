using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;

namespace GH.DAL.SQLDAL
{
    public class ProductTypeManager
    {
        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.ProductTypes.Where(m => m.sDescription.Contains(searching)).Count();
            }
        }

        public static List<ProductType> GetCountDuplicate(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.ProductTypes
                        .Where(m => m.sDescription.Equals(searching))
                        .ToList();
            }
        }

        public static List<ProductType> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.ProductTypes
                        .OrderBy(m => m.sDescription)
                        .ToList();
            }
        }

        public static List<ProductType> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.ProductTypes
                          .Where(m => m.sDescription.Contains(searching))
                          .ToList();
            }
        }

        public static List<ProductType> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results = db.ProductTypes
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

        public static ProductType GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                ProductType model = db.ProductTypes.Find(id);
                return model;
            }
        }

        public static void Create(ProductType model)
        {
            using (DataContext db = new DataContext())
            {
                db.ProductTypes.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(ProductType model)
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
                ProductType model = db.ProductTypes.Find(id);
                db.ProductTypes.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
