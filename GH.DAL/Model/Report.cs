using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using GH.DAL.Helpers;

namespace GH.DAL.Model
{
    public class ReportDay
    {
        public int row { get; set; }
        public List<Repair> Repairs { get; set; }
        public List<Claim> Claims { get; set; }

        public List<Staff> TCStaffs { get; set; }
        public List<Staff> STCStaffs { get; set; }
        public List<Staff> QCStaffs { get; set; }
        public List<Insurance> Insurances { get; set; }
        public List<WorkingStatus> WorkingStatuies { get; set; }
    }

    public class ReportMonth
    {
        public List<Repair> Repairs { get; set; }
        public List<Claim> Claims { get; set; }

        [Display(Name = "จำนวนเครื่องมาซ่อม")]
        public Int16 TotalMachine
        {
            get
            {
                if (Repairs != null)
                    return (Int16)Repairs.Count();
                else
                    return 0;
            }
        }

        [Display(Name = "จำนวนเครื่องที่มารับแล้ว")]
        public Int16 TotalMachineComplete
        {
            get
            {
                if (Repairs != null)
                    return (Int16)Repairs.Where(m => m.IsComplete == true && m.IsCustomerRecieved == true).Count();
                else
                    return 0;
            }
        }

        [Display(Name = "จำนวนเครื่องปกติ")]
        public Int16 TotalMachineNormal
        {
            get
            {
                if (Repairs != null)
                    return (Int16)Repairs.Where(m => m.IsComplete == true && m.IsCustomerRecieved == true && m.kCloseStatusId.ToString().ToUpper().Equals("1C54073A-BFD8-41B4-B9AE-91FD9042514C")).Count();
                else
                    return 0;
            }
        }

        [Display(Name = "จำนวนเครื่องตีกลับ")]
        public Int16 TortalMachineBack
        {
            get
            {
                if (Repairs != null)
                    return (Int16)Repairs.Where(m => m.IsComplete == true && m.IsCustomerRecieved == true && m.kCloseStatusId.ToString().ToUpper().Equals("47AC8A37-C1B0-419A-975E-76F1C17B8C70")).Count();
                else
                    return 0;
            }
        }

        [Display(Name = "จำนวนเครื่องยกเลิก")]
        public Int16 TotalMachineCancle
        {
            get
            {
                if (Repairs != null)
                    return (Int16)Repairs.Where(m => m.IsComplete == true && m.IsCustomerRecieved == true && m.kCloseStatusId.ToString().ToUpper().Equals("49CBC40D-310A-4E09-AC4C-8934C5A0F900")).Count();
                else
                    return 0;
            }
        }

        [Display(Name = "มีประกัน")]
        public Int16 TotalMachineHasInsurance
        {
            get
            {
                if (Repairs != null)
                    return (Int16)Repairs.Where(m => m.IsComplete == true && m.IsCustomerRecieved == true && m.kCloseStatusId.ToString().ToUpper().Equals("38DBF61A-038E-4AAB-8921-FD24FF842698")).Count();
                else
                    return 0;
            }
        }

        [Display(Name = "จำนวนเครื่องบริการฟรี")]
        public Int16 TotalMachineServiceFree
        {
            get
            {
                Int16 totalMachine = 0;
                if (Repairs != null)
                {
                    foreach (var r in Repairs)
                    {
                        if (r.IsComplete == true && r.IsCustomerRecieved == true && r.kCloseStatusId.ToString().ToUpper().Equals("C78CD4C7-F334-4874-999F-A86090588E16"))
                        {
                            totalMachine++;
                        }
                    }
                }
                return totalMachine;
            }
        }

        [Display(Name = "มีค่าบริการ")]
        public Int16 TotalMachineServiceFees
        {
            get
            {
                Int16 totalMachine = 0;
                if (Repairs != null)
                {
                    foreach (var r in Repairs)
                    {
                        if (r.IsComplete == true && r.IsCustomerRecieved == true && r.kCloseStatusId.ToString().ToUpper().Equals("AAC726FA-1637-40F7-A312-A8BE41893F83"))
                        {
                            totalMachine++;
                        }
                    }
                }
                return totalMachine;  
            }
        }

        [Display(Name = "HP on oite")]
        public Int16 TotalMachineHpOnSite
        {
            get
            {
                Int16 totalMachine = 0;
                if (Repairs != null)
                {
                    foreach (var r in Repairs)
                    {
                        if (r.IsComplete == true && r.IsCustomerRecieved == true && r.kCloseStatusId.ToString().ToUpper().Equals("B4DF2696-6E8E-49E7-8323-974982BE726B"))
                        {
                            totalMachine++;
                        }
                    }
                }
                return totalMachine;
            }
        }
    }

    public class ReportServices
    {
        public List<Repair> Repairs { get; set; }
        public List<Claim> Claims { get; set; }

        public List<Staff> Staffs { get; set; }
        public List<Staff> QCStaffs { get; set; }
        public List<Staff> SuperStaffs { get; set; }

        [Display(Name = "รายรับ-รายจ่ายสุทธิ")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal TotalSummary { get; set; }

        #region รายรับ
        [Display(Name = "ไม่มีต้นทุน")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Received
        {
            get
            {
                if (Repairs != null)
                    return (decimal)Repairs.Sum(m => m.RepairCauses.Sum(n => n.dPrice));
                else
                    return 0;
            }
        }

        [Display(Name = "มีต้นทุน")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Received_HasCost { get; set; }
        
        [Display(Name = "สต๊อกนอก")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal StockOut { get; set; }

        //for input
        [Display(Name = "ไม่มีต้นทุน(เชื่อ)")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Received_IsCredit
        {
            get
            {
                if (Repairs != null)
                    return (decimal)Repairs.Where(m => m.IsNoCredit != true).Sum(m => m.RepairCauses.Sum(n => n.dPrice));
                else
                    return 0;
            }
        }

        [Display(Name = "Express")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Received_Express
        {
            get
            {
                if (Repairs != null)
                    return (decimal)Repairs.Where(m => m.sRepairNo.Contains("E")).Sum(m => m.RepairCauses.Sum(n => n.dPrice));
                else
                    return 0;
            }
        }

        //for input
        [Display(Name = "สดศูนย์")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Received_Ins{ get; set; }
        
        //for input
        [Display(Name = "เชื่อศูนย์")]
        public decimal Received_Ins_IsCredit { get; set; }


        [Display(Name = "รวม")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Received_Summary
        {
            get
            {
                decimal m_result = (Received - Received_HasCost) + StockOut + Received_IsCredit + Received_Express + Received_Ins + Received_Ins_IsCredit;
                return m_result;
            }
        }
        #endregion

        #region รายจ่าย
        [Display(Name = "ค่าส่งซ่อม(เงินสด)")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Pay_Claim_IsNoCredit
        {
            get
            {
                if (Claims != null)
                    return (decimal)Claims.Where(m => m.IsNoCredit == true).Sum(m => m.ClaimCauses.Sum(n => n.dPrice));
                else
                    return 0;
            }
        }
        [Display(Name = "ค่าส่งซ่อม(เงินเชื่อ)")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Pay_Claim_IsCredit
        {
            get
            {
                if (Claims != null)
                    return (decimal)Claims.Where(m=>m.IsNoCredit != true).Sum(m => m.ClaimCauses.Sum(n => n.dPrice));
                else
                    return 0;
            }
        }

        //for input 
        [Display(Name = "เบิกภายใน")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Widen { get; set; }

        //for input
        [Display(Name = "เงินเดือนแผนกบริการ")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Salary_Service { get; set; }

        //for input
        [Display(Name = "รายจ่ายแผนกบริการ(จ่ายเชื่อ)")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Salary_Service_IsCredit { get; set; }


       
        #endregion
    }

    public class ReportServiceDay
    {
        public int row { get; set; }
        public List<Repair> Repairs { get; set; }
    }
}
