using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;

namespace ZenithCardRepo.Services.BLL.Query
{
    public class ReportQueryBLL : IReportQueryBLL
    {
        private IQueryRepository<AuditRecord> _auditRecQueryBLL;
        public ReportQueryBLL(IQueryRepository<AuditRecord> auditRecQueryBLL)
        {
            _auditRecQueryBLL = auditRecQueryBLL;
        }
        public IEnumerable<AuditRecord> GetAuditRecords(string instID)
        {
            var auditRecord = _auditRecQueryBLL.GetAllIncluding(x => x.Institution).ToList();
            if (!string.IsNullOrEmpty(instID))
            {
                int institutionID = Convert.ToInt32(instID);

                auditRecord = auditRecord.Where(x => x.InstitutionID == institutionID).ToList();
            }
            return auditRecord;
        }

        public IEnumerable<AuditRecord> GetAuditRecordsSearch(AuditViewModel auditVM,string instID)
        {
            var auditRecords = _auditRecQueryBLL.GetAllIncluding(x => x.Institution);

            if (!string.IsNullOrEmpty(instID))
            {
                int institutionID = Convert.ToInt32(instID);
                auditRecords = auditRecords.Where(x => x.InstitutionID == institutionID);
            }

            int resultFound = 0;

            if (!string.IsNullOrEmpty(auditVM.StartDate) && !string.IsNullOrEmpty(auditVM.EndDate))
            {
                var startDate = DateTime.ParseExact(auditVM.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture); //Convert.ToDateTime(auditVM.StartDate);
                var endDate = DateTime.ParseExact(auditVM.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture); //Convert.ToDateTime(auditVM.EndDate);
                auditRecords = auditRecords.Where(x => x.TimeAccessed >= startDate && x.TimeAccessed <= endDate);
                resultFound = auditRecords.Count();
            }
            if (!string.IsNullOrEmpty(auditVM.StartDate))
            {
                var startDate = DateTime.ParseExact(auditVM.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                auditRecords = auditRecords.Where(x => x.TimeAccessed >= startDate);
                resultFound = auditRecords.Count();
            }
            if (!string.IsNullOrEmpty(auditVM.EndDate))
            {
                var endDate = DateTime.ParseExact(auditVM.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                auditRecords = auditRecords.Where(x => x.TimeAccessed <= endDate);
                resultFound = auditRecords.Count();
            }
            if (!string.IsNullOrEmpty(auditVM.Institution))
            {
                int institutionID = Convert.ToInt32(auditVM.Institution);
                auditRecords = auditRecords.Where(x => x.InstitutionID == institutionID);
                resultFound = auditRecords.Count();
            }
            if (!string.IsNullOrEmpty(auditVM.Username))
            {
                auditRecords = auditRecords.Where(x => x.UserName.ToLower() == auditVM.Username.ToLower());
                resultFound = auditRecords.Count();
            }

            if (resultFound > 0)
            {
                return auditRecords;
            }
            else
            {
                return new List<AuditRecord> { };
            }

        }
    }
}
