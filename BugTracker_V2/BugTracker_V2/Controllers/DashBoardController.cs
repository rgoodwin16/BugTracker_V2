using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker_V2.Models;
using Microsoft.AspNet.Identity;
namespace BugTracker_V2.Controllers
{
    
    [RequireHttps]
    [Authorize]
    public class DashBoardController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: DashBoard
        public ActionResult Index()
        {
            //instantiate a new dashbard view model
            //populate the dashboard view model with necessary fields
            //return view, passing in the dashboard view model
            var user = db.Users.Find(User.Identity.GetUserId());

            var projects = user.Projects.ToList();
            var tickets = projects.SelectMany(p=>p.Tickets).ToList();
            var users = db.Users.ToList();

            var model = new DashBoardViewModel
            {
                Projects = projects,
                Tickets = tickets,
                Users = users,
            };


            return View(model);
        }
    }
}