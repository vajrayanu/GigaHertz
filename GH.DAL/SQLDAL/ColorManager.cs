using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;

namespace GH.DAL.SQLDAL
{
    public class ColorManager
    {
        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Colors.Where(m => m.sDescription.Contains(searching)).Count();
            }
        }

        public static int GetCountDuplicate(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Colors
                        .Where(m => m.sDescription.Equals(searching))
                        .Count();
            }
        }

        public static List<Color> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Colors
                        .OrderBy(m => m.sDescription)
                        .ToList();
            }
        }

        public static List<Color> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Colors
                             .Where(m => m.sDescription.Contains(searching))
                             .OrderBy(m => m.sDescription)
                             .ToList();
            }
        }

        public static List<Color> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results = db.Colors
                               .Where(m => m.sDescription.Contains(searching))
                               .OrderByDescending(m => m.sDescription)
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

        public static Color GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                Color model = db.Colors.Find(id);
                return model;
            }
        }

        public static void Create(Color model)
        {
            using (DataContext db = new DataContext())
            {
                db.Colors.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(Color model)
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
                Color model = db.Colors.Find(id);
                db.Colors.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
