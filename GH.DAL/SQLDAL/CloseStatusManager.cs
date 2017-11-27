using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using GH.DAL.Helpers;

namespace GH.DAL.SQLDAL
{
    public class CloseStatusManager
    {
        public static List<CloseStatus> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.CloseStatus
                           .OrderBy(m => m.iOrder)
                           .OrderBy(m => m.sDescription)
                           .ToList();
            }
        }

        public static List<CloseStatus> GetByPosition(string text)
        {
            using (DataContext db = new DataContext())
            {
                if (text == "ช่าง")
                {
                    return db.CloseStatus
                          .OrderBy(m => m.sDescription)
                          .Where(m => m.iOrder == (int)Working.Repair 
                              || m.iOrder == (int)Working.RepairSuccess 
                              || m.iOrder == (int)Working.Claim
                              //|| m.iOrder == (int)Close.Cancle
                              || m.iOrder == (int)Working.RemindCause
                              || m.iOrder == (int)Working.Remind)
                          .ToList();
                }
                else if (text == "ฝ่ายเครม")
                {
                    return db.CloseStatus
                         .OrderBy(m => m.sDescription)
                         .Where(m => m.iOrder == (int)Working.Repair
                              || m.iOrder == (int)Working.Claim
                              || m.iOrder == (int)Working.QC)
                          .ToList();
                }
                else if (text == "ฝ่ายตรวจสอบคุณภาพ")
                {
                    return db.CloseStatus
                         .OrderBy(m => m.sDescription)
                          .Where(m => m.iOrder == (int)Working.Repair
                              || m.iOrder == (int)Working.Remind
                              || m.iOrder == (int)Working.QC)
                          .ToList();
                }
                else if (text == "ฝ่ายรับสินค้า")
                {
                    return db.CloseStatus
                         .OrderBy(m => m.sDescription)
                          .Where(m => m.iOrder == (int)Working.Repair
                              || m.iOrder == (int)Working.Remind
                              || m.iOrder == (int)Working.QC)
                          .ToList();
                }
                else
                {
                    return db.CloseStatus.ToList();
                }
            }
        }

 
        public static List<CloseStatus> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.CloseStatus
                             .Where(m => m.sDescription.Contains(searching))
                             .OrderBy(m => m.sDescription)
                             .ToList();
            }
        }

        public static List<CloseStatus> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results = db.CloseStatus
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

        public static CloseStatus GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                CloseStatus model = db.CloseStatus.Find(id);
                return model;
            }
        }

        public static Guid GetFromName(String text)
        {
            using (DataContext db = new DataContext())
            {
                CloseStatus model = db.CloseStatus.Find(text);
                return model.kCloseStatusId;
            }
        }
    }
}
