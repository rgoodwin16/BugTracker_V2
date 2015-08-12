using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BugTracker_V2.Models;

namespace BugTracker_V2.Controllers
{
    [Authorize(Roles="Admin")]
    [RequireHttps]
    public class AdminController : Controller
    {

        UserRolesHelper helper = new UserRolesHelper();
        ApplicationDbContext db = new ApplicationDbContext();
        
        
        // ==============================================
        // USER / USERROLES - GET/EDIT
        // ============================================== 

        //GET: Users/UserRoles
        public ActionResult Users()
        {

            var roles = db.Roles.ToList();
            var users = db.Users.ToList();

            var model = new UserRoles
            {
                UserList = users,
                RoleList = roles,
                noRoles = db.Users.Where(u => u.Roles.All(r => r.UserId != u.Id)).ToList()
            };

            return View(model);

            
            //return View();
        }

        //GET: Users/UserRoles EDIT
        public ActionResult EditRole(string RoleName, string querry)
        {
            var usersInRole = helper.UsersInRole(RoleName).Select(u => u.Id);

            var model = new UnifiedRoleView
            {
                RoleId = db.Roles.FirstOrDefault(r => r.Name == RoleName).Id,
                RoleName = RoleName,
                Users = new MultiSelectList(db.Users, "Id", "DisplayName", usersInRole),
            };


            return View(model);
        }

        //POST: User/UserRoles EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(UnifiedRoleView model)
        {
            if (ModelState.IsValid)
            {
                foreach (var user in db.Users.Where(u=>u.Email != "rgoodwin16@outlook.com" &&
                                                       u.Email != "admin@guest.com" &&
                                                       u.Email != "projectmanager@guest.com" &&
                                                       u.Email != "developer@guest.com" &&
                                                       u.Email != "submitter@guest.com"))
                {
                    
                    if (model.Selected != null && model.Selected.Contains(user.Id))
                    {
                        helper.AddUserToRole(user.Id, model.RoleName);
                    }
                    else
                    {
                        helper.RemoveUserFromRole(user.Id, model.RoleName);
                    }

                }
            }

            return RedirectToAction("EditRole", new { RoleName = model.RoleName });
        }

    }
}