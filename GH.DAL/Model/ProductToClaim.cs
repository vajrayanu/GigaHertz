using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GH.DAL.SQLDAL;


namespace GH.DAL.Model
{
    public class ProductToClaim
    {
        public Guid kProductId { get; set; }

        [Display(Name = "คำอธิบายสินค้า"), Required(ErrorMessage = "* This field is required"), StringLength(100)]
        public String sProductName { get; set; }

        public DateTime? dtDateAdd { get; set; }

        public ProductToClaim()
        {
            dtDateAdd = DateTime.Now;
        }
    }    
}
