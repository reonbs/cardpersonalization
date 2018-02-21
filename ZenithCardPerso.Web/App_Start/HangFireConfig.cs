using Hangfire;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZenithCardPerso.Web.App_Start
{
    public class HangFireConfig
    {
        public static void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}