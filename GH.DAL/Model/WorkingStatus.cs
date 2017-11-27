using System;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class WorkingStatus
    {
        [Key]
        public Guid kWorkingStatusId { get; set; }

        public String sDescription { get; set; }

        public int iDefault { get; set; }

    }
}
