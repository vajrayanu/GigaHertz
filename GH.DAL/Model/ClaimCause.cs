using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections.Generic;
using GH.DAL.Helpers;
using GH.DAL.SQLDAL;

namespace GH.DAL.Model
{
    public class ClaimCause
    {
         [Key]
        public Guid kClaimCauseId { get; set; }

        public Guid kClaimId { get; set; }

        public Guid kStaffId { get; set; }

        public DateTime? dtDateAdd { get; set; }

        [Display(Name = "อาการ"), Required(ErrorMessage = "* This field is required"), StringLength(100)]
        public String sDescription { get; set; }

        [Display(Name = "ค่าบริการส่งซ่อม"), DisplayFormat(DataFormatString = "{0:c}")]
        public Decimal? dPrice { get; set; }

        [Display(Name = "จำนวน")]
        public int? iQty { get; set; }


        [Display(Name = "หมายเหตุ"), StringLength(100)]
        public String sNote { get; set; }


        public ClaimCause()
        {
            dtDateAdd = DateTime.Now;
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

        public dynamic vStaffName
        {
            get
            {
                if (kStaffId != Guid.Empty)
                    return StaffManager.GetById(kStaffId).sStaffName;
                else
                    return "- ";
            }
        }
    }
}
