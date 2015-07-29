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
            
            //Create Default Admin 
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

            //Add Default Admin to Admin Role
            var userId = userManager.FindByEmail(adminEmail).Id;
            userManager.AddToRole(userId, "Admin");




            //Create Project Manager Role
            var roleManager2 = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager2.Create(new IdentityRole { Name = "ProjectManger" });
            }

            //Create Default Project Manager 
            var userManger2 = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            var pManagerEmail = ConfigurationManager.AppSettings["PMEmail"];

            if (!context.Users.Any(u => u.Email == pManagerEmail))
            {
                userManger2.Create(new ApplicationUser
                    {
                        UserName = ConfigurationManager.AppSettings["PMEmail"],
                        Email = ConfigurationManager.AppSettings["PMEmail"],
                        FirstName = ConfigurationManager.AppSettings["PMFirstName"],
                        LastName = ConfigurationManager.AppSettings["PMLastName"],
                        DisplayName = ConfigurationManager.AppSettings["PMDisplayName"]
                    }, ConfigurationManager.AppSettings["PMPassword"]);
            }

            //Add Default Project Manager to Project Manger Role
            var pmId = userManger2.FindByEmail(pManagerEmail).Id;
            userManger2.AddToRole(pmId, "ProjectManger");




            //Create Developer Role
            var roleManager3 = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager3.Create(new IdentityRole { Name = "Developer" });
            }

            //Create Default Developer
            var userManger3 = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            var developerEmail = ConfigurationManager.AppSettings["DVEmail"];

            if (!context.Users.Any(u => u.Email == developerEmail))
            {
                userManger3.Create(new ApplicationUser
                {
                    UserName = ConfigurationManager.AppSettings["DVEmail"],
                    Email = ConfigurationManager.AppSettings["DVEmail"],
                    FirstName = ConfigurationManager.AppSettings["DVFirstName"],
                    LastName = ConfigurationManager.AppSettings["DVLastName"],
                    DisplayName = ConfigurationManager.AppSettings["DVDisplayName"]
                }, ConfigurationManager.AppSettings["DVPassword"]);
            }

            //Add Default Developer to Developer Role
            var dvId = userManger3.FindByEmail(developerEmail).Id;
            userManger3.AddToRole(dvId, "Developer");




            //Create Default Submitter Role
            var roleManager4 = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager4.Create(new IdentityRole { Name = "Submitter" });
            }

            //Create Default Submitter
            var userManager4 = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            var submitterEmail = ConfigurationManager.AppSettings["SMEmail"];

            if (!context.Users.Any(u => u.Email == submitterEmail))
            {
                userManager4.Create(new ApplicationUser
                {
                    UserName = ConfigurationManager.AppSettings["SMEmail"],
                    Email = ConfigurationManager.AppSettings["SMEmail"],
                    FirstName = ConfigurationManager.AppSettings["SMFirstName"],
                    LastName = ConfigurationManager.AppSettings["SMLastName"],
                    DisplayName = ConfigurationManager.AppSettings["SMDisplayName"]
                }, ConfigurationManager.AppSettings["SMPassword"]);
            }

            //Add Default Submitter to Submitter Role
            var smId = userManager4.FindByEmail(submitterEmail).Id;
            userManager4.AddToRole(smId, "Submitter");

        }
    }
}
