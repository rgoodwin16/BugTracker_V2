using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker_V2.Models
{
    public class DashBoardViewModel
    {
        public  List<Project> Projects { get; set; }
        public  List<Ticket> Tickets { get; set; }
        public  List<ApplicationUser> Users { get; set; }
    }
}