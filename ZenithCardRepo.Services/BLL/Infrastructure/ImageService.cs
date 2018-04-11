using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZenithCardPerso.Infrastructure;
using ZenithCardRepo.Services.BLL.Infrastructure;
using ZenithCardRepo.Services.BLL.Query;

namespace ZenithCardRepo.Services.BLL.Infrastructure
{
    public class ImageService : IImageService
    {
        private IImageSettingQueryBLL _imgSettingQueryBLL;
        private ILog _log;
        public ImageService(IImageSettingQueryBLL imgSettingQueryBLL, ILog log)
        {
            _imgSettingQueryBLL = imgSettingQueryBLL;
            _log = log;
        }
        public string ImageURL(string url)
        {
            try
            {
                //POXS200001
                string ext = url.Length <= 4 ? "" : url.Substring(url.Length - 4);
                string value = "";
                if (!string.IsNullOrEmpty(ext))
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory;
                    string fullpath = path + "ImageUpload";
                    if (!Directory.Exists(fullpath))
                    {
                        Directory.CreateDirectory(fullpath);
                    }

                    //get the full path
                    string n = string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}.bin", DateTime.Now);
                    string filename = fullpath + "\\" + n + ext;
                    //string filename = fullpath + "\\imagesample" + ext;

                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(new Uri(url), filename);
                    }

                    // var isValid = ValidationBL.VerifyIfTokenExist(token);

                    //value = ValidateImage(filename, "URL", "", url);


                    //Delete the file
                    try
                    {
                        if (File.Exists(filename))
                        {
                            File.Delete(filename);
                        }
                    }
                    catch (Exception)
                    {
                    }

                }

                return value;
            }
            catch (Exception ex)
            {
                ValidationResponse enumVar = ValidationResponse.UnknownError;
                string value = Utilities.GetEnumDescription(enumVar);
                ex.Message.ToString();
                return value;
            }
        }

        public List<string> ImageBase64String(string base64Str)
        {
            string filename = string.Empty;
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string fullpath = path + "ImageUpload";
                //check if the destination folder exist
                if (!Directory.Exists(fullpath))
                {
                    Directory.CreateDirectory(fullpath);
                }
                //get the full path
                string n = string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}.bin", DateTime.Now);
                filename = fullpath + "\\" + n + ".jpg";

                //convert base64string to byte array
                string toReplace = $"data:image/jpeg;base64,";
                string img = base64Str.Replace(toReplace, string.Empty);

                Bitmap bmpReturn = null;
                byte[] byteBuffer = Convert.FromBase64String(img);

                using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
                {
                    memoryStream.Position = 0;
                    bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

                    //Save the image to physical path
                    Bitmap bm3 = new Bitmap(bmpReturn);
                    bm3.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                    bmpReturn.Dispose();
                    bm3.Dispose();

                    memoryStream.Close();
                    memoryStream.Dispose();
                    //memoryStream = null;
                    byteBuffer = null;
                }

                if (File.Exists(filename))
                {
                    List<string> value = new List<string>() ;
                    value = ValidateImage(filename, "Base64String", img, "");

                    //Delete the file
                    try
                    {
                        //File.Delete(filename);
                        if (File.Exists(filename))
                        {
                            File.Delete(filename);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    return value;
                }
                else
                {
                    return new List<string> { "Convertion from base64string failed." };
                }

                //return new List<string>();

            }
            catch (Exception ex)
            {
                //ValidationResponse enumVar = ValidationResponse.UnknownError;
                //string value = Utilities.GetEnumDescription(enumVar);
                //ex.Message.ToString();
                //return value;
                throw;
            }
            //finally
            //{
            //    if (File.Exists(filename))
            //    {
            //        File.Delete(filename);
            //    }
            //}

            //return new List<string>();

        }

        //public string ValidateImage1(string imagePath, string validationMethod, string base64Str, string imgUrl)
        //{
        //    throw new NotImplementedException();
        //}

        public List<string> ValidateImage(string imagePath, string validationMethod, string base64Str, string imgUrl)
        {
            try
            {
                var imgSetting = _imgSettingQueryBLL.GetImageSetting();
                //var b = ImageValidator.SingleFaceDetection(imagePath);
                List<string> valMsgs = new List<string>();

                StringBuilder sb = new StringBuilder();
                int counter = 0;

                string responseCode = "";
                string validationType = string.Empty;

                if (true)
                {
                    validationType = "Eyes Validation - ";
                    responseCode = ImageValidator.EyesValidation(imagePath);
                    var desc = validationType + Utilities.GetValidationResponse(responseCode);
                    valMsgs.Add(desc);

                }

                if (true)
                {

                    validationType = "Single Face Detection - ";
                    responseCode = ImageValidator.SingleFaceDetection(imagePath);
                    var desc = validationType + Utilities.GetValidationResponse(responseCode);
                    valMsgs.Add(desc);

                }

                if (true)
                {
                    validationType = "Multiple Faces Validation - ";
                    //int noOfFaces = item.NoOfFaces == null ? 0 : (int)item.NoOfFaces;
                    responseCode = ImageValidator.MultipleFaceDetection(imagePath, 1);
                    var desc = validationType + Utilities.GetValidationResponse(responseCode);
                    valMsgs.Add(desc);
                }




                if (imgSetting.HeadTilt != null)
                {
                    validationType = "Head Tilt - ";
                    int degreeOfTilt = Convert.ToInt32(imgSetting.HeadTilt);
                    responseCode = ImageValidator.HeadTilt(imagePath, degreeOfTilt);
                    var desc = validationType + Utilities.GetValidationResponse(responseCode);
                    valMsgs.Add(desc);

                }

                //if (true)
                //{
                //    //validationType = "Resolution Check";
                //    responseCode = "10"; //ResolutionCheck(imagePath);
                //    var desc = Utilities.GetValidationResponse(responseCode);
                //    valMsgs.Add(desc);
                    
                //}


                if (true)
                {
                    validationType = "Image Dimension - ";
                    responseCode = ImageValidator.Dimension(imgSetting.Width, imgSetting.Height, imagePath);
                    var desc = validationType + Utilities.GetValidationResponse(responseCode);
                    valMsgs.Add(desc);
                }


                if (true)
                {
                    validationType = "Blur Detection - ";
                    responseCode = ImageValidator.BlurDetection(imagePath);
                    var desc = validationType + Utilities.GetValidationResponse(responseCode);
                    valMsgs.Add(desc);
                }

                if (true)
                {

                    validationType = "Facial Scan Validation - ";
                    responseCode = ImageValidator.InvalidImage(imagePath);
                    var desc = validationType + Utilities.GetValidationResponse(responseCode);
                    valMsgs.Add(desc);

                }


                if (true)
                {
                    validationType = "Image Size Validation - ";
                    responseCode = ImageValidator.ImageSize(imgSetting.ImageSize, imagePath);
                    var desc = validationType + Utilities.GetValidationResponse(responseCode);
                    valMsgs.Add(desc);
                }


                //if (true)
                //{
                //    validationType = "Image Format Validation - ";
                //    responseCode = ImageValidator.ImageFormatvalidator(imgSetting.ImageFormat, imagePath);
                //    var desc = validationType + Utilities.GetValidationResponse(responseCode);
                //    valMsgs.Add(desc);
                //}

                return valMsgs;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }

            return new List<string> { };
        }
    }
}
