using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZenithCardPerso.Web.Filters;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Data.ViewModel;
using ZenithCardRepo.Services.BLL.Command;
using ZenithCardRepo.Services.BLL.Query;

namespace ZenithCardPerso.Web.Controllers
{
    public class OrganisationController : Controller
    {
        private IOrganisationCmdBLL _orgCMDBLL;
        private IOrganisationQueryBLL _orgQueryBLL;
        private ILog _log;
        public OrganisationController(IOrganisationCmdBLL orgCMDBLL, IOrganisationQueryBLL orgQueryBLL, ILog log)
        {
            _orgCMDBLL = orgCMDBLL;
            _orgQueryBLL = orgQueryBLL;
            _log = log;
        }

        // GET: Organisation
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrganizationProfile organizationProfile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _orgCMDBLL.AddOrganisation(organizationProfile);
                    ViewData["Message"] = "Success";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();

                ModelState.AddModelError("", "An error has occurred, Kindly contact your system administrator");

            }

            ModelState.AddModelError("", "Verify that all data submitted is valid");

            return View(organizationProfile);
        }

        [ValidateUserPermission(Permissions = "can_create_institution")]
        public ActionResult CreateInstitution()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_create_institution")]
        public ActionResult CreateInstitution(Institution institution)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //verify institution code
                    bool codeExists = _orgQueryBLL.VerifyInstitutionCode(institution.Code);
                    if (codeExists)
                    {
                        ModelState.AddModelError("", "Institution code already exists");
                        return View(institution);
                    }

                    _orgCMDBLL.CreateInstitution(institution);

                    ModelState.Clear();
                    TempData["Message"] = "Success";

                    return View();
                }


            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }
            return View(institution);
        }
        [ValidateUserPermission(Permissions = "can_view_institutions")]
        public ActionResult Institutions()
        {
            try
            {
                var institutions = _orgQueryBLL.GetInstitutions();

                return View(institutions);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        [ValidateUserPermission(Permissions = "can_edit_institution")]
        public ActionResult InstitutionEdit(int ID)
        {
            try
            {
                var institutions = _orgQueryBLL.GetInstitution(ID);

                return View(institutions);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_edit_institution")]
        public ActionResult InstitutionEdit(Institution institution)
        {
            try
            {
                _orgCMDBLL.UpdateInstitution(institution);

                TempData["Message"] = "Success";

                return RedirectToAction("Institutions");

            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }
        [ValidateUserPermission(Permissions = "can_delete_institution")]
        public ActionResult InstitutionDelete(int ID)
        {
            try
            {
                _orgCMDBLL.InstitutionDelete(ID);

                TempData["Message"] = "Success";

                return RedirectToAction("Institutions");
            }
            catch (Exception ex)
            {
                _log.Error(ex);

                throw;
            }
        }
        [ValidateUserPermission(Permissions = "can_create_department")]
        public ActionResult CreateDepartment()
        {
            try
            {
                var institutions = _orgQueryBLL.GetInstitutions();
                ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");

                return View();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_create_department")]
        public ActionResult CreateDepartment(Department department)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var institutions = _orgQueryBLL.GetInstitutions();
                    //verify department code
                    bool codeExists = _orgQueryBLL.VerifyDepartmentCode(department.Code);

                    if (codeExists)
                    {
                        ModelState.AddModelError("", "Department code already exists");
                        ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");
                        return View(department);

                    }

                    _orgCMDBLL.CreateDepartment(department);
                    TempData["Message"] = "Success";

                    ModelState.Clear();

                    ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }


            return View();
        }

        public JsonResult GetDepartments(string instID)
        {
            var departments = _orgQueryBLL.GetDepartments(instID).Select(x => x.Name).ToList();

            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        [ValidateUserPermission(Permissions = "can_view_departments")]
        public ActionResult Departments()
        {
            try
            {
                var depts = _orgQueryBLL.GetDepartments();
                ViewData["Depts"] = depts;

                LoadInstitution();

                return View();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        [HttpPost]
        [ValidateUserPermission(Permissions = "can_view_departments")]
        public ActionResult Departments(DepartmentViewModel deptVM)
        {
            var depts = _orgQueryBLL.GetDepartments(deptVM);
            ViewData["Depts"] = depts;
            LoadInstitution();
            return View();
        }

        [ValidateUserPermission(Permissions = "can_edit_department")]
        public ActionResult DepartmentEdit(int ID)
        {
            var dept = _orgQueryBLL.GetDepartment(ID);
            LoadInstitution();
            return View(dept);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_edit_department")]
        public ActionResult DepartmentEdit(Department department)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _orgCMDBLL.UpdateDepartent(department);

                    TempData["Message"] = "Success";
                    LoadInstitution();
                    return View(); 
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
            return View();
        }
        [ValidateUserPermission(Permissions = "can_delete_department")]
        public ActionResult DepartmentDelete(int ID)
        {
            try
            {
                _orgCMDBLL.DepartmentDelete(ID);

                TempData["Message"] = "Success";

                return RedirectToAction("Departments");

            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
            
        }
        public void LoadInstitution()
        {
            var institutions = _orgQueryBLL.GetInstitutions();
            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");
        }

    }
}