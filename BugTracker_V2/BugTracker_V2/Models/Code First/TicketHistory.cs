using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker_V2.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/d/yy}")]
        public System.DateTimeOffset Changed { get; set; }
        public string EditId { get; set; }


        public virtual ApplicationUser User { get; set; }
        public virtual Ticket Ticket { get; set; }

    }
}