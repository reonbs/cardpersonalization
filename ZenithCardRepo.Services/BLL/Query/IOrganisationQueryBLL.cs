using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;

namespace ZenithCardRepo.Services.BLL.Query
{
    public interface IOrganisationQueryBLL
    {
        List<Institution> GetInstitutions();
        List<Department> GetDepartments(string institution);
        Institution GetInstitution(int ID);
        string GetInstitutionCode(string InstitutionID);

        bool VerifyInstitutionCode(string instCode);
        bool VerifyDepartmentCode(string deptCode);
        List<Department> GetDepartments();
        List<Department> GetDepartments(DepartmentViewModel deptVM);
        Department GetDepartment(int ID);

    }
}
