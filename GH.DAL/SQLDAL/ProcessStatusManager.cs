using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace GH.DAL.SQLDAL
{
    public class ProcessStatusManager
    {
        public static List<ProcessStatus> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.ProcessStatus
                           .OrderBy(m => m.sDescription)
                           .ToList();
            }
        }

        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.ProcessStatus.Where(m => m.sDescription.Contains(searching)).Count();
            }
        }

        public static int GetCountDuplicate(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.ProcessStatus
                        .Where(m => m.sDescription.Equals(searching))
                        .Count();
            }
        }

        public static List<ProcessStatus> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.ProcessStatus
                             .Where(m => m.sDescription.Contains(searching))
                             .OrderBy(m => m.sDescription)
                             .ToList();
            }
        }

        public static List<ProcessStatus> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results = db.ProcessStatus
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

        public static ProcessStatus GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                ProcessStatus model = db.ProcessStatus.Find(id);
                return model;
            }
        }

        public static void Create(ProcessStatus model)
        {
            using (DataContext db = new DataContext())
            {
                db.ProcessStatus.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(ProcessStatus model)
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
                ProcessStatus model = db.ProcessStatus.Find(id);

                db.ProcessStatus.Remove(model);
                db.SaveChanges();
            }
        }

        public static Guid GetFromName(String text)
        {
            using (DataContext db = new DataContext())
            {
                ProcessStatus model = db.ProcessStatus.Find(text);
                return model.kProcessStatusId;
            }
        }
    }
}
