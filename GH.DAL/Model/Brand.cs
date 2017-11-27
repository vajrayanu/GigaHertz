using System;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class Brand
    {
        [Key]
        public Guid kBrandId { get; set; }

        [Required(ErrorMessage = "* This field is required"), StringLength(50)]
        public String sBrandName { get; set; }

        public DateTime? dtDateAdd { get; set; }
        public DateTime? dtDateUpdate { get; set; }

        public Brand()
        {
            dtDateAdd = DateTime.Now;
        }
    }
}
