using System;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class ProcessStatus
    {
        [Key]
        public Guid kProcessStatusId { get; set; }

        public String sDescription { get; set; }

    }
}
