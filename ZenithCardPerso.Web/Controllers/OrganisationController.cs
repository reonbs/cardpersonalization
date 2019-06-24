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
using ZenithCardRepo.Services.BLL.Infrastructure;
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
        [ValidateUserPermission(Permissions = "can_create_organisation")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_create_organisation")]
        [Audit]
        public ActionResult Create(OrganizationProfile organizationProfile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _orgCMDBLL.AddOrganisation(organizationProfile);
                    ViewData["Message"] = "Success";
                    TempData[Utilities.Activity_Log_Details] = $"organtisation {organizationProfile.Name} has been added";
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception ex)
            {
                

                ModelState.AddModelError("", "An error has occurred, Kindly contact your system administrator");
                _log.Error(ex);
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
        [Audit]
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

                    TempData[Utilities.Activity_Log_Details] = $"institution has been added";
                    TempData["Message"] = "Success";

                    return View();
                }


            }
            catch (Exception ex)
            {
                _log.Error(ex);
               
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
                
            }

            return View();
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
                
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_edit_institution")]
        [Audit]
        public ActionResult InstitutionEdit(Institution institution)
        {
            try
            {
                _orgCMDBLL.UpdateInstitution(institution);

                TempData[Utilities.Activity_Log_Details] = $"institution with ID:  {institution.ID} has been edited";
                TempData["Message"] = "Success";

                return RedirectToAction("Institutions");

            }
            catch (Exception ex)
            {
                _log.Error(ex);
                
            }

            return View();
        }


        [ValidateUserPermission(Permissions = "can_delete_institution")]
        [Audit]
        public ActionResult InstitutionDelete(int ID)
        {
            try
            {
                _orgCMDBLL.InstitutionDelete(ID);

                TempData[Utilities.Activity_Log_Details] = $"institution {ID} has been deleted";
                TempData["Message"] = "Success";

                return RedirectToAction("Institutions");
            }
            catch (Exception ex)
            {
                _log.Error(ex);

                
            }

            ModelState.AddModelError("","Error deleteting institution");
            return View();
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
                _log.Error(ex);
                
            }
            return View();

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_create_department")]
        [Audit]
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

                    TempData[Utilities.Activity_Log_Details] = $"department has been added";
                    TempData["Message"] = "Success";

                    ModelState.Clear();

                    ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
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
                
            }

            return View();
        }

        [HttpPost]
        [ValidateUserPermission(Permissions = "can_view_departments")]
        [Audit]
        public ActionResult Departments(DepartmentViewModel deptVM)
        {
            var depts = _orgQueryBLL.GetDepartments(deptVM);
            ViewData["Depts"] = depts;
            TempData[Utilities.Activity_Log_Details] = $"department has been added";
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
        [Audit]
        public ActionResult DepartmentEdit(Department department)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _orgCMDBLL.UpdateDepartent(department);
                    TempData[Utilities.Activity_Log_Details] = $"department has been edited";
                    TempData["Message"] = "Success";
                    LoadInstitution();
                    return View(); 
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex);
                
            }
            return View();
        }
        [ValidateUserPermission(Permissions = "can_delete_department")]
        public ActionResult DepartmentDelete(int ID)
        {
            try
            {
                _orgCMDBLL.DepartmentDelete(ID);

                TempData[Utilities.Activity_Log_Details] = $"department has been deleted";
                TempData["Message"] = "Success";

                return RedirectToAction("Departments");

            }
            catch (Exception ex)
            {
                _log.Error(ex);
              
            }

            ModelState.AddModelError("",$"Error deleting department : ID : {ID}");
            return View();
            
        }
        public void LoadInstitution()
        {
            var institutions = _orgQueryBLL.GetInstitutions();
            ViewData["Institution"] = new SelectList(institutions, "ID", "Name", "");
        }

    }
}