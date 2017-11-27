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
    public class ClaimController : Controller
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
            ViewBag.ClientName = User.Identity.Name;
            ViewBag.ClaimList = "first active";

            BookingClaimViewModel model = new BookingClaimViewModel();
            ViewBag.WorkingStatus = new SelectList(WorkingStatusManager.GetAll().Where(m=>m.iDefault>0), "kWorkingStatusId", "sDescription");


            if (!User.IsInRole("Admin"))
            {
                if (Staff.vStaffPositionDescription == "ฝ่ายเครม")
                    return View("ListsCR", model);
                else
                    return View("ListsUser", model);
            }

            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.ClaimCreate = "first active";
            ViewBag.ProductTypes = ProductTypeManager.GetAll();
            ViewBag.Brands = BrandManager.GetAll();

            BookingClaimViewModel model = new BookingClaimViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(BookingClaimViewModel model, FormCollection collection)
        {
            try
            {
                //initial working status
                var workingStatusId = WorkingStatusManager.GetAll().Where(m => m.iDefault == (int)Working.Claiming).SingleOrDefault();

                if (model.Product != null)
                    model.Product.Brand = BrandManager.GetById(model.Product.kBrandId);
                    model.Product.ProductType = ProductTypeManager.GetById(model.Product.kProductTypeId);

                #region SAVE CUSTOMER
                var insCount = InsuranceManager.GetCountDuplicate(model.Insurance.sInsuranceName.Trim());
                if (insCount.Count <= 0)
                {
                    Insurance insurance = new Insurance
                    {
                        kInsuranceId = Guid.NewGuid(),
                        sInsuranceName = model.Insurance.sInsuranceName,
                        sAddress1 = model.Insurance.sAddress1,
                        sCity = model.Insurance.sCity,
                        sZip = model.Insurance.sZip,
                        sPhone = model.Insurance.sPhone,
                        sMobile = model.Insurance.sMobile,
                        sFax = model.Insurance.sFax,
                        sEmailAddress = model.Insurance.sEmailAddress
                    };
                    InsuranceManager.Create(insurance);
                    model.Claim.Insurance = insurance;
                }
                else
                {
                    model.Claim.Insurance = insCount[0];
                }
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
                        kProductTypeId = productTypeCount.SingleOrDefault().kProductTypeId,
                        kBrandId = brandCount.SingleOrDefault().kBrandId,
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

                #region SAVE CLAIM BOOKING
                var repair = RepairManager.GetByRepairNo(model.Claim.sRepairNo ?? "");

                //initial booking first character
                string char_number = CharBooking.C.ToString();

                Claim claim = new Claim();
                claim.dtDateUpdate = DateTime.Now;
                claim.kClaimId = Guid.NewGuid();
                claim.kStaffId = (Guid)Membership.GetUser().ProviderUserKey;
                claim.kOwnerId = repair.kOwnerId ?? (Guid)Membership.GetUser().ProviderUserKey;
                claim.kInsuranceId = model.Claim.Insurance.kInsuranceId;
                claim.kProductId = model.Product.kProductId;
                claim.sRepairNo = model.Claim.sRepairNo;
                claim.sClaimNo = String.Format("{0}{1}", char_number, ClaimNextItemNoManager.GetNextItemNo());
                claim.sSerial = model.Claim.sSerial;
                if (model.Claim.dtInsuranceExpire != null)
                    claim.dtInsuranceExpire = model.Claim.dtInsuranceExpire.Value.AddYears(-543);

               
                model.Claim = claim;
                ClaimManager.Create(model.Claim);
                ClaimNextItemNoManager.IncreaseNextItemNo();

                #region also pdate repair side
                
                repair.dtDateUpdate = DateTime.Now;
                repair.kStaffId = (Guid)Membership.GetUser().ProviderUserKey;
                RepairManager.Edit(repair);

                #endregion

                #endregion

                #region SAVE REPAIR WORKING STATUS

                ClaimStatus claimStatus = new ClaimStatus
                {
                    kClaimStatusId = Guid.NewGuid(),
                    kWorkingStatusId = workingStatusId.kWorkingStatusId,
                    kStaffId = (Guid)Membership.GetUser().ProviderUserKey,
                    kClaimId = model.Claim.kClaimId
                };
                ClaimStatusManager.Create(claimStatus);

                #endregion

                #region UPDATE REPAIR WORKING STATUS
                if (!String.IsNullOrEmpty(model.Claim.sRepairNo))
                {
                    repair = RepairManager.GetByRepairNo(model.Claim.sRepairNo);

                    RepairStatus repairStatus = new RepairStatus
                    {
                        kRepairStatusId = Guid.NewGuid(),
                        kWorkingStatusId = workingStatusId.kWorkingStatusId,
                        kStaffId = (Guid)Membership.GetUser().ProviderUserKey,
                        kRepairId = repair.kRepairId
                    };
                    RepairStatusManager.Create(repairStatus);
                }
                #endregion

             

                #region SAVE CAUSE OF CLAIM

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
                    model.ClaimCauses = new List<ClaimCause>();
                    for (int i = 1; i <= countvalue / 4; i++)
                    {
                        ClaimCause claimCause = new ClaimCause();
                        claimCause.kClaimCauseId = Guid.NewGuid();
                        claimCause.kClaimId = model.Claim.kClaimId;
                        claimCause.kStaffId = (Guid)Membership.GetUser().ProviderUserKey;
                        claimCause.sDescription = collection[String.Format("cause_description_{0}", i)];
                        claimCause.sNote = collection[String.Format("cause_note_{0}", i)];

                        if (!String.IsNullOrEmpty(claimCause.sDescription))
                        {
                            int qty = 0;
                            if (int.TryParse(collection[String.Format("cause_qty_{0}", i)], out qty))
                            {
                                claimCause.iQty = qty;
                            }

                            Decimal price = 0;
                            if (Decimal.TryParse(collection[String.Format("cause_price_{0}", i)], out price))
                            {
                                claimCause.dPrice = price;
                            }

                            model.ClaimCauses.Add(claimCause);
                            ClaimCourseManager.Create(claimCause);
                        }
                    }
                }

                if (model.ClaimCauses != null)
                {
                    foreach (var item in model.ClaimCauses)
                    {
                        var causeCount = CauseManager.GetCountDuplicate(item.sDescription.Trim());
                        if (causeCount <= 0)
                        {
                            Cause cause = new Cause
                            {
                                kCauseId = Guid.NewGuid(),
                                sDescription = item.sDescription.Trim()
                            };
                            CauseManager.Create(cause);
                        }
                    }
                }
                #endregion


            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }

            Claim model2 = ClaimManager.GetById(model.Claim.kClaimId);

            //create remind for history
            RemindHistory remind = new RemindHistory
            {
                sRemind = model2.vMessage,
                kStaffId = model2.kStaffId
            };
            RemindHistoryManager.Create(remind);

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
            ViewBag.ClaimCreate = "first active";
            ViewBag.SuperUser = StaffManager.GetAll().Where(m => m.vStaffPositionDescription == "หัวหน้าช่าง");
            ViewBag.ProductTypes = ProductTypeManager.GetAll();
            ViewBag.Brands = BrandManager.GetAll();

            BookingClaimViewModel model = new BookingClaimViewModel();
            var claim = ClaimManager.GetById(id);
            if (claim.dtInsuranceExpire.HasValue)
                claim.dtInsuranceExpire = claim.dtInsuranceExpire.Value.AddYears(543);



            model.Claim = claim;
            //model.Repair.RepairCauses = repair.RepairCauses;
            model.Insurance = claim.Insurance;
            model.Product = claim.Product;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit2(BookingClaimViewModel model, FormCollection collection)
        {
            try
            {
                if (model.Claim != null)
                {
                    //initial value
                    var item_found = ClaimManager.GetById(model.Claim.kClaimId);

                    #region SAVE EDIT CLAIM BOOKING
                    //Insurance
                    model.Insurance.kInsuranceId = item_found.Insurance.kInsuranceId;
                    InsuranceManager.Edit(model.Insurance);

                    //Product
                    model.Product.kProductId = item_found.Product.kProductId;
                    ProductManager.Edit(model.Product);

                    //Claim
                    model.Claim.kClaimId = item_found.kClaimId;
                    model.Claim.kStaffId = item_found.kStaffId;
                    model.Claim.kOwnerId = item_found.kOwnerId;
                    model.Claim.sClaimNo = item_found.sClaimNo;
                    model.Claim.IsComplete = item_found.IsComplete;
                    model.Claim.IsRecieved = item_found.IsRecieved;
                    model.Claim.IsNoCredit = item_found.IsNoCredit;

                    model.Claim.kInsuranceId = model.Insurance.kInsuranceId;
                    model.Claim.kProductId = model.Product.kProductId;
                    model.Claim.dtInsuranceExpire = model.Claim.dtInsuranceExpire.Value.AddYears(-543);

                    model.Claim.dtDateAdd = item_found.dtDateAdd;
                    model.Claim.dtDateUpdate = item_found.dtDateUpdate;
                    ClaimManager.Edit(model.Claim);


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
        public ActionResult Confirm(BookingClaimViewModel model, FormCollection collection)
        {
            // initial a reference number //
          
            model.Claim.sClaimNo = String.Format("{0}{1}", CharBooking.C.ToString(), ClaimNextItemNoManager.GetNextItemNo());

            if (model.Product != null)
                model.Product.Brand = BrandManager.GetById(model.Product.kBrandId);
                model.Product.ProductType = ProductTypeManager.GetById(model.Product.kProductTypeId);

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
                model.ClaimCauses = new List<ClaimCause>();
                for (int i = 1; i <= countvalue / 4; i++)
                {
                    ClaimCause claimCause = new ClaimCause();
                    claimCause.kClaimCauseId = Guid.NewGuid();
                    claimCause.sDescription = collection[String.Format("cause_description_{0}", i)];
                    claimCause.sNote = collection[String.Format("cause_note_{0}", i)];

                    if (!String.IsNullOrEmpty(claimCause.sDescription))
                    {
                        int qty = 0;
                        if (int.TryParse(collection[String.Format("cause_qty_{0}", i)], out qty))
                        {
                            claimCause.iQty = qty;
                        }

                        Decimal price = 0;
                        if (Decimal.TryParse(collection[String.Format("cause_price_{0}", i)], out price))
                        {
                            claimCause.dPrice = price;
                        }
                        model.ClaimCauses.Add(claimCause);
                    }
                }
            }

            this.HttpContext.Session["BookingViewModel"] = model;

            return PartialView("Confirm", model);
        }

        [HttpPost]
        public JsonResult GetAll()
        {
            try
            {
                var items = ClaimManager.GetAll();

                return Json(new { Result = "OK", Options = items.Select(m => new { DisplayText = m.sClaimNo, Value = m.kClaimId }) });
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
                var items = ClaimManager.GetBySearch(term);

                return Json(items.Select(m => m.sClaimNo + " - " + m.Product.sProductName), JsonRequestBehavior.AllowGet);
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
                var items = ClaimManager.GetBySearch(term);

                return Json(items.Select(m => new
                {
                    label = String.Format("{0}", m.sClaimNo),
                    description = String.Format("เลขอ้างอิง {0} สินค้า {1}", m.sClaimNo, m.Product.sProductName)
                }), JsonRequestBehavior.AllowGet);
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
                var itemCount = ClaimManager.GetCountByFiltering(jtSearching,jtStatus);
                var items = ClaimManager.GetByFilterings(jtSearching, jtStartIndex, jtPageSize, jtSorting, jtStatus).ToList();

                if (User.IsInRole("User"))
                {
                    if (Staff.StaffPosition.sDescription == "ช่าง")
                    {
                        int totalSize = 0;
                        items = ClaimManager.GetByFilteringsForStaff(jtSearching, jtStartIndex, jtPageSize, jtSorting, jtStatus, (Guid)Membership.GetUser().ProviderUserKey,out totalSize).ToList();
                        itemCount = totalSize;
                    }
                }
                //else if (User.IsInRole("SuperUser"))
                //{
                //    //items = items.Where(m => m.kOwnerId == (Guid)Membership.GetUser().ProviderUserKey).ToList();
                //    //itemCount = items.Count;
                //}

                //if (!string.IsNullOrEmpty(jtStatus))
                //{
                //    //items = items.Where(m => m.kWorkingStatusId.ToString() == jtStatus.ToString()).ToList();
                //    itemCount = items.Count;
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
                Claim itemFound = ClaimManager.GetById(Id);

                return Json(itemFound, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //pass insurance id
        public JsonResult GetByInsurance(Guid Id)
        {
            try
            {
                var itemFounds = ClaimManager.GetByInsurance(Id);
                return Json(new { Result = "OK", Records = itemFounds }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Edit(Claim model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Claim itemFound = ClaimManager.GetById(model.kClaimId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                if (itemFound.IsComplete == true)
                {
                    return Json(new { Result = "ERROR", Message = "ปิด job!" });
                }

                model.dtDateUpdate = DateTime.Now;
                ClaimManager.Edit(model);


                var clientName = User.Identity.Name;
                Task.Factory.StartNew(() =>
                {
                    var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                    clients.RecordUpdated(clientName, model);
                });

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EditClaim(Claim claim_sending, ClaimStatus status_sending)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Repair repair = null;
                Claim itemFound = ClaimManager.GetById(claim_sending.kClaimId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                if (itemFound.IsComplete == true)
                {
                    return Json(new { Result = "ERROR", Message = "ปิด job!" });
                }

                var status = WorkingStatusManager.GetById(status_sending.kWorkingStatusId);
                if (status.iDefault == (int)Working.ConfirmRepair && claim_sending.IsRecieved != true)
                {
                    return Json(new { Result = "ERROR", Message = "ตรวจรับสินค้า!" });
                }


                bool isStatusChange = false;
                if (status_sending.kWorkingStatusId != itemFound.kWorkingStatusId)
                {
                    #region update claim status side
                    if (status.iDefault == (int)Working.ConfirmRepair)
                    {
                        status_sending.kStaffId = itemFound.kOwnerId.Value;
                    }
                    else
                    {
                        status_sending.kStaffId = (Guid)Membership.GetUser().ProviderUserKey;
                    }
                    status_sending.dtDateAdd = DateTime.Now;
                    status_sending.kClaimStatusId = Guid.NewGuid();
                    ClaimStatusManager.Create(status_sending);

                    #endregion

                    #region also update repair status side
                    if (!String.IsNullOrEmpty(itemFound.sRepairNo))
                    {
                        repair = RepairManager.GetByRepairNo(itemFound.sRepairNo);
                        RepairStatus repairStatus = new RepairStatus
                        {
                            kStaffId = itemFound.kOwnerId.Value,
                            kRepairId = repair.kRepairId,
                            kWorkingStatusId = status_sending.kWorkingStatusId,
                            kRepairStatusId = Guid.NewGuid()
                        };
                        RepairStatusManager.Create(repairStatus);
                    }
                    #endregion

                    isStatusChange = true;
                }


                #region update claim
                //assign claim
                itemFound.dtDateUpdate = DateTime.Now;
                itemFound.IsRecieved = claim_sending.IsRecieved;
                itemFound.IsNoCredit = claim_sending.IsNoCredit;
                if (isStatusChange)
                {
                    itemFound.kStaffId = itemFound.kOwnerId.Value;
                }
                ClaimManager.Edit(itemFound);
                #endregion

                #region update repair
                if (isStatusChange)
                {
                    if(repair!=null)
                        repair.kStaffId = status_sending.kStaffId;
                        RepairManager.Edit(repair);
                }
                #endregion

                if (isStatusChange)
                {
                    //create remind for history
                    RemindHistory remind = new RemindHistory
                    {
                        sRemind = itemFound.vMessage,
                        kStaffId = itemFound.kOwnerId.Value
                    };
                    RemindHistoryManager.Create(remind);

                    var clientName = User.Identity.Name;
                    Task.Factory.StartNew(() =>
                    {
                        var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                        clients.RecordUpdated(clientName, itemFound);
                    });
                }

                return Json(new { Result = "OK", Records = itemFound });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult Delete(Guid kClaimId)
        {
            try
            {
                //Thread.Sleep(50);
                Claim itemFound = ClaimManager.GetById(kClaimId);
                ClaimManager.Delete(kClaimId);

                //create remind for history
                RemindHistory remind = new RemindHistory
                {
                    sRemind = string.Format("ลบ {0}", itemFound.sClaimNo),
                    kStaffId = (Guid)Membership.GetUser().ProviderUserKey
                };
                RemindHistoryManager.Create(remind);

                var clientName = User.Identity.Name;
                Task.Factory.StartNew(() =>
                {
                    var clients = Hub.GetClients<RealTimeJTableDemoHub>();
                    clients.RecordDeleted(clientName, itemFound.sClaimNo);
                });
                
                
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult Summary(Guid Id)
        {
            var itemFounds = ClaimManager.GetClaim(Id);

            BookingClaimViewModel model = new BookingClaimViewModel();

            if (itemFounds != null)
            {
                model.Claim = itemFounds[0];
                model.Insurance = itemFounds[0].Insurance;
                model.Product = itemFounds[0].Product;

                if (itemFounds[0].ClaimCauses.Count > 0)
                {
                    model.ClaimCauses = new List<ClaimCause>();
                    for (int i = 0; i < itemFounds[0].ClaimCauses.Count; i++)
                    {
                        ClaimCause cause = new ClaimCause
                        {
                            sDescription = itemFounds[0].ClaimCauses[i].sDescription,
                            iQty = itemFounds[0].ClaimCauses[i].iQty,
                            sNote = itemFounds[0].ClaimCauses[i].sNote
                        };
                        model.ClaimCauses.Add(cause);
                    }
                }

            }

            this.HttpContext.Session["BookingViewModel"] = model;
            return View(model);
            //return PartialView( model);
        }
      

        #region Claim Cause
        public JsonResult GetClaimCause(Guid Id)
        {
            try
            {
                var itemFounds = ClaimManager.GetClaimCause(Id);
                return Json(new { Result = "OK", Records = itemFounds });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult CreateClaimCause(ClaimCause model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                Claim itemFound = ClaimManager.GetById(model.kClaimId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                if (itemFound.IsComplete == true)
                {
                    return Json(new { Result = "ERROR", Message = "ปิด job!" });
                }


                model.kClaimCauseId = Guid.NewGuid();
                model.kStaffId = (Guid)Membership.GetUser().ProviderUserKey;
                ClaimCourseManager.Create(model);

                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult EditClaimCause(ClaimCause model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Claim itemFound = ClaimManager.GetById(model.kClaimId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                if (itemFound.IsComplete == true)
                {
                    return Json(new { Result = "ERROR", Message = "ปิด job!" });
                }

                model.kStaffId = (Guid)Membership.GetUser().ProviderUserKey;
                ClaimCourseManager.Edit(model);

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult DeleteClaimCause(ClaimCause model)
        {
            try
            {
                Claim itemFound = ClaimManager.GetById(model.kClaimId);
                if (itemFound == null)
                {
                    return Json(new { Result = "ERROR", Message = "Item Not Found" });
                }
                if (itemFound.IsComplete == true)
                {
                    return Json(new { Result = "ERROR", Message = "ปิด job!" });
                }

                ClaimCourseManager.Delete(model.kClaimCauseId);

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        #endregion

        public JsonResult GetClaim(Guid Id)
        {
            try
            {
                var itemFounds = ClaimManager.GetClaim(Id);
                return Json(new { Result = "OK", Records = itemFounds });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult GetClaimStatus(Guid Id)
        {
            try
            {
                var itemFounds = ClaimManager.GetClaimStatus(Id);
                return Json(new { Result = "OK", Records = itemFounds });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

    }
}
