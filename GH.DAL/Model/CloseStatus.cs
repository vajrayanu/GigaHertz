using System;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class CloseStatus
    {
        [Key]
        public Guid kCloseStatusId { get; set; }

        public String sDescription { get; set; }

        public int iOrder { get; set; }

    }
}
