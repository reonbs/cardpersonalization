using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Services.BLL.Command;
using ZenithCardRepo.Services.BLL.Query;

namespace ZenithCardPerso.Web.Controllers
{
    public class LegendController : Controller
    {
        private ILog _log;
        private ILegendCMDBLL _legendCMDBLL;
        private ILegendQueryBLL _legendQueryBLL;

        public LegendController(ILog log, ILegendCMDBLL legendCMDBLL, ILegendQueryBLL legendQueryBLL)
        {
            _log = log;
            _legendCMDBLL = legendCMDBLL;
            _legendQueryBLL = legendQueryBLL;
        }
        // GET: Legend
        public ActionResult Title()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Title(TitleCode title)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var codeExists = _legendQueryBLL.VerifyTitleCode(title.Code);
                    if (codeExists)
                    {
                        ModelState.AddModelError("", "Title Code exists.. Enter a different code");
                        return View(title);
                    }
                    _legendCMDBLL.ADDTitle(title);

                    TempData["Message"] = "Success";

                    ModelState.Clear();
                    return View();

                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }

        public ActionResult Titles()
        {
            try
            {
                var titles = _legendQueryBLL.Titles();
                return View(titles);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return View();
            }

        }
        public ActionResult TitleEdit(int ID)
        {
            try
            {
                if (ID > 0)
                {
                    var title = _legendQueryBLL.GetTitleByID(ID);

                    return View(title);
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex);

            }

            ModelState.AddModelError("", "Select a valid title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TitleEdit(TitleCode title)
        {
            try
            {
                _legendCMDBLL.UpdateTite(title);

                TempData["Message"] = "Success";


                return RedirectToAction("Titles");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }

        public ActionResult Sexes()
        {
            try
            {
                var sexes = _legendQueryBLL.Sexes();

                return View(sexes);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }

        public ActionResult SexEdit(int ID)
        {
            try
            {
                if (ID > 0)
                {
                    var sex = _legendQueryBLL.GetSex(ID);
                    return View(sex);
                }


            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Select a valid sex");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SexEdit(Sex sex)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.UpdateSex(sex);

                    TempData["Message"] = "Success";

                    return View();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Update failed");
            return View(sex);
        }
        public ActionResult AddMaritalStatus()
        {
            return View();
        }
        public ActionResult AddMaritalStatus(MaritalStatus maritalStatus)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.AddMaritasStatus(maritalStatus);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }

        public ActionResult MaritalStatusEdit(int ID)
        {
            try
            {
                if (ID > 0)
                {
                    var maritalStatus = _legendQueryBLL.MaritalStatus(ID);

                    return View(maritalStatus);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Select a Marital status to edit");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MaritalStatusEdit(MaritalStatus maritalStatus)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.UpdateMaritalStatus(maritalStatus);

                    TempData["Message"] = "Success";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }

        public ActionResult MaritalStatusList()
        {
            try
            {
                var maritalStatuses = _legendQueryBLL.MaritalStatusList();

                return View(maritalStatuses);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }
        public ActionResult AddCity()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCity(City city)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.AddCity(city);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Enter Required details");
            return View(city);
        }

        public ActionResult CityEdit(int ID)
        {
            try
            {
                if (ID > 0)
                {
                    var city = _legendQueryBLL.City(ID);

                    return View(city);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Select a city to edit");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CityEdit(City city)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.UpdateCity(city);

                    TempData["Message"] = "Success";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return View(city);
        }

        public ActionResult Cities()
        {
            try
            {
                var cityList = _legendQueryBLL.CityList();

                return View(cityList);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }

        public ActionResult AddState()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddState(State state)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.AddState(state);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Enter Required details");
            return View(state);
        }

        public ActionResult StateEdit(int ID)
        {
            try
            {
                if (ID > 0)
                {
                    var city = _legendQueryBLL.State(ID);

                    return View(city);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Select a city to edit");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StateEdit(State state)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.UpdateState(state);

                    TempData["Message"] = "Success";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return View(state);
        }

        public ActionResult States()
        {

            try
            {
                var stateList = _legendQueryBLL.StateList();

                return View(stateList);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }

        public ActionResult AddIDCardType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddIDCardType(IDCardType idCardType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.AddIDCardType(idCardType);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Enter Required details");
            return View(idCardType);
        }

        public ActionResult IDCardTypeEdit(int ID)
        {
            try
            {
                if (ID > 0)
                {
                    var idCardType = _legendQueryBLL.IDCardType(ID);

                    return View(idCardType);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Select a city to edit");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IDCardTypeEdit(IDCardType idCardType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.UpdateIDCardType(idCardType);

                    TempData["Message"] = "Success";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return View(idCardType);
        }

        public ActionResult IDCardTypes()
        {
            try
            {
                var idCardTypes = _legendQueryBLL.IDCardTypes();

                return View(idCardTypes);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }

        public ActionResult AddSocioProfCode()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSocioProfCode(SocioProfCode socioProfCode)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.AddSocioProfCode(socioProfCode);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Enter Required details");
            return View(socioProfCode);
        }

        public ActionResult SocioProfCodeEdit(int ID)
        {
            try
            {
                if (ID > 0)
                {
                    var socioProfCode = _legendQueryBLL.SocioProfCode(ID);

                    return View(socioProfCode);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Select a socio prof code to edit");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SocioProfCodeEdit(SocioProfCode socioProfCode)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.UpdateSocioProfCode(socioProfCode);

                    TempData["Message"] = "Success";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return View(socioProfCode);
        }

        public ActionResult SocioProfCodes()
        {
            try
            {
                var socioProfCodes = _legendQueryBLL.SocioProfCodes();

                return View(socioProfCodes);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }
        public ActionResult AddProductCode()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProductCode(ProductCode productCode)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.AddProductCode(productCode);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Enter Required details");
            return View(productCode);
        }

        public ActionResult ProductCodeEdit(int ID)
        {
            try
            {
                if (ID > 0)
                {
                    var productCode = _legendQueryBLL.ProductCode(ID);

                    return View(productCode);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Select a product code to edit");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductCodeEdit(ProductCode productCode)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.UpdateProductCode(productCode);

                    TempData["Message"] = "Success";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return View(productCode);
        }

        public ActionResult ProductCodes()
        {
            try
            {
                var productCodes = _legendQueryBLL.ProductCodes();

                return View(productCodes);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }

        public ActionResult AddNationalityCode()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNationalityCode(NationalityCode nationalityCode)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.AddNationalityCode(nationalityCode);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Enter Required details");
            return View(nationalityCode);
        }

        public ActionResult NationalityCodeEdit(int ID)
        {
            try
            {
                if (ID > 0)
                {
                    var nationalityCode = _legendQueryBLL.NationalityCode(ID);

                    return View(nationalityCode);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            ModelState.AddModelError("", "Select a nationality code to edit");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NationalityCodeEdit(NationalityCode nationalityCode)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _legendCMDBLL.UpdateNationalityCode(nationalityCode);

                    TempData["Message"] = "Success";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return View(nationalityCode);
        }
        public ActionResult NationalityCodes()
        {
            try
            {
                var nationalityCodes = _legendQueryBLL.NationalityCodes();

                return View(nationalityCodes);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return View();
        }
    }
}