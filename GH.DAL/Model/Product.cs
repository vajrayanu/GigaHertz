using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GH.DAL.SQLDAL;


namespace GH.DAL.Model
{
    public class Product
    {
        [Key]
        public Guid kProductId { get; set; }

        [ForeignKey("ProductType")]
        [Display(Name = "ประเภทสินค้า")]
        public Guid kProductTypeId { get; set; }

        [ForeignKey("Brand")]
        [Display(Name = "ยี่ห้อ")]
        public Guid kBrandId { get; set; }

        [Display(Name = "คำอธิบายสินค้า"), Required(ErrorMessage = "* This field is required"), StringLength(100)]
        public String sProductName { get; set; }

        [Display(Name = "รุ่น"), StringLength(100)]
        public String sProductModel { get; set; }

        public DateTime? dtDateAdd { get; set; }

        public DateTime? dtDateUpdate { get; set; }

        public ProductType ProductType { get; set; }

        public Brand Brand { get; set; }

        
        public Product()
        {
            dtDateAdd = DateTime.Now;
        }


        #region View Models

        public dynamic sBrandName { get; set; }
        public dynamic vProductTypeDescription
        {
            get
            {
                if (ProductType != null)
                    return ProductType.sDescription;
                else
                    return " -";
            }
        }

        public dynamic vBrandDescription
        {
            get
            {
                if (Brand != null)
                    return Brand.sBrandName;
                else
                    return " -";
                         
            }
        }

        public dynamic vFullProductDescription
        {
            get
            {
                return String.Format("<b>{0}</b></br>รุ่น {1}</br>ยี่ห้อ {2}</br>ประเภท {3}",
                    sProductName, sProductModel ?? " -", vBrandDescription, vProductTypeDescription);
            }
        }
        #endregion
    }    
}
