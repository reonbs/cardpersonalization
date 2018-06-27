using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using ZenithCardPerso.Web.Filters;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;
using ZenithCardRepo.Services.BLL.Infrastructure;
using ZenithCardRepo.Services.BLL.Query;

namespace ZenithCardPerso.Web.Controllers
{
    public class ReportController : Controller
    {
        private IReportQueryBLL _reportQueryBLL;
        private IOrganisationQueryBLL _orgQueryBLL;
        public ReportController(IReportQueryBLL reportQueryBLL, IOrganisationQueryBLL orgQueryBLL)
        {
            _reportQueryBLL = reportQueryBLL;
            _orgQueryBLL = orgQueryBLL;
        }

        [ValidateUserPermission(Permissions = "can_view_audittrail")]
        public ActionResult AuditTrail()
        {
            TempData[Utilities.Activity_Log_Details] = "Accessed Audit trail records";

            var institutionID = ((ClaimsIdentity)User.Identity).FindFirst(Utilities.InstitutionID).Value;

            ViewData["AuditRecords"] =(User.IsInRole("InstitutionAdmin")) ? _reportQueryBLL.GetAuditRecords(institutionID) : _reportQueryBLL.GetAuditRecords(null);
            //var permissions = (User.Identity as ClaimsIdentity).FindFirst("UserPermissions").Value;//.FindFirstValue("UserPermissions");

            List<Institution> institutions = new List<Institution>();

            if (User.IsInRole("InstitutionAdmin"))
            {
                institutions = _orgQueryBLL.GetInstitutions().Where(x => x.ID == Convert.ToInt32(institutionID)).ToList();
            }
            else
            {
                institutions = _orgQueryBLL.GetInstitutions();
            }
           

            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", institutionID);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_view_audittrail")]
        public ActionResult AuditTrailSearch(AuditViewModel auditVM)
        {
            var auditRecords =  _reportQueryBLL.GetAuditRecordsSearch(auditVM,null);

            ViewData["AuditRecords"] = auditRecords.ToList();

            var institutions = _orgQueryBLL.GetInstitutions();
            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");

            return View("AuditTrail");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_view_audittrail")]
        public ActionResult AuditTrailSearchInst(AuditViewModel auditVM)
        {
            var institutionID = ((ClaimsIdentity)User.Identity).FindFirst(Utilities.InstitutionID).Value;

            var auditRecords = _reportQueryBLL.GetAuditRecordsSearch(auditVM,institutionID);

            ViewData["AuditRecords"] = auditRecords.ToList();

            var institutions = _orgQueryBLL.GetInstitutions();
            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");

            return View("AuditTrail");
        }


    }
}