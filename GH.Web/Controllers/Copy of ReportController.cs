using System.Web.Mvc;
using GH.Web.Models;
using System;
using GH.DAL.Helpers;
using GH.DAL.Model;
using GH.DAL.SQLDAL;
using System.Linq;

namespace GH.Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ReportList = "first active";
            ApplicationHeaderViewModel model = new ApplicationHeaderViewModel();
           
            return View(model);
        }
       
        //สรุปรายการซ่อมประจำวัน
        public ActionResult ReportDay()
        {
            ViewBag.ReportList = "first active";

            ReportDayViewModel model = new ReportDayViewModel();
            model.Report = new ReportDay();

            DateTime start = DateTime.Today;
            model.DateStart = start.AddYears(543);

            DateTime end = DateTime.Today.AddDays(1);
            model.DateEnd = Convert.ToDateTime(end.AddYears(543).ToString("MM/dd/yyyy"));


            model.Report.Repairs = ReportManager.ReportRepairDay(start);
            model.Report.Claims = ReportManager.ReportClaimDay(start);
            
            var workingstatus = WorkingStatusManager.GetAll().Where(m => m.iDefault>=5 && m.iDefault<=7);
            model.Report.WorkingStatuies = workingstatus.OrderBy(m => m.iDefault).ToList();
            model.Report.Insurances = InsuranceManager.GetAll().OrderBy(m => m.sInsuranceName).ToList();
            model.Report.TCStaffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "ช่าง").OrderBy(m => m.sStaffName).ToList();
            model.Report.STCStaffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "หัวหน้าช่าง").OrderBy(m => m.sStaffName).ToList();
            model.Report.QCStaffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "ฝ่ายตรวจสอบคุณภาพ").OrderBy(m => m.sStaffName).ToList();

            ViewBag.ReportHeader = String.Format("รายงานบริการประจำวันที่ {0}", DateExtension.DateThaiFormat(start));
            return View(model);
        }

        //สรุปรายการซ่อมประจำวัน
        [HttpPost]
        public ActionResult ReportDay(ReportDayViewModel model, FormCollection collection)
        {
            ViewBag.ReportList = "first active";

            DateTime start = model.DateStart;
            model.DateStart = start.AddYears(-543);

            DateTime end = model.DateEnd;
            model.DateEnd = end.AddYears(-543);

            model.Report = new ReportDay();
            model.Report.Repairs = ReportManager.ReportRepairDay(model.DateStart);
            model.Report.Claims = ReportManager.ReportClaimDay(model.DateStart);

            var workingstatus = WorkingStatusManager.GetAll().Where(m => m.iDefault >= 5 && m.iDefault <= 7);
            model.Report.WorkingStatuies = workingstatus.OrderBy(m => m.iDefault).ToList();
            model.Report.Insurances = InsuranceManager.GetAll().OrderBy(m => m.sInsuranceName).ToList();
            model.Report.TCStaffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "ช่าง").OrderBy(m => m.sStaffName).ToList();
            model.Report.STCStaffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "หัวหน้าช่าง").OrderBy(m => m.sStaffName).ToList();
            model.Report.QCStaffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "ฝ่ายตรวจสอบคุณภาพ").OrderBy(m => m.sStaffName).ToList();

            ViewBag.ReportHeader = String.Format("รายงานบริการประจำวันที่ {0}", DateExtension.DateThaiFormat(model.DateStart));
            return View(model);
        }

        //สรุปรายการซ่อมประจำเดือน
        public ActionResult ReportMonth()
        {
            ViewBag.ReportList = "first active";

            ReportMonthViewModel model = new ReportMonthViewModel();
            model.Report = new ReportMonth();

            DateTime start = DateExtension.FirstDayOfMonthFromDateTime(DateTime.Now);
            model.DateStart = Convert.ToDateTime(start.AddYears(543).ToString("MM/dd/yyyy"));

            DateTime end = DateExtension.LastDayOfMonthFromDateTime(DateTime.Now);
            model.DateEnd = Convert.ToDateTime(end.AddYears(543).ToString("MM/dd/yyyy"));

            model.Report.Repairs = ReportManager.ReportRepairMonth(start, end);
            //model.Report.Claims = ReportManager.ReportClaim(start, end);

            model.Insurances = InsuranceManager.GetAll().OrderBy(m => m.sInsuranceName).ToList();
            ViewBag.ReportHeader = String.Format("สรุปบริการประจำเดือน {0}", DateExtension.DateThaiFormat2(start));
            return View(model);
        }

        //สรุปรายการซ่อมประจำเดือน
        [HttpPost]
        public ActionResult ReportMonth(ReportMonthViewModel model, FormCollection collection)
        {
            ViewBag.ReportList = "first active";

            DateTime start = model.DateStart;
            model.DateStart = start.AddYears(-543);

            DateTime end = model.DateEnd;
            model.DateEnd = end.AddYears(-543);

            model.Report = new ReportMonth();
            model.Report.Repairs = ReportManager.ReportRepairMonth(model.DateStart, model.DateEnd);
            
            model.Insurances = InsuranceManager.GetAll().OrderBy(m => m.sInsuranceName).ToList();
            ViewBag.ReportHeader = String.Format("สรุปบริการประจำเดือน {0}", DateExtension.DateThaiFormat2(model.DateStart));
            return View(model);
        }



        //สรุปผลงานช่างประจำเดือน
        public ActionResult ReportServices()
        {
            ViewBag.ReportList = "first active";

            ReportServicesViewModel model = new ReportServicesViewModel();
            model.Report = new ReportServices();

            DateTime start = DateExtension.FirstDayOfMonthFromDateTime(DateTime.Now);
            model.DateStart = Convert.ToDateTime(start.AddYears(543).ToString("MM/dd/yyyy"));

            DateTime end = DateExtension.LastDayOfMonthFromDateTime(DateTime.Now);
            model.DateEnd = Convert.ToDateTime(end.AddYears(543).ToString("MM/dd/yyyy"));

            model.Report.Repairs = ReportManager.ReportRepair(start, end);
            model.Report.Claims = ReportManager.ReportClaim(start, end);


            model.Report.Staffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "ช่าง").OrderBy(m => m.sStaffName).ToList();
            model.Report.SuperStaffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "หัวหน้าช่าง").OrderBy(m => m.sStaffName).ToList();
            model.Report.QCStaffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "ฝ่ายตรวจสอบคุณภาพ").OrderBy(m => m.sStaffName).ToList();
           
            ViewBag.ReportHeader = String.Format("รายรับ-รายจ่ายค่าบริการทั้งหมดประจำเดือน {0}", DateExtension.DateThaiFormat2(start));
            return View(model);
        }

        //สรุปผลงานช่างประจำเดือน
        [HttpPost]
        public ActionResult ReportServices(ReportServicesViewModel model, FormCollection collection)
        {
            ViewBag.ReportList = "first active";

            DateTime start = model.DateStart;
            model.DateStart = start.AddYears(-543);

            DateTime end = model.DateEnd;
            model.DateEnd = end.AddYears(-543);

            model.Report = new ReportServices();
            model.Report.Repairs = ReportManager.ReportRepair(model.DateStart, model.DateEnd);
            model.Report.Claims = ReportManager.ReportClaim(model.DateStart, model.DateEnd);

            model.Report.Staffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "ช่าง").OrderBy(m => m.sStaffName).ToList();
            model.Report.SuperStaffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "หัวหน้าช่าง").OrderBy(m => m.sStaffName).ToList();
            model.Report.QCStaffs = StaffManager.GetAll().Where(m => m.StaffPosition.sDescription == "ฝ่ายตรวจสอบคุณภาพ").OrderBy(m => m.sStaffName).ToList();

            ViewBag.ReportHeader = String.Format("รายรับ-รายจ่ายค่าบริการทั้งหมดประจำเดือน {0}", DateExtension.DateThaiFormat2(model.DateStart));
            return View(model);
        }


        //รายงานคสามเคลื่อนไหวประจำวัน
        public ActionResult ReportServiceDay()
        {
            ViewBag.ReportList = "first active";

            ReportServiceDayViewModel model = new ReportServiceDayViewModel();
            model.Report = new ReportServiceDay();

            DateTime start = DateTime.Today;
            model.DateStart = Convert.ToDateTime(start.AddYears(543).ToString("MM/dd/yyyy"));

            model.Report.Repairs = ReportManager.ReportServiceDay(start);
            ViewBag.ReportHeader = String.Format("รายงานสรุปความเคลื่อนไหวของฟร้อนต์ประจำวันที่ {0}", DateExtension.DateThaiFormat(start));
            return View(model);
        }

        //รายงานคสามเคลื่อนไหวประจำวัน
        [HttpPost]
        public ActionResult ReportServiceDay(ReportServiceDayViewModel model, FormCollection collection)
        {
            ViewBag.ReportList = "first active";

            DateTime start = model.DateStart;
            model.DateStart = start.AddYears(-543);

            model.Report = new ReportServiceDay();
            model.Report.Repairs = ReportManager.ReportServiceDay(model.DateStart);
          

            //DateTime DateStart;
            //if (DateTime.TryParse(Request.Form["DateStart"], out DateStart))
            //{
            //    model.DateStart = DateStart.AddYears(-543);
            //    model.Report = new ReportServiceDay();
            //    model.Report.Repairs = ReportManager.ReportServiceDay(DateStart);
            //    ViewBag.ReportHeader = String.Format("รายงานสรุปความเคลื่อนไหวของฟร้อนต์ประจำวันที่ {0}", DateExtension.DateThaiFormat(model.DateStart));
            //}
            ViewBag.ReportHeader = String.Format("รายงานสรุปความเคลื่อนไหวของฟร้อนต์ประจำวันที่ {0}", DateExtension.DateThaiFormat(model.DateStart));
            return View(model);
        }
    }
}
