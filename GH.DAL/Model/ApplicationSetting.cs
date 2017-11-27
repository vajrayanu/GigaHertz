using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GH.DAL.Model
{
    public class ApplicationSetting
    {
        [Key]
        public Guid kApplicationSettingId { get; set; }

        [Required(ErrorMessage = "* This field is required"), Display(Name = "ชื่อบริษัท"), StringLength(100)]
        public String sApplicationName { get; set; }

        [Display(Name = "ที่อยู่"), StringLength(200)]
        public String sApplicationAddress { get; set; }

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

        [Display(Name = "หัวโปรแกรม"), StringLength(50)]
        public String sApplicationHeader { get; set; }

        [Display(Name = "หัวรีพอท"), StringLength(50)]
        public String sReportHeader { get; set; }

        [Display(Name = "โลโก"), StringLength(100)]
        public String sLogoUrl { get; set; }


        public dynamic vFullApplicationName
        {
            get
            {
                return String.Format("<b>{0}</b></br>{1} {2} {3}</br>โทรศัพท์ {4}</br>โทรสาร {5}</br>มือถือ {6}",
                    sApplicationName, sApplicationAddress, sCity, sZip, sPhone ?? " -", sFax ?? " -", sMobile ?? " -");
            }
        }

        public dynamic vApplicationAddress
        {
            get
            {
                return String.Format("{0} จ.{1} {2}", sApplicationAddress, sCity, sZip);
            }
        }
    }
}
