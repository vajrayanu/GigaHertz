using System;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using GH.DAL.Model;
using System.Collections.Generic;
using System.Reflection;
using GH.DAL.Helpers;
using GH.Web.Models;

namespace GH.Web.Reports
{
    public partial class RepairSummary : System.Web.UI.Page
    {

        BookingRepairViewModel model = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    model = (BookingRepairViewModel)System.Web.HttpContext.Current.Session["BookingViewModel"];

                    if (model != null)
                    {
                        runRptViewer();

                        #region Application Setting
                        //ReportParameter ApplicationLogo;
                        ReportParameter ApplicationSettingName;
                        ReportParameter ApplicationSettingAddress;
                        ReportParameter ApplicationTel;
                        ReportParameter ApplicationFax;
                        if (model.ApplicationSetting != null)
                        {
                            //if (!String.IsNullOrEmpty(model.ApplicationSetting.sApplicationName))
                            //{
                            //    ApplicationLogo = new ReportParameter("ApplicationLogo", model.ApplicationSetting.sLogoUrl);
                            //}
                            //else
                            //{
                            //    ApplicationLogo = new ReportParameter("ApplicationLogo", "");
                            //}

                            if (!String.IsNullOrEmpty(model.ApplicationSetting.sApplicationName))
                            {
                                ApplicationSettingName = new ReportParameter("ApplicationSettingName", model.ApplicationSetting.sApplicationName);
                            }
                            else
                            {
                                ApplicationSettingName = new ReportParameter("ApplicationSettingName", "-");
                            }

                            if (!String.IsNullOrEmpty(model.ApplicationSetting.vApplicationAddress))
                            {
                                ApplicationSettingAddress = new ReportParameter("ApplicationSettingAddress", model.ApplicationSetting.vApplicationAddress);
                            }
                            else
                            {
                                ApplicationSettingAddress = new ReportParameter("ApplicationSettingAddress", "-");
                            }


                            if (!String.IsNullOrEmpty(model.ApplicationSetting.sPhone))
                            {
                                ApplicationTel = new ReportParameter("ApplicationTel", model.ApplicationSetting.sPhone);
                            }
                            else
                            {
                                ApplicationTel = new ReportParameter("ApplicationTel", "-");
                            }

                            if (!String.IsNullOrEmpty(model.ApplicationSetting.sFax))
                            {
                                ApplicationFax = new ReportParameter("ApplicationFax", model.ApplicationSetting.sFax);
                            }
                            else
                            {
                                ApplicationFax = new ReportParameter("ApplicationFax", "-");
                            }
                        }
                        else
                        {
                            ApplicationSettingName = new ReportParameter("ApplicationSettingName", "-");
                            ApplicationSettingAddress = new ReportParameter("ApplicationSettingAddress", "-");
                            ApplicationTel = new ReportParameter("ApplicationTel", "-");
                            ApplicationFax = new ReportParameter("ApplicationFax", "-");
                        }
                        #endregion

                        #region Customer

                        ReportParameter CustomerName;
                        ReportParameter CustomerMobile;
                        ReportParameter CustomerPhone;
                        if (model.Customer != null)
                        {
                            if (!String.IsNullOrEmpty(model.Customer.sCustomerName))
                            {
                                CustomerName = new ReportParameter("CustomerName", model.Customer.sCustomerName);
                            }
                            else
                            {
                                CustomerName = new ReportParameter("CustomerName", "-");
                            }

                            if (!String.IsNullOrEmpty(model.Customer.sMobile))
                            {
                                CustomerMobile = new ReportParameter("CustomerMobile", model.Customer.sMobile);
                            }
                            else
                            {
                                CustomerMobile = new ReportParameter("CustomerMobile", "-");
                            }

                            if (!String.IsNullOrEmpty(model.Customer.sPhone))
                            {
                                CustomerPhone = new ReportParameter("CustomerPhone", model.Customer.sPhone);
                            }
                            else
                            {
                                CustomerPhone = new ReportParameter("CustomerPhone", "-");
                            }
                        }
                        else
                        {
                            CustomerName = new ReportParameter("CustomerName", "-");
                            CustomerMobile = new ReportParameter("CustomerMobile", "-");
                            CustomerPhone = new ReportParameter("CustomerPhone", "-");
                        }
                        #endregion

                        #region Product

                        ReportParameter ProductType;
                        ReportParameter ProductBrand;
                        ReportParameter ProductModel;
                       

                        if (model.Product != null)
                        {
                            if (!String.IsNullOrEmpty(model.Product.vProductTypeDescription))
                            {
                                ProductType = new ReportParameter("ProductType", model.Product.vProductTypeDescription);
                            }
                            else
                            {
                                ProductType = new ReportParameter("ProductType", "-");
                            }

                            if (!String.IsNullOrEmpty(model.Product.vBrandDescription))
                            {
                                ProductBrand = new ReportParameter("ProductBrand", model.Product.vBrandDescription);
                            }
                            else
                            {
                                ProductBrand = new ReportParameter("ProductBrand", "-");
                            }

                            if (!String.IsNullOrEmpty(model.Product.sProductModel))
                            {
                                ProductModel = new ReportParameter("ProductModel", model.Product.sProductModel);
                            }
                            else
                            {
                                ProductModel = new ReportParameter("ProductModel", "-");
                            }
                        }
                        else
                        {
                            ProductType = new ReportParameter("ProductType", "-");
                            ProductBrand = new ReportParameter("ProductBrand", "-");
                            ProductModel = new ReportParameter("ProductModel", "-");
                        }
                        #endregion

                        #region Repair
                        ReportParameter ReferenceNumber;
                        ReportParameter DateAdd;
                        ReportParameter TimeAdd;
                        ReportParameter Serial;
                        ReportParameter ProductAccessories;
                        ReportParameter ProductColor;
                        ReportParameter DayWarranty;


                        if (model.Repair != null)
                        {
                            if (!String.IsNullOrEmpty(model.Repair.sRepairNo))
                            {
                                ReferenceNumber = new ReportParameter("ReferenceNumber", model.Repair.sRepairNo);
                            }
                            else
                            {
                                ReferenceNumber = new ReportParameter("ReferenceNumber", "-");
                            }

                            if (model.Repair.dtDateAdd.HasValue)
                            {
                                DateAdd = new ReportParameter("DateAdd", DateExtension.DateThaiFormat(model.Repair.dtDateAdd.Value));
                            }
                            else
                            {
                                DateAdd = new ReportParameter("DateAdd", "-");
                            }

                            if (model.Repair.dtDateAdd.HasValue)
                            {
                                TimeAdd = new ReportParameter("TimeAdd", DateTime.Now.ToString(DateExtension.TimeFormat()));
                            }
                            else
                            {
                                TimeAdd = new ReportParameter("TimeAdd", "-");
                            }

                            if (!String.IsNullOrEmpty(model.Repair.sSerial))
                            {
                                Serial = new ReportParameter("Serial", model.Repair.sSerial);
                            }
                            else
                            {
                                Serial = new ReportParameter("Serial", "-");
                            }

                            if (!String.IsNullOrEmpty(model.Repair.sProductAccessories))
                            {
                                ProductAccessories = new ReportParameter("ProductAccessories", model.Repair.sProductAccessories);
                            }
                            else
                            {
                                ProductAccessories = new ReportParameter("ProductAccessories", "-");
                            }

                            if (!String.IsNullOrEmpty(model.Repair.sColor))
                            {
                                ProductColor = new ReportParameter("ProductColor", model.Repair.sColor);
                            }
                            else
                            {
                                ProductColor = new ReportParameter("ProductColor", "-");
                            }

                            if (model.Repair.iDayWarranty.HasValue)
                            {
                                DayWarranty = new ReportParameter("DayWarranty", model.Repair.iDayWarranty.ToString());
                            }
                            else
                            {
                                DayWarranty = new ReportParameter("DayWarranty", "-");
                            }
                        }
                        else
                        {
                            ReferenceNumber = new ReportParameter("ReferenceNumber", "-");
                            DateAdd = new ReportParameter("DateAdd", "-");
                            TimeAdd = new ReportParameter("TimeAdd", "-");
                            Serial = new ReportParameter("Serial", "-");
                            ProductAccessories = new ReportParameter("ProductAccessories", "-");
                            ProductColor = new ReportParameter("ProductColor", "-");
                            DayWarranty = new ReportParameter("DayWarranty", "-");
                        }

                        #endregion

                        ReportParameter InsuranceExpire;
                        if (model.Repair.dtInsuranceExpire != null)
                        {
                            if (model.Repair.dtInsuranceExpire.HasValue)
                            {
                                InsuranceExpire = new ReportParameter("InsuranceExpire", DateExtension.DateThaiFormat(model.Repair.dtInsuranceExpire.Value));
                            }
                            else
                            {
                                InsuranceExpire = new ReportParameter("InsuranceExpire", "-");
                            }
                        }
                        else
                        {
                            InsuranceExpire = new ReportParameter("InsuranceExpire", "-");
                        }

                        ReportParameter DueDate;
                        if (model.Repair.dtDueDate != null)
                        {
                            if (model.Repair.dtDueDate.HasValue)
                            {
                                DueDate = new ReportParameter("DueDate", DateExtension.DateThaiFormat(model.Repair.dtDueDate.Value));
                            }
                            else
                            {
                                DueDate = new ReportParameter("DueDate", "-");
                            }
                        }
                        else
                        {
                            DueDate = new ReportParameter("DueDate", "-");
                        }

                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { 
                                                                ApplicationSettingName, 
                                                                ApplicationSettingAddress, 
                                                                CustomerName,
                                                                CustomerMobile,
                                                                CustomerPhone,
                                                                ProductType,
                                                                ProductBrand,
                                                                ReferenceNumber,
                                                                DateAdd,
                                                                TimeAdd,
                                                                ApplicationTel,
                                                                ProductModel,
                                                                Serial,
                                                                InsuranceExpire,
                                                                ProductAccessories,
                                                                ProductColor,
                                                                DayWarranty,
                                                                DueDate,
                                                                ApplicationFax
                                                            });
                        Session["BookingViewModel"] = null;
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
       
        private DataTable getData()
        {
            if (model.Causes != null)
            {
                DataTable dt = ToDataTable(model.Causes);

                return dt;
            }
            else
            {
                DataTable dt = new DataTable();
                return dt;
            }
        }


        private static DataTable ConvertListToDataTable(List<Cause[]> list)
	    {
	        // New table.
	        DataTable table = new DataTable();

	        // Get max columns.
	        int columns = 0;
	        foreach (var array in list)
	        {
		    if (array.Length > columns)
		    {
		        columns = array.Length;
		    }
	        }

	        // Add columns.
	        for (int i = 0; i < columns; i++)
	        {
		    table.Columns.Add();
	        }

	        // Add rows.
	        foreach (var array in list)
	        {
		    table.Rows.Add(array);
	        }

	        return table;
        }
        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        private DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof (T).Name);

            PropertyInfo[] props = typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }
        private static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }

        private void runRptViewer()
        {
            this.ReportViewer1.Reset();
           
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/") + "//Reports//InformRepair.rdlc";
            ReportDataSource rds = new ReportDataSource("dsNewDataSet_Table", getData());

            
            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.DataSources.Add(rds);
            this.ReportViewer1.DataBind();
            this.ReportViewer1.LocalReport.Refresh();

            //Response.Clear();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("Content-Disposition", "attachment; filename=");
          
            //Response.End();
        }
    }
}