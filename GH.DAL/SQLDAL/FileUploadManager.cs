using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System.Collections.Generic;
using System;
using System.Data;

namespace GH.DAL.SQLDAL
{
    public class FileUploadManager
    {
        public static List<FileUpload> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.FileUploads
                        .ToList();
            }
        }

        public static List<FileUpload> GetBySession(Guid sessionid)
        {
            using (DataContext db = new DataContext())
            {
                return db.FileUploads
                        .Where(m=>m.kSessionId==sessionid)
                        .ToList();
            }
        }

        public static List<FileUpload> GetByRepair(Guid repairid)
        {
            using (DataContext db = new DataContext())
            {
                return db.FileUploads
                        .Where(m => m.kRepairId == repairid)
                        .ToList();
            }
        }

        public static FileUpload GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                FileUpload model = db.FileUploads.Find(id);
                return model;
            }
        }

        public static void Create(FileUpload model)
        {
            using (DataContext db = new DataContext())
            {
                db.FileUploads.Add(model);
                db.SaveChanges();
            }
        }

        public static void Edit(FileUpload model)
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
                FileUpload model = db.FileUploads.Find(id);
                db.FileUploads.Remove(model);
                db.SaveChanges();
            }
        }
    }
}
