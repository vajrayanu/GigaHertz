using System;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class Cause
    {
        [Key]
        public Guid kCauseId { get; set; }

        [Display(Name = "รหัส"), StringLength(50)]
        public String sCode { get; set; }

        [Display(Name = "อาการ"), Required(ErrorMessage = "* This field is required"), StringLength(100)]
        public String sDescription { get; set; }

        [Display(Name = "ค่าซ่อม"), DisplayFormat(DataFormatString = "{0:c}")]
        public Decimal? dPrice { get; set; }

        public DateTime? dtDateAdd { get; set; }

        public DateTime? dtDateUpdate { get; set; }

        public Cause()
        {
            dtDateAdd = DateTime.Now;
        }

        //public dynamic sCauseDescription
        //{
        //    get
        //    {
        //        return string.Format("{0} {1}",sCode,sDescription);
        //    }
        //}
    }
}
