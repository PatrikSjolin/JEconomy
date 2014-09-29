namespace JEconomy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingKeyToCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "Key", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "Key");
        }
    }
}
