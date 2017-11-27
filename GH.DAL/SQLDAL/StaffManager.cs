using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using GH.DAL.Helpers;

namespace GH.DAL.SQLDAL
{
    public class StaffManager
    {
        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Staffs
                         .Include(m => m.StaffPosition)
                         .Where(m => m.sStaffName.Contains(searching)).Count();
            }
        }

        public static List<Staff> GetCountDuplicate(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Staffs
                        .Where(m => m.sStaffName.Equals(searching))
                        .ToList();
            }
        }


        public static List<Staff> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Staffs
                        .Include(m => m.StaffPosition)
                        .OrderBy(m => m.sStaffName)
                        .ToList();
            }
        }

        public static List<Staff> GetByPosition(string text)
        {
            using (DataContext db = new DataContext())
            {
                if (text == "หัวหน้าช่าง")
                {
                    return db.Staffs
                          .OrderBy(m => m.sStaffName)
                          .Where(m => m.StaffPosition.sDescription == "ฝ่ายตรวจสอบคุณภาพ"
                              || m.StaffPosition.sDescription == "ฝ่ายรับสินค้า"
                              || m.StaffPosition.sDescription == "ฝ่ายเครม"
                              || m.StaffPosition.sDescription == "ช่าง"
                              || m.StaffPosition.sDescription =="หัวหน้าช่าง"
                            )
                          .ToList();
                }
                else if (text == "ช่าง")
                {
                    return db.Staffs
                          .OrderBy(m => m.sStaffName)
                          .Where(m => m.StaffPosition.sDescription == "ฝ่ายตรวจสอบคุณภาพ"
                              || m.StaffPosition.sDescription == "ฝ่ายเครม"
                              || m.StaffPosition.sDescription == "ช่าง"
                              || m.StaffPosition.sDescription == "หัวหน้าช่าง")
                          .ToList();
                }
                else if (text == "ฝ่ายเครม")
                {
                    return db.Staffs
                         .OrderBy(m => m.sStaffName)
                         .Where
                         (
                            m => m.StaffPosition.sDescription == "ฝ่ายตรวจสอบคุณภาพ"
                            || m.StaffPosition.sDescription == "ฝ่ายเครม"
                            || m.StaffPosition.sDescription == "ช่าง"
                            || m.StaffPosition.sDescription == "หัวหน้าช่าง"
                         ).ToList();
                }
                else if (text == "ฝ่ายตรวจสอบคุณภาพ")
                {
                    return db.Staffs
                         .OrderBy(m => m.sStaffName)
                         .Where
                         (
                            m => m.StaffPosition.sDescription == "ฝ่ายตรวจสอบคุณภาพ"
                            || m.StaffPosition.sDescription == "ฝ่ายรับสินค้า"
                            || m.StaffPosition.sDescription == "ช่าง"
                            || m.StaffPosition.sDescription == "หัวหน้าช่าง"
                         ).ToList();
                }
                else if (text == "ฝ่ายรับสินค้า")
                {
                    return db.Staffs
                         .OrderBy(m => m.sStaffName)
                         .Where(
                            m => m.StaffPosition.sDescription == "ช่าง"
                            || m.StaffPosition.sDescription == "หัวหน้าช่าง"
                            || m.StaffPosition.sDescription == "ฝ่ายรับสินค้า"
                         ).ToList();
                }
                else
                {
                    return db.Staffs.ToList();
                }
            }
        }
       
        public static List<Staff> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Staffs
                         .Include(m => m.StaffPosition)
                         .Where(m => m.sStaffName.Contains(searching))
                         .ToList();
            }
        }

        public static List<Staff> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";


                var m_results = db.Staffs
                               .Include(m => m.StaffPosition)
                               .Include(m=>m.StaffPosition)
                               .Where(m => m.sStaffName.Contains(searching))
                               .OrderBy(m => m.dtDateUpdate)
                               .OrderByDescending(m => m.dtDateAdd)
                               .Skip(startIndex).Take(pageSize)
                               .ToList();

                if (sorting.Contains("ASC"))
                {
                    if (sorting.Contains("vFullName"))
                    {
                        m_results = m_results.OrderBy(m => m.vFullName).ToList();
                    }
                    if (sorting.Contains("vStaffPositionDescription"))
                    {
                        m_results = m_results.OrderBy(m => m.vStaffPositionDescription).ToList();
                    }
                    if (sorting.Contains("sPhone"))
                    {
                        m_results = m_results.OrderBy(m => m.sPhone).ToList();
                    }
                    if (sorting.Contains("sMobile"))
                    {
                        m_results = m_results.OrderBy(m => m.sMobile).ToList();
                    }
                    if (sorting.Contains("sEmailAddress"))
                    {
                        m_results = m_results.OrderBy(m => m.sEmailAddress).ToList();
                    }
                }
                else
                {
                    if (sorting.Contains("vFullName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vFullName).ToList();
                    }
                    if (sorting.Contains("vStaffPositionDescription"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vStaffPositionDescription).ToList();
                    }
                    if (sorting.Contains("sPhone"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sPhone).ToList();
                    }
                    if (sorting.Contains("sMobile"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sMobile).ToList();
                    }
                    if (sorting.Contains("sEmailAddress"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sEmailAddress).ToList();
                    }
                }

                return m_results;
            }
        }

        public static Staff GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {

                Staff model = db.Staffs
                               .Include(m=>m.StaffPosition)
                               .Where(m => m.kStaffId.Equals(id))
                               .SingleOrDefault();

                return model;
            }
        }

        public static void Create(Staff model)
        {
            using (DataContext db = new DataContext())
            {
                db.Staffs.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(Staff model)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Staff model)
        {
            using (DataContext db = new DataContext())
            {
                User user = db.Users 
                            .Include(m=>m.Roles)
                            .Where(m=>m.UserId==model.kStaffId)
                            .SingleOrDefault();

                if (user != null)
                {
                    if (user.Roles.SingleOrDefault().RoleName == "Admin")
                    {
                        throw new Exception("Can not delete role Admin!");
                    }
                }

                db.Database.ExecuteSqlCommand("DeleteStaffUser @StaffId", new SqlParameter("StaffId", model.kStaffId));
            }
        }

        public static List<Repair> GetRepairs(Staff model)
        {
            using (DataContext db = new DataContext())
            {
                return db.Repairs
                         .Include(m => m.Product)
                         .Include(m=>m.RepairStatuies)
                         .Where(m=>m.kStaffId==model.kStaffId)
                         .OrderByDescending(m=>m.sRepairNo)
                         .ToList();
            }
        }

        public static List<Repair> GetRepairByStaffId(Guid id,DateTime start,DateTime end)
        {
            using (DataContext db = new DataContext())
            {
                return db.Repairs
                         .Where(m => m.kStaffId == id && m.dtDateAdd>=start && m.dtDateAdd<=end )
                         .OrderByDescending(m => m.sRepairNo)
                         .ToList();
            }
        }
    }
}
