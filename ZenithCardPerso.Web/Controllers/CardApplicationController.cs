using Hangfire;
using Ionic.Zip;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
        private string _institution;
        public CardApplicationController(
            ICardApplicationCmdBLL cardAPPCmdBLL,
            IGetApplicationLegends applicationLegends,
            ICardApplicationQueryBLL cardAppQueryBLL,
            IOrganisationQueryBLL orgQueryBLL,
            ILog log
            )
        {
            _cardAPPCmdBLL = cardAPPCmdBLL;
            _applicationLegends = applicationLegends;
            _cardAppQueryBLL = cardAppQueryBLL;
            _orgQueryBLL = orgQueryBLL;
            _log = log;
        }

        [Audit]
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
            var valMsgs = _cardAPPCmdBLL.ImageBase64String(HDImageByte);

            return Json(valMsgs, JsonRequestBehavior.AllowGet);
        }

        [Audit(AuditingLevel = 2)]
        [HttpPost]
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

                    ViewData["Message"] = "Success";
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
                throw;
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
                ex.Message.ToList();
                throw;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult CardApplications(CardAppViewModel cardAppVM)
        {
            try
            {
                var cardApps = _cardAppQueryBLL.CardApplicationSearch(cardAppVM).ToList();
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
                throw;
            }

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
                throw;
            }
            return RedirectToAction("CardApplications");
        }

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
                throw;
            }
        }

        [Audit]
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
                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
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
                throw;
            }
            LoadApplicationLegends(_institution);
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
                throw;
            }
        }
        public ActionResult CardDownloadApproval(string batchNo)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult ExportToExcel(CardAppViewModel cardAppVM)
        {
            try
            {
                var cardApps = _cardAppQueryBLL.CardApplicationSearch(cardAppVM).ToList();
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
                throw;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ProcessCard()
        {
            try
            {
                var saveLocation = Server.MapPath("~/images/CardApplication/");

                var cardApplToExport = _cardAppQueryBLL.CardApplicationToExport().ToList();

                string[] columns = {
                    "FirstName", "MiddleName", "LastName","Sex","MaritalStatus", "OfficePhoneNo",
                    "GSMNo", "EmailAddress", "OfficeAddress1", "OfficeAddress2","City","State","RequestingBranchCode","MainAccountNo",
                    "OtherAccountNo","NameonCard","IDCardType","IDNo","IDIssueDate","IDExpiryDate","SocioProfCode","ProductCode","DateofBirth",
                    "TitleCode","Nationality"
                };
                byte[] filecontent = ExcelExportHelper.ExportExcel(cardApplToExport, "", true, columns);

                string[] filePaths = Directory.GetFiles(saveLocation, "*.jpeg");

                _cardAPPCmdBLL.UpdateBatchNo(cardApplToExport);

                CreateFiles(filePaths, cardApplToExport);
                var saveAs = string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);

                System.IO.File.WriteAllBytes(@"c:\pc\cardsapps" + saveAs + ".xls", filecontent);
                //BackgroundJob.Enqueue(() => CreateFiles(filePaths));

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
        public void CreateFiles(string[] sourceFiles, List<CardApplication> cardAppsList)
        {
            string dest = @"C:\pc\";
            using (ZipFile zip = new ZipFile())
            {
                string[] filenames = sourceFiles;

                foreach (var cardApp in cardAppsList)
                {
                    var fileName = filenames.Where(x => x.Contains(cardApp.OfficeAddress2)).FirstOrDefault();//.Contains(cardApp.IDNo);
                    ZipEntry e = zip.AddFile(fileName, "/cardspix");
                    e.Comment = "Added";
                }
                //foreach (String filename in filenames)
                //{


                //    ZipEntry e = zip.AddFile(filename,"/cardspix");
                //    e.Comment = "Added";
                //}

                zip.Comment = String.Format("The downloaded file are for the just generated card application on machine '{0}'",
                      System.Net.Dns.GetHostName());
                var path = CreateIfMissing(@"C:\pc\");
                //var fileName = "CardsApplication" + DateTime.Today.ToString("dd-mm-yyyy");
                var saveAs = string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
                var filePath = path + "myfile" + saveAs + ".zip";
                zip.Save(filePath);

                _cardAPPCmdBLL.UpdateStatus(cardAppsList);
                //foreach (var item in sourceFiles)
                //{
                //    var fN = Path.GetFileName(item);
                //    System.IO.File.Copy(item, dest + fN);
                //}
            }
        }
        private string CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
                Directory.CreateDirectory(path);
            return path;
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}