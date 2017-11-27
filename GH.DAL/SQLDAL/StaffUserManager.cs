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

namespace GH.DAL.SQLDAL
{
    public class StaffUserManager
    {
        //pass staffid
        public static User GetStaffUser(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                User model = db.Users
                                    .Where(m => m.UserId == id)
                                    .SingleOrDefault();

                return model;
            }
        }

        public static User GetStaffByName(String name)
        {
            using (DataContext db = new DataContext())
            {
                User model = db.Users
                                    .Include(m=>m.Roles)
                                    .Where(m => m.Username == name)
                                    .SingleOrDefault();

                return model;
            }
        }

        public static List<User> GetUser(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Users
                    .Include(m=>m.Roles)
                    .Where(m => m.UserId == id)
                    .ToList();
            }
        }

        public static Role GetRole(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                return db.Roles
                    .Find(id);
            }
        }
    }
}
