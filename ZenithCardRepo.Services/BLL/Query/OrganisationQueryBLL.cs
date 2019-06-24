using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Repository.Query;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;

namespace ZenithCardRepo.Services.BLL.Query
{
    public class OrganisationQueryBLL : IOrganisationQueryBLL
    {
        private IQueryRepository<Institution> _institutionQueryRepo;
        private IQueryRepository<Department> _departmentQueryRepo;
        public OrganisationQueryBLL(IQueryRepository<Institution> institutionQueryRepo, IQueryRepository<Department> departmentQueryRepo)
        {
            _institutionQueryRepo = institutionQueryRepo;
            _departmentQueryRepo = departmentQueryRepo;
        }

        public Department GetDepartment(int ID)
        {
            return _departmentQueryRepo.FindByKey(ID);
        }

        public List<Department> GetDepartments(string institution)
        {
            int institutionID = Convert.ToInt32(institution);

            return _departmentQueryRepo.GetBy(x => x.InstitutionID == institutionID).ToList();
        }

        public List<Department> GetDepartments()
        {
            return _departmentQueryRepo.GetAll().ToList();
        }

        public List<Department> GetDepartments(DepartmentViewModel deptVM)
        {
            var depts = _departmentQueryRepo.GetAll();
            int count = 0;
            if (!string.IsNullOrEmpty(deptVM.Institution))
            {
                var inst = Convert.ToInt32(deptVM.Institution);
                depts = depts.Where(x => x.InstitutionID == inst);
                count = depts.Count();
            }

            if (count > 0)
            {
                return depts.ToList();
            }
            else
            {
                return new List<Department> { };
            }
        }

        public Institution GetInstitution(int ID)
        {
            return _institutionQueryRepo.GetBy(x => x.ID == ID).FirstOrDefault();
        }

        public string GetInstitutionCode(string InstitutionID)
        {
            var instID = Convert.ToInt32(InstitutionID);
            return _institutionQueryRepo.GetBy(x => x.ID == instID).FirstOrDefault().Code;
        }

        public List<Institution> GetInstitutions()
        {
            return _institutionQueryRepo.GetAll().ToList();
        }

        public bool VerifyDepartmentCode(string deptCode)
        {
            return _departmentQueryRepo.GetBy(x => x.Code.ToLower() == deptCode.ToLower()).Any();
        }

        public bool VerifyInstitutionCode(string instCode)
        {
            return _institutionQueryRepo.GetBy(x => x.Code.ToLower() == instCode.ToLower()).Any();
        }
    }
}
