using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using GH.DAL.Helpers;

namespace GH.DAL.Model
{
    public class RemindHistory
    {
        [Key]
        public int kRemindHistoryId { get; set; }
        public Guid? kStaffId { get; set; }

        [Display(Name = "ข้อความ"), StringLength(40)]
        public string sRemind { get; set; }
        public DateTime dtDateAdd { get; set; }

        public Staff Staff { get; set; }


        public RemindHistory()
        {
            dtDateAdd = DateTime.Now;
        }


        public dynamic vDateAdd
        {
            get
            {
                if (dtDateAdd != null)
                    return string.Format("{0}, {1}", dtDateAdd.ToShortTimeString(), DateExtension.DateThaiFormatShort(dtDateAdd));
                else
                    return "";
            }
        }
    }
}
