using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections.Generic;
using GH.DAL.Helpers;

namespace GH.DAL.Model
{

    public class RepairStatus
    {
        [Key]
        public Guid kRepairStatusId { get; set; }
        public Guid kRepairId { get; set; }
        public DateTime? dtDateAdd { get; set; }
      
        public Guid kStaffId { get; set; }
        public Staff Staff { get; set; }
      
        public Guid kWorkingStatusId { get; set; }
        public WorkingStatus WorkingStatus { get; set; }
        
        public RepairStatus()
        {
            dtDateAdd = DateTime.Now;
        }

        public dynamic vWorkingTime
        {
            get
            {
                return dtDateAdd.Value.ToShortTimeString();
            }
        }
        public dynamic vWorkingDate
        {
            get
            {
                if (dtDateAdd != null)
                    return string.Format("{0}", DateExtension.DateThaiFormatShort(dtDateAdd.Value));
                else
                    return "- ";
            }
        }
        public dynamic vStaffName
        {
            get
            {
                if (Staff != null)
                {
                    return Staff.sStaffName;
                }
                else
                {
                    return "- ";
                }
            }
        }
        public dynamic vWorkingStatus
        {
            get
            {
                if (WorkingStatus != null)
                {
                    return WorkingStatus.sDescription;
                }
                else
                {
                    return "- ";
                }
            }
        }
    }
}
