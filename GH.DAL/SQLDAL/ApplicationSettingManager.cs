using System.Linq;
using GH.DAL.Context;
using GH.DAL.Model;
using System;
using System.Data;

namespace GH.DAL.SQLDAL
{
    public class ApplicationSettingManager
    {
        public static ApplicationSetting GetApplicationSetting()
        {
            using (DataContext db = new DataContext())
            {
                return db.ApplicationSettings.SingleOrDefault();
            }
        }

        public static void Save(ApplicationSetting model)
        {
            using (DataContext db = new DataContext())
            {
                if (model.kApplicationSettingId != Guid.Empty)
                {
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    model.kApplicationSettingId = Guid.NewGuid();
                    db.ApplicationSettings.Add(model);
                }
                db.SaveChanges();
            }
        }
    }
}
