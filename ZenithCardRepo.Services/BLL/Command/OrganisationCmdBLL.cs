using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Command;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Services.BLL.Command
{

    public class OrganisationCmdBLL : IOrganisationCmdBLL
    {
        private ICommandRepository<OrganizationProfile> _orgProfileRepo;
        private ICommandRepository<Institution> _institution;
        private IQueryRepository<Institution> _institutionQuery;
        private ICommandRepository<Department> _department;
        private IQueryRepository<Department> _departmentQuery;

        public OrganisationCmdBLL(
            ICommandRepository<OrganizationProfile> orgProfileRepo,
            ICommandRepository<Institution> institution,
            ICommandRepository<Department> department,
            IQueryRepository<Institution> institutionQuery,
            IQueryRepository<Department> departmentQuery
            )
        {
            _orgProfileRepo = orgProfileRepo;
            _institution = institution;
            _department = department;
            _institutionQuery = institutionQuery;
            _departmentQuery = departmentQuery;

        }
        public void AddOrganisation(OrganizationProfile organisationProfile)
        {
            _orgProfileRepo.Insert(organisationProfile);
            _orgProfileRepo.Save();
        }

        public void CreateInstitution(Institution institution)
        {
            _institution.Insert(institution);
            _institution.Save();
        }

        public void CreateDepartment(Department department)
        {
            _department.Insert(department);
            _department.Save();
        }

        public void UpdateInstitution(Institution institution)
        {
            _institution.Update(institution);
            _institution.Save();

        }

        public void InstitutionDelete(int ID)
        {
            var institution = _institutionQuery.GetBy(x => x.ID == ID).FirstOrDefault();
            _institution.Delete(institution);
            _institution.Save();
        }

        public void UpdateDepartent(Department department)
        {
            _department.Update(department);
            _department.Save();
        }

        public void DepartmentDelete(int ID)
        {
            var dept = _departmentQuery.GetBy(x => x.ID == ID).FirstOrDefault();

            _department.Delete(dept);
            _department.Save();
        }
    }
}
