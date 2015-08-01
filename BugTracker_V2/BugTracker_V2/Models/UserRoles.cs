using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker_V2.Models
{
    public class UserRoles
    {
        public List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole> RoleList { get; set; }
        public List<ApplicationUser> UserList { get; set; }
        public List<ApplicationUser> noRoles { get; set; }
    }
}