using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using GH.DAL.Helpers;
namespace GH.DAL.SQLDAL
{
    public class CauseManager
    {
        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Causes
                        .Where(m => m.sDescription.Contains(searching))
                        .Count();
            }
        }

        public static int GetCountDuplicate(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Causes
                        .Where(m => m.sDescription.Equals(searching))
                        .Count();
            }
        }

        public static List<Cause> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Causes
                        .OrderBy(m => m.sDescription)
                        .ToList();
            }
        }

        public static List<Cause> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Causes
                          .Where(m => m.sDescription.Contains(searching) || m.sCode.Contains(searching))
                          .ToList();
            }
        }

        public static List<Cause> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results = db.Causes
                           .Where(m => m.sDescription.Contains(searching) || m.sCode.Contains(searching))
                           .OrderByDescending(m => m.dtDateUpdate)
                           .ToList();

                if (sorting.Contains("ASC"))
                {
                    if (sorting.Contains("sCode"))
                    {
                        m_results = m_results.OrderBy(m => m.sCode).ToList();
                    }
                    if (sorting.Contains("sDescription"))
                    {
                        m_results = m_results.OrderBy(m => m.sDescription).ToList();
                    }
                    if (sorting.Contains("dPrice"))
                    {
                        m_results = m_results.OrderBy(m => m.dPrice).ToList();
                    }
                }
                else
                {
                    if (sorting.Contains("sCode"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sCode).ToList();
                    }
                    if (sorting.Contains("sDescription"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sDescription).ToList();
                    }
                    if (sorting.Contains("dPrice"))
                    {
                        m_results = m_results.OrderByDescending(m => m.dPrice).ToList();
                    }
                }

                return pageSize > 0
                      ? m_results.Skip(startIndex).Take(pageSize).ToList() //Paging
                      : m_results.ToList(); //No paging
            }
        }

        public static Cause GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                Cause model = db.Causes.Find(id);

                return model;
            }
        }

        public static Cause GetByName(String name)
        {
            using (DataContext db = new DataContext())
            {
                Cause model = db.Causes.Where(m => m.sDescription.Equals(name)).SingleOrDefault();

                return model;
            }
        }

        public static void Create(Cause model)
        {
            using (DataContext db = new DataContext())
            {
                db.Causes.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(Cause model)
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
                Cause model = db.Causes.Find(id);
                db.Causes.Remove(model);
                db.SaveChanges();
            }
        }

        public static List<Cause> FulltextSearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Causes
                        .FullTextSearch(searching, true)
                        .ToList();
            }
        }
    }
}
