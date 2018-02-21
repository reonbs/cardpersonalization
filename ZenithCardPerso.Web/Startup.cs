using AutoMapper;
using Microsoft.Owin;
using Owin;
using ZenithCardPerso.Web.App_Start;

[assembly: OwinStartupAttribute(typeof(ZenithCardPerso.Web.Startup))]
namespace ZenithCardPerso.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AutofacConfig.Register(app);
            
            HangFireConfig.Configuration(app);
            ConfigureAuth(app);
        }
    }
}
