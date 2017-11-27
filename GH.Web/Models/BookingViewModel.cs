using GH.DAL.Model;
using System.Collections.Generic;
using GH.DAL.SQLDAL;
using System;
using System.ComponentModel.DataAnnotations;
namespace GH.Web.Models
{

    public class BookingRepairViewModel : ApplicationHeaderViewModel
    {
        public Customer Customer { get; set; }
        public Product Product { get; set; }
        public List<Cause> Causes { get; set; }
        public Repair Repair { get; set; }
        public List<FileUpload> FileUploads { get; set; }
        public List<RemindHistory> RemindHistorys { get; set; }
    }

    public class BookingClaimViewModel : ApplicationHeaderViewModel
    {
        public Insurance Insurance { get; set; }
        public Product Product { get; set; }
        public List<ClaimCause> ClaimCauses { get; set; }
        public List<Cause> Causes { get; set; }
        public Claim Claim { get; set; }
    }

    public class ApplicationHeaderViewModel
    {
        public String MenuHeader { get; set; }
        public Staff Staff { get; set; }
        public UserProfileChangePasswordModel User { get; set; }
        
        public ApplicationSetting ApplicationSetting { get;internal set; }
        public ApplicationHeaderViewModel()
        {
            ApplicationSetting = ApplicationSettingManager.GetApplicationSetting();
        }
    }
    /*
    public class ReportServiceViewModel : ApplicationHeaderViewModel
    {
        [Display(Name = "ตั้งแต่ (ด/ว/ป)")]
        public dynamic DateStart { get; set; }
        [Display(Name = "ถึง (ด/ว/ป)")]
        public dynamic DateEnd { get; set; }

        public ReportService Report { get; set; }
        //public List<Repair> Repairs { get; set; }
        public List<Staff> Staffs { get; set; }
    }

    public class ReportTechnicianViewModel : ApplicationHeaderViewModel
    {
        [Display(Name = "ตั้งแต่ (ด/ว/ป)")]
        public dynamic DateStart { get; set; }
        [Display(Name = "ถึง (ด/ว/ป)")]
        public dynamic DateEnd { get; set; }

        public ReportTechnician Report { get; set; }
        //public List<Repair> Repairs { get; set; }
        public List<Staff> Staffs { get; set; }
    }

    public class ReportCashViewModel : ApplicationHeaderViewModel
    {
        [Display(Name = "ตั้งแต่ (ด/ว/ป)")]
        public dynamic DateStart { get; set; }
        [Display(Name = "ถึง (ด/ว/ป)")]
        public dynamic DateEnd { get; set; }

        public ReportCash Report { get; set; }
        //public List<Repair> Repairs { get; set; }
        public List<Staff> Staffs { get; set; }
    }
    */
    public class ReportDayViewModel : ApplicationHeaderViewModel
    {
        [Display(Name = "ตั้งแต่ (ด/ว/ป)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "MM/dd/yyyy")]
        public DateTime DateStart { get; set; }

        [Display(Name = "ถึง (ด/ว/ป)")]
        public DateTime DateEnd { get; set; }
      
        public ReportDay Report { get; set; }
        public List<Staff> Staffs { get; set; }
    }
    public class ReportMonthViewModel : ApplicationHeaderViewModel
    {
        [Display(Name = "ตั้งแต่ (ด/ว/ป)")]
        public DateTime DateStart { get; set; }
        [Display(Name = "ถึง (ด/ว/ป)")]
        public DateTime DateEnd { get; set; }

        public ReportMonth Report { get; set; }
        public List<Staff> Staffs { get; set; }
        public List<Insurance> Insurances { get; set; }
    }
    public class ReportServicesViewModel : ApplicationHeaderViewModel
    {
        [Display(Name = "ตั้งแต่ (ด/ว/ป)")]
        public DateTime DateStart { get; set; }
        [Display(Name = "ถึง (ด/ว/ป)")]
        public DateTime DateEnd { get; set; }

        public ReportServices Report { get; set; }
    }
    public class ReportServiceDayViewModel : ApplicationHeaderViewModel
    {
        [Display(Name = "ตั้งแต่ (ด/ว/ป)")]
        public DateTime DateStart { get; set; }

        [Display(Name = "ถึง (ด/ว/ป)")]
        public DateTime DateEnd { get; set; }

        public ReportServiceDay Report { get; set; }
        public List<Staff> Staffs { get; set; }
    }
}
