namespace CSC_Task_6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateStripeEventTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StripeEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Json = c.String(),
                        Date = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StripeEvents", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.StripeEvents", new[] { "User_Id" });
            DropTable("dbo.StripeEvents");
        }
    }
}
