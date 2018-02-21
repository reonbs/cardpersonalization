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
    public class ImageController : Controller
    {
        private IImageSettingCmdBLL _imgSettingCmdBLL;
        private IImageSettingQueryBLL _imgSettingQueryBLL;

        public ImageController(IImageSettingCmdBLL imgSettingCmdBLL,IImageSettingQueryBLL imgSettingQueryBLL)
        {
            _imgSettingCmdBLL = imgSettingCmdBLL;
            _imgSettingQueryBLL = imgSettingQueryBLL;
        }
        // GET: Image
        public ActionResult AddSetting()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSetting(ImageValidationSetting imageSetting)
        {
            try
            {
                _imgSettingCmdBLL.AddSetting(imageSetting);
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }

        public JsonResult GetImageSetting()
        {
            var imgSetting = _imgSettingQueryBLL.GetImageSetting();
            return Json(imgSetting,JsonRequestBehavior.AllowGet);

        }

        public ActionResult ImageSettings()
        {
            try
            {
                var imgSettings = _imgSettingQueryBLL.GetImageSettings();

                return View(imgSettings);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }
            //return View();
        }

        public ActionResult ImageSettingEdit()
        {
            try
            {
                var imgSetting = _imgSettingQueryBLL.GetImageSetting();
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
        public ActionResult ImageSettingEdit(ImageValidationSetting imgValSetting)
        {
            try
            {
                _imgSettingCmdBLL.UpdateSetting(imgValSetting);

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