using System.Collections.Generic;
using System.Linq;
using GH.DAL.Model;
using GH.DAL.Context;
using System;

namespace GH.DAL.SQLDAL
{
    public class StaffPositionManager
    {
        public static List<StaffPosition> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.StaffPositions
                        .OrderBy(m => m.sDescription)
                        .ToList();
            }
        }

        public static StaffPosition GetById(Guid id)
        {
            using (DataContext db = new DataContext())
            {
                StaffPosition model = db.StaffPositions.Find(id);
                return model;
            }
        }
    }
}
