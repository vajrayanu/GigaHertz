using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using GH.DAL.Helpers;
namespace GH.DAL.SQLDAL
{
    public class ProductManager
    {
        public static int GetCountByFiltering(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Products
                        .Include(m=>m.ProductType)
                        .Include(m => m.Brand)
                        .Where(m => m.sProductName.Contains(searching))
                        .Count();
            }
        }

        public static List<Product> GetCountDuplicate(string product)
        {
            using (DataContext db = new DataContext())
            {
                return db.Products
                        .Where(m => m.sProductModel.Equals(product))
                        //.Where(m=>m.Brand.sBrandName.Equals(brand))
                        //.Where(m => m.ProductType.sDescription.Equals(produtType))
                        .ToList();
            }
        }

        public static List<Product> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Products.Include(m=>m.Brand).Include(m=>m.ProductType)
                        .OrderBy(m => m.sProductName)
                        .ToList();
            }
        }

        public static List<Product> GetBySearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Products
                          .Include(m => m.ProductType)
                          .Include(m => m.Brand)
                          .Where(m => m.sProductName.Contains(searching))
                          .Distinct()
                          .ToList();
            }
        }

      

        public static List<Product> GetByFilterings(string searching, int startIndex, int pageSize, string sorting)
        {
            using (DataContext db = new DataContext())
            {
                if (sorting == null)
                    sorting = "";

                var m_results = db.Products
                                   .Include(m => m.ProductType)
                                   .Include(m => m.Brand)
                                   .Where(m => m.sProductName.Contains(searching))
                                   .OrderByDescending(m => m.dtDateAdd).OrderByDescending(m => m.dtDateUpdate)
                                   .Skip(startIndex)
                                   .Take(pageSize)
                                   .ToList();


                if (sorting.Contains("ASC"))
                {
                    if (sorting.Contains("sProductName"))
                    {
                        m_results = m_results.OrderBy(m => m.sProductName).ToList();
                    }
                    if (sorting.Contains("sProductModel"))
                    {
                        m_results = m_results.OrderBy(m => m.sProductModel).ToList();
                    }
                    if (sorting.Contains("sBrandDescription"))
                    {
                        m_results = m_results.OrderBy(m => m.vBrandDescription).ToList();
                    }
                    if (sorting.Contains("sProductTypeDescription"))
                    {
                        m_results = m_results.OrderBy(m => m.vProductTypeDescription).ToList();
                    }
                }
                else
                {
                    if (sorting.Contains("sProductName"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sProductName).ToList();
                    }
                    if (sorting.Contains("sProductModel"))
                    {
                        m_results = m_results.OrderByDescending(m => m.sProductModel).ToList();
                    }
                    if (sorting.Contains("sBrandDescription"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vBrandDescription).ToList();
                    }
                    if (sorting.Contains("sProductTypeDescription"))
                    {
                        m_results = m_results.OrderByDescending(m => m.vProductTypeDescription).ToList();
                    }
                }

                return m_results;
            }
        }

        public static Product GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                Product model = db.Products
                                .Include(m=>m.Brand)
                                .Include(m=>m.ProductType)
                                .Where(m=>m.kProductId == id)
                                .SingleOrDefault();
                              
                return model;
            }
        }

        public static List<Product> GetFromRepair(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Products 
                    .Include(m=>m.Brand)
                    .Include(m=>m.ProductType)
                    .Where(m => m.kProductId == id).ToList();
            }
        }

        public static Product GetByName(String name)
        {
            using (DataContext db = new DataContext())
            {
                Product model = db.Products.Where(m => m.sProductName.Equals(name)).SingleOrDefault();
              
                return model;
            }
        }

        public static void Create(Product model)
        {
            using (DataContext db = new DataContext())
            {
                db.Products.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(Product model)
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
                Product model = db.Products.Find(id);
                db.Products.Remove(model);
                db.SaveChanges();
            }
        }

        public static List<Product> FulltextSearch(string searching)
        {
            using (DataContext db = new DataContext())
            {
                return db.Products
                        .FullTextSearch(searching,true)
                        .ToList();
            }
        }
    }
}
