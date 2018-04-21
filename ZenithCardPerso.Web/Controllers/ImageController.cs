using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZenithCardPerso.Web.Filters;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Services.BLL.Command;
using ZenithCardRepo.Services.BLL.Infrastructure;
using ZenithCardRepo.Services.BLL.Query;

namespace ZenithCardPerso.Web.Controllers
{
    public class ImageController : Controller
    {
        private IImageSettingCmdBLL _imgSettingCmdBLL;
        private IImageSettingQueryBLL _imgSettingQueryBLL;
        private ILog _log;

        public ImageController(IImageSettingCmdBLL imgSettingCmdBLL,IImageSettingQueryBLL imgSettingQueryBLL, ILog log)
        {
            _imgSettingCmdBLL = imgSettingCmdBLL;
            _imgSettingQueryBLL = imgSettingQueryBLL;
            _log = log;
        }
        // GET: Image
        public ActionResult AddSetting()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        [ValidateUserPermission(Permissions ="can_add_imagesetting")]
        public ActionResult AddSetting(ImageValidationSetting imageSetting)
        {
            try
            {
                _imgSettingCmdBLL.AddSetting(imageSetting);

                TempData[Utilities.Activity_Log_Details] = $"Image setting has been added";

                TempData["Message"] = "Success";
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return View(imageSetting);
        }

        public JsonResult GetImageSetting()
        {
            var imgSetting = _imgSettingQueryBLL.GetImageSetting();
            return Json(imgSetting,JsonRequestBehavior.AllowGet);

        }

        [ValidateUserPermission(Permissions = "can_view_imagesetting")]
        [Audit]
        public ActionResult ImageSettings()
        {
            try
            {
                var imgSettings = _imgSettingQueryBLL.GetImageSettings();
                TempData[Utilities.Activity_Log_Details] = $"Image setting was viewed";
                return View(imgSettings);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }
            //return View();
        }
        [ValidateUserPermission(Permissions = "can_view_imagesetting")]
        [Audit]
        public ActionResult ImageSettingEdit()
        {
            try
            {
                var imgSetting = _imgSettingQueryBLL.GetImageSetting();
                TempData[Utilities.Activity_Log_Details] = $"Image setting edit was viewed";
                return View(imgSetting);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUserPermission(Permissions = "can_edit_imagesetting")]
        [Audit]
        public ActionResult ImageSettingEdit(ImageValidationSetting imgValSetting)
        {
            try
            {
                _imgSettingCmdBLL.UpdateSetting(imgValSetting);

                TempData[Utilities.Activity_Log_Details] = $"Image setting was edited";
                TempData["Message"] = "Success";

                return View();
            }
            catch (Exception)
            {

                throw;
            }
            
        }


    }
}