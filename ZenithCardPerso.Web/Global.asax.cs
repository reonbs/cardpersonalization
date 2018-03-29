
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ZenithCardPerso.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private ILog _log;
        protected void Application_Start()
        {
            //log4net configuration region
            XmlConfigurator.Configure();
            _log = log4net.LogManager.GetLogger("");
            _log.InfoFormat(" <<<< Staring ::: Zentih Card APP checking Startup Config Setting @ : {0} >>>>>>", DateTime.Now);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            _log.InfoFormat(" <<<< Ending ::: Zentih Card APP Startup Config Setting ok @ : {0} >>>>>>", DateTime.Now);

        }
    }
}
