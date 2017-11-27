using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using GH.DAL.SQLDAL;
using System.Web.Mvc;
using GH.DAL.Helpers;

namespace GH.DAL.Model
{
    [Bind(Exclude = "dTotalPrice")]
    public class Claim
    {
        public Claim()
        {
            dtDateAdd = DateTime.Today;
        }

        [Key]
        public Guid kClaimId { get; set; }

        public Boolean? IsComplete { get; set; }
        public Boolean? IsRecieved { get; set; }
        public Boolean? IsNoCredit { get; set; }

        [Display(Name = "เลขอ้างอิง"), StringLength(10)]
        public String sClaimNo { get; set; }

        [Display(Name = "เลขที่ซ่อม"), StringLength(10)]
        public String sRepairNo { get; set; }

        [Display(Name = "Serial Number"), StringLength(50)]
        public String sSerial { get; set; }

        [Display(Name = "วันหมดประกัน (ด/ว/ป)")]
        public DateTime? dtInsuranceExpire { get; set; }

        [Display(Name = "วันรับซ่อมสินค้า (ด/ว/ป)")]
        public DateTime? dtDateAdd { get; set; }

        public DateTime? dtDateUpdate { get; set; }

        #region Join Table
        [ForeignKey("Insurance")]
        public Guid kInsuranceId { get; set; }
        public Insurance Insurance { get; set; }

        [ForeignKey("Product")]
        public Guid kProductId { get; set; }
        public Product Product { get; set; }


        public Guid? kStaffId { get; set; }
        public Guid? kOwnerId { get; set; }

        public Guid kWorkingStatusId
        {
            get
            {
                if (ClaimStatuies.Count > 0)
                    return ClaimStatuies[0].kWorkingStatusId;
                else
                    return Guid.Empty;
            }
        }

        public List<ClaimStatus> ClaimStatuies
        {
            get
            {
                return ClaimStatusManager.GetByClaimId(kClaimId);
            }
        }
        public List<ClaimCause> ClaimCauses
        {
            get
            {
                return ClaimCourseManager.GetByClaimId(kClaimId);
            }
        }


        #endregion

        #region ViewModel
        
        public dynamic vCustomerDescription
        {
            get
            {
                if (!String.IsNullOrEmpty(sRepairNo))
                { 
                    var repair = RepairManager.GetByRepairNo(sRepairNo);
                    if (repair != null)
                    {
                        return String.Format("<b>{0}</b></br>ที่อยู่: {1} {2} {3}</br>โทรศัพท์: {4}</br>มือถือ: {5}</br>โทรสาร: {6}</br>อีเมล: {6}"
                            , repair.Customer.sCustomerName ?? " -"
                            , repair.Customer.sAddress1 ?? " -"
                            , repair.Customer.sCity ?? " -"
                            , repair.Customer.sZip ?? " -"
                            , repair.Customer.sPhone ?? " -"
                            , repair.Customer.sMobile ?? " -"
                            , repair.Customer.sFax ?? " -"
                            , repair.Customer.sEmailAddress ?? " -"
                        );
                    }
                    else
                    {
                        return String.Format("-  [{0}]", sRepairNo);
                    }
                }
                else
                    return "- ";
            }
        }

        public dynamic vInsuranceName
        {
            get
            {
                if (Insurance != null)
                    return Insurance.sInsuranceName;
                else
                    return "- ";
            }
        }

        public dynamic vStaffName
        {
            get
            {
                if (ClaimStatuies.Count > 0)
                    return ClaimStatuies[0].Staff.sStaffName;
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

        public dynamic vInsuranceDescription
        {
            get
            {
                if (Insurance != null)
                    return String.Format("<b>{0}</b></br>ที่อยู่: {1} {2} {3}</br>โทรศัพท์: {4}</br>มือถือ: {5}</br>โทรสาร: {6}</br>อีเมล: {6}"
                        , Insurance.sInsuranceName ?? " -"
                        , Insurance.sAddress1 ?? " -"
                        , Insurance.sCity ?? " -"
                        , Insurance.sZip ?? " -"
                        , Insurance.sPhone ?? " -"
                        , Insurance.sMobile ?? " -"
                        , Insurance.sFax ?? " -"
                        , Insurance.sEmailAddress ?? " -"
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
                    return String.Format("<b>{0}</b></br>รุ่น: {1}</br>ยี่ห้อ: {2}</br>ประเภท: {3}</br>SN: {4}</br>วันหมดประกัน: {5}"
                        , Product.sProductName ?? " -"
                        , Product.sProductModel ?? " -"
                        , Product.vBrandDescription ?? " -"
                        , Product.vProductTypeDescription ?? " -"
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
                if (ClaimStatuies.Count > 0)
                    return ClaimStatuies[0].WorkingStatus.sDescription;
                else
                    return "- ";
            }
        }

        public dynamic vWorkingStatusId
        {
            get
            {
                if (ClaimStatuies.Count > 0)
                    return ClaimStatuies[0].WorkingStatus.iDefault;
                else
                    return "- ";
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

        public dynamic vInsuranceExpire
        {
            get
            {
                if (dtInsuranceExpire != null)
                    return string.Format("{0}", DateExtension.DateThaiFormatShort(dtInsuranceExpire.Value));
                else
                    return "";
            }
        }

        private Decimal m_return=Decimal.Zero;
        
        public Decimal dTotalPrice
        {
            get
            {
                if(ClaimCauses!=null)
                {
                    foreach (var m in ClaimCauses)
                    {
                        if (m.dPrice > 0)
                            m_return += (Decimal)m.dPrice;
                    }
                    return m_return;
                }else
                {
                    return 0;
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

    public class Claims
    {
        [Key]
        public Guid kClaimId { get; set; }

        public Boolean? IsComplete { get; set; }
        public Boolean? IsRecieved { get; set; }
        public Boolean? IsNoCredit { get; set; }

        [Display(Name = "เลขอ้างอิง"), StringLength(10)]
        public String sClaimNo { get; set; }

        [Display(Name = "เลขที่ซ่อม"), StringLength(10)]
        public String sRepairNo { get; set; }

        [Display(Name = "Serial Number"), StringLength(50)]
        public String sSerial { get; set; }

        [Display(Name = "วันหมดประกัน (ด/ว/ป)")]
        public DateTime? dtInsuranceExpire { get; set; }

        [Display(Name = "วันรับซ่อมสินค้า (ด/ว/ป)")]
        public DateTime? dtDateAdd { get; set; }

        public DateTime? dtDateUpdate { get; set; }



        public Guid kStaffId { get; set; }
        public Guid kInsuranceId { get; set; }
        public Guid kProductId { get; set; }
        public Guid kOwnerId { get; set; }

        public Insurance Insurance { get; set; }

    }
}
