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
            
            var user = db.Users.Find(User.Identity.GetUserId());//grab the current user from the db

            var myProjects = user.Projects.ToList();//grab every project associated with this user
            var myTickets = myProjects.SelectMany(p=> p.Tickets).ToList();//grab every ticket associated with this user's projects
            var devTickets = user.AssignedTickets.ToList();//grab every ticket for this developer
            
            var projects = db.Projects.ToList();//grab every project from the db
            var tickets = db.Tickets.ToList();//grab every ticket from the db
            
            var users = db.Users.ToList();//grab every user in the db

            var model = new DashBoardViewModel
            {
                MyProjects = myProjects,
                MyTickets = myTickets,
                DevTickets = devTickets,
                Projects = projects,
                Tickets = tickets,
                Users = users,
            };


            return View(model);
        }
    }
}