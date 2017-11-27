using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GH.DAL.SQLDAL;


namespace GH.DAL.Model
{
    public class CauseToClaim
    {
        public Guid kCauseId { get; set; }

        [Display(Name = "อาการของปัญหา"), Required(ErrorMessage = "* This field is required"), StringLength(100)]
        public String sCauseDescription { get; set; }
        public Int16 iQty { get; set; }
        public String sNote { get; set; }
        
    }
}
