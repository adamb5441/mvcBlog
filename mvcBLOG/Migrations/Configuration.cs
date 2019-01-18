namespace mvcBLOG.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using mvcBLOG.Models;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<mvcBLOG.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(mvcBLOG.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            
       
            //Add admin if one does not exist
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if(!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "mod@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "mod@gmail.com",
                    Email = "mod@gmail.com",
                    FirstName = "mod",
                    LastName = "the man",
                    DisplayName = "mod1"
                }, "Abc!123");
            }
            var userId = userManager.FindByEmail("mod@gmail.com").Id;
            userManager.AddToRole(userId, "Moderator");

            if (!context.Users.Any(u => u.Email == "adamb5441@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "adamb5441@gmail.com",
                    Email = "adamb5441@gmail.com",
                    FirstName = "Adam",
                    LastName = "Brown",
                    DisplayName = " TacoMan5441"
                }, "Abc!123");
            }

            userId = userManager.FindByEmail("adamb5441@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");
        }
    }
}
