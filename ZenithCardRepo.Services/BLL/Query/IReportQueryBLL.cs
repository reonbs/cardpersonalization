using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;

namespace ZenithCardRepo.Services.BLL.Query
{
    public interface IReportQueryBLL
    {
        IEnumerable<AuditRecord> GetAuditRecords(string instID);

        IEnumerable<AuditRecord> GetAuditRecordsSearch(AuditViewModel auditVM, string instID);

        
    }
}
