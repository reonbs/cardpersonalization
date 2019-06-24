using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Command;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Command
{
    public class FilterCMDBLL : IFilterCMDBLL
    {
        private ICommandRepository<AuditRecord> _auditRecordCmdRepo;
        public FilterCMDBLL(ICommandRepository<AuditRecord> auditRecordCmdRepo)
        {
            _auditRecordCmdRepo = auditRecordCmdRepo;
        }
        public void AddAuditRecord(AuditRecord auditRecord)
        {
            _auditRecordCmdRepo.Insert(auditRecord);
            _auditRecordCmdRepo.Save();

        }
    }
}
