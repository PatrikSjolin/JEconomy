namespace JEconomy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingSkipCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "Global", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "Global");
        }
    }
}
