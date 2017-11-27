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
    public class WorkingStatusManager
    {
        public static List<WorkingStatus> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.WorkingStatus
                           .OrderBy(m => m.iDefault)
                           .ToList();
            }
        }

        public static List<WorkingStatus> GetByPosition(string text)
        {
            using (DataContext db = new DataContext())
            {
                if (text == "ช่าง")
                {
                    return db.WorkingStatus
                          .OrderBy(m => m.iDefault)
                          .Where( m => m.iDefault == (int)Working.Default
                              || m.iDefault == (int)Working.Repair 
                              || m.iDefault == (int)Working.RepairSuccess 
                              || m.iDefault == (int)Working.Claim
                              || m.iDefault == (int)Working.Cancle
                              || m.iDefault == (int)Working.RemindCause)
                          .ToList();
                }
                else if (text == "ฝ่ายเครม")
                {
                    return db.WorkingStatus
                         .OrderBy(m => m.iDefault)
                         .Where(m => m.iDefault == (int)Working.Repair
                              || m.iDefault == (int)Working.Claim
                              || m.iDefault == (int)Working.QC)
                          .ToList();
                }
                else if (text == "ฝ่ายตรวจสอบคุณภาพ")
                {
                    return db.WorkingStatus
                          .OrderBy(m => m.iDefault)
                          .Where(m => m.iDefault == (int)Working.Default 
                              || m.iDefault == (int)Working.Repair
                              || m.iDefault == (int)Working.Remind
                              || m.iDefault == (int)Working.RepairCheck
                              || m.iDefault == (int)Working.QC)
                          .ToList();
                }
                else if (text == "ฝ่ายรับสินค้า")
                {
                    return db.WorkingStatus
                         .OrderBy(m => m.iDefault)
                          .Where(m => m.iDefault == (int)Working.Default
                              || m.iDefault == (int)Working.Open
                              || m.iDefault == (int)Working.OpenBack
                              || m.iDefault == (int)Working.Remind
                              || m.iDefault == (int)Working.ConfirmRepair
                              || m.iDefault == (int)Working.Cancle
                              || m.iDefault == (int)Working.Close
                              || m.iDefault == (int)Working.RemindCause)
                          .ToList();
                }
                else
                {
                    return db.WorkingStatus.OrderBy(m => m.iDefault).ToList();
                    //return db.WorkingStatus.Where(m => m.iDefault != (int)Working.Close).OrderBy(m => m.iDefault).ToList();
                }
            }
        }

        //public static List<WorkingStatus> GetByRole(string text)
        //{
        //    using (DataContext db = new DataContext())
        //    {
        //        if (text == "Admin" || text == "SuperUser")
        //        {
        //            return db.WorkingStatus
        //                    .OrderBy(m => m.iDefault)
        //                    .OrderBy(m => m.sDescription)
        //                    .ToList();
        //        }
        //        else
        //        {
        //            return db.WorkingStatus
        //                   .OrderBy(m => m.iDefault)
        //                   .OrderBy(m => m.sDescription)
        //                   .Where(m => m.iDefault == (int)Working.Repair || m.iDefault == (int)Working.Claim || m.iDefault == (int)Working.QC)
        //                   .ToList();
        //        }
        //    }
        //}

        public static void EditRepairStatus(RepairStatus model)
        {
            using (DataContext db = new DataContext())
            {
                db.Database.ExecuteSqlCommand("UpdateWorkingStatus @RepairId, @WorkingStatusId, @StaffId",
                    new SqlParameter("RepairId", model.kRepairId),
                    new SqlParameter("WorkingStatusId", model.kWorkingStatusId),
                    new SqlParameter("StaffId", model.kStaffId)
                );
            }
        }

        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.WorkingStatus.Where(m => m.sDescription.Contains(searching)).Count();
            }
        }

        public static int GetCountDuplicate(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.WorkingStatus
                        .Where(m => m.sDescription.Equals(searching))
                        .Count();
            }
        }

        public static List<WorkingStatus> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.WorkingStatus
                             .Where(m => m.sDescription.Contains(searching))
                             .OrderBy(m => m.sDescription)
                             .ToList();
            }
        }

        public static List<WorkingStatus> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results = db.WorkingStatus
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

        public static WorkingStatus GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                WorkingStatus model = db.WorkingStatus.Find(id);
                return model;
            }
        }

        public static void Create(WorkingStatus model)
        {
            using (DataContext db = new DataContext())
            {
                db.WorkingStatus.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(WorkingStatus model)
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
                WorkingStatus model = db.WorkingStatus.Find(id);

                if (model.iDefault==0)
                {
                    db.WorkingStatus.Remove(model);
                    db.SaveChanges();
                }
            }
        }

        public static Guid GetFromName(String text)
        {
            using (DataContext db = new DataContext())
            {
                WorkingStatus model = db.WorkingStatus.Find(text);
                return model.kWorkingStatusId;
            }
        }
    }
}
