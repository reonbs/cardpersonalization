using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Command
{
    public interface IOrganisationCmdBLL
    {
        void AddOrganisation(OrganizationProfile organisationProfile);
        void CreateInstitution(Institution institution);
        void CreateDepartment(Department department);

        void UpdateInstitution(Institution institution);

        void InstitutionDelete(int ID);

        void UpdateDepartent(Department department);
        void DepartmentDelete(int ID);
    }
}
