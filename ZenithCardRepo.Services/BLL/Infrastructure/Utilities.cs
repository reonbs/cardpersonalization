using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Services.BLL.Infrastructure
{
    public static class Utilities
    {
        public const string RegistrationType = "RegistrationType";
        public const string OrganisationCode = "OrganizationCode";
        public const string InstitutionID = "InstitutionID";
        public const string Activity_Log_Details = "Activity_Log_Details";
        public const string Approve = "Approve";
        public const string Decline = "Decline";
        public const int Approve_Rank = 99;
        public const int Decline_Rank = 98;

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        public static string GetValidationResponse(string code)
        {
            string response = string.Empty;
            switch (code)
            {
                case "00":
                    response = "Successful";
                    break;
                case "01":
                    response = "Eyes Not visible";
                    break;
                case "02":
                    response = "Face Not Found";
                    break;
                case "03":
                    response = "Multiple Faces";
                    break;
                case "04":
                    response = "Face Found";
                    break;
                case "05":
                    response = "Wrong Dimension";
                    break;
                case "06":
                    response = "Blur Image";
                    break;
                case "07":
                    response = "Invalid Image Size";
                    break;
                case "08":
                    response = "Invalid Image Format";
                    break;
                case "09":
                    response = "Head was tilted ";
                    break;
                case "10":
                    response = "Invalid Image";
                    break;
                default:
                    response = code;
                    break;
            }

            return response;
        }
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static async Task<bool> ExecuteEmail(string toEmail,string toName, CardEnums emailType)
        {

            var apiKey = ConfigurationManager.AppSettings["SendGrid_APIKey"];//Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient(apiKey);
            var emailFrom = ConfigurationManager.AppSettings["SendGrid_From"];
            var fromName = ConfigurationManager.AppSettings["SendGrid_FromName"];
            var from = new EmailAddress(emailFrom, fromName);
            var subject = ConfigurationManager.AppSettings["SendGrid_Subject"];
            var to = new EmailAddress(toEmail, toName);
            var plainTextContent = "Card Application";
            var htmlContent = HTMLContent(emailType);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return true;
        }

        public static string HTMLContent(CardEnums emailType)
        {
            StreamReader reader = null;
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string emailPath = string.Empty;
                string compPath = string.Empty;
                string body = string.Empty;

                if (CardEnums.CardApplicationEmail == emailType)
                {
                    emailPath = ConfigurationManager.AppSettings["Template_Application"];
                    compPath = path + emailPath;
                    reader = new StreamReader(compPath);
                    body = reader.ReadToEnd();
                }
                else if (CardEnums.CardApprovalEmail == emailType)
                {
                    emailPath = ConfigurationManager.AppSettings["Template_Approval"];
                    compPath = path + emailPath;
                    reader = new StreamReader(compPath);
                    body = reader.ReadToEnd();
                }
                else if (CardEnums.UserCreation == emailType)
                {
                    emailPath = ConfigurationManager.AppSettings["Template_Approval"];
                    compPath = path + emailPath;
                    reader = new StreamReader(compPath);
                    body = reader.ReadToEnd();
                    body = body.Replace("{UserName}", "");
                    body = body.Replace("{Password}", "");
                    body = body.Replace("{Institution}", "");
                    body = body.Replace("{Url}", "");

                }

                return body;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }


        }
    }
    public enum ValidationResponse
    {
        [Description("Successful - 00")]
        Successful,
        [Description("Eyes Not Visible - 01")]
        EyesNotVisible,
        [Description("Face Not Found - 02")]
        FaceNotFound,
        [Description("Multiple Faces - 03")]
        MultipleFaces,
        [Description("Face Found - 04")]
        FaceFound,
        [Description("Wrong Dimension - 05")]
        WrongDemension,
        [Description("Blur Image - 06")]
        BlurImage,
        [Description("Invalid Image Size - 07")]
        InvalidImageSize,
        [Description("Invalid Image Format - 08")]
        InvalidImageFormat,
        [Description("Invalid HeadTilt - 09")]
        HeadTilt,
        [Description("Invalid Image - 10")]
        InvalidImage,
        [Description("Token Verification Failed - 11")]
        TokenVerificationFailed,
        [Description("Unknown Error - 12")]
        UnknownError
    }

}
