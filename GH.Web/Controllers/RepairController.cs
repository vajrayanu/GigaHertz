using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using GH.DAL.Helpers;
using GH.DAL.Model;
using GH.DAL.SQLDAL;
using SignalR.Hubs;
using GH.Web.Models;

namespace GH.Web.Controllers
{
    [Authorize]
    public class RepairController : Controller
    {
        private Staff Staff
        {
            get
            {
                var userId = (Guid)Membership.GetUser().ProviderUserKey;
                var staff = StaffManager.GetById(userId);
                return staff;
            }
        }

        private User Users
        {
            get
            {
                var user = StaffUserManager.GetStaffByName(User.Identity.Name);
                return user;
            }
        }

        public ActionResult Index()
        {
            ViewBag.RepairList = "first active";
            ViewBag.ClientName = User.Identity.Name;

            BookingRepairViewModel model = new BookingRepairViewModel();
            ViewBag.WorkingStatus = new SelectList(WorkingStatusManager.GetAll().Where(m => m.iDefault > 0), "kWorkingStatusId", "sDescription");

            if (!User.IsInRole("Admin"))
            {
                if (Staff.vStaffPositionDescription == "ฝ่ายตรวจสอบคุณภาพ")
                    return View("ListsQC", model);

                if (Staff.vStaffPositionDescription == "หัวหน้าช่าง")
                    return View("ListsSTC", model);

                if (Staff.vStaffPositionDescription == "ช่าง")
                    return View("ListsTC", model);

                if (Staff.vStaffPositionDescription == "ฝ่ายรับสินค้า")
                    return View("ListsFR", model);

                if (Staff.vStaffPositionDescription == "ฝ่ายเครม")
                    return View("ListsCR", model);
            }

            //if (User.IsInRole("User"))
            //{
            //    return View("ListsUser",model);
            //}
            //else if(User.IsInRole("SuperUser"))
            //{
            //    return View("ListsSuperUser", model);
            //}




            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.RepairCreate = "first active";
            ViewBag.SuperUser = StaffManager.GetAll().Where(m => m.vStaffPositionDescription == "หัวหน้าช่าง");
            ViewBag.ProductTypes = ProductTypeManager.GetAll();
            ViewBag.Brands = BrandManager.GetAll();

            BookingRepairViewModel model = new BookingRepairViewModel();

            ViewBag.SessionId = Guid.NewGuid();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(BookingRepairViewModel model, FormCollection collection)
        {
            try
            {
                //initial working status
                var allWorkingId = WorkingStatusManager.GetAll();
                var workingStatusId = allWorkingId.Where(m => m.iDefault == (int)Working.Open).SingleOrDefault();
                if (!string.IsNullOrEmpty(model.Repair.sRepairNo))
                {   //รับสินค้าตีกลับ
                    workingStatusId = allWorkingId.Where(m => m.iDefault == (int)Working.OpenBack).SingleOrDefault();
                }

                if (model.Product != null)
                    model.Product.Brand = BrandManager.GetById(model.Product.kBrandId);
                model.Product.ProductType = ProductTypeManager.GetById(model.Product.kProductTypeId);


                #region SAVE CUSTOMER
                var customerCount = CustomerManager.GetCountDuplicate(model.Customer.sCustomerName.Trim());
                //if (customerCount.Count <= 0)
                //{
                    Customer customer = new Customer
                    {
                        kCustomerId = Guid.NewGuid(),
                        sCustomerName = model.Customer.sCustomerName,
                        sAddress1 = model.Customer.sAddress1,
                        sCity = model.Customer.sCity,
                        sZip = model.Customer.sZip,
                        sPhone = model.Customer.sPhone,
                        sMobile = model.Customer.sMobile,
                        sFax = model.Customer.sFax,
                        sEmailAddress = model.Customer.sEmailAddress
                    };
                    CustomerManager.Create(customer);
                    model.Customer = customer;
                //}
                //else
               // {
                //    model.Customer = customerCount[0];
                //}


                #endregion

                #region SAVE Brand
                var brandCount = BrandManager.GetCountDuplicate(model.Product.Brand.sBrandName.Trim());
                if (brandCount.Count <= 0)
                {
                    Brand brand = new Brand
                    {
                        kBrandId = Guid.NewGuid(),
                        sBrandName = model.Product.Brand.sBrandName
                    };
                    BrandManager.Create(brand);
                    model.Product.Brand = brand;
                }
                else
                {
                    model.Product.Brand = brandCount[0];
                }

                #endregion

                #region SAVE Product Type
                var productTypeCount = ProductTypeManager.GetCountDuplicate(model.Product.ProductType.sDescription.Trim());
                if (productTypeCount.Count <= 0)
                {
                    ProductType productType = new ProductType
                    {
                        kProductTypeId = Guid.NewGuid(),
                        sDescription = model.Product.ProductType.sDescription
                    };
                    ProductTypeManager.Create(productType);
                    model.Product.ProductType = productType;
                }
                else
                {
                    model.Product.ProductType = productTypeCount[0];
                }

                #endregion

                #region SAVE PRODUCT
                var productCount = ProductManager.GetCountDuplicate(
                                    model.Product.sProductName.Trim()
                                    //, model.Product.Brand.sBrandName.Trim()
                                    //, model.Product.ProductType.sDescription.Trim()
                );
                if (productCount.Count <= 0)
                {
                    Product product = new Product
                    {
                        kProductId = Guid.NewGuid(),
                        kProductTypeId = model.Product.ProductType.kProductTypeId,
                        kBrandId = model.Product.Brand.kBrandId,
                        sProductName = model.Product.sProductName,
                        sProductModel = model.Product.sProductModel
                    };
                    ProductManager.Create(product);
                    model.Product = product;
                }
                else
                {
                    model.Product = productCount[0];
                }

                #endregion

                #region SAVE ProductAccessorie
                if (!String.IsNullOrEmpty(model.Repair.sProductAccessories))
                {
                    string[] words = model.Repair.sProductAccessories.Split(',');

                    foreach (var word in words)
                    {
                        var productAccessorieCount = ProductAccessorieManager.GetCountDuplicate(word.Trim());
                        if (productAccessorieCount <= 0)
                        {
                            if (word.Trim() != "")
                            {
                                ProductAccessorie productAccessorie = new ProductAccessorie
                                {
                                    kProductAccessorieId = Guid.NewGuid(),
                                    sDescription = word
                                };
                                ProductAccessorieManager.Create(productAccessorie);
                            }
                        }
                    }
                }
                #endregion

                #region SAVE COLOR
                if (!String.IsNullOrEmpty(model.Repair.sColor))
                {
                    string[] words = model.Repair.sColor.Split(',');

                    foreach (var word in words)
                    {
                        var colorCount = ColorManager.GetCountDuplicate(word.Trim());
                        if (colorCount <= 0)
                        {
                            if (word.Trim() != "")
                            {
                                Color color = new Color
                                {
                                    kColorId = Guid.NewGuid(),
                                    sDescription = word
                                };
                                ColorManager.Create(color);
                            }
                        }
                    }
                }
                #endregion

                #region SAVE REPAIR BOOKING

                //initial booking first character
                string char_number = collection["RepairType"].Equals("1") ? CharBooking.SV.ToString() : CharBooking.E.ToString();

                Repair repair = new Repair();
                repair.dtDateUpdate = DateTime.Now;
                repair.kRepairId = Guid.NewGuid();
                repair.kBookingId = (Guid)Membership.GetUser().ProviderUserKey; // who create job
                repair.kOwnerId = model.Repair.kOwnerId; // who owner (super technical)
                repair.kStaffId = (Guid)Membership.GetUser().ProviderUserKey; // who support on this
                repair.kCustomerId = model.Customer.kCustomerId;
                repair.kProductId = model.Product.kProductId;
                repair.sRepairNo = String.Format("{0}{1}", char_number, NextItemNoManager.GetNextItemNo());
                if (!string.IsNullOrEmpty(model.Repair.sRepairNo))
                {
                    repair.sRepairBackNo = model.Repair.sRepairNo;
                    repair.kCloseStatusId = CloseStatusManager.GetAll().Where(m => m.iOrder == (int)Close.Cancle).SingleOrDefault().kCloseStatusId;
                    repair.IsBack = true;
                }
                repair.iDayWarranty = 30;
                repair.IsComplete = false;
                repair.IsCustomerRecieved = false;
                repair.sSerial = model.Repair.sSerial;
                repair.sColor = model.Repair.sColor;
                repair.sNote = model.Repair.sNote;
                repair.sProductAccessories = model.Repair.sProductAccessories;
                if (model.Repair.dtInsuranceExpire != null)
                {
                    repair.dtInsuranceExpire = model.Repair.dtInsuranceExpire.Value.AddYears(-543);
                    repair.kCloseStatusId = CloseStatusManager.GetAll().Where(m => m.iOrder == (int)Close.IsInsurance).SingleOrDefault().kCloseStatusId;
                    //repair.IsInsurance = true;
                }
                if (model.Repair.dtDueDate != null)
                    repair.dtDueDate = model.Repair.dtDueDate.Value.AddYears(-543);

                model.Repair = repair;
                RepairManager.Create(model.Repair);
                NextItemNoManager.IncreaseNextItemNo();

                #endregion

                #region SAVE REPAIR WORKING STATUS

                //who booking (front)
                RepairStatus repairStatus = new RepairStatus
                {
                    kRepairStatusId = Guid.NewGuid(),
                    kWorkingStatusId = workingStatusId.kWorkingStatusId,
                    kStaffId = (Guid)Membership.GetUser().ProviderUserKey,
                    kRepairId = model.Repair.kRepairId
                };
                RepairStatusManager.Create(repairStatus);

                #endregion

                #region SAVE REPAIR BACK
                //if (!string.IsNullOrEmpty(model.Repair.sRepairNo))
                //{
                //    RepairBack repairBack = new RepairBack
                //    {
                //        kRepairBackId = Guid.NewGuid(),
                //        sRepairBeforeNo = model.Repair.sRepairNo,
                //        sRepairAfterNo = repair.sRepairNo
                //    };
                //    RepairBackManager.Create(repairBack);
                //}
                #endregion

                #region SAVE CAUSE OF REPAIR

                //initial cause
                string keyname;
                string keyvalue;
                int countvalue = 0;
                for (int i = 0; i <= collection.Count - 1; i++)
                {
                    keyname = collection.AllKeys[i];
                    keyvalue = collection[i];

                    if (keyname.Contains("cause"))
                    {
                        countvalue++;
                    }
                }

                if (countvalue > 1)
                {
                    model.Causes = new List<Cause>();
                    for (int i = 1; i <= countvalue / 2; i++)
                    {
                        Cause cause = new Cause();
                        cause.kCauseId = Guid.NewGuid();
                        cause.sDescription = collection[String.Format("cause_description_{0}", i)];

                        if (!String.IsNullOrEmpty(cause.sDescription))
                        {
                            Decimal price = 0;
                            if (Decimal.TryParse(collection[String.Format("cause_price_{0}", i)], out price))
                            {
                                cause.dPrice = price;
                            }
                            model.Causes.Add(cause);
                        }
                    }
                }

                if (model.Causes != null)
                {
                    foreach (var item in model.Causes)
                    {
                        var causeCount = CauseManager.GetCountDuplicate(item.sDescription.Trim());
                        if (causeCount <= 0)
                        {
                            Cause cause = new Cause
                            {
                                kCauseId = Guid.NewGuid(),
                                sDescription = item.sDescription.Trim(),
                                dPrice = item.dPrice
                            };
                            CauseManager.Create(cause);
                        }

                        //save to repair cause estimate (front key for this)
                        RepairCauseEstimate repairCause = new RepairCauseEstimate
                        {
                            kRepairCauseEstimateId = Guid.NewGuid(),
                            kRepairId = model.Repair.kRepairId,
                            kStaffId = (Guid)Membership.GetUser().ProviderUserKey,
                            sDescription = item.sDescription,
                            dPrice = item.dPrice
                        };
                        RepairCauseEstimateManager.Create(repairCause);
                    }
                }
                #endregion

                #region SAVE FileUpload
                var session = collection["SessionId"];
                if (session != null)
                {
                    model.FileUploads = FileUploadManager.GetBySession(new Guid(session));
                    if (model.FileUploads != null)
                    {
                        foreach (var item in model.FileUploads)
                        {
                            item.kRepairId = model.Repair.kRepairId;
                            FileUploadManager.Edit(item);
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }

            Repair model2 = RepairManager.GetById(model.Repair.kRepairId);
            //create remind for history
            RemindHistory remind = new RemindHistory
            {
                sRemind = model2.vMessage,
                kStaffId = model2.kStaffId
            };
            RemindHistoryManager.Create(remind);

            //var staff = StaffManager.GetById(status_sending.kStaffId);
            var clientName = User.Identity.Name;
            Task.Factory.StartNew(() =>
            {
                var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                clients.RecordCreated(clientName, model2);
            });

            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid id)
        {
            ViewBag.RepairCreate = "first active";
            ViewBag.SuperUser = StaffManager.GetAll().Where(m => m.vStaffPositionDescription == "หัวหน้าช่าง");
            ViewBag.ProductTypes = ProductTypeManager.GetAll();
            ViewBag.Brands = BrandManager.GetAll();

            BookingRepairViewModel model = new BookingRepairViewModel();
            var repair = RepairManager.GetById(id);
            if (repair.dtInsuranceExpire.HasValue)
                repair.dtInsuranceExpire = repair.dtInsuranceExpire.Value.AddYears(543);
            if (repair.dtDateUpdate.HasValue)
                repair.dtDateUpdate = repair.dtDateUpdate.Value.AddYears(543);
            if (repair.dtDueDate.HasValue)
                repair.dtDueDate = repair.dtDueDate.Value.AddYears(543);

            model.Repair = repair;

            //model.Repair.RepairCauses = repair.RepairCauses;
            model.Customer = repair.Customer;
            model.Product = repair.Product;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BookingRepairViewModel model, FormCollection collection)
        {
            try
            {
                if (model.Repair != null)
                {
                    //initial value
                    var item_found = RepairManager.GetByRepairNo(model.Repair.sRepairNo);


                    #region Customer
                    //model.Customer.kCustomerId = item_found.Customer.kCustomerId;
                    //CustomerManager.Edit(model.Customer);
                    //bool isCustomerChange = false;
                    //var customerCount = CustomerManager.GetCountDuplicateAndAddress(model.Customer.sCustomerName.Trim(), model.Customer.sAddress1 ?? "");
                    //if (customerCount.Count >= 0)
                    //{
                        //Customer customer = new Customer
                        //{
                        //    kCustomerId = Guid.NewGuid(),
                        //    sCustomerName = model.Customer.sCustomerName,
                        //    sAddress1 = model.Customer.sAddress1,
                        //    sCity = model.Customer.sCity,
                        //    sZip = model.Customer.sZip,
                        //    sPhone = model.Customer.sPhone,
                        //    sMobile = model.Customer.sMobile,
                        //    sFax = model.Customer.sFax,
                        //    sEmailAddress = model.Customer.sEmailAddress
                        //};
                        //CustomerManager.Create(customer);
                        //model.Customer = customer;
                        //isCustomerChange = true;
                    //}
                    //else
                    //{
                    //    model.Customer = customerCount[0];
                    //}
                    model.Customer.kCustomerId = item_found.Customer.kCustomerId;
                    CustomerManager.Edit(model.Customer);
                    #endregion

                    #region Product
                    model.Product.kProductId = item_found.Product.kProductId;
                    ProductManager.Edit(model.Product);
                    #endregion


                    #region SAVE EDIT REPAIR BOOKING
                    //if (isCustomerChange)
                    //{
                    //    model.Repair.kCustomerId = model.Customer.kCustomerId;
                    //}
                        model.Repair.kRepairId = item_found.kRepairId;
                        model.Repair.kBookingId = item_found.kBookingId;
                        model.Repair.kOwnerId = item_found.kOwnerId;
                        model.Repair.kStaffId = item_found.kStaffId;
                        model.Repair.kQCId = item_found.kQCId;
                        model.Repair.iDayWarranty = 30;
                        model.Repair.IsNormal = item_found.IsNormal;
                        model.Repair.IsNoCredit = item_found.IsNoCredit;
                        model.Repair.IsQCPass = item_found.IsQCPass;
                        model.Repair.IsCustomerRecieved = item_found.IsCustomerRecieved;
                        model.Repair.IsBack = item_found.IsBack;
                        model.Repair.IsInsurance = item_found.IsInsurance;
                        model.Repair.IsCancle = item_found.IsCancle;
                        model.Repair.iRemind = item_found.iRemind;
                        model.Repair.dtEndWarranty = item_found.dtEndWarranty;
                        model.Repair.dtDateAdd = item_found.dtDateAdd;
                        //model.Repair.dtInsuranceExpire = model.Repair.dtInsuranceExpire.Value.AddYears(-543);
                        model.Repair.dtDueDate = model.Repair.dtDueDate.Value.AddYears(-543);
                        model.Repair.dtDateUpdate = model.Repair.dtDateUpdate.Value.AddYears(-543);
                        //model.Repair.sRepairNo = item_found.sSerial;
                        RepairManager.Edit(model.Repair);

                    #endregion

                    #region SAVE FileUpload
                    var session = collection["SessionId"];
                    if (session != null)
                    {
                        model.FileUploads = FileUploadManager.GetBySession(new Guid(session));
                        if (model.FileUploads != null)
                        {
                            foreach (var item in model.FileUploads)
                            {
                                item.kRepairId = model.Repair.kRepairId;
                                FileUploadManager.Edit(item);
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Confirm(BookingRepairViewModel model, FormCollection collection)
        {
            // initial a reference number //
            string char_number = collection["RepairType"].Equals("1") ? CharBooking.SV.ToString() : CharBooking.E.ToString();

            model.Repair.sRepairNo = String.Format("{0}{1}", char_number, NextItemNoManager.GetNextItemNo());

            if (model.Product != null)
                model.Product.Brand = BrandManager.GetById(model.Product.kBrandId);
            model.Product.ProductType = ProductTypeManager.GetById(model.Product.kProductTypeId);

            model.Repair.iDayWarranty = 30;

            string keyname;
            string keyvalue;
            int countvalue = 0;
            for (int i = 0; i <= collection.Count - 1; i++)
            {
                keyname = collection.AllKeys[i];
                keyvalue = collection[i];

                if (keyname.Contains("cause"))
                {
                    countvalue++;
                }
            }

            if (countvalue > 1)
            {
                model.Causes = new List<Cause>();
                for (int i = 1; i <= countvalue / 2; i++)
                {
                    Cause cause = new Cause();
                    cause.kCauseId = Guid.NewGuid();
                    cause.dtDateAdd = DateTime.Now;
                    cause.sDescription = collection[String.Format("cause_description_{0}", i)];

                    if (!String.IsNullOrEmpty(cause.sDescription))
                    {
                        Decimal price = 0;
                        if (Decimal.TryParse(collection[String.Format("cause_price_{0}", i)], out price))
                        {
                            cause.dPrice = price;
                        }
                        model.Causes.Add(cause);
                    }
                }
            }

            if (model.Repair.dtDueDate != null)
            {
                model.Repair.dtDueDate = model.Repair.dtDueDate.Value.AddYears(-543);
            }
            if (model.Repair.dtInsuranceExpire != null)
            {
                model.Repair.dtInsuranceExpire = model.Repair.dtInsuranceExpire.Value.AddYears(-543);
            }

            //var session = collection["SessionId"];
            //if (session != null)
            //{
            //    model.FileUploads = new FileUpload();
            //    model.FileUploads.kSessionId = new Guid(session);
            //}

            this.HttpContext.Session["BookingViewModel"] = model;

            return PartialView("Confirm", model);
        }

        [HttpPost]
        public JsonResult GetAll()
        {
            try
            {
                var items = RepairManager.GetAll();

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sRepairNo, Value = m.kRepairId }) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult Search(string term)
        {
            try
            {
                var items = RepairManager.GetBySearch(term);

                if (Users.Roles.FirstOrDefault().RoleName == "User")
                {
                    if (Staff.StaffPosition.sDescription == "ช่าง")
                    {
                        items = items.Where(m => m.kStaffId == (Guid)Membership.GetUser().ProviderUserKey).ToList();
                    }
                }

                if (Users.Roles.FirstOrDefault().RoleName == "SuperUser")
                {
                    //items = items.Where(m => m.kOwnerId == (Guid)Membership.GetUser().ProviderUserKey).ToList();
                }

                return Json(items.Select(m => m.sRepairNo + " - " + m.Product.sProductName), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult Search2(string term)
        {
            try
            {
                var items = RepairManager.GetBySearch(term);

                return Json(items.Select(m => new
                {
                    label = String.Format("{0}", m.sRepairNo),
                    description = String.Format("เลขอ้างอิง {0} สินค้า {1}", m.sRepairNo, m.vFullProductDescription)
                })
                 , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Lists(string jtSearching = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null, string jtStatus = null)
        {
            try
            {
                var itemCount = RepairManager.GetCountByFiltering(jtSearching, jtStatus);
                var items = RepairManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting, jtStatus).ToList();

                if (User.IsInRole("User"))
                {
                    if (Staff.StaffPosition.sDescription == "ช่าง")
                    {
                        int totalSize = 0;
                        items = RepairManager.GetByFilteringsForStaff(jtSearching, jtStartIndex, jtPageSize, jtSorting, jtStatus, (Guid)Membership.GetUser().ProviderUserKey, out totalSize).ToList();
                        itemCount = totalSize;
                    }
                }
                //else if (User.IsInRole("SuperUser"))
                //{
                //    //items = items.Where(m => m.kOwnerId == (Guid)Membership.GetUser().ProviderUserKey).ToList();
                //    //itemCount = items.Count;
                //}

                return Json(new
                {
                    Result = "OK",
                    Records = items,
                    TotalRecordCount = itemCount
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult GetById(Guid Id)
        {
            try
            {
                Repair itemFound = RepairManager.GetById(Id);

                return Json(itemFound, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //pass customerid
        public JsonResult GetByCustomer(Guid Id)
        {
            try
            {
                var itemFounds = RepairManager.GetByCustomer(Id);
                return Json(new { Result = "OK", Records = itemFounds }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult GetColor(string q)
        {
            try
            {
                q = q.Trim();
                var items = ColorManager.GetAll();

                if (!string.IsNullOrEmpty(q))
                {
                    items = ColorManager.GetBySearch(q);
                }
                return Json(items.Select(m => m.sDescription), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid kRepairId)
        {
            try
            {
                //Thread.Sleep(50);
                Repair itemFound = RepairManager.GetById(kRepairId);
                RepairManager.Delete(kRepairId);

                //create remind for history
                RemindHistory remind = new RemindHistory
                {
                    sRemind = string.Format("ลบ {0}", itemFound.sRepairNo),
                    kStaffId = (Guid)Membership.GetUser().ProviderUserKey
                };
                RemindHistoryManager.Create(remind);

                var clientName = User.Identity.Name;
                Task.Factory.StartNew(() =>
                {
                    var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                    clients.RecordUpdated(clientName, itemFound.sRepairNo);
                });

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //get repair from repaircause (tecnical or super technical key)
        public ActionResult Summary(Guid Id)
        {
            var itemFounds = RepairManager.GetRepair(Id);

            BookingRepairViewModel model = new BookingRepairViewModel();

            if (itemFounds != null)
            {
                model.Repair = itemFounds[0];
                model.Customer = itemFounds[0].Customer;
                model.Product = itemFounds[0].Product;

                if (itemFounds[0].RepairCauses.Count > 0)
                {
                    model.Causes = new List<Cause>();
                    for (int i = 0; i < itemFounds[0].RepairCauses.Count; i++)
                    {
                        Cause cause = new Cause
                        {
                            sDescription = itemFounds[0].RepairCauses[i].sDescription,
                            dPrice = itemFounds[0].RepairCauses[i].dPrice
                        };
                        model.Causes.Add(cause);
                    }
                }

            }

            this.HttpContext.Session["BookingViewModel"] = model;
            return View(model);
            //return PartialView( model);
        }


        //get repair from repaircause estimate (front key)
        public ActionResult RepairSummary(Guid Id)
        {
            var itemFounds = RepairManager.GetRepair(Id);

            BookingRepairViewModel model = new BookingRepairViewModel();

            if (itemFounds != null)
            {
                model.Repair = itemFounds[0];
                model.Customer = itemFounds[0].Customer;
                model.Product = itemFounds[0].Product;

                if (itemFounds[0].RepairCauseEstimates.Count > 0)
                {
                    model.Causes = new List<Cause>();
                    for (int i = 0; i < itemFounds[0].RepairCauseEstimates.Count; i++)
                    {
                        Cause cause = new Cause
                        {
                            sDescription = itemFounds[0].RepairCauseEstimates[i].sDescription,
                            dPrice = itemFounds[0].RepairCauseEstimates[i].dPrice
                        };
                        model.Causes.Add(cause);
                    }
                }

            }

            this.HttpContext.Session["BookingViewModel"] = model;
            return View(model);
            //return PartialView( model);
        }

        #region Repair Cause
        public JsonResult GetRepairCause(Guid Id)
        {
            try
            {
                var itemFounds = RepairManager.GetRepairCause(Id);
                return Json(new { Result = "OK", Records = itemFounds });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult CreateRepairCause(RepairCause model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                if (!Users.Roles.Select(m => m.RoleName == "Admin").SingleOrDefault())
                {
                    Repair itemFound = RepairManager.GetById(model.kRepairId);
                    if (itemFound == null)
                    {
                        return Json(new { Result = "ERROR", Message = "Item Not Found" });
                    }
                    if (itemFound.IsComplete == true)
                    {
                        return Json(new { Result = "ERROR", Message = "ปิด job!" });
                    }
                }


                model.kRepairCauseId = Guid.NewGuid();
                model.kStaffId = (Guid)Membership.GetUser().ProviderUserKey;
                RepairCauseManager.Create(model);

                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult EditRepairCause(RepairCause model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                if (!Users.Roles.Select(m => m.RoleName == "Admin").SingleOrDefault())
                {
                    var repairCause = RepairCauseManager.GetById(model.kRepairCauseId);
                    if (repairCause.kStaffId != (Guid)Membership.GetUser().ProviderUserKey)
                    {
                        return Json(new { Result = "ERROR", Message = "No permission for this item!" });
                    }

                    Repair repair = RepairManager.GetById(repairCause.kRepairId);
                    if (repair == null)
                    {
                        return Json(new { Result = "ERROR", Message = "Item Not Found" });
                    }
                    if (repair.IsComplete == true)
                    {
                        return Json(new { Result = "ERROR", Message = "ปิด job!" });
                    }
                }

                model.kStaffId = (Guid)Membership.GetUser().ProviderUserKey;
                RepairCauseManager.Edit(model);

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult DeleteRepairCause(RepairCause model)
        {
            try
            {
                var repairCause = RepairCauseManager.GetById(model.kRepairCauseId);
                if (repairCause.kStaffId != (Guid)Membership.GetUser().ProviderUserKey)
                {
                    return Json(new { Result = "ERROR", Message = "No permission for this item!" });
                }

                if (!Users.Roles.Select(m => m.RoleName == "Admin").SingleOrDefault())
                {
                    Repair repair = RepairManager.GetById(repairCause.kRepairId);
                    if (repair == null)
                    {
                        return Json(new { Result = "ERROR", Message = "Item Not Found" });
                    }
                    if (repair.IsComplete == true)
                    {
                        return Json(new { Result = "ERROR", Message = "ปิด job!" });
                    }
                }

                RepairCauseManager.Delete(model.kRepairCauseId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        #endregion

        #region Repair Cause Estimate
        public JsonResult GetRepairCauseEstimate(Guid Id)
        {
            try
            {
                var itemFounds = RepairManager.GetRepairCauseEstimate(Id);
                return Json(new { Result = "OK", Records = itemFounds });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult CreateRepairCauseEstimate(RepairCauseEstimate model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                if (!Users.Roles.Select(m => m.RoleName == "Admin").SingleOrDefault())
                {
                    Repair itemFound = RepairManager.GetById(model.kRepairId);
                    if (itemFound == null)
                    {
                        return Json(new { Result = "ERROR", Message = "Item Not Found" });
                    }
                    if (itemFound.IsComplete == true)
                    {
                        return Json(new { Result = "ERROR", Message = "ปิด job!" });
                    }
                }


                model.kRepairCauseEstimateId = Guid.NewGuid();
                model.kStaffId = (Guid)Membership.GetUser().ProviderUserKey;
                RepairCauseEstimateManager.Create(model);

                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult EditRepairCauseEstimate(RepairCauseEstimate model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                if (!Users.Roles.Select(m => m.RoleName == "Admin").SingleOrDefault())
                {
                    var repairCause = RepairCauseEstimateManager.GetById(model.kRepairCauseEstimateId);
                    if (repairCause.kStaffId != (Guid)Membership.GetUser().ProviderUserKey)
                    {
                        return Json(new { Result = "ERROR", Message = "No permission for this item!" });
                    }

                    Repair repair = RepairManager.GetById(repairCause.kRepairId);
                    if (repair == null)
                    {
                        return Json(new { Result = "ERROR", Message = "Item Not Found" });
                    }
                    if (repair.IsComplete == true)
                    {
                        return Json(new { Result = "ERROR", Message = "ปิด job!" });
                    }
                }

                model.kStaffId = (Guid)Membership.GetUser().ProviderUserKey;
                RepairCauseEstimateManager.Edit(model);

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult DeleteRepairCauseEstimate(RepairCauseEstimate model)
        {
            try
            {
                var repairCause = RepairCauseEstimateManager.GetById(model.kRepairCauseEstimateId);
                if (repairCause.kStaffId != (Guid)Membership.GetUser().ProviderUserKey)
                {
                    return Json(new { Result = "ERROR", Message = "No permission for this item!" });
                }

                if (!Users.Roles.Select(m => m.RoleName == "Admin").SingleOrDefault())
                {
                    Repair repair = RepairManager.GetById(repairCause.kRepairId);
                    if (repair == null)
                    {
                        return Json(new { Result = "ERROR", Message = "Item Not Found" });
                    }
                    if (repair.IsComplete == true)
                    {
                        return Json(new { Result = "ERROR", Message = "ปิด job!" });
                    }
                }

                RepairCauseEstimateManager.Delete(model.kRepairCauseEstimateId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        #endregion

        #region Remind
        public JsonResult GetRemind(Guid Id)
        {
            try
            {
                var itemFounds = RemindManager.GetByRepairId(Id);
                return Json(new { Result = "OK", Records = itemFounds });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult CreateRemind(Remind model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Repair itemFound = RepairManager.GetById(model.kRepairId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                if (itemFound.IsComplete == true)
                {
                    return Json(new { Result = "ERROR", Message = "ปิด job!" });
                }


                model.kRemindId = Guid.NewGuid();
                model.kStaffId = (Guid)Membership.GetUser().ProviderUserKey;
                RemindManager.Create(model);

                if (itemFound.iRemind != null)
                {
                    itemFound.iRemind = itemFound.iRemind + 1;
                }
                else
                {
                    itemFound.iRemind = 1;
                }

                RepairManager.Edit(itemFound);

                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult EditRemind(Remind model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                //var itemFound = RemindManager.GetById(model.kRemindId);
                var id = (Guid)Membership.GetUser().ProviderUserKey;
                //if (itemFound.kStaffId != id)
                //{
                //    return Json(new { Result = "ERROR", Message = "Can not edit this item." });
                //}

                model.kStaffId = id;
                RemindManager.Edit(model);

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult DeleteRemind(Remind model)
        {
            try
            {
                var itemFound = RemindManager.GetById(model.kRemindId);
                var id = (Guid)Membership.GetUser().ProviderUserKey;
                //if (itemFound.kStaffId != id)
                //{
                //    return Json(new { Result = "ERROR", Message = "Can not delete this item." });
                //}

                RemindManager.Delete(model.kRemindId);

                Repair repair = RepairManager.GetById(itemFound.kRepairId);
                if (repair.iRemind != null)
                {
                    repair.iRemind = repair.iRemind - 1;
                }
                else
                {
                    repair.iRemind = null;
                }

                RepairManager.Edit(repair);

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        #endregion

        public JsonResult GetRepair(Guid Id)
        {
            try
            {
                var itemFounds = RepairManager.GetRepair(Id);
                return Json(new { Result = "OK", Records = itemFounds });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult GetRepairStatus(Guid Id)
        {
            try
            {
                var itemFounds = RepairManager.GetRepairStatus(Id);


                return Json(new { Result = "OK", Records = itemFounds });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #region Admin ROle
        [HttpPost]
        public JsonResult EditRepair(Repair repair_sending, RepairStatus status_sending, FormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Repair itemFound = RepairManager.GetById(repair_sending.kRepairId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                if (itemFound.kStaffId != (Guid)Membership.GetUser().ProviderUserKey)
                {
                    //return Json(new { Result = "ERROR", Message = "No permission for this job!" });
                }
                if (itemFound.IsComplete == true)
                {
                    //return Json(new { Result = "ERROR", Message = "ปิด job!" });
                }

                //chack status
                var status = WorkingStatusManager.GetById(status_sending.kWorkingStatusId);
                if (status.iDefault == (int)Working.Default)
                {
                    return Json(new { Result = "ERROR", Message = "เลือกสถานะงาน!" });
                }
                if (status.iDefault == (int)Working.Close && repair_sending.IsCustomerRecieved != true)
                {
                    return Json(new { Result = "ERROR", Message = "ตรวจสอบลูกค้ารับสินค้า!" });
                }

                bool isStatusChange = false;
                if (status_sending.kWorkingStatusId != itemFound.kWorkingStatusId || status_sending.kStaffId != itemFound.kStaffId)
                {
                    if (repair_sending.IsCustomerRecieved == true)
                    {
                        Guid closeStaffId;
                        if (Guid.TryParse(collection["kCloseStaffId"], out closeStaffId))
                        {
                            if (closeStaffId == Guid.Empty)
                            {
                                return Json(new { Result = "ERROR", Message = "เลือกผลงานช่าง!" });
                            }
                            else
                            {
                                //close job
                                itemFound.IsComplete = true;
                                status_sending.kStaffId = closeStaffId;
                            }
                        }
                    }
                    //assign status
                    status_sending.dtDateAdd = DateTime.Now;
                    status_sending.kRepairStatusId = Guid.NewGuid();
                    RepairStatusManager.Create(status_sending);
                    isStatusChange = true;
                }
                else
                {
                    status_sending.kStaffId = itemFound.kOwnerId.Value;
                }

                //assign repair
                if (isStatusChange)
                {
                    itemFound.kStaffId = status_sending.kStaffId;
                }
                int closeStatusId;
                if (int.TryParse(collection["vCloseStatusId"], out closeStatusId))
                {
                    if (closeStatusId > 0)
                    {
                        itemFound.kCloseStatusId = CloseStatusManager.GetAll().Where(m => m.iOrder == closeStatusId).SingleOrDefault().kCloseStatusId;
                    }
                }
                itemFound.IsNoCredit = repair_sending.IsNoCredit;
                itemFound.IsQCPass = repair_sending.IsQCPass;
                itemFound.IsCustomerRecieved = repair_sending.IsCustomerRecieved;
                itemFound.dtDateUpdate = DateTime.Now;
                RepairManager.Edit(itemFound);

                #region update claim side
                Claim claim = ClaimManager.GetByRepairNo(itemFound.sRepairNo);
                if (claim != null)
                {
                    ClaimStatus claimStatus = new ClaimStatus
                    {
                        kStaffId = status_sending.kStaffId,
                        kClaimId = claim.kClaimId,
                        kWorkingStatusId = status_sending.kWorkingStatusId,
                        kClaimStatusId = Guid.NewGuid()
                    };
                    ClaimStatusManager.Create(claimStatus);

                    if (itemFound.IsComplete == true)
                    {
                        claim.IsComplete = true;
                    }
                    if (isStatusChange)
                    {
                        claim.kStaffId = status_sending.kStaffId;
                    }
                    ClaimManager.Edit(claim);
                }
                #endregion

                if (isStatusChange)
                {
                    //create remind for history
                    RemindHistory remind = new RemindHistory
                    {
                        sRemind = itemFound.vMessage,
                        kStaffId = status_sending.kStaffId
                    };
                    RemindHistoryManager.Create(remind);

                    var clientName = User.Identity.Name;
                    Task.Factory.StartNew(() =>
                    {
                        var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                        clients.RecordUpdated(clientName, itemFound);
                    });
                }

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        #endregion

        #region Super User
        [HttpPost]
        public JsonResult EditSTCRepair(Repair repair_sending, RepairStatus status_sending, FormCollection col)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Repair itemFound = RepairManager.GetById(repair_sending.kRepairId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                if (itemFound.kStaffId != (Guid)Membership.GetUser().ProviderUserKey)
                {
                    //return Json(new { Result = "ERROR", Message = "No permission for this job!" });
                }
                if (itemFound.IsComplete == true)
                {
                    return Json(new { Result = "ERROR", Message = "ปิด job!" });
                }

                //chack status
                var status = WorkingStatusManager.GetById(status_sending.kWorkingStatusId);
                if (status.iDefault == (int)Working.Default)
                {
                    return Json(new { Result = "ERROR", Message = "เลือกสถานะงาน!" });
                }
                if (status.iDefault == (int)Working.Close && repair_sending.IsCustomerRecieved != true)
                {
                    return Json(new { Result = "ERROR", Message = "ตรวจสอบลูกค้ารับสินค้า!" });
                }


                bool isStatusChange = false;
                if (status_sending.kWorkingStatusId != itemFound.kWorkingStatusId || status_sending.kStaffId != itemFound.kStaffId)
                {
                    //assign status
                    status_sending.dtDateAdd = DateTime.Now;
                    status_sending.kRepairStatusId = Guid.NewGuid();
                    RepairStatusManager.Create(status_sending);
                    isStatusChange = true;
                }

                if (status.iDefault == (int)Working.Close)
                {
                    itemFound.IsComplete = true;
                    itemFound.dtEndWarranty = DateTime.Now.AddDays(30);
                }

                //itemFound.IsInsurance = repair_sending.IsInsurance;
                //itemFound.IsFree = repair_sending.IsFree;
                //itemFound.IsNormal = repair_sending.IsNormal;
                //itemFound.IsBack = repair_sending.IsBack;
                //itemFound.IsCancle = repair_sending.IsCancle;
                //itemFound.IsNoCredit = repair_sending.IsNoCredit;
                //itemFound.IsQCPass = repair_sending.IsQCPass;
                //itemFound.IsCustomerRecieved = repair_sending.IsCustomerRecieved;

                //assign repair
                if (isStatusChange)
                {
                    itemFound.kStaffId = status_sending.kStaffId;
                }
                itemFound.dtDateUpdate = DateTime.Now;
                RepairManager.Edit(itemFound);

                #region update claim side
                Claim claim = ClaimManager.GetByRepairNo(itemFound.sRepairNo);
                if (claim != null)
                {
                    ClaimStatus claimStatus = new ClaimStatus
                    {
                        kStaffId = status_sending.kStaffId,
                        kClaimId = claim.kClaimId,
                        kWorkingStatusId = status_sending.kWorkingStatusId,
                        kClaimStatusId = Guid.NewGuid()
                    };
                    ClaimStatusManager.Create(claimStatus);

                    if (itemFound.IsComplete == true)
                    {
                        claim.IsComplete = true;
                    }
                    if (isStatusChange)
                    {
                        claim.kStaffId = status_sending.kStaffId;
                    }
                    ClaimManager.Edit(claim);
                }
                #endregion

                if (isStatusChange)
                {
                    //create remind for history
                    RemindHistory remind = new RemindHistory
                    {
                        sRemind = itemFound.vMessage,
                        kStaffId = status_sending.kStaffId
                    };
                    RemindHistoryManager.Create(remind);

                    var clientName = User.Identity.Name;
                    Task.Factory.StartNew(() =>
                    {
                        var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                        clients.RecordUpdated(clientName, itemFound);
                    });
                }

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        #endregion

        #region FR Role
        [HttpPost]
        public JsonResult EditFRRepair(Repair repair_sending, RepairStatus status_sending, FormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Repair itemFound = RepairManager.GetById(repair_sending.kRepairId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                if (itemFound.kStaffId != (Guid)Membership.GetUser().ProviderUserKey)
                {
                    //return Json(new { Result = "ERROR", Message = "No permission for this job!" });
                }
                if (itemFound.IsComplete == true)
                {
                    return Json(new { Result = "ERROR", Message = "ปิด job!" });
                }

                //chack status
                var status = WorkingStatusManager.GetById(status_sending.kWorkingStatusId);
                if (status.iDefault == (int)Working.Default)
                {
                    return Json(new { Result = "ERROR", Message = "เลือกสถานะงาน!" });
                }
                if (status.iDefault == (int)Working.Close && repair_sending.IsCustomerRecieved != true)
                {
                    return Json(new { Result = "ERROR", Message = "ตรวจสอบลูกค้ารับสินค้า!" });
                }

                bool isStatusChange = false;
                if (status_sending.kWorkingStatusId != itemFound.kWorkingStatusId)
                {
                    if (repair_sending.IsCustomerRecieved == true)
                    {
                        Guid closeStaffId;
                        if (Guid.TryParse(collection["kCloseStaffId"], out closeStaffId))
                        {
                            if (closeStaffId == Guid.Empty)
                            {
                                return Json(new { Result = "ERROR", Message = "เลือกผลงานช่าง!" });
                            }
                            else
                            {
                                //close job
                                itemFound.IsComplete = true;
                                status_sending.kStaffId = closeStaffId;
                            }
                        }
                    }
                    else
                    {
                        status_sending.kStaffId = itemFound.kOwnerId.Value;
                    }
                    //assign status
                    status_sending.dtDateAdd = DateTime.Now;
                    status_sending.kRepairStatusId = Guid.NewGuid();
                    RepairStatusManager.Create(status_sending);
                    isStatusChange = true;
                }

                //assign repair
                if (isStatusChange)
                {
                    itemFound.kStaffId = status_sending.kStaffId;
                }
                int closeStatusId;
                if (int.TryParse(collection["vCloseStatusId"], out closeStatusId))
                {
                    if (closeStatusId > 0)
                    {
                        itemFound.kCloseStatusId = CloseStatusManager.GetAll().Where(m => m.iOrder == closeStatusId).SingleOrDefault().kCloseStatusId;
                    }
                }

                itemFound.IsNoCredit = repair_sending.IsNoCredit;
                itemFound.IsCustomerRecieved = repair_sending.IsCustomerRecieved;
                itemFound.dtDateUpdate = DateTime.Now;
                RepairManager.Edit(itemFound);


                //เมื่อ cancle ยังไม่อัพเดตฝั่ง claim
                #region update claim side
                Claim claim = ClaimManager.GetByRepairNo(itemFound.sRepairNo);
                if (claim != null)
                {
                    ClaimStatus claimStatus = new ClaimStatus
                    {
                        kStaffId = itemFound.kStaffId.Value,
                        kClaimId = claim.kClaimId,
                        kWorkingStatusId = status_sending.kWorkingStatusId,
                        kClaimStatusId = Guid.NewGuid()
                    };
                    ClaimStatusManager.Create(claimStatus);

                    if (itemFound.IsComplete == true)
                    {
                        claim.IsComplete = true;
                    }
                    if (isStatusChange)
                    {
                        claim.kStaffId = status_sending.kStaffId;
                    }
                    ClaimManager.Edit(claim);
                }
                #endregion

                if (isStatusChange)
                {
                    //create remind for history
                    RemindHistory remind = new RemindHistory
                    {
                        sRemind = itemFound.vMessage,
                        kStaffId = status_sending.kStaffId
                    };
                    RemindHistoryManager.Create(remind);

                    var clientName = User.Identity.Name;
                    Task.Factory.StartNew(() =>
                    {
                        var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                        clients.RecordUpdated(clientName, itemFound);
                    });
                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        #endregion

        #region QC Role

        [HttpPost]
        public JsonResult EditQCRepair(Repair repair_sending, RepairStatus status_sending, FormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Repair itemFound = RepairManager.GetById(repair_sending.kRepairId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                if (itemFound.IsComplete == true)
                {
                    return Json(new { Result = "ERROR", Message = "ปิด job!" });
                }

                //chack status
                var status = WorkingStatusManager.GetById(status_sending.kWorkingStatusId);
                if (status.iDefault == (int)Working.Default)
                {
                    return Json(new { Result = "ERROR", Message = "เลือกสถานะงาน!" });
                }

                bool isStatusChange = false;
                if (status_sending.kWorkingStatusId != itemFound.kWorkingStatusId)
                {
                    //assign status
                    if (status.iDefault == (int)Working.Repair)
                    {
                        status_sending.kStaffId = itemFound.kOwnerId.Value;
                    }
                    else if (status.iDefault == (int)Working.Remind)
                    {
                        status_sending.kStaffId = itemFound.kBookingId.Value;
                    }
                    else if (status.iDefault == (int)Working.RepairCheck)
                    {
                        status_sending.kStaffId = itemFound.kBookingId.Value;
                    }
                    status_sending.dtDateAdd = DateTime.Now;
                    status_sending.kRepairStatusId = Guid.NewGuid();
                    RepairStatusManager.Create(status_sending);
                    isStatusChange = true;
                }


                //assign repair
                itemFound.dtDateUpdate = DateTime.Now;
                itemFound.IsQCPass = repair_sending.IsQCPass;
                if (itemFound.IsQCPass == true)
                {
                    itemFound.kQCId = (Guid)Membership.GetUser().ProviderUserKey;
                }
                if (isStatusChange)
                {
                    itemFound.kStaffId = status_sending.kStaffId;
                }
                RepairManager.Edit(itemFound);


                //เมื่อ cancle ยังไม่อัพเดตฝั่ง claim
                #region update claim side
                Claim claim = ClaimManager.GetByRepairNo(itemFound.sRepairNo);
                if (claim != null)
                {
                    ClaimStatus claimStatus = new ClaimStatus
                    {
                        kStaffId = status_sending.kStaffId,
                        kClaimId = claim.kClaimId,
                        kWorkingStatusId = status_sending.kWorkingStatusId,
                        kClaimStatusId = Guid.NewGuid()
                    };
                    ClaimStatusManager.Create(claimStatus);

                    if (itemFound.IsComplete == true)
                    {
                        claim.IsComplete = true;
                    }
                    if (isStatusChange)
                    {
                        claim.kStaffId = status_sending.kStaffId;
                    }
                    ClaimManager.Edit(claim);
                }
                #endregion

                if (isStatusChange)
                {
                    //create remind for history
                    RemindHistory remind = new RemindHistory
                    {
                        sRemind = itemFound.vMessage,
                        kStaffId = status_sending.kStaffId
                    };
                    RemindHistoryManager.Create(remind);

                    var clientName = User.Identity.Name;
                    Task.Factory.StartNew(() =>
                    {
                        var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                        clients.RecordUpdated(clientName, itemFound);
                    });
                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        #endregion

        #region Techical Role
        public JsonResult EditTCRepair(Repair repair_sending, RepairStatus status_sending, FormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Repair itemFound = RepairManager.GetById(repair_sending.kRepairId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                if (itemFound.kStaffId != (Guid)Membership.GetUser().ProviderUserKey)
                {
                    return Json(new { Result = "ERROR", Message = "No permission for this job!" });
                }
                if (itemFound.IsComplete == true)
                {
                    return Json(new { Result = "ERROR", Message = "ปิด job!" });
                }

                //chack status
                var status = WorkingStatusManager.GetById(status_sending.kWorkingStatusId);
                if (status.iDefault == (int)Working.Default)
                {
                    return Json(new { Result = "ERROR", Message = "เลือกสถานะงาน!" });
                }

                bool isStatusChange = false;
                if (status_sending.kWorkingStatusId != itemFound.kWorkingStatusId)
                {
                    //assign status
                    status_sending.dtDateAdd = DateTime.Now;
                    status_sending.kRepairStatusId = Guid.NewGuid();
                    if (status.iDefault == (int)Working.RemindCause)
                    {
                        status_sending.kStaffId = itemFound.kBookingId.Value;
                    }
                    else
                    {
                        status_sending.kStaffId = itemFound.kOwnerId.Value;
                    }
                    RepairStatusManager.Create(status_sending);
                    isStatusChange = true;
                }

                //assign repair
                if (isStatusChange)
                {
                    itemFound.kStaffId = status_sending.kStaffId;
                }
                itemFound.dtDateUpdate = DateTime.Now;
                RepairManager.Edit(itemFound);

                #region update claim side
                Claim claim = ClaimManager.GetByRepairNo(itemFound.sRepairNo);
                if (claim != null)
                {
                    ClaimStatus claimStatus = new ClaimStatus
                    {
                        kStaffId = itemFound.kOwnerId.Value,
                        kClaimId = claim.kClaimId,
                        kWorkingStatusId = status_sending.kWorkingStatusId,
                        kClaimStatusId = Guid.NewGuid()
                    };
                    ClaimStatusManager.Create(claimStatus);

                    if (isStatusChange)
                    {
                        claim.kStaffId = status_sending.kStaffId;
                    }
                    ClaimManager.Edit(claim);
                }
                #endregion

                if (isStatusChange)
                {
                    //create remind for history
                    RemindHistory remind = new RemindHistory
                    {
                        sRemind = itemFound.vMessage,
                        kStaffId = status_sending.kStaffId
                    };
                    RemindHistoryManager.Create(remind);

                    var clientName = User.Identity.Name;
                    Task.Factory.StartNew(() =>
                    {
                        var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                        clients.RecordUpdated(clientName, itemFound);
                    });
                }

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        #endregion



    }
}
