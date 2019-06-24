namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmodels5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CardApplications", "FirstName", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.CardApplications", "MiddleName", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.CardApplications", "LastName", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.CardApplications", "Sex", c => c.String(nullable: false, maxLength: 1));
            AlterColumn("dbo.CardApplications", "MaritalStatus", c => c.String(nullable: false, maxLength: 1));
            AlterColumn("dbo.CardApplications", "OfficePhoneNo", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("dbo.CardApplications", "GSMNo", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("dbo.CardApplications", "EmailAddress", c => c.String(maxLength: 50));
            AlterColumn("dbo.CardApplications", "OfficeAddress1", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.CardApplications", "OfficeAddress2", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.CardApplications", "City", c => c.String(nullable: false, maxLength: 5));
            AlterColumn("dbo.CardApplications", "State", c => c.String(nullable: false, maxLength: 3));
            AlterColumn("dbo.CardApplications", "MainAccountNo", c => c.String(maxLength: 10));
            AlterColumn("dbo.CardApplications", "OtherAccountNo", c => c.String(maxLength: 10));
            AlterColumn("dbo.CardApplications", "NameonCard", c => c.String(nullable: false, maxLength: 23));
            AlterColumn("dbo.CardApplications", "IDCardType", c => c.String(nullable: false, maxLength: 2));
            AlterColumn("dbo.CardApplications", "IDNo", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.CardApplications", "SocioProfCode", c => c.String(nullable: false, maxLength: 3));
            AlterColumn("dbo.CardApplications", "TitleCode", c => c.String(nullable: false, maxLength: 2));
            AlterColumn("dbo.CardApplications", "Nationality", c => c.String(nullable: false, maxLength: 3));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CardApplications", "Nationality", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "TitleCode", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "SocioProfCode", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "IDNo", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "IDCardType", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "NameonCard", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "OtherAccountNo", c => c.String());
            AlterColumn("dbo.CardApplications", "MainAccountNo", c => c.String());
            AlterColumn("dbo.CardApplications", "State", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "City", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "OfficeAddress2", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "OfficeAddress1", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "EmailAddress", c => c.String());
            AlterColumn("dbo.CardApplications", "GSMNo", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "OfficePhoneNo", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "MaritalStatus", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "Sex", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "MiddleName", c => c.String(nullable: false));
            AlterColumn("dbo.CardApplications", "FirstName", c => c.String(nullable: false));
        }
    }
}
