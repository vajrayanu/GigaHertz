using System;
using System.ComponentModel.DataAnnotations;


namespace GH.DAL.Model
{
    public class ProductAccessorie
    {
        [Key]
        public Guid kProductAccessorieId { get; set; }

        [Display(Name = "อุปกรณ์ที่นำมาด้วย"), Required(ErrorMessage = "* This field is required"), StringLength(50)]
        public String sDescription { get; set; }

        public DateTime? dtDateAdd { get; set; }
        public DateTime? dtDateUpdate { get; set; }

        public ProductAccessorie()
        {
            dtDateAdd = DateTime.Now;
        }
    }   
}
