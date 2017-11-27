using System;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class Insurance
    {
        [Key]
        public Guid kInsuranceId { get; set; }

        [Required(ErrorMessage = "* This field is required"), Display(Name = "ชื่อศูนย์ประกัน"), StringLength(100)]
        public String sInsuranceName { get; set; }

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

        [Display(Name = "โทรสาร"), StringLength(50)]
        public String sFax { get; set; }

        [Display(Name = "อีเมล"), StringLength(50),
        RegularExpression("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Email Address not valid")]
        public String sEmailAddress { get; set; }

        public DateTime? dtDateAdd { get; set; }

        public DateTime? dtDateUpdate { get; set; }

        public bool? IsAgent { get; set; }

        public Insurance()
        {
            dtDateAdd = DateTime.Now;
        }


        public String vFullName
        {
            get
            {
                return String.Format("<b>{0}</b></br>{1} {2} {3}", sInsuranceName, sAddress1, sCity, sZip);
            }
        }

        public dynamic vFullContactName
        {
            get
            {
                return String.Format("<b>{0}</b></br>{1} {2} {3}</br>โทรศัพท์ {4}</br>โทรสาร {5}</br>มือถือ {6}",
                    sInsuranceName, sAddress1, sCity, sZip, sPhone ?? " -", sFax ?? " -", sMobile ?? " -");
            }
        }
    }
}
