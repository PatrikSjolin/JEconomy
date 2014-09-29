namespace JEconomy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovingKeyFromCategory : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Categories", "Key");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Categories", "Key", c => c.String());
        }
    }
}
