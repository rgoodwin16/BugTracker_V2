using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker_V2.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string AuthorId { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }      
        public System.DateTimeOffset Created { get; set; }
        public Nullable<System.DateTimeOffset> Updated { get; set; }
        public string UpdateReason { get; set; }
        public string MediaURL { get; set; }
        public string Type { get; set; }

        public virtual ApplicationUser Author { get; set; }
        public virtual Ticket Ticket { get; set; }
    }

}