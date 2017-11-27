using System;
using System.ComponentModel.DataAnnotations;
using GH.DAL.SQLDAL;
using System.Collections.Generic;
using System.Web.Mvc;


namespace GH.DAL.Model
{
    //[Bind(Exclude = "UserName")]
    public class Staff
    {
     
        public dynamic UserName { get; set; }

        [Key]
        public Guid kStaffId { get; set; }

        [Required(ErrorMessage = "* This field is required"), Display(Name = "ชื่อพนักงาน"), StringLength(100)]
        public String sStaffName { get; set; }

        [Display(Name = "ที่อยู่"), StringLength(200)]
        public String sAddress1 { get; set; }

        [Display(Name = "ที่อยู่"), StringLength(200)]
        public String sAddress2 { get; set; }

        [Display(Name = "จังหวัด"), StringLength(50)]
        public String sCity { get; set; }

        [Display(Name = "รหัสไปรษณีย์"), StringLength(10), RegularExpression("[0-9]+", ErrorMessage = "Zip not valid")]
        public String sZip { get; set; }

        [Display(Name = "โทรศัพท์"), StringLength(50)]
        public String sPhone { get; set; }

        [Display(Name = "มือถือ"), StringLength(50)]
        public String sMobile { get; set; }

        [Display(Name = "อีเมล"), StringLength(50),
        RegularExpression("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Email Address not valid")]
        public String sEmailAddress { get; set; }

        [Display(Name = "วันเกิด")]
        public DateTime? dtDateBirth { get; set; }

        public DateTime? dtDateAdd { get; set; }

        public DateTime? dtDateUpdate { get; set; }

        public Guid kStaffPositionId { get; set; }
        public StaffPosition StaffPosition { get; set; }

        public dynamic vStaffPositionDescription
        {
            get
            {
                if (StaffPosition != null)
                    return StaffPosition.sDescription;
                else
                    return " -";
            }
        }

        public dynamic vFullName
        {
            get
            {
                return String.Format("<b>{0}</b></br>{1} {2} {3}", sStaffName, sAddress1, sCity, sZip);
            }
        }


        public Staff()
        {
            dtDateAdd = DateTime.Now;
        }

        public List<Repair> Repairs;
    }

    public class StaffRepair
    {
        //public Staff Staff { get; set; }
        public List<Staff> Staffs { get; set; }
        //public List<Repair> Repairs { get; set; }
    }

}
