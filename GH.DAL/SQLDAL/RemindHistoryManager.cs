using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GH.DAL.Model;
using GH.DAL.Context;
using System.Data.Entity;
namespace GH.DAL.SQLDAL
{
    public class RemindHistoryManager
    {
        public static List<RemindHistory> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.RemindHistories
                        .Include(m=>m.Staff)
                        .OrderByDescending(m => m.dtDateAdd)
                        .ToList();
            }
        }

        //public static List<RemindHistory> GetPerPage(int page)
        //{
        //    // list = (from x in db.TxtNotes orderby x.Created descending select x).Take(count).ToList();
        //        //isMore = (from x in db.TxtNotes select x.Id).Take(count + 1).Count() - count > 0;
        //    using (DataContext db = new DataContext())
        //    {
        //        var m_result = db.RemindHistories
        //                        .Include(m=>m.Staff)
        //                        .Take(page)
        //                        .OrderByDescending(m => m.dtDateAdd);

        //        return m_result.ToList();
        //    }
        //}

        public static List<RemindHistory> GetLasts(DateTime dt)
        {
            using (DataContext db = new DataContext())
            {
                return db.RemindHistories
                        .Include(m => m.Staff)
                        .Where(m => m.dtDateAdd.CompareTo(dt) >= 0)
                        .OrderByDescending(m => m.dtDateAdd)
                        .ToList();
            }
        }

        public static List<RemindHistory> GetByPage(int startIndex, int pageSize)
        {
            using (DataContext db = new DataContext())
            {
                return db.RemindHistories
                        .Skip(startIndex).Take(pageSize)
                        .OrderByDescending(m => m.dtDateAdd)
                        .ToList();
            }
        }

        public static void Create(RemindHistory model)
        {
            using (DataContext db = new DataContext())
            {
                db.RemindHistories.Add(model);
                db.SaveChanges();
            }
        }
    }
}
