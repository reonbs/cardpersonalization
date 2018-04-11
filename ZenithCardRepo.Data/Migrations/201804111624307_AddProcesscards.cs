namespace ZenithCardRepo.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProcesscards : DbMigration
    {
        public override void Up()
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
        
        public override void Down()
        {
            DropTable("dbo.ProcessedCards");
        }
    }
}
