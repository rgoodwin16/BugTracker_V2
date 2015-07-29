namespace BugTracker_V2.Migrations
{
    using BugTracker_V2.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker_V2.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "BugTracker_V2.Models.ApplicationDbContext";
        }

        protected override void Seed(BugTracker_V2.Models.ApplicationDbContext context)
        {
            //Create Admin Role
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            var adminEmail = ConfigurationManager.AppSettings["AdminEmail"];

            if (!context.Users.Any(u => u.Email == adminEmail))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = ConfigurationManager.AppSettings["AdminEmail"],
                    Email = ConfigurationManager.AppSettings["AdminEmail"],
                    FirstName = ConfigurationManager.AppSettings["AdminFirstName"],
                    LastName = ConfigurationManager.AppSettings["AdminLastName"],
                    DisplayName = ConfigurationManager.AppSettings["AdminDisplayName"]
                }, ConfigurationManager.AppSettings["AdminPassword"]);
            }

            var userId = userManager.FindByEmail(adminEmail).Id;
            userManager.AddToRole(userId, "Admin");
        }
    }
}
