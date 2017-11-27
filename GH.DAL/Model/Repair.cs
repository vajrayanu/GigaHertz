using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using GH.DAL.SQLDAL;
using GH.DAL.Helpers;
using System.Globalization;

namespace GH.DAL.Model
{
    //[Bind(Exclude = "kWorkingStatusId,kStaffId")]
    public class Repair
    {
        public Repair()
        {
            dtDateAdd = DateTime.Today;
        }

        [Key]
        public Guid kRepairId { get; set; }

        public Boolean? IsComplete { get; set; }
        public Boolean? IsNormal { get; set; }
        public Boolean? IsNoCredit { get; set; }
        public Boolean? IsQCPass { get; set; }
        public Boolean? IsCustomerRecieved { get; set; }
        public Boolean? IsBack { get; set; }
        public Boolean? IsInsurance { get; set; }
        public Boolean? IsCancle { get; set; }
        //public Boolean? IsClaim { get; set; }
        public int? iRemind { get; set; }

        public Boolean? IsFree { get; set; }
        public Boolean? IsHpOnSite { get; set; }


        [Display(Name = "เลขอ้างอิง"), StringLength(10)]
        public String sRepairNo { get; set; }

        [Display(Name = "เลขที่สินค้าตีกลับ"), StringLength(10)]
        public String sRepairBackNo { get; set; }


        [Display(Name = "สี"), StringLength(50)]
        public String sColor { get; set; }

        [Display(Name = "Serial Number"), StringLength(50)]
        public String sSerial { get; set; }

        [Display(Name = "วันหมดประกัน (ด/ว/ป)")]
        public DateTime? dtInsuranceExpire { get; set; }

        [Display(Name = "วันนัดรับเครื่อง (ด/ว/ป)")]
        public DateTime? dtDueDate { get; set; }

        [Display(Name = "วันรับซ่อมสินค้า (ด/ว/ป)")]
        public DateTime? dtDateAdd { get; set; }

         [Display(Name = "วันที่แก้ไข (ด/ว/ป)")]
        public DateTime? dtDateUpdate { get; set; }

        [Display(Name = "อุปกรณ์พ่วง"), StringLength(100)]
        public String sProductAccessories { get; set; }

        [Display(Name = "อื่นๆ"), StringLength(100)]
        public String sNote { get; set; }

        [Display(Name = "รับประกันการซ่อม (ด/ว/ป")]
        public int? iDayWarranty { get; set; }

        [Display(Name = "วันสิ้นสุดรับประกัน")]
        public DateTime? dtEndWarranty { get; set; }

        #region Join Table
        [ForeignKey("Customer")]
        public Guid kCustomerId { get;set; }
        public Customer Customer { get; set; }

        [ForeignKey("Product")]
        public Guid kProductId { get; set; }
        public Product Product { get; set; }


        public Guid kWorkingStatusId
        {
            get
            {
                if (RepairStatuies.Count > 0)
                    return RepairStatuies[0].kWorkingStatusId;
                else
                    return Guid.Empty;
            }
        }
        public dynamic vCloseStatusId
        {
            get
            {
                if (kCloseStatusId != null && kCloseStatusId.Value != Guid.Empty)
                    return CloseStatusManager.GetById(kCloseStatusId.Value).iOrder;
                else 
                    return "";
            }
        }
        public Guid? kCloseStatusId { get; set; } 
        public Guid? kBookingId { get; set; } 
        public Guid? kOwnerId { get; set; }
        public Guid? kStaffId{ get; set;  }
        public Guid? kQCId { get; set; }
        public Staff Staff { get; set; }

        public List<RepairStatus> RepairStatuies 
        {
            get
            {
                return RepairStatusManager.GetByRepairId(kRepairId);
            }
        }
        public List<RepairCause> RepairCauses
        {
            get
            {
                return RepairCauseManager.GetByRepairId(kRepairId);
            }
        }
        public List<RepairCauseEstimate> RepairCauseEstimates
        {
            get
            {
                return RepairCauseEstimateManager.GetByRepairId(kRepairId);
            }
        }
        public List<FileUpload> FileUploads
        {
            get
            {
                return FileUploadManager.GetByRepair(kRepairId);
            }
        }


        public Claim Claim
        {
            get
            {
                if (!string.IsNullOrEmpty(sRepairNo))
                    return ClaimManager.GetByRepairNo(sRepairNo);
                else
                    return new Claim();
            }
        }
        //public List<ClaimCause> ClaimCauses
        //{
        //    get
        //    {
        //        if (Claim != null)
        //            return ClaimManager.GetByRepair(Claim.kClaimId);
        //        else
        //            return null;
        //    }
        //}

        #endregion

        #region ViewModel
        public  dynamic vCustomerName
        {
            get
            {
                if (Customer != null)
                    return Customer.sCustomerName;
                else
                    return "- ";
            }
        }

        public dynamic vCustomerPhone
        {
            get
            {
                if (Customer != null)
                    return Customer.sPhone;
                else
                    return "- ";
            }
        }

        public dynamic vStaffName
        {
            get
            {
                if (RepairStatuies.Count > 0)
                    return RepairStatuies[0].Staff.sStaffName;
                else
                    return "- ";
            }
        }

        public dynamic vProductName
        {
            get
            {
                if (Product != null)
                    return Product.sProductName;
                else
                    return "- ";

            }
        }

        public dynamic vFullProductDescription
        {
            get
            {
                if (Product != null)
                    return Product.vFullProductDescription;
                else
                    return "- ";
            }
        }

        public dynamic vCustomerDescription
        {
            get
            {
                if (Customer != null)
                    return String.Format("<b>{0}</b></br>ที่อยู่: {1} {2} {3}</br>โทรศัพท์: {4}</br>มือถือ: {5}</br>โทรสาร: {6}</br>อีเมล: {6}"
                        , Customer.sCustomerName ?? " -"
                        , Customer.sAddress1 ?? " -"
                        , Customer.sCity ?? " -"
                        , Customer.sZip ?? " -"
                        , Customer.sPhone ?? " -"
                        , Customer.sMobile ?? " -"
                        , Customer.sFax ?? " -"
                        , Customer.sEmailAddress ?? " -"
                    );
                else
                    return "- ";
            }
        }

        public dynamic vProductDescription
        {
            get
            {
                if (Product != null)
                    return String.Format("<b>{0}</b></br>รุ่น: {1}</br>ยี่ห้อ: {2}</br>ประเภท: {3}</br>สี: {4}</br>SN: {5}</br>วันหมดประกัน: {6}"
                        , Product.sProductName ?? " -"
                        , Product.sProductModel ?? " -"
                        , Product.vBrandDescription ?? " -"
                        , Product.vProductTypeDescription ?? " -"
                        , sColor ?? " -"
                        , sSerial ?? " -"
                        , dtInsuranceExpire != null ? DateExtension.DateThaiFormatShort(dtInsuranceExpire.Value) : " -"
                    );
                else
                    return "- ";
            }
        }

        public dynamic vWorkingStatus
        {
            get
            {
                if (RepairStatuies.Count > 0)
                    return RepairStatuies[0].WorkingStatus.sDescription;
                else
                    return "- ";
            }
        }

        public dynamic vWorkingStatusId
        {
            get
            {
                if (RepairStatuies.Count > 0)
                    return RepairStatuies[0].WorkingStatus.iDefault;
                else
                    return "0";
            }
        }

        public dynamic vMessage
        {
            get
            {
                if (vWorkingStatus != null)
                    return string.Format("{0} {1}", vWorkingStatus, sRepairNo);
                else
                    return "- ";
            }
        }

        public dynamic vFileUploads
        {
            get
            {
                if (FileUploads != null){
                    string m_return="";
                    foreach(var i in FileUploads){
                        m_return += string.Format("<img onclick='$(this).dialog();' style='padding:5px' width='100px' height='80px' src='{0}'></img>", i.sFileUrl);
                    }
                    return m_return;
                }
                else
                    return "";
            }
        }

        public dynamic vCauseDescription { get; set; }

        public dynamic vCausePrice { get; set; }

        public dynamic vDateAdd
        {
            get
            {
                if (dtDateAdd != null)
                    return string.Format("{0}", DateExtension.DateThaiFormatShort(dtDateAdd.Value));
                else
                    return "";
            }
        }

        public dynamic vDateClose
        {
            get
            {
                if (IsComplete == true)
                    return string.Format("{0}", DateExtension.DateThaiFormatShort(dtDateUpdate.Value));
                else
                    return "";
            }
        }

        public dynamic vInsuranceExpire
        {
            get
            {
                if (dtInsuranceExpire != null)
                    return string.Format("{0}", DateExtension.DateThaiFormatShort( dtInsuranceExpire.Value));
                else
                    return "";
            }
        }

        public dynamic vDueDate
        {
            get
            {
                if (dtDueDate != null)
                    if(IsCustomerRecieved !=true && dtDueDate<DateTime.Now){
                        return string.Format("<b style='color:#ff0000;'>{0}</b>", DateExtension.DateThaiFormatShort(dtDueDate.Value));
                    }
                    else{
                        return string.Format("{0}", DateExtension.DateThaiFormatShort(dtDueDate.Value));
                    }
                else
                    return "";
            }
        }

        public dynamic vDateAddEN
        {
            get
            {
                if (dtDateAdd != null)
                    return string.Format("{0}", dtDateAdd.Value.ToString(DateExtension.DateFormat()));
                else
                    return "";
            }
        }

        public dynamic vDueDateEN
        {
            get
            {
                if (dtDueDate != null)
                    if (IsQCPass != true && dtDueDate < DateTime.Now)
                    {
                        return string.Format("<b style='color:#ff0000;'>{0}</b>", dtDueDate.Value.ToString(DateExtension.DateFormat()));
                    }
                    else
                    {
                        return string.Format("{0}", dtDueDate.Value.ToString(DateExtension.DateFormat()));
                    }
                else
                    return "";
            }
        }

        public dynamic vProductTracking
        {
            get
            {
                string message="";
                string messageEn = "";
                int i = Convert.ToInt16(vWorkingStatusId);
                switch (i)
                {
                    case 5: message = "อยู่ในระหว่างดำเนินการเคลม"; messageEn = "Claiming";
                        break;
                    case 6: message = "อยู่ในระหว่างดำเนินการเคลม"; messageEn = "Claiming";
                        break;
                    case 11: message = "ซ่อมเสร็จแล้ว"; messageEn = "Repaired Complete";
                        break;
                    case 14: message = "ลูกค้ารับสินค้าไปแล้ว"; messageEn = "Customer Already Recieve";
                        break;
                    default: message = "อยู่ในระหว่างดำเนินการซ่อม"; messageEn = "Repairing";
                        break;
                }
                //if (IsQCPass == true)
                //{
                //    message = "ซ่อมเสร็จ"; messageEn = "Repaired.";
                //}
               
                return String.Format("{0}</br>วันที่ส่งซ่อม: {1}</br>วันนัดรับสินค้า: {2}<br/>สถานะ: {3}<br/><br/>Open: {4}<br/>Due: {5}<br/>Status: {6}",
                    Product.sProductName, vDateAdd, vDueDate, message, vDateAddEN, vDueDateEN, messageEn);
            }
        }

        public dynamic vRepairNo
        {
            get
            {
                if (!string.IsNullOrEmpty(sRepairBackNo))
                {
                    return string.Format("{0} ตีกลับจาก {1}", sRepairNo , sRepairBackNo);
                }
                else
                {
                    return sRepairNo;
                }
            }
        }

        public dynamic vOwner
        {
            get
            {
                return StaffManager.GetById(kOwnerId.Value).sStaffName;
            }
        }
        
        #endregion

    }


    public class Repairs
    {
        [Key]
        public Guid kRepairId { get; set; }

        public Guid kCloseStatusId { get; set; } 
        public Guid kCustomerId { get; set; }
        public Guid kProductId { get; set; }
        public Guid kStaffId { get; set; }
        public Guid kOwnerId { get; set; }
        public Guid kQCId { get; set; }
        public Guid kBookingId { get; set; } 

        public Staff Staff { get; set; }

        public Boolean? IsComplete { get; set; }
        public Boolean? IsNormal { get; set; }
        public Boolean? IsNoCredit { get; set; }
        public Boolean? IsQCPass { get; set; }
        public Boolean? IsCustomerRecieved { get; set; }
        public Boolean? IsBack { get; set; }
        public Boolean? IsInsurance { get; set; }
        public Boolean? IsCancle { get; set; }
        public int? iRemind { get; set; }
        public Boolean? IsFree { get; set; }
        public Boolean? IsHpOnSite { get; set; }

        [Display(Name = "เลขอ้างอิง"), StringLength(10)]
        public String sRepairNo { get; set; }

        [Display(Name = "สี"), StringLength(50)]
        public String sColor { get; set; }

        [Display(Name = "Serial Number"), StringLength(50)]
        public String sSerial { get; set; }

        [Display(Name = "วันหมดประกัน (ด/ว/ป)")]
        public DateTime? dtInsuranceExpire { get; set; }

        [Display(Name = "วันนัดรับเครื่อง (ด/ว/ป)")]
        public DateTime? dtDueDate { get; set; }

        [Display(Name = "วันรับซ่อมสินค้า (ด/ว/ป)")]
        public DateTime? dtDateAdd { get; set; }

        public DateTime? dtDateUpdate { get; set; }

        [Display(Name = "อุปกรณ์พ่วง"), StringLength(100)]
        public String sProductAccessories { get; set; }

        [Display(Name = "อื่นๆ"), StringLength(100)]
        public String sNote { get; set; }

        [Display(Name = "รับประกันการซ่อม (ด/ว/ป")]
        public int? iDayWarranty { get; set; }

        [Display(Name = "วันสิ้นสุดรับประกัน")]
        public DateTime? dtEndWarranty { get; set; }
    }

}
