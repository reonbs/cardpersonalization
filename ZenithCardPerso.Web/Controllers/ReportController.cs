using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        // GET: Report
        public ActionResult AuditTrail()
        {
            TempData[Utilities.Activity_Log_Details] = "Accessed Audit trail records";

            ViewData["AuditRecords"] = _reportQueryBLL.GetAuditRecords();

            var institutions = _orgQueryBLL.GetInstitutions();
            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuditTrailSearch(AuditViewModel auditVM)
        {
            var auditRecords =  _reportQueryBLL.GetAuditRecordsSearch(auditVM);

            ViewData["AuditRecords"] = auditRecords.ToList();

            var institutions = _orgQueryBLL.GetInstitutions();
            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");

            return View("AuditTrail");
        }
    }
}