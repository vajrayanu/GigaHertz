using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.Entity;
using System.Transactions;
using System.Data.Objects;
using GH.DAL.Helpers;
using System.Data.SqlClient;
using System.IO;


namespace GH.DAL.SQLDAL
{
    public class RepairManager
    {
        public static int GetCountByFiltering(string searching, string status)
        {
            using (DataContext db = new DataContext())
            {
                var m_results = db.Repairs
                        .FullTextSearch(searching, true)
                        .ToList();

                if (!string.IsNullOrEmpty(status))
                    m_results = m_results.Where(m => m.kWorkingStatusId.ToString() == status.ToString()).ToList();

                return m_results.Count();

            }
        }

        public static List<Repair> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Repairs
                        //.Include(m => m.Product)
                        //.Include(m => m.Customer)
                        //.Include(m => m.Staff)
                        .OrderBy(m => m.sRepairNo)
                        .ToList();
            }
        }

        public static List<Repair> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                //return m_results;
                var m_results = db.Repairs
                                //.FullTextSearch(searching)
                               .Include(m => m.Product)
                               .Include(m => m.Customer)
                               .Where(m => m.sRepairNo.Contains(searching) 
                               //.Where(m => m.IsComplete != true && m.sRepairNo.Contains(searching) 
                                   || m.Product.sProductName.Contains(searching) 
                                   || m.Customer.sCustomerName.Contains(searching))
                               .OrderByDescending(m => m.dtDateAdd)
                               .OrderByDescending(m => m.dtDateUpdate)
                               .ToList();

                return m_results;
            }
        }

        public static List<Repair> GetBySearchForClaim(string searching)
        {
            using (DataContext db = new DataContext())
            {
                var m_results = db.Repairs
                               .Include(m => m.Product)
                               .Include(m => m.Customer)
                               .Where(m => 
                                   (m.IsComplete != true || m.IsComplete == null) 
                                   && (m.sRepairNo.Contains(searching) 
                                       || m.Product.sProductName.Contains(searching)
                                       || m.Customer.sCustomerName.Contains(searching)))
                               .OrderByDescending(m => m.dtDateAdd)
                               .OrderByDescending(m => m.dtDateUpdate)
                               .ToList();

                return m_results;
            }
        }

        public static Repair GetByTracking(string searching)
        {
            using (DataContext db = new DataContext())
            {
                var m_results = db.Repairs
                                .Include(m => m.Product)
                                .Include(m => m.Customer)
                                .SingleOrDefault(x => x.sRepairNo.Equals(searching));
                              

                return m_results;
            }
        }

        public static List<Repair> GetByFilterings(string searching, int startIndex, int pageSize, string sorting ,string status)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results = db.Repairs
                                //.FullTextSearch(searching)
                                .Include(m => m.Product)
                                .Include(m => m.Customer)
                                .Where(m=> m.sRepairNo.Contains(searching) || m.Product.sProductName.Contains(searching) || m.Customer.sCustomerName.Contains(searching))
                                //.OrderBy(m => m.IsComplete) 
                                .OrderByDescending(m => m.dtDateUpdate)
                                //.OrderByDescending(m => m.dtDateAdd)
                                
                                //.Skip(startIndex).Take(pageSize)
                                .ToList();

                if (!string.IsNullOrEmpty(status))
                    m_results = m_results.Where(m => m.kWorkingStatusId.ToString() == status.ToString()).ToList();
                

                if (sorting.Contains("ASC"))
                {
                    if (sorting.Contains("sRepairNo"))
                    {
                        m_results = m_results.OrderBy(m => m.sRepairNo).ToList();
                    }
                    if (sorting.Contains("vCustomerName"))
                    {
                        m_results = m_results.OrderBy(m => m.vCustomerName).ToList();
                    }
                    if (sorting.Contains("vProductName"))
                    {
                        m_results = m_results.OrderBy(m => m.vProductName).ToList();
                    }
                    if (sorting.Contains("vWorkingStatus"))
                    {
                        m_results = m_results.OrderBy(m => m.vWorkingStatus).ToList();
                    }
                    if (sorting.Contains("vStaffName"))
                    {
                        m_results = m_results.OrderBy(m => m.vStaffName).ToList();
                    }
                    if (sorting.Contains("vDueDate"))
                    {
                        m_results = m_results.OrderBy(m => m.dtDueDate).ToList();
                    }
                    if (sorting.Contains("vDateAdd"))
                    {
                        m_results = m_results.OrderBy(m => m.dtDateAdd).ToList();
                    }
                }
                else
                {
                    if (sorting.Contains("sRepairNo"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sRepairNo).ToList();
                    }
                    if (sorting.Contains("vCustomerName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vCustomerName).ToList();
                    }
                    if (sorting.Contains("vProductName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vProductName).ToList();
                    }
                    if (sorting.Contains("vWorkingStatus"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vWorkingStatus).ToList();
                    }
                    if (sorting.Contains("vStaffName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vStaffName).ToList();
                    }
                    if (sorting.Contains("vDueDate"))
                    {
                        m_results = m_results.OrderByDescending(m => m.dtDueDate).ToList();
                    }
                    if (sorting.Contains("vDateAdd"))
                    {
                        m_results = m_results.OrderByDescending(m => m.dtDateAdd).ToList();
                    }
                }

                return pageSize > 0
                      ? m_results.Skip(startIndex).Take(pageSize).ToList() //Paging
                      : m_results.ToList(); //No paging

                //return m_results;
            }
        }

        public static List<Repair> GetByFilteringsForStaff(string searching, int startIndex, int pageSize, string sorting, string status,Guid staffId,out int totalsize)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var query1 = from r in db.Repairs
                             join rs in db.RepairStatuies on r.kRepairId equals rs.kRepairId
                             where rs.kStaffId.Equals(staffId)
                             select r;

                var m_results = query1
                                .Include(m => m.Product)
                                .Include(m => m.Customer)
                                .Where(m => m.sRepairNo.Contains(searching) || m.Product.sProductName.Contains(searching) || m.Customer.sCustomerName.Contains(searching))
                                .OrderByDescending(m => m.dtDateUpdate)
                                .ToList().Distinct();

                if (!string.IsNullOrEmpty(status))
                    m_results = m_results.Where(m => m.kWorkingStatusId.ToString() == status.ToString()).ToList();

                if (sorting.Contains("ASC"))
                {
                    if (sorting.Contains("sRepairNo"))
                    {
                        m_results = m_results.OrderBy(m => m.sRepairNo).ToList();
                    }
                    if (sorting.Contains("vCustomerName"))
                    {
                        m_results = m_results.OrderBy(m => m.vCustomerName).ToList();
                    }
                    if (sorting.Contains("vProductName"))
                    {
                        m_results = m_results.OrderBy(m => m.vProductName).ToList();
                    }
                    if (sorting.Contains("vWorkingStatus"))
                    {
                        m_results = m_results.OrderBy(m => m.vWorkingStatus).ToList();
                    }
                    if (sorting.Contains("vStaffName"))
                    {
                        m_results = m_results.OrderBy(m => m.vStaffName).ToList();
                    }
                    if (sorting.Contains("vDueDate"))
                    {
                        m_results = m_results.OrderBy(m => m.dtDueDate).ToList();
                    }
                    if (sorting.Contains("vDateAdd"))
                    {
                        m_results = m_results.OrderBy(m => m.dtDateAdd).ToList();
                    }
                }
                else
                {
                    if (sorting.Contains("sRepairNo"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sRepairNo).ToList();
                    }
                    if (sorting.Contains("vCustomerName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vCustomerName).ToList();
                    }
                    if (sorting.Contains("vProductName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vProductName).ToList();
                    }
                    if (sorting.Contains("vWorkingStatus"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vWorkingStatus).ToList();
                    }
                    if (sorting.Contains("vStaffName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vStaffName).ToList();
                    }
                    if (sorting.Contains("vDueDate"))
                    {
                        m_results = m_results.OrderByDescending(m => m.dtDueDate).ToList();
                    }
                    if (sorting.Contains("vDateAdd"))
                    {
                        m_results = m_results.OrderByDescending(m => m.dtDateAdd).ToList();
                    }
                }

                totalsize = m_results.Count();

                return pageSize > 0
                      ? m_results.Skip(startIndex).Take(pageSize).ToList() //Paging
                      : m_results.ToList(); //No paging

                //return m_results;
            }
        }


        public static Repair GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                Repair model = db.Repairs
                                    .Include(m => m.Product)
                                    .Include(m => m.Customer)
                                    //.Include(m => m.WorkingStatus)
                                    //.Include(m => m.Staff)
                                    .SingleOrDefault(x => x.kRepairId == id);
                return model;
            }
        }

        public static Repair GetByRepairNo(String repairNo)
        {
            using (DataContext db = new DataContext())
            {
                var m_return = db.Repairs
                       .Include(m => m.Customer)
                       .Include(m=>m.Product)
                       .FirstOrDefault(x => x.sRepairNo == repairNo);
                return m_return;
            }
        }

        public static List<Repair> GetRepair(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Repairs
                    .Include(m => m.Product)
                    .Include(m => m.Product.ProductType)
                    .Include(m => m.Product.Brand)
                    .Include(m => m.Customer)
                    .Include(m => m.FileUploads)
                    .Where(m => m.kRepairId == id)
                    .OrderByDescending(m=>m.dtDateAdd)
                    .ToList();
            }
        }

        public static List<RepairCause> GetRepairCause(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.RepairCauses
                    .Where(m => m.kRepairId == id)
                    .OrderByDescending(m => m.dtDateAdd)
                    .ToList();
            }
        }

        public static List<RepairCauseEstimate> GetRepairCauseEstimate(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.RepairCauseEstimates
                    .Where(m => m.kRepairId == id)
                    .OrderByDescending(m => m.dtDateAdd)
                    .ToList();
            }
        }

        public static List<RepairStatus> GetRepairStatus(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.RepairStatuies
                    .Include(m => m.WorkingStatus)
                    .Include(m => m.Staff)
                    .Where(m => m.kRepairId == id)
                    .OrderByDescending(m => m.dtDateAdd)
                    .ToList();
            }
        }

        public static List<Repair> GetByCustomer(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Repairs
                        .Include(m => m.Product)
                        .Include(m => m.Customer)
                        //.Include(//m => m.WorkingStatus)
                        //.Include(m => m.Staff)

                        .Where(m => m.kCustomerId.Equals(id))
                        .ToList();
            }
        }

        public static void Create(Repair model)
        {
            bool saved = false;
            using (DataContext db = new DataContext())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        //db.RepairCauses.Add(model.RepairCause);
                        //db.RepairStatus.Add(model.RepairStatus);
                        db.Repairs.Add(model);
                        db.SaveChanges();
                        saved = true;
                    }
                    catch(Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if(saved)
                        {
                            scope.Complete();
                            db.Dispose();
                        }
                    }
                }
            }
        }

        public static void Edit(Repair model)
        {
            using (DataContext2 db = new DataContext2())
            {
                Repairs Repairs = new Repairs
                {
                    kRepairId = model.kRepairId,
                    kCustomerId = model.kCustomerId,
                    kProductId = model.kProductId,
                    kBookingId = model.kBookingId ?? Guid.Empty,
                    kOwnerId = model.kOwnerId ?? Guid.Empty,
                    kStaffId = model.kStaffId ?? Guid.Empty,
                    kQCId = model.kQCId ?? Guid.Empty,
                    sRepairNo = model.sRepairNo,
                    sColor = model.sColor,
                    sSerial = model.sSerial,
                    dtInsuranceExpire = model.dtInsuranceExpire,
                    sNote = model.sNote,
                    sProductAccessories = model.sProductAccessories,
                    iDayWarranty = model.iDayWarranty,
                    dtEndWarranty = model.dtEndWarranty,
                    dtDueDate = model.dtDueDate,
                    dtDateAdd = model.dtDateAdd,
                    dtDateUpdate = model.dtDateUpdate,
                    IsComplete = model.IsComplete,
                    IsCustomerRecieved = model.IsCustomerRecieved,
                    iRemind = model.iRemind,
                    kCloseStatusId = model.kCloseStatusId ?? Guid.Empty,
                    IsNormal = model.IsNormal,
                    IsNoCredit = model.IsNoCredit,
                    IsQCPass = model.IsQCPass,
                    IsBack = model.IsBack,
                    IsInsurance = model.IsInsurance,
                    IsCancle = model.IsCancle,
                    IsFree = model.IsFree
                };
                db.Entry(Repairs).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void EditQC(Repair repair,string username,string password)
        {
            using (DataContext db = new DataContext())
            {
                db.Database.ExecuteSqlCommand("UpdateRepairQC @RepairId, @UseName, @Password, @IsQCPass",
                    new SqlParameter("RepairId", repair.kRepairId),
                    new SqlParameter("UseName", username),
                    new SqlParameter("Password", password),
                    new SqlParameter("IsQCPass", repair.IsQCPass)
                );
            }
        }

        public static void Delete(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                List<RepairStatus> RepairStatus = db.RepairStatuies.Where(m => m.kRepairId.Equals(id)).ToList();
                foreach (var i in RepairStatus)
                {
                    db.RepairStatuies.Remove(i);
                    db.SaveChanges();
                }

                List<RepairCause> RepairCauses = db.RepairCauses.Where(m => m.kRepairId.Equals(id)).ToList();
                foreach (var i in RepairCauses)
                {
                    db.RepairCauses.Remove(i);
                    db.SaveChanges();
                }

                List<RepairCauseEstimate> RepairCauseEstimates = db.RepairCauseEstimates.Where(m => m.kRepairId.Equals(id)).ToList();
                foreach (var i in RepairCauseEstimates)
                {
                    db.RepairCauseEstimates.Remove(i);
                    db.SaveChanges();
                }

                List<FileUpload> FileUploads = db.FileUploads.Where(m => m.kRepairId == id ).ToList();
                foreach (var i in FileUploads)
                {
                    db.FileUploads.Remove(i);
                    db.SaveChanges();

                    var filePost = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + i.sFileUrl);
                    File.Delete(filePost);
                }

                Repair model = db.Repairs.Find(id);
                db.Repairs.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
