namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProcessCard : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.ProcessedCards");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProcessedCards",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BatchNo = c.String(),
                        DownloadLink = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
    }
}
