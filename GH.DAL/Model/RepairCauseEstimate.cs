using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections.Generic;
using GH.DAL.Helpers;
using GH.DAL.SQLDAL;

namespace GH.DAL.Model
{
    public class RepairCauseEstimate
    {
        [Key]
        public Guid kRepairCauseEstimateId { get; set; }

        public Guid kRepairId { get; set; }

        public Guid kStaffId { get; set; }

        public DateTime? dtDateAdd { get; set; }

        [Display(Name = "อาการ"), Required(ErrorMessage = "* This field is required"), StringLength(100)]
        public String sDescription { get; set; }

        //{0:0,0}
        [Display(Name = "ค่าซ่อม"), DisplayFormat(DataFormatString = "{0:c}")]
        public Decimal? dPrice { get; set; }

        // IsHidden จะเป็น true เมื่อ front เป็นคน input และจะแสดงผลใน ใบรายงานต่อลูกค้า
        // IsHidden จะเป็น false เมื่อช่าง + หัวหน้าช่างเป็นคน input และจะไม่แสดงผลใน ใบรายงานต่อลูกค้า
        //public Boolean? IsHidden { get; set; }

        public RepairCauseEstimate()
        {
            dtDateAdd = DateTime.Now;
        }


        //public Staff Staff { get; set; }
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
