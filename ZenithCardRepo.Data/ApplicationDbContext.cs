using ZenithCardRepo.Data.IdentityModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZenithCardRepo.Data.Models;

namespace ZenithCardRepo.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext():base("DefaultConnection")
        {

        }

        public DbSet<CardApplication> CardApplications { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<IDCardType> IDCardTypes { get; set; }
        public DbSet<MaritalStatus> MaritalStatus { get; set; }
        public DbSet<NationalityCode> NationalityCode { get; set; }
        public DbSet<ProductCode> ProductCodes { get; set; }
        public DbSet<Sex> Sex { get; set; }
        public DbSet<SocioProfCode> SocioProfCodes { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<TitleCode> TitleCodes { get; set; }
        public DbSet<OrganizationProfile> OrganizationProfiles { get; set; }
        public DbSet<ImageValidationSetting> ImageValidationSettings { get; set; }

        public DbSet<Institution> Institutions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<AuditRecord> AuditRecords { get; set; }

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }


    }


}
