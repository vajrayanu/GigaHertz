using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;
using GH.DAL.Helpers;
using System.Data.Entity;

namespace GH.DAL.SQLDAL
{
    public class CustomerManager 
    {
        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Customers
                        .Where(m => m.sCustomerName.Contains(searching))
                        .Count();
            }
        }

        public static List<Customer> GetCountDuplicate(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Customers
                        .Where(m => m.sCustomerName.Equals(searching))
                        .ToList();
            }
        }

        public static List<Customer> GetCountDuplicateAndAddress(string name,string address)
        {
            using (DataContext db = new DataContext())
            {
                return db.Customers
                        .Where(m => m.sCustomerName.Equals(name) || m.sAddress1.Equals(address))
                        .ToList();
            }
        }

        public static List<Customer> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Customers
                        .OrderBy(m => m.sCustomerName)
                        .ToList();
            }
        }

        public static List<Customer> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Customers
                          .FullTextSearch(searching, true)
                          .ToList();
            }
        }

        public static List<Customer> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results =  db.Customers
                                .FullTextSearch(searching,true)
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
                    if (sorting.Contains("sEmailAddress"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sEmailAddress).ToList();
                    }
                }

                return m_results;
            }
        }

        public static Customer GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                Customer model = db.Customers.Find(id);
                return model;
            }
        }

        public static Customer GetByName(String name)
        {
            using (DataContext db = new DataContext())
            {
                Customer model = db.Customers.Where(m => m.sCustomerName.Equals(name)).SingleOrDefault();
                //Customer model = db.Customers.FullTextSearch(name, true).SingleOrDefault();
                return model;
            }
        }

        //public static List<Customer> GetByRepair(Guid id)
        //{
        //    using (DataContext db = new DataContext())
        //    {
        //        return db.Customers
        //               .Where(m=>m.kCustomerId==id)
        //               .ToList();
                       
        //    }
        //}

       

        public static void Create(Customer model)
        {
            using (DataContext db = new DataContext())
            {
                db.Customers.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(Customer model)
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
                Customer model = db.Customers.Find(id);
                db.Customers.Remove(model);
                db.SaveChanges();
            }
        }

        public static List<Repair> GetRepairs(Customer model)
        {
            using (DataContext db = new DataContext())
            {
                return db.Repairs
                         .Include(m => m.Product)
                         .Include(m => m.RepairStatuies)
                         .Where(m => m.kCustomerId == model.kCustomerId)
                         .OrderByDescending(m => m.sRepairNo)
                         .ToList();
            }
        }
    }
}
