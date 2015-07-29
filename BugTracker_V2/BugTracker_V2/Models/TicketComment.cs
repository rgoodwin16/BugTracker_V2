using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker_V2.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string AuthorId { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public Nullable<System.DateTimeOffset> Updated { get; set; }
        public string UpdateReason { get; set; }

        public virtual ApplicationUser Author { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}