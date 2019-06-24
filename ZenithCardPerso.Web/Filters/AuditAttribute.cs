using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using ZenithCardRepo.Data;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Services.BLL.Command;
using ZenithCardRepo.Services.BLL.Infrastructure;

namespace ZenithCardPerso.Web.Filters
{
    public class AuditAttribute : ActionFilterAttribute
    {
        //private IFilterCMDBLL _filterCMDBLL { get; set; }
        public int AuditingLevel { get; set; }
        public ILog _log;

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    // Stores the Request in an Accessible object
        //    var request = filterContext.HttpContext.Request;

        //    //var sessionIdentifier = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(request.Cookies[FormsAuthentication.FormsCookieName].Value)).Select(s => s.ToString("x2")));

        //    // Generate an audit
        //    AuditRecord auditRecord = new AuditRecord()
        //    {
        //        //SessionID = sessionIdentifier,
        //        // Your Audit Identifier     
        //        AuditRecordID = Guid.NewGuid(),
        //        // Our Username (if available)
        //        UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
        //        // The IP Address of the Request
        //        IPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
        //        // The URL that was accessed
        //        AreaAccessed = request.RawUrl,
        //        // Creates our Timestamp
        //        TimeAccessed = DateTime.UtcNow,
        //        Data = SerializeRequest(request),
        //        Module = request.RequestContext.RouteData.Values["Controller"].ToString(),
        //        Action = request.RequestContext.RouteData.Values["Action"].ToString()
        //    };

        //    // Stores the Audit in the Database
        //    //_filterCMDBLL.AddAuditRecord(auditRecord);
        //    using (ApplicationDbContext db = new ApplicationDbContext())
        //    {
        //        db.AuditRecords.Add(auditRecord);
        //        db.SaveChanges();
        //    }

        //    base.OnActionExecuting(filterContext);
        //}

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                
                // Stores the Request in an Accessible object
                if (filterContext != null)
                {
                    var request = filterContext.HttpContext.Request;

                    ClaimsIdentity claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                    var instID = claimsIdentity.FindFirst(Utilities.InstitutionID);

                    if (instID != null)
                    {
                        var institutionID = int.Parse(instID.Value);

                        var description = "No user Action took place";

                        if (filterContext.Controller.TempData.Count > 0)
                        {
                            //if (!string.IsNullOrEmpty(filterContext.Controller.TempData[Utilities.Activity_Log_Details].ToString()))
                            //{
                            //    description = filterContext.Controller.TempData[Utilities.Activity_Log_Details].ToString();
                            //}
                            description = filterContext.Controller.TempData[Utilities.Activity_Log_Details].ToString();
                        }
                        else
                        {
                            description = "No user Action took place";
                        }

                        //var sessionIdentifier = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(request.Cookies[FormsAuthentication.FormsCookieName].Value)).Select(s => s.ToString("x2")));

                        // Generate an audit
                        AuditRecord auditRecord = new AuditRecord()
                        {
                            //SessionID = sessionIdentifier,
                            // Your Audit Identifier     
                            AuditRecordID = Guid.NewGuid(),
                            // Our Username (if available)
                            UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
                            // The IP Address of the Request
                            IPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
                            // The URL that was accessed
                            AreaAccessed = request.RawUrl,
                            // Creates our Timestamp
                            TimeAccessed = DateTime.UtcNow,
                            Data = SerializeRequest(request),
                            Module = request.RequestContext.RouteData.Values["Controller"].ToString(),
                            Action = request.RequestContext.RouteData.Values["Action"].ToString(),
                            Description = description,
                            InstitutionID = Convert.ToInt32(institutionID)
                        };

                        // Stores the Audit in the Database
                        //_filterCMDBLL.AddAuditRecord(auditRecord);
                        using (ApplicationDbContext db = new ApplicationDbContext())
                        {
                            db.AuditRecords.Add(auditRecord);
                            db.SaveChanges();
                        }

                        base.OnActionExecuted(filterContext);
                    }

                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);

            }
            finally
            {

            }
        }
        private string SerializeRequest(HttpRequestBase request)
        {
            switch (AuditingLevel)
            {
                // No Request Data will be serialized
                case 0:
                default:
                    return "";
                // Basic Request Serialization - just stores Data
                case 1:
                    return Json.Encode(new { request.Cookies, request.Headers, request.Files });
                // Middle Level - Customize to your Preferences
                case 2:
                    return Json.Encode(new { request.Cookies, request.Headers, request.Files, request.Form, request.QueryString, request.Params });
                // Highest Level - Serialize the entire Request object (As mentioned earlier, this will blow up)
                case 3:
                    // We can't simply just Encode the entire 
                    // request string due to circular references 
                    // as well as objects that cannot "simply" 
                    // be serialized such as Streams, References etc.
                    return Json.Encode(request);
            }
        }
    }
}