using Calabonga.OperationResults;
using Hangfire;
using Ionic.Zip;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
//using System.Net.Mail;
using System.Runtime.Remoting.Messaging;
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
        private IPermissionQueryBLL _permissionQueryBLL;

        public CardApplicationController(
            ICardApplicationCmdBLL cardAPPCmdBLL,
            IGetApplicationLegends applicationLegends,
            ICardApplicationQueryBLL cardAppQueryBLL,
            IOrganisationQueryBLL orgQueryBLL,
            ILog log,
            IApprovalCMDBLL approvalCMDBLL,
            IApprovalQueryBLL approvalQueryBLL,
            IPermissionQueryBLL permissionQueryBLL
            )
        {
            _cardAPPCmdBLL = cardAPPCmdBLL;
            _applicationLegends = applicationLegends;
            _cardAppQueryBLL = cardAppQueryBLL;
            _orgQueryBLL = orgQueryBLL;
            _log = log;
            _approvalCMDBLL = approvalCMDBLL;
            _approvalQueryBLL = approvalQueryBLL;
            _permissionQueryBLL = permissionQueryBLL;
        }


        [ValidateUserPermission(Permissions = "can_create_cardapplication")]
        [Audit]
        public async Task<ActionResult> CardApplicationCreate()
        {
            var institution = User.Identity.GetInstitutionID();
            if (institution == "" || institution == "0  ")
            {
                return RedirectToAction("Login", "Account");
            }

            _institution = institution;
            await LoadApplicationLegends(_institution);

            TempData[Utilities.Activity_Log_Details] = "Card Application view was loaded";
            return View();
        }

        public JsonResult ValidateCaptureImage(string HDImageByte)
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

        [HttpPost]
        public JsonResult ValidateUploadImage(HttpPostedFileBase fileUpload)
        {
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                byte[] FileByteArray = new byte[fileUpload.ContentLength];
                fileUpload.InputStream.Read(FileByteArray, 0, fileUpload.ContentLength);

                var HDImageByte = Convert.ToBase64String(FileByteArray);

                var valMsgs = _cardAPPCmdBLL.ImageBase64String(HDImageByte);

                return Json(valMsgs, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    statusCode = 400,
                    status = "Bad Request! Upload Failed",
                    file = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateUserPermission(Permissions = "can_create_cardapplication")]
        [Audit]
        public async Task<ActionResult> CardApplicationCreate(CardApplicationsDTO cardApplication, string HDImageByte, string IDIssueDate, string IDExpiryDate, string DateofBirth)
        {
            TempData[Utilities.Activity_Log_Details] = "Card Application has been captured Successfully";
            var instID = string.Empty;
            try
            {
                instID = User.Identity.GetInstitutionID();
                if (string.IsNullOrEmpty(HDImageByte))
                {
                    ModelState.AddModelError("", "Image is required");
                    await LoadApplicationLegends(instID);
                    return View(cardApplication);
                }

                if (instID == "" || instID == "0  ")
                {
                    return RedirectToAction("Login", "Account");
                }

                if (string.IsNullOrEmpty(HDImageByte))
                {
                    ModelState.AddModelError("", "Image is required");
                    await LoadApplicationLegends(instID);
                    return View(cardApplication);
                }

                var appCheck = _cardAppQueryBLL.ValidatedApplication(cardApplication.IDNo);
                if (appCheck)
                {
                    ModelState.AddModelError("", "Card application already exist for this applicant");
                    await LoadApplicationLegends(instID);
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

                    //send emails on card application
                    //BackgroundJob.Enqueue(() => Utilities.ExecuteEmail("raphkens@live.com", "Ekene Egonu", CardEnums.CardApplicationEmail));

                    ModelState.Clear();

                    if (instID == "" || instID == "0  ")
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    var departments = _orgQueryBLL.GetDepartments(instID);
                    ViewBag.Department = new SelectList(departments, "Code", "Name", "");
                    await LoadApplicationLegends(instID);
                    return View();
                }

            }
            catch (Exception ex)
            {
                await LoadApplicationLegends(instID);
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Validate that all fields are entered correctly");

            await LoadApplicationLegends(instID);
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
        public async Task<bool> LoadApplicationLegends(string institution)
        {
            ViewData["MaritalStatusView"] = new SelectList(await _applicationLegends.MaritalStatusList(), "Code", "Description", "");
            ViewData["StateView"] = new SelectList(await _applicationLegends.StateList(), "StateCode", "RegionName", "");
            ViewData["SocioProfCodeView"] = new SelectList(await _applicationLegends.SocioProfCodeList(), "Code", "Description", "");
            ViewData["NationalityView"] = new SelectList(await _applicationLegends.NationalityCodeList(), "CountryCode", "Wording", "");
            ViewData["TitleCodeView"] = new SelectList(await _applicationLegends.TitleCodeList(), "Code", "Wording", "");
            ViewData["ProductCodeView"] = new SelectList(await _applicationLegends.ProductCodeList(), "Code", "Description", "");
            ViewData["IDCardTypeView"] = new SelectList(await _applicationLegends.IDCardTypeList(), "DocumentCode", "ABRVWording", "");
            ViewData["SexView"] = new SelectList(await _applicationLegends.SexList(), "Code", "Description", "");

            var departments = _orgQueryBLL.GetDepartments(institution);
            ViewBag.DepartmentView = new SelectList(departments, "Code", "Name", "");

            return true;

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


        [ValidateUserPermission(Permissions = "can_view_cardapplications")]
        [Audit]
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

            ModelState.AddModelError("", "Card applications could not be retrieved");
            CardSearchOptions();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_view_cardapplications")]
        [Audit]
        public async Task<ActionResult> CardApplications(CardAppViewModel cardAppVM)
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
                return View(cardAppVM);
            }
            catch (Exception ex)
            {
                _log.Error(ex);

            }

            CardSearchOptions();
            ModelState.AddModelError("", "Card applications could not be retrieved");
            return View();
        }


        [ValidateUserPermission(Permissions = "can_view_cardapplications")]
        [Audit]
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

            Dictionary<string, string> isApproved = new Dictionary<string, string>();
            isApproved.Add("Yes", "true");
            isApproved.Add("No", "false");

            ViewData["IsProcessed"] = new SelectList(isProcessed, "Value", "Key", "");
            ViewData["isApproved"] = new SelectList(isApproved, "Value", "Key", "");

            var institutions = _orgQueryBLL.GetInstitutions();
            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_delete_cardapplication")]
        [Audit]
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


        [ValidateUserPermission(Permissions = "can_view_mycardapplications")]
        [Audit]
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

            ModelState.AddModelError("", "Card application could not be retrieved");
            return View();
        }


        [ValidateUserPermission(Permissions = "can_edit_cardapplication")]
        [Audit]
        public async Task<ActionResult> CardApplicationEdit(int ID)
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
                await LoadApplicationLegends(institution);

                TempData[Utilities.Activity_Log_Details] = "Card Application was selected to be edited with ID =" + ID + "for" + cardApplication.FullName;
                return View("CardApplicationEdit", cardApplication);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "There was an error editing card application");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_edit_cardapplication")]
        [Audit]
        public async Task<ActionResult> CardApplicationEdit(CardApplicationsDTO cardApplicationsDTO, string HDImageByte)
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
                    await LoadApplicationLegends(institution);
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

                    _cardAPPCmdBLL.UpdateCardApplication(cardApplicationsDTO, HDImageByte, saveLocation, instCode);

                    TempData["Message"] = "Success";

                    TempData[Utilities.Activity_Log_Details] = "Card applications was updated for" + cardApplicationsDTO.FullName;

                    return RedirectToAction("CardApplicationEdit", new { id = cardApplicationsDTO.ID });
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);

            }
            await LoadApplicationLegends(_institution);

            ModelState.AddModelError("", "There was an error editing card application");
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
        [ValidateUserPermission(Permissions = "can_request_approval")]
        [Audit]
        public ActionResult CardDownloadApproval(List<CardApplicationsDTO> cardApps, string HDComment)
        {
            try
            {
                var selectedCardapp = cardApps.Where(x => x.IsSelected == true);
                if (selectedCardapp.Any())
                {
                    var requestBy = User.Identity.Name;

                    _approvalCMDBLL.AddApproval(cardApps, requestBy, HDComment);

                    TempData["Message"] = "Successonapproval";

                    TempData[Utilities.Activity_Log_Details] = "Card applications approval request has been initiated";


                    var approvers = _permissionQueryBLL.GetApprovers("can_approve_cardrequest");

                    foreach (var approver in approvers)
                    {
                        BackgroundJob.Enqueue(() => Utilities.ExecuteEmail(approver.Email, approver.FullName, CardEnums.CardApprovalEmail));
                    }

                    return RedirectToAction("CardApplications");
                }
                else
                {
                    ModelState.AddModelError("", "Select a cardapplication for your request");
                    return View("CardApplications");
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Error requesting card approval, Try again");
            return View("CardApplications");
        }


        [ValidateUserPermission(Permissions = "can_approve_cardrequest")]
        [Audit]
        public ActionResult CardRequestApprovals()
        {
            List<Approval> approvals = new List<Approval> { };
            try
            {
                approvals = _approvalQueryBLL.GetApprovals();
                TempData[Utilities.Activity_Log_Details] = "Card application approvals has been viewed";
                return View(approvals);

            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Error retrieving card application");
            return View(approvals);
        }


        [ValidateUserPermission(Permissions = "can_approve_cardrequest")]
        [Audit]
        public ActionResult ViewApplicationsForApproval(int approvalID)
        {
            try
            {
                List<int> cardAppID = _approvalQueryBLL.GetCardsToApprove(approvalID);
                var cardApps = _cardAppQueryBLL.GetCardApplicationsByIDs(cardAppID);

                ViewData["ApprovalID"] = approvalID;

                TempData[Utilities.Activity_Log_Details] = "Card applications approval has been viewed";

                return View("CardsForApproval", cardApps);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            //ModelState.AddModelError("", "Error viewing card application for approval");
            TempData["Message"] = "Failed";
            TempData[Utilities.Activity_Log_Details] = "Error viewing card application for approval";
            return RedirectToAction("CardRequestApprovals");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardApps"></param>
        /// <param name="comment"></param>
        /// <param name="hdApprovalId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_approve_cardrequest")]
        [Audit]
        public ActionResult ViewApplicationsForApproval(List<CardApplicationsDTO> cardApps, string comment, string hdApprovalId)
        {
            try
            {
                var cards = cardApps.Where(x => x.IsSelected);
                if (cards.Any())
                {
                    //cardApps = cardApps.Where(x => x.IsSelected).ToList();
                    _cardAPPCmdBLL.CardApplicationApprovalUpdate(cardApps, comment);

                    var allApproved = cardApps.Count == cards.Count();

                    var approvalID = Convert.ToInt32(hdApprovalId);

                    _approvalCMDBLL.UpdateApproval(approvalID, Utilities.Approve, cardApps, allApproved);

                    TempData[Utilities.Activity_Log_Details] = "Card applications has been approved";

                    TempData["Message"] = "Success";

                    return RedirectToAction("CardRequestApprovals");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "No Card Application was selected");
            return View();
        }


        [ValidateUserPermission(Permissions = "can_approve_cardrequest")]
        [Audit]
        public ActionResult CardApplicationDecline(int approvalID)
        {
            try
            {
                _approvalCMDBLL.UpdateApproval(approvalID, Utilities.Decline, null, false);

                TempData[Utilities.Activity_Log_Details] = $"Card applications with appprovalid {approvalID} has been viewed";

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
        [ValidateUserPermission(Permissions = "can_download_cardapplicationreport")]
        [Audit]
        public async Task<ActionResult> ExportToExcel(CardAppViewModel cardAppVM)
        {
            try
            {
                TempData[Utilities.Activity_Log_Details] = "Card Applications report has been exported";
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



                return File(filecontent, ExcelExportHelper.ExcelContentType, "CardApplication.xls");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            CardSearchOptions();
            ModelState.AddModelError("", "Card applications could not be downloaded");
            return View("CardApplications");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_process_cardapplications")]
        [Audit]
        public async Task<ActionResult> ProcessCard(List<CardApplicationsDTO> cardApps)
        {
            try
            {
                if (cardApps.Count == 0)
                {
                    TempData["Message"] = "failedprocess";
                    TempData[Utilities.Activity_Log_Details] = "Card Applications was processed but failed";
                    return View("CardApplications");
                }
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


                string appPath = AppDomain.CurrentDomain.BaseDirectory;


                var path = CreateIfMissing(appPath + "picturecards\\cardsapps");
                var saveAs = string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);

                string excelPath = path + saveAs + ".xls";
                System.IO.File.WriteAllBytes(excelPath, filecontent);
                //BackgroundJob.Enqueue(() => CreateFiles(filePaths));
                //var downloadlink = CreateFiles(excelPath,filePaths, cardApplToExport);
                CreateFiles(excelPath, filePaths, cardApplToExport);
                await _cardAPPCmdBLL.UpdateBatchNo("", cardApplToExport);

                if (System.IO.File.Exists(excelPath))
                {
                    System.IO.File.Delete(excelPath);
                }

                TempData["Message"] = "Success";

                TempData[Utilities.Activity_Log_Details] = "Card Applications was processed";

                CardSearchOptions();
                return View("CardApplications");
                //return RedirectToAction("CardApplications");
                //return File(filecontent, ExcelExportHelper.ExcelContentType, "CardApplication.xlxs");
            }
            catch (Exception ex)
            {
                _log.Error(ex);

            }

            TempData["Message"] = "failedprocess";

            return RedirectToAction("CardApplications");

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

                    zip.AddFile(excelPath, "/cardcsv");

                    zip.Comment = "The downloaded file are for the just generated card application";

                    string appPath = AppDomain.CurrentDomain.BaseDirectory;
                    var path = CreateIfMissing(appPath + "picturecards");

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
        public ActionResult InstitutionReport()
        {

            var instID = User.Identity.GetInstitutionID();

            ViewBag.CardApplications = _cardAppQueryBLL.GetCardApplications(Convert.ToInt32(instID)).ToList();

            InstReportOption(instID);

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> InstitutionReport(CardAppViewModel cardAppVM)
        {
            try
            {
                var instID = User.Identity.GetInstitutionID();

                await CardSearch(cardAppVM, Convert.ToInt32(instID));

                InstReportOption(instID);

                return View(cardAppVM);
            }
            catch (Exception ex)
            {
                _log.Error(ex);

            }

            CardSearchOptions();
            ModelState.AddModelError("", "Card applications could not be retrieved");
            return View();
        }

        public async Task<bool> CardSearch(CardAppViewModel cardAppVM, int instID)
        {
            var cardApplications = await _cardAppQueryBLL.CardApplicationSearchByInst(cardAppVM, instID);
            var cardApps = cardApplications.ToList();
            ViewBag.CardApplications = cardApps;
            CardSearchOptions();

            //Verify if download is approved 
            //ViewData["IsDownloadRequired"] = _cardAppQueryBLL.CheckProcessedStatus(cardApps);

            TempData[Utilities.Activity_Log_Details] = "Search was carried out on Card Applications by institution ID: " + instID;

            return true;
        }

        public void InstReportOption(string instID)
        {

            var depts = _orgQueryBLL.GetDepartments(instID);

            ViewData["instDepartment"] = new SelectList(depts, "Code", "Name", "");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_download_cardapplicationreport")]
        [Audit]
        public async Task<ActionResult> ExportToExcelByInst(CardAppViewModel cardAppVM)
        {
            var instID = string.Empty;
            try
            {

                instID = User.Identity.GetInstitutionID();

                TempData[Utilities.Activity_Log_Details] = "Card Applications report has been exported";
                var cardApplications = await _cardAppQueryBLL.CardApplicationSearchByInst(cardAppVM, Convert.ToInt32(instID));

                if (cardApplications.Any())
                {
                    var cardApps = cardApplications.ToList();
                    ViewBag.CardApplications = cardApps;

                    string[] columns = {
                        "FirstName", "MiddleName", "LastName","Sex","MaritalStatus", "OfficePhoneNo",
                        "GSMNo", "EmailAddress", "OfficeAddress1", "OfficeAddress2","City","State","RequestingBranchCode","MainAccountNo",
                        "OtherAccountNo","NameonCard","IDCardType","IDNo","IDIssueDate","IDExpiryDate","SocioProfCode","ProductCode","DateofBirth",
                        "TitleCode","Nationality"
                    };
                    byte[] filecontent = ExcelExportHelper.ExportExcel(cardApps, "", true, columns);

                    InstReportOption(instID);

                    ////Verify if download is approved 
                    //ViewData["IsDownloadRequired"] = _cardAppQueryBLL.CheckProcessedStatus(cardApps);



                    return File(filecontent, ExcelExportHelper.ExcelContentType, "CardApplication.xls");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            InstReportOption(instID);
            ModelState.AddModelError("", "Card applications could not be downloaded");
            return View("InstitutionReport");
        }



    }
}