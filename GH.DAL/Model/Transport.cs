using System;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class Transport
    {
        [Key]
        public Guid kTransportId { get; set; }

        [Required(ErrorMessage = "* This field is required"),StringLength(100)]
        public String sTransportName { get; set; }
       
        [StringLength(200)]
        public String sAddress1 { get; set; }

        [StringLength(200)]
        public String sAddress2 { get; set; }

        [StringLength(50)]
        public String sCity { get; set; }

        [StringLength(10)]
        public String sZip { get; set; }

        [StringLength(50)]
        public String sPhone { get; set; }

        [StringLength(50)]
        public String sMobile { get; set; }

        [StringLength(50)]
        public String sFax { get; set; }

        [StringLength(50)]
        public String sEmailAddress { get; set; }

        public DateTime? dtDateAdd { get; set; }

        public DateTime dtDateUpdate { get; set; }


        public String vFullName
        {
            get
            {
                return String.Format("<b>{0}</b></br>{1} {2} {3}",sTransportName,sAddress1,sCity,sZip);
            }
        }

        public Transport()
        {
            dtDateAdd = DateTime.Now;
        }
    }
}
