using System;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class NextItemNo
    {
        [Key]
        public Int32 kNextItemNo { get; set; }
        public DateTime? dtLastMod { get; set; }
    }

    public class ClaimNextItemNo
    {
        [Key]
        public Int32 kNextItemNo { get; set; }
        public DateTime? dtLastMod { get; set; }
    }
}
