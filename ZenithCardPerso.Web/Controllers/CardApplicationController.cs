using Hangfire;
using Ionic.Zip;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ZenithCardPerso.Web.Filters;
using ZenithCardPerso.Web.Models;
using ZenithCardRepo.Data.DTOs;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;
using ZenithCardRepo.Services.BLL.Command;
using ZenithCardRepo.Services.BLL.Infrastructure;
using ZenithCardRepo.Services.BLL.Query;

namespace ZenithCardPerso.Web.Controllers
{
    public class CardApplicationController : Controller
    {
        private ICardApplicationCmdBLL _cardAPPCmdBLL;
        private IGetApplicationLegends _applicationLegends;
        private ICardApplicationQueryBLL _cardAppQueryBLL;
        private IOrganisationQueryBLL _orgQueryBLL;
        private ILog _log;
        private IApprovalCMDBLL _approvalCMDBLL;
        private IApprovalQueryBLL _approvalQueryBLL;
        private string _institution;
        public CardApplicationController(
            ICardApplicationCmdBLL cardAPPCmdBLL,
            IGetApplicationLegends applicationLegends,
            ICardApplicationQueryBLL cardAppQueryBLL,
            IOrganisationQueryBLL orgQueryBLL,
            ILog log,
            IApprovalCMDBLL approvalCMDBLL,
            IApprovalQueryBLL approvalQueryBLL
            )
        {
            _cardAPPCmdBLL = cardAPPCmdBLL;
            _applicationLegends = applicationLegends;
            _cardAppQueryBLL = cardAppQueryBLL;
            _orgQueryBLL = orgQueryBLL;
            _log = log;
            _approvalCMDBLL = approvalCMDBLL;
            _approvalQueryBLL = approvalQueryBLL;
        }

        [Audit]
        [ValidateUserPermission(Permissions = "can_create_cardapplication")]
        public ActionResult CardApplicationCreate()
        {
            var institution = User.Identity.GetInstitutionID();
            if (institution == "" || institution == "0  ")
            {
                return RedirectToAction("Login", "Account");
            }
            
            _institution = institution;
            LoadApplicationLegends(_institution);

            TempData[Utilities.Activity_Log_Details] = "Card Application view was loaded";
            return View();
        }

        public JsonResult ValidateImage(string HDImageByte)
        {
            try
            {
                var valMsgs = _cardAPPCmdBLL.ImageBase64String(HDImageByte);

                return Json(valMsgs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        [Audit(AuditingLevel = 2)]
        [HttpPost]
        [ValidateUserPermission(Permissions = "can_create_cardapplication")]
        public ActionResult CardApplicationCreate(CardApplicationsDTO cardApplication, string HDImageByte, string IDIssueDate, string IDExpiryDate, string DateofBirth)
        {
            string instID = string.Empty;
            try
            {
                instID = User.Identity.GetInstitutionID();
                if (instID == "" || instID == "0  ")
                {
                    return RedirectToAction("Login", "Account");
                }

                if (string.IsNullOrEmpty(HDImageByte))
                {
                    ModelState.AddModelError("", "Image is required");
                    LoadApplicationLegends(instID);
                    return View(cardApplication);
                }
                cardApplication.InstitutionID = Convert.ToInt32(instID);
                cardApplication.CreatedBy = User.Identity.Name;

                var m = ModelState.Where(x => x.Value.Errors.Count > 0);

                if (ModelState.IsValid)
                {
                    var instCode = _orgQueryBLL.GetInstitutionCode(instID);
                    var saveLocation = Server.MapPath("~/images/CardApplication/");

                    _cardAPPCmdBLL.AddCardApplication(cardApplication, HDImageByte, saveLocation, instCode);

                    TempData[Utilities.Activity_Log_Details] = "Card Application has been captured Successfully";

                    TempData["Message"] = "Success";

                    BackgroundJob.Enqueue(() => Utilities.ExecuteEmail("raphkens@live.com", "Ekene Egonu", CardEnums.CardApplicationEmail));
                    

                    ModelState.Clear();

                    if (instID == "" || instID == "0  ")
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    var departments = _orgQueryBLL.GetDepartments(instID);
                    ViewBag.Department = new SelectList(departments, "Code", "Name", "");
                    LoadApplicationLegends(instID);
                    return View();
                }

            }
            catch (Exception ex)
            {
                LoadApplicationLegends(instID);
                _log.Error(ex);
            }

            LoadApplicationLegends(instID);
            return View(cardApplication);
        }

        public ActionResult CardApplicationUpload()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
            return View();

        }
        public void LoadApplicationLegends(string institution)
        {
            ViewData["MaritalStatusView"] = new SelectList(_applicationLegends.MaritalStatusList(), "Code", "Description", "");
            ViewData["StateView"] = new SelectList(_applicationLegends.StateList(), "StateCode", "RegionName", "");
            ViewData["SocioProfCodeView"] = new SelectList(_applicationLegends.SocioProfCodeList(), "Code", "Description", "");
            ViewData["NationalityView"] = new SelectList(_applicationLegends.NationalityCodeList(), "CountryCode", "Wording", "");
            ViewData["TitleCodeView"] = new SelectList(_applicationLegends.TitleCodeList(), "Code", "Wording", "");
            ViewData["ProductCodeView"] = new SelectList(_applicationLegends.ProductCodeList(), "Code", "Description", "");
            ViewData["IDCardTypeView"] = new SelectList(_applicationLegends.IDCardTypeList(), "DocumentCode", "ABRVWording", "");
            ViewData["SexView"] = new SelectList(_applicationLegends.SexList(), "Code", "Description", "");

            var departments = _orgQueryBLL.GetDepartments(institution);
            ViewBag.DepartmentView = new SelectList(departments, "Code", "Name", "");

        }

        public JsonResult GetCity(string stateCode)
        {
            try
            {
                var cityList = _applicationLegends.CityList(stateCode);
                return Json(cityList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }
        }

        [Audit]
        [ValidateUserPermission(Permissions = "can_view_cardapplications")]
        public ActionResult CardApplications()
        {
            try
            {
                CardSearchOptions();

                var cardAppls = _cardAppQueryBLL.GetCardApplications().ToList();
                //var cardApps  = new CardAppViewModel { CardApplicationsDTOs = cardAppls };
                ViewBag.CardApplications = cardAppls;

                //Verify if download is approved 
                ViewData["IsDownloadRequired"] = _cardAppQueryBLL.CheckProcessedStatus(cardAppls);
                TempData[Utilities.Activity_Log_Details] = "Card Applications was loaded";

                return View();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("","Card applications could not be retrieved");
            CardSearchOptions();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Audit]
        [ValidateUserPermission(Permissions = "can_view_cardapplications")]
        public async Task<ActionResult> CardApplicationsSearch(CardAppViewModel cardAppVM)
        {
            try
            {
                var cardApplications = await _cardAppQueryBLL.CardApplicationSearch(cardAppVM);
                var cardApps = cardApplications.ToList();
                ViewBag.CardApplications = cardApps;
                CardSearchOptions();

                //Verify if download is approved 
                ViewData["IsDownloadRequired"] = _cardAppQueryBLL.CheckProcessedStatus(cardApps);

                TempData[Utilities.Activity_Log_Details] = "Search was carried out on Card Applications";
                return View("CardApplications",cardAppVM);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                
            }

            CardSearchOptions();
            ModelState.AddModelError("", "Card applications could not be retrieved");
            return View(cardAppVM);
        }

        [Audit]
        [ValidateUserPermission(Permissions = "can_view_cardapplications")]
        public ActionResult CheckSearch()
        {
            try
            {
                CardSearchOptions();

                var cardAppls = _cardAppQueryBLL.GetCardApplications().ToList();
                //var cardApps  = new CardAppViewModel { CardApplicationsDTOs = cardAppls };
                ViewBag.CardApplications = cardAppls;

                //Verify if download is approved 
                ViewData["IsDownloadRequired"] = _cardAppQueryBLL.CheckProcessedStatus(cardAppls);
                TempData[Utilities.Activity_Log_Details] = "Card Applications was loaded";

                return View();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Card applications could not be retrieved");
            CardSearchOptions();
            return View();
        }

        public ActionResult SearchResult()
        {
            return PartialView();
        }

        private void CardSearchOptions()
        {
            Dictionary<string, string> isProcessed = new Dictionary<string, string>();
            isProcessed.Add("Yes", "true");
            isProcessed.Add("No", "false");

            ViewData["IsProcessed"] = new SelectList(isProcessed, "Value", "Key", "");

            var institutions = _orgQueryBLL.GetInstitutions();
            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        [ValidateUserPermission(Permissions = "can_delete_cardapplication")]
        public ActionResult DeleteCardApplications(List<CardApplicationsDTO> cardApps)
        {
            try
            {
                _cardAPPCmdBLL.DeleteCardApplication(cardApps);

                TempData["Message"] = "Success";
                TempData[Utilities.Activity_Log_Details] = "Card Application was deleted";
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return RedirectToAction("CardApplications");
        }

        [Audit]
        [ValidateUserPermission(Permissions = "can_view_mycardapplications")]
        public ActionResult MyCardApplications()
        {
            try
            {
                var loggedOnUser = User.Identity.Name;

                var myApplications = _cardAppQueryBLL.MyCardApplications(loggedOnUser);

                TempData[Utilities.Activity_Log_Details] = "My CardApplication was viewed";

                return View(myApplications);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("","Card application could not be retrieved");
            return View();
        }

        [Audit]
        [ValidateUserPermission(Permissions = "can_edit_cardapplication")]
        public ActionResult CardApplicationEdit(int ID)
        {
            try
            {
                var institution = User.Identity.GetInstitutionID();
                if (institution == "" || institution == "0  ")
                {
                    return RedirectToAction("Login", "Account");
                }

                _institution = institution;
                var cardApplication = _cardAppQueryBLL.GetCardApplication(ID);
                LoadApplicationLegends(institution);

                TempData[Utilities.Activity_Log_Details] = "Card Application was selected to be edited with ID =" + ID + "for" + cardApplication.FullName;
                return View("CardApplicationEdit", cardApplication);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("","There was an error editing card application");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        [ValidateUserPermission(Permissions = "can_edit_cardapplication")]
        public ActionResult CardApplicationEdit(CardApplicationsDTO cardApplicationsDTO, string HDImageByte)
        {
            try
            {
                var institution = User.Identity.GetInstitutionID();
                if (institution == "" || institution == "0  ")
                {
                    return RedirectToAction("Login", "Account");
                }

                _institution = institution;

                if (string.IsNullOrEmpty(HDImageByte))
                {
                    ModelState.AddModelError("", "Image is required");
                    LoadApplicationLegends(institution);
                    return View(cardApplicationsDTO);
                }

                cardApplicationsDTO.DateModified = DateTime.Now;
                cardApplicationsDTO.ModifiedBy = User.Identity.Name;
                cardApplicationsDTO.InstitutionID = Convert.ToInt32(_institution);
                //var m = ModelState.Where(x => x.Value.Errors.Count > 0);
                if (ModelState.IsValid)
                {
                    var instCode = _orgQueryBLL.GetInstitutionCode(_institution);

                    var saveLocation = Server.MapPath("~/images/CardApplication/");

                    _cardAPPCmdBLL.UpdateCardApplication(cardApplicationsDTO,HDImageByte,saveLocation, instCode);

                    TempData["Message"] = "Success";

                    TempData[Utilities.Activity_Log_Details] = "Card applications was updated for" + cardApplicationsDTO.FullName;

                    return RedirectToAction("CardApplicationEdit",new { id = cardApplicationsDTO.ID});
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                
            }
            LoadApplicationLegends(_institution);

            ModelState.AddModelError("","There was an error editing card application");
            return View(cardApplicationsDTO);
        }

        public JsonResult GetCardApplicationEdit(int ID)
        {
            var cardApplication = _cardAppQueryBLL.GetCardApplication(ID);
            return Json(cardApplication, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartment(string instID)
        {
            try
            {
                var depts = _orgQueryBLL.GetDepartments(instID);

                return Json(depts, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                
            }

            return Json(new { Error = "Error retrieving departments" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        [ValidateUserPermission(Permissions = "can_request_approval")]
        public async Task<ActionResult> CardDownloadApproval(List<CardApplicationsDTO> cardApps,string HDComment)
        {
            try
            {
                var selectedCardapp = cardApps.Where(x => x.IsSelected == true);
                if (selectedCardapp.Count() > 0)
                {
                    var requestBy = User.Identity.Name;

                    _approvalCMDBLL.AddApproval(cardApps, requestBy, HDComment);

                    TempData["Message"] = "Successonapproval";

                    
                    BackgroundJob.Enqueue(() => Utilities.ExecuteEmail("raphkens@live.com", "Ekene Approver", CardEnums.CardApprovalEmail));

                    return RedirectToAction("CardApplications");
                }
                else
                {
                    ModelState.AddModelError("","Select a cardapplication for your request");
                    return View();
                }
                
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("","Error approving card application");
            return View();
        }
        [ValidateUserPermission(Permissions = "can_approve_cardrequest")]
        public ActionResult CardRequestApprovals()
        {
            try
            {
                var approvals = _approvalQueryBLL.GetApprovals();

                return View(approvals);

            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("","Error retrieving card application");
            return View();
        }

        [ValidateUserPermission(Permissions = "can_approve_cardrequest")]
        public ActionResult ViewApplicationsForApproval(int approvalID)
        {
            try
            {
                List<int> cardAppID =_approvalQueryBLL.GetCardsToApprove(approvalID);
                var cardApps = _cardAppQueryBLL.GetCardApplicationsByIDs(cardAppID);

                ViewData["ApprovalID"] = approvalID;

                return View("CardsForApproval", cardApps);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Error viewing card application for approval");
            return View();
        }
        /// <summary>
        /// Approve Selected Card applications
        /// </summary>
        /// <param name="approvalID"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_approve_cardrequest")]
        public ActionResult ViewApplicationsForApproval(List<CardApplicationsDTO> cardApps, string Comment,string HDApprovalID)
        {
            try
            {
                var cards = cardApps.Where(x => x.IsSelected == true);
                if (cards.Count() > 0 )
                {
                    _cardAPPCmdBLL.CardApplicationApprovalUpdate(cardApps,Comment);
                    var approvalID = Convert.ToInt32(HDApprovalID);

                    _approvalCMDBLL.UpdateApproval(approvalID, Utilities.Approve);

                    TempData["Message"] = "Success";

                    return RedirectToAction("CardRequestApprovals");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("","No Card Application was selected");
            return View();
        }

        [ValidateUserPermission(Permissions = "can_approve_cardrequest")]
        public ActionResult CardApplicationDecline(int approvalID)
        {
            try
            {
                _approvalCMDBLL.UpdateApproval(approvalID, Utilities.Decline);

                TempData["Message"] = "Decline";

                return RedirectToAction("CardRequestApprovals");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }

        public ActionResult CardsForApproval()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        [ValidateUserPermission(Permissions = "can_download_cardapplicationreport")]
        public async Task<ActionResult> ExportToExcel(CardAppViewModel cardAppVM)
        {
            try
            {
                var cardApplications = await _cardAppQueryBLL.CardApplicationSearch(cardAppVM);
                var cardApps = cardApplications.ToList();
                ViewBag.CardApplications = cardApps;

                string[] columns = {
                    "FirstName", "MiddleName", "LastName","Sex","MaritalStatus", "OfficePhoneNo",
                    "GSMNo", "EmailAddress", "OfficeAddress1", "OfficeAddress2","City","State","RequestingBranchCode","MainAccountNo",
                    "OtherAccountNo","NameonCard","IDCardType","IDNo","IDIssueDate","IDExpiryDate","SocioProfCode","ProductCode","DateofBirth",
                    "TitleCode","Nationality"
                };
                byte[] filecontent = ExcelExportHelper.ExportExcel(cardApps, "", true, columns);

                CardSearchOptions();

                ////Verify if download is approved 
                //ViewData["IsDownloadRequired"] = _cardAppQueryBLL.CheckProcessedStatus(cardApps);

                TempData[Utilities.Activity_Log_Details] = "Card Applications has been exported";

                return File(filecontent, ExcelExportHelper.ExcelContentType, "CardApplication.xls");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            ModelState.AddModelError("", "Card applications could not be downloaded");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        [ValidateUserPermission(Permissions = "can_process_cardapplications")]
        public async Task<ActionResult> ProcessCard(List<CardApplicationsDTO> cardApps)
        {
            try
            {
                var saveLocation = Server.MapPath("~/images/CardApplication/");

                var cardApplToExport = _cardAppQueryBLL.CardApplicationToExport(cardApps).ToList();

                string[] columns = {
                    "FirstName", "MiddleName", "LastName","Sex","MaritalStatus", "OfficePhoneNo",
                    "GSMNo", "EmailAddress", "OfficeAddress1", "OfficeAddress2","City","State","RequestingBranchCode","MainAccountNo",
                    "OtherAccountNo","NameonCard","IDCardType","IDNo","IDIssueDate","IDExpiryDate","SocioProfCode","ProductCode","DateofBirth",
                    "TitleCode","Nationality"
                };
                byte[] filecontent = ExcelExportHelper.ExportExcel(cardApplToExport, "", true, columns);

                string[] filePaths = Directory.GetFiles(saveLocation, "*.jpeg");

                

                
                var saveAs = string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
                string excelPath = @"c:\picturecards\cardsapps" + saveAs + ".xls";
                System.IO.File.WriteAllBytes(excelPath, filecontent);
                //BackgroundJob.Enqueue(() => CreateFiles(filePaths));
                //var downloadlink = CreateFiles(excelPath,filePaths, cardApplToExport);
                CreateFiles(excelPath, filePaths, cardApplToExport);
                await _cardAPPCmdBLL.UpdateBatchNo("", cardApplToExport);
                TempData["Message"] = "Success";

                TempData[Utilities.Activity_Log_Details] = "Card Applications was processed";

                CardSearchOptions();
                return View("CardApplications");
                //return File(filecontent, ExcelExportHelper.ExcelContentType, "CardApplication.xlxs");
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }

        }
        public void CreateFiles(string excelPath, string[] sourceFiles, List<CardApplicationsDTO> cardAppsList)
        {
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    string[] filenames = sourceFiles;

                    foreach (var cardApp in cardAppsList)
                    {
                        var fileName = filenames.Where(x => x.Contains(cardApp.OfficeAddress2)).FirstOrDefault();//.Contains(cardApp.IDNo);
                        ZipEntry e = zip.AddFile(fileName, "/cardspix");
                        
                        e.Comment = "Added";
                    }

                    zip.AddFile(excelPath,"/cardcsv");

                    zip.Comment = "The downloaded file are for the just generated card application";
                    var path = CreateIfMissing(@"C:\picturecards\");

                    var saveAs = string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
                    var filePath = path + "CardPerso_" + saveAs + ".zip";
                    //zip.Save(filePath);

                    _cardAPPCmdBLL.UpdateStatus(cardAppsList);

                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.AppendHeader("content-disposition", "attachment;filename=Proccessed_Cards.zip");
                    zip.Save(Response.OutputStream);

                    //return filePath;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }
        private string CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
                Directory.CreateDirectory(path);
            return path;
        }
        
        public ActionResult ImageTracking()
        {
            return View();
        }

        //public ActionResult ViewProcessedCards()
        //{
        //    var processedCards =_cardAppQueryBLL.GetProcessedCard();
        //    return View(processedCards);
        //}
    }
}