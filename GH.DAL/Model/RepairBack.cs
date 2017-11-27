using System;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class RepairBack
    {
        [Key]
        public Guid kRepairBackId { get; set; }
      
        public string sRepairBeforeNo { get; set; }

        public string sRepairAfterNo { get; set; }
    }
}
