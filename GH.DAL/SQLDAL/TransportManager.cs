using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;
namespace GH.DAL.SQLDAL
{
    public class TransportManager
    {
        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Transports.Where(m => m.sTransportName.Contains(searching)).Count();
            }
        }

        public static List<Transport> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Transports
                          .Where(m => m.sTransportName.Contains(searching))
                          .ToList();
            }
        }

        public static List<Transport> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results = db.Transports
                               .Where(m => m.sTransportName.Contains(searching))
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

        public static Transport GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                Transport model = db.Transports.Find(id);
                return model;
            }
        }

        public static void Create(Transport model)
        {
            using (DataContext db = new DataContext())
            {
                db.Transports.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(Transport model)
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
                Transport model = db.Transports.Find(id);
                db.Transports.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
