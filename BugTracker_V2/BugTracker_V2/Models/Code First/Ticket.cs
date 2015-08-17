using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker_V2.Models
{
    public class Ticket
    {
        public Ticket()
        {
            this.Comments = new HashSet<TicketComment>();
            this.Attachments = new HashSet<TicketAttachment>();
            this.History = new HashSet<TicketHistory>();
        }

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set;}
        [AllowHtml]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/d/yy}")]
        public System.DateTimeOffset Created { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/d/yy}")]
        public Nullable<System.DateTimeOffset> Updated { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public int TicketTypeId { get; set; }

        public string OwnedById { get; set; }
        public string AssignedToId { get; set; }

        public virtual ApplicationUser OwnedBy { get; set; }
        public virtual ApplicationUser AssignedTo { get; set; }

        public virtual Project Project { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketType TicketType { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; }
        public virtual ICollection<TicketAttachment> Attachments { get; set; }
        public virtual ICollection<TicketHistory> History { get; set; }


    }
}