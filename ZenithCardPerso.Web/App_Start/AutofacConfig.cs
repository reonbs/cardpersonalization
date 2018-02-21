using Autofac;
using Autofac.Integration.Mvc;
using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication6.IdentityModels;
using ZenithCardPerso.Repository.Command;
using ZenithCardPerso.Repository.Query;
using ZenithCardPerso.Repository.Utility;
using ZenithCardPerso.Web.Filters;
using ZenithCardPerso.Web.LoggingService;
using ZenithCardRepo.Data;
using ZenithCardRepo.Data.IdentityModels;
using ZenithCardRepo.Data.Models;
using ZenithCardRepo.Services.BLL.Command;
using ZenithCardRepo.Services.BLL.Infrastructure;
using ZenithCardRepo.Services.BLL.Query;

namespace ZenithCardPerso.Web.App_Start
{
    public class AutofacConfig
    {
        public static void Register(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            //var config = GlobalConfiguration.Configuration;

            // Register the controller in scope 
            //Register  MVC controllers all at once using assembly scanning...
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            //Register webAPI Controller
            //builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            //builder.RegisterWebApiFilterProvider(config);

           

            builder.RegisterType<ApplicationDbContext>().InstancePerLifetimeScope();//.InstancePerRequest();
            //builder.RegisterType<ApplicationDbContext>().InstancePerBackgroundJob();


            builder.RegisterGeneric(typeof(QueryRepository<>)).As(typeof(IQueryRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(CommandRepository<>)).As(typeof(ICommandRepository<>)).InstancePerLifetimeScope();


            builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationRoleManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationSignInManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser>>().InstancePerRequest();
            //builder.RegisterType<RoleStore<ApplicationRole>>().As<IRoleStore<ApplicationRole, string>>().InstancePerRequest();
            builder.RegisterType<ApplicationRoleStore>().As<IRoleStore<ApplicationRole, string>>().InstancePerRequest();
            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();
            builder.Register<IDataProtectionProvider>(c => app.GetDataProtectionProvider()).InstancePerRequest();

            builder.RegisterType<CardApplicationCmdBLL>().As<ICardApplicationCmdBLL>();
            builder.RegisterType<GetApplicationLegends>().As<IGetApplicationLegends>();
            builder.RegisterType<CardApplicationQueryBLL>().As<ICardApplicationQueryBLL>();
            builder.RegisterType<OrganisationCmdBLL>().As<IOrganisationCmdBLL>();
            builder.RegisterType<Repository.Utility.Utilities>().As<IUtilities>();
            builder.RegisterType<ImageSettingCmdBLL>().As<IImageSettingCmdBLL>();
            builder.RegisterType<ImageSettingQueryBLL>().As<IImageSettingQueryBLL>();
            builder.RegisterType<ImageService>().As<IImageService>();
            builder.RegisterType<OrganisationQueryBLL>().As<IOrganisationQueryBLL>();
            builder.RegisterType<FilterCMDBLL>().As<IFilterCMDBLL>().InstancePerRequest();
            builder.RegisterType<ReportQueryBLL>().As<IReportQueryBLL>();
            builder.RegisterType<ManageUserQueryBLL>().As<IManageUserQueryBLL>();
            builder.RegisterType<ManageUserCMDBLL>().As<IManageUserCMDBLL>();


            builder.RegisterModule(new LoggerModule());

            // OPTIONAL: Enable property injection into action filters
            builder.RegisterFilterProvider();
            builder.RegisterType<AuditAttribute>().PropertiesAutowired();

            // BUILD THE CONTAINER
            var container = builder.Build();

            // REPLACE THE MVC DEPENDENCY RESOLVER WITH AUTOFAC
            GlobalConfiguration.Configuration.UseAutofacActivator(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // REGISTER WITH OWIN
            //app.UseAutofacMiddleware(container);
            //app.UseAutofacMvc();
        }
    }
}