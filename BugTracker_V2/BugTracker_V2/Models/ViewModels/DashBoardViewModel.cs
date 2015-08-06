using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker_V2.Models
{
    public class DashBoardViewModel
    {
        public List<Project> Projects { get; set; }//all projects
        public List<Ticket> Tickets { get; set; }//all tickets

        public List<Project> MyProjects { get; set; }//user projects
        public List<Ticket> MyTickets { get; set; }// user project's tickets

        public List<Ticket> DevTickets { get; set; }//dev user tickets
        
        public List<ApplicationUser> Users { get; set; }//all users
    }
}