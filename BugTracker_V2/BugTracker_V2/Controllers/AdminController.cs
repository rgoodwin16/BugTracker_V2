using BugTracker_V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker_V2.Controllers
{
    [Authorize]
    [RequireHttps]
    public class AdminController : Controller
    {

        UserRolesHelper helper = new UserRolesHelper();
        ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        //GET: Admin/UserList
        public ActionResult UserList()
        {
            var users = db.Users.ToList();
            var roles = db.Roles.ToList();
            var model = new UserRoles
            
            {
                RoleList = roles,
                UserList = users,
            };
            
            return View(model);
        }



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
                foreach (var user in db.Users)
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