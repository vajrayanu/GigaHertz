using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using GH.DAL.SQLDAL;
using GH.DAL.Helpers;

namespace GH.DAL.Model
{
    public class Remind
    {
        [Key]
        public Guid kRemindId { get; set; }

        public Guid kRepairId { get; set; }

        public Guid kStaffId { get; set; }

        public string sNote { get; set; }

        public DateTime? dtDateAdd { get; set; }


     

        public Remind()
        {
            dtDateAdd = DateTime.Now;
        }

        public dynamic vStaffName
        {
            get
            {
                return StaffManager.GetById(kStaffId).sStaffName;
            }
        }

        public dynamic vWorkingTime
        {
            get
            {
                return dtDateAdd.Value.ToShortTimeString();
            }
        }
        public dynamic vWorkingDate
        {
            get
            {
                if (dtDateAdd != null)
                    return string.Format("{0}", DateExtension.DateThaiFormatShort(dtDateAdd.Value));
                else
                    return "";
            }
        }
    }
}
