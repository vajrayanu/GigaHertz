using System;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class Color
    {
        [Key]
        public Guid kColorId { get; set; }

        [Required(ErrorMessage = "* This field is required"), StringLength(20)]
        public String sDescription { get; set; }
    }
}
