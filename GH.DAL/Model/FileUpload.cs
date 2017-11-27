using System;
using System.ComponentModel.DataAnnotations;


namespace GH.DAL.Model
{
    public class FileUpload
    {
        [Key]
        public Guid kFileUploadId { get; set; }

        public Guid? kRepairId { get; set; }

        public Guid? kSessionId { get; set; }

        [Display(Name = "รูป"), StringLength(200)]
        public String sFileUrl { get; set; }

        public dynamic vFileUrl
        {
            get
            {
                return string.Format("<img width='120px' height='100px' src='{0}'></img>", sFileUrl);
            }
        }
    }
}
