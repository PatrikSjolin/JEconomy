namespace JEconomy.Migrations
{
    using JEconomy.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<JEconomy.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(JEconomy.Models.ApplicationDbContext context)
        {
            //context.Database.ExecuteSqlCommand("delete from Transactions");
            //context.Database.ExecuteSqlCommand("delete from Categories");
            context.Categories.Add(new Category
            {
                Global = true,
                Id = Guid.NewGuid(),
                IdentityUser = null,
                Name = "Skip"
            });
            //context.Categories.Add(new Category
            //{
            //    Id = Guid.NewGuid(),
            //    Key = "Misc",
            //    Name = "Misc"
            //});
            context.SaveChanges();
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
