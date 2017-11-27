using System;
using System.ComponentModel.DataAnnotations;


namespace GH.DAL.Model
{
    public class ProductType
    {
        [Key]
        public Guid kProductTypeId { get; set; }

        [Required(ErrorMessage = "* This field is required"), StringLength(50)]
        public String sDescription { get; set; }

        public DateTime? dtDateAdd { get; set; }
        public DateTime? dtDateUpdate { get; set; }

        public ProductType()
        {
            dtDateAdd = DateTime.Now;
        }
    }   
}
