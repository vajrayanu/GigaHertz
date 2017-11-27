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


namespace GH.DAL.SQLDAL
{
    public class ClaimManager
    {
        public static int GetCountByFiltering(string searching, string status)
        {
            using (DataContext db = new DataContext())
            {
                var m_results = db.Claims
                                .FullTextSearch(searching, true)
                                .ToList();

                if (!string.IsNullOrEmpty(status))
                    m_results = m_results.Where(m => m.kWorkingStatusId.ToString() == status.ToString()).ToList();

                return m_results.Count();
            }
        }

        public static List<Claim> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Claims
                    //.Include(m => m.Product)
                    //.Include(m => m.Customer)
                    //.Include(m => m.Staff)
                        .OrderBy(m => m.sClaimNo)
                        .ToList();
            }
        }

        public static List<Claim> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                var m_results = db.Claims
                               .Include(m => m.Product)
                               .Include(m => m.Insurance)
                               .Where(m => m.sClaimNo.Contains(searching) || m.sRepairNo.Contains(searching)|| m.Product.sProductName.Contains(searching) || m.Insurance.sInsuranceName.Contains(searching))
                               //.OrderBy(m => m.IsComplete) 
                               .OrderByDescending(m => m.dtDateAdd)
                               .OrderByDescending(m => m.dtDateUpdate)
                               .ToList();

                return m_results;
            }
        }

        public static List<Claim> GetByFilterings(string searching, int startIndex, int pageSize, string sorting, string status)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

               var m_results = db.Claims
                               .Include(m => m.Product)
                               .Include(m => m.Insurance)
                               .Where(m => m.sClaimNo.Contains(searching) || m.sRepairNo.Contains(searching)|| m.Product.sProductName.Contains(searching) || m.Insurance.sInsuranceName.Contains(searching))
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
                    if (sorting.Contains("sClaimNo"))
                    {
                        m_results = m_results.OrderBy(m => m.sClaimNo).ToList();
                    }
                    if (sorting.Contains("vInsuranceName"))
                    {
                        m_results = m_results.OrderBy(m => m.vInsuranceName).ToList();
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
                    if (sorting.Contains("dtDateAdd"))
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
                    if (sorting.Contains("sClaimNo"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sClaimNo).ToList();
                    }
                    if (sorting.Contains("vInsuranceName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vInsuranceName).ToList();
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
                    if (sorting.Contains("vDateAdd"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vDateAdd).ToList();
                    }
                }
                return pageSize > 0
                     ? m_results.Skip(startIndex).Take(pageSize).ToList() //Paging
                     : m_results.ToList(); //No paging
                //return m_results;
            }
        }

        public static List<Claim> GetByFilteringsForStaff(string searching, int startIndex, int pageSize, string sorting, string status, Guid staffId, out int totalsize)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var query1 = from c in db.Claims
                             join r in db.Repairs on c.sRepairNo equals r.sRepairNo 
                             join rs in db.ClaimStatuies on c.kClaimId equals rs.kClaimId
                             where rs.kStaffId.Equals(staffId)
                             select c;

                var m_results = query1
                                .Include(m => m.Product)
                                .Include(m => m.Insurance)
                                .Where(m => m.sRepairNo.Contains(searching) || m.Product.sProductName.Contains(searching) || m.Insurance.sInsuranceName.Contains(searching))
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
                    if (sorting.Contains("sClaimNo"))
                    {
                        m_results = m_results.OrderBy(m => m.sClaimNo).ToList();
                    }
                    if (sorting.Contains("vInsuranceName"))
                    {
                        m_results = m_results.OrderBy(m => m.vInsuranceName).ToList();
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
                    if (sorting.Contains("sClaimNo"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sClaimNo).ToList();
                    }
                    if (sorting.Contains("vInsuranceName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vInsuranceName).ToList();
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



        public static Claim GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                Claim model = db.Claims
                                    .Include(m => m.Product)
                                    .Include(m => m.Insurance)
                                    //.Include(m => m.ClaimStatuies)
                                    //.Include(m => m.Staff)
                                    .SingleOrDefault(x => x.kClaimId == id);

                return model;
            }
        }

        public static Claim GetByRepairNo(String repairNo)
        {
            using (DataContext db = new DataContext())
            {
                var m_return = db.Claims
                                .Include(m=>m.ClaimCauses)
                                .FirstOrDefault(x=>x.sRepairNo==repairNo);
                                //.SingleOrDefault(x => x.sRepairNo  == repairNo);
                return m_return;
            }
        }

        public static List<ClaimCause> GetByRepair(Guid claimid)
        {
            using (DataContext db = new DataContext())
            {
                return db.ClaimCauses
                       .Where(m => m.kClaimId == claimid).ToList();
            }
        }

        public static List<Claim> GetClaim(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Claims
                    .Include(m => m.Product)
                    .Include(m => m.Product.ProductType)
                    .Include(m => m.Product.Brand)
                    .Include(m => m.Insurance)
                    .Where(m => m.kClaimId == id)
                    .OrderByDescending(m => m.dtDateAdd)
                    .ToList();
            }
        }

        public static List<ClaimCause> GetClaimCause(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.ClaimCauses
                    .Where(m => m.kClaimId == id)
                    .OrderByDescending(m => m.dtDateAdd)
                    .ToList();
            }
        }

        public static List<ClaimStatus> GetClaimStatus(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.ClaimStatuies
                    .Include(m => m.WorkingStatus)
                    .Include(m => m.Staff)
                    .Where(m => m.kClaimId == id)
                    .OrderByDescending(m => m.dtDateAdd)
                    .ToList();
            }
        }

        public static List<Claim> GetByInsurance(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Claims
                        .Include(m => m.Product)
                        .Include(m => m.Insurance)
                        //.Include(m => m.WorkingStatus)
                        //.Include(m => m.Staff)

                        .Where(m => m.kInsuranceId.Equals(id))
                        .ToList();
            }
        }

        public static void Create(Claim model)
        {
            bool saved = false;
            using (DataContext db = new DataContext())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        //db.ClaimCauses.Add(model.ClaimCause);
                        //db.ClaimStatus.Add(model.ClaimStatus);
                        db.Claims.Add(model);
                        db.SaveChanges();
                        saved = true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (saved)
                        {
                            scope.Complete();
                            db.Dispose();
                        }
                    }
                }
            }
        }

        public static void Edit(Claim model)
        {
            using (DataContext2 db = new DataContext2())
            {
                Claims Claims = new Claims
                {
                    kClaimId = model.kClaimId,
                    kInsuranceId = model.kInsuranceId,
                    kProductId = model.kProductId,
                    kStaffId = model.kStaffId ?? Guid.Empty,
                    kOwnerId = model.kOwnerId ?? Guid.Empty,
                    sClaimNo = model.sClaimNo,
                    sRepairNo = model.sRepairNo,
                    sSerial = model.sSerial,
                    dtInsuranceExpire = model.dtInsuranceExpire,
                    dtDateAdd = model.dtDateAdd,
                    dtDateUpdate = model.dtDateUpdate,
                    IsComplete = model.IsComplete,
                    IsRecieved = model.IsRecieved,
                    IsNoCredit = model.IsNoCredit
                };
                db.Entry(Claims).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                List<ClaimStatus> ClaimStatus = db.ClaimStatuies.Where(m => m.kClaimId.Equals(id)).ToList();
                foreach (var i in ClaimStatus)
                {
                    db.ClaimStatuies.Remove(i);
                    db.SaveChanges();
                }

                List<ClaimCause> ClaimCauses = db.ClaimCauses.Where(m => m.kClaimId.Equals(id)).ToList();
                foreach (var i in ClaimCauses)
                {
                    db.ClaimCauses.Remove(i);
                    db.SaveChanges();
                }
                Claim model = db.Claims.Find(id);
                db.Claims.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
