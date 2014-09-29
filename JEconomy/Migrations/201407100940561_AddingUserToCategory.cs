namespace JEconomy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingUserToCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "IdentityUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Categories", "IdentityUser_Id");
            AddForeignKey("dbo.Categories", "IdentityUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Categories", "IdentityUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Categories", new[] { "IdentityUser_Id" });
            DropColumn("dbo.Categories", "IdentityUser_Id");
        }
    }
}
