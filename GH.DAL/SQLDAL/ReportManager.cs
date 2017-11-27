using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data.Entity;

namespace GH.DAL.SQLDAL
{
    public class ReportManager
    {
        //DAY//
        public static List<Repair> ReportRepairDay(DateTime start,DateTime end)
        {
            using (DataContext db = new DataContext())
            {
                end = end.AddHours(23);
                //start = start.AddDays(-1);
                //return วันที่ add มากกว่า start
                var query1 = db.Repairs
                            .Include(m => m.Customer)
                            .Include(m => m.Staff)
                            .Where(m => m.dtDateAdd.Value >= start && m.dtDateAdd.Value <= end);

                var query2 = db.Repairs
                            .Include(m => m.Customer)
                            .Include(m => m.Staff)
                            .Where(m => m.IsComplete == true 
                                    && m.IsCustomerRecieved == true 
                                    && (m.dtDateUpdate >= start && m.dtDateUpdate <= end)
                            );

                //debugging
                var result1 = query1.ToList();
                var result2 = query2.ToList();


                // union => intersect
                // concat => union all
                var m_return = query1.Concat(query2).OrderByDescending(m=>m.dtDateAdd).Distinct().ToList();
                return m_return;
            }
        }
        public static List<Claim> ReportClaimDay(DateTime start)
        {
            using (DataContext db = new DataContext())
            {
                var query1 = from r in db.Repairs
                             join c in db.Claims on r.sRepairNo equals c.sRepairNo
                             select c;

                query1 = query1.Include(m => m.Insurance).Include(m=>m.ClaimCauses)
                            .Where(m => m.dtDateAdd.Value >= start && DateTime.Compare(start,m.dtDateAdd.Value) <= 0);

                var query2 = from r in db.Repairs
                             join c in db.Claims on r.sRepairNo equals c.sRepairNo
                             select c;

                query2 = query2.Include(m => m.Insurance).Include(m => m.ClaimCauses)
                           .Where(m => m.IsComplete == true && m.IsRecieved == true && DateTime.Compare(start ,m.dtDateUpdate.Value) <= 0);


                // union => intersect
                // concat => union all
                var m_return = query1.Union(query2).OrderByDescending(m => m.dtDateAdd).ToList();

                return m_return;
            }
        }

        //Month//
        public static List<Repair> ReportRepairMonth(DateTime start, DateTime end)
        {
            using (DataContext db = new DataContext())
            {
                return db.Repairs
                        .Where(m => m.dtDateAdd >= start && m.dtDateAdd <= end)
                        .ToList();
            }
        }
        public static List<Claim> ReportClaimMonth(DateTime start, DateTime end)
        {
            using (DataContext db = new DataContext())
            {
                var query1 = from r in db.Repairs
                             join c in db.Claims on r.sRepairNo equals c.sRepairNo
                             select c;

                query1 = query1.Include(m => m.Insurance).Include(m => m.ClaimCauses)
                            .Where(m => m.dtDateAdd.Value >= start && m.dtDateAdd.Value <= end);

                var query2 = from r in db.Repairs
                             join c in db.Claims on r.sRepairNo equals c.sRepairNo
                             select c;

                var m_return = query1.Union(query2).OrderByDescending(m => m.dtDateAdd).ToList();

                return m_return;
            }
        }


        //service//
        public static List<Repair> ReportRepair(DateTime start, DateTime end)
        {
            using (DataContext db = new DataContext())
            {
                var query = db.Repairs
                            .Include(m=>m.RepairStatuies)
                            .Where(m=>m.IsComplete==true)
                            .Where(m => m.dtDateUpdate >= start && m.dtDateUpdate <= end)
                            .ToList();

                var m_result = query.OrderByDescending(m=>m.dtDateAdd).ToList();
                return m_result;
            }
        }
        public static List<Claim> ReportClaim(DateTime start, DateTime end)
        {
            using (DataContext db = new DataContext())
            {
                var query1 = from r in db.Repairs
                             join c in db.Claims on r.sRepairNo equals c.sRepairNo
                             select c;

                query1 = query1.Include(m => m.ClaimCauses)
                            .Where(m => m.IsComplete == true)
                            .Where(m => m.dtDateUpdate.Value >= start && m.dtDateUpdate.Value <= end);

                var m_return = query1.OrderByDescending(m=>m.dtDateAdd).ToList();

                return m_return;
            }
        }


        //service day//
        public static List<Repair> ReportServiceDay(DateTime start,DateTime end)
        {
            using (DataContext db = new DataContext())
            {
                //start = start.AddDays(-1);
                end = end.AddHours(23);

                var query1 = db.Repairs
                            .Include(m => m.Customer)
                            .Include(m => m.Staff)
                            .Where(m => m.dtDateAdd >= start && m.dtDateAdd <= end);
                         

                var query2 = db.Repairs
                            .Include(m => m.Customer)
                            .Include(m => m.Staff)
                            .Where(m => m.IsComplete == true 
                                    &&  m.IsCustomerRecieved == true
                                    && (m.dtDateUpdate >= start && m.dtDateUpdate <= end)
                            );

                var query3 = db.Repairs
                            .Include(m => m.Customer)
                            .Include(m => m.Staff)
                            .Where(m => m.IsComplete != true && m.IsCustomerRecieved != true ); 
                            //.Where(m => m.IsCustomerRecieved == true && m.IsCancle == true && DateTime.Compare(start, m.dtDateUpdate.Value) <= 0);
                            //.Where(m => (m.IsCustomerRecieved == null || m.IsCustomerRecieved != true) && DateTime.Compare(start, m.dtDateUpdate.Value) <= 0);

                //debugging
                var result1 = query1.ToList();
                var result2 = query2.ToList();
                var result3 = query3.ToList();


                // union => intersect
                // concat => union all
                var m_return = query1.Concat(query2).Concat(query3).OrderByDescending(m => m.dtDateAdd).Distinct().ToList();
                return m_return;
            }
        }
    }
}
