namespace JEconomy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingCategoryAndTransactions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        Value = c.Double(nullable: false),
                        Balance = c.Double(nullable: false),
                        Place = c.String(),
                        Category_Id = c.Guid(),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.IdentityUser_Id)
                .Index(t => t.Category_Id)
                .Index(t => t.IdentityUser_Id);
            
            AlterColumn("dbo.AspNetUsers", "UserName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "IdentityUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Transactions", "Category_Id", "dbo.Categories");
            DropIndex("dbo.Transactions", new[] { "IdentityUser_Id" });
            DropIndex("dbo.Transactions", new[] { "Category_Id" });
            AlterColumn("dbo.AspNetUsers", "UserName", c => c.String());
            DropTable("dbo.Transactions");
            DropTable("dbo.Categories");
        }
    }
}
