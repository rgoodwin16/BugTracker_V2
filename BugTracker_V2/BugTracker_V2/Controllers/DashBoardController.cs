using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker_V2.Models;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Threading.Tasks;
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

            DashBoardViewModel model;

            var user = db.Users.Find(User.Identity.GetUserId());    //grab the current user from the db

            if (User.IsInRole("Admin"))
            {
                var projects = db.Projects.ToList();    //grab every project from the db
                var tickets = db.Tickets.ToList();      //grab every ticket from the db
                var users = db.Users.ToList();          //grab every user in the db
                
                model = new DashBoardViewModel
                {
                    Projects = projects,
                    Tickets = tickets,
                    Users = users,
                };

                
            }
           
            else if (User.IsInRole("ProjectManager") || User.IsInRole("Developer"))
            
            {

                ViewBag.Error = TempData["ErrorMessage"];

                var myProjects = user.Projects.ToList();                            //grab every project associated with this user
                var myTickets = myProjects.SelectMany(p => p.Tickets).ToList();     //grab every ticket associated with this user's projects

                var devTickets = user.AssignedTickets.ToList();                     //grab every ticket for this developer

                model = new DashBoardViewModel
                {
                    MyProjects = myProjects,
                    MyTickets = myTickets,
                    DevTickets = devTickets,
                };

                
                
            }
            
            else 
            
            {
                var myProjects = user.Projects.ToList();    //grab every project from the db
                model = new DashBoardViewModel
                {
                   MyTickets = db.Tickets.Where(t=> t.OwnedById == user.Id).ToList(),
                   MyProjects = myProjects,
                };

                
            }

            return View(model);
                        
        }


    }
}